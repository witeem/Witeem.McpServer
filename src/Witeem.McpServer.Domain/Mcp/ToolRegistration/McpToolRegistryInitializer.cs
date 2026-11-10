using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Startup assembly entry point for the tool registry. Previously the scanner
/// had no caller and the registry stayed empty; hosts now get registration for
/// free through the domain module initialization.
/// </summary>
public class McpToolRegistryInitializer : ITransientDependency
{
    private readonly IMcpToolProvider _toolProvider;
    private readonly IMcpToolScanner _toolScanner;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IOptions<McpToolOptions> _options;
    private readonly ILogger<McpToolRegistryInitializer> _logger;

    public McpToolRegistryInitializer(
        IMcpToolProvider toolProvider,
        IMcpToolScanner toolScanner,
        IUnitOfWorkManager unitOfWorkManager,
        IOptions<McpToolOptions> options,
        ILogger<McpToolRegistryInitializer> logger)
    {
        _toolProvider = toolProvider;
        _toolScanner = toolScanner;
        _unitOfWorkManager = unitOfWorkManager;
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var assemblies = _options.Value.ToolAssemblies;
        if (assemblies.Count == 0)
        {
            return;
        }

        var descriptors = assemblies
            .SelectMany(assembly => _toolScanner.Scan(assembly))
            .ToList();

        if (descriptors.Count == 0)
        {
            return;
        }

        try
        {
            using (var unitOfWork = _unitOfWorkManager.Begin(requiresNew: true))
            {
                await _toolScanner.EnrichFromDatabaseAsync(descriptors);
                await unitOfWork.CompleteAsync();
            }
        }
        catch (Exception ex)
        {
            // Log-and-continue: a database outage must not take the host down.
            // Tools are registered un-enriched (empty ids, enabled by default).
            _logger.LogWarning(ex, "Enriching MCP tool registry from database failed; registering tools with default state.");
        }

        try
        {
            _toolProvider.ReplaceAll(descriptors);
            _logger.LogInformation("Registered {ToolCount} MCP tool(s): {ToolNames}",
                descriptors.Count, string.Join(", ", descriptors.Select(d => d.ToolName)));
        }
        catch (Exception ex)
        {
            // Duplicate names or other consistency failures keep the previous snapshot intact.
            _logger.LogError(ex, "Replacing the MCP tool registry failed; the previous snapshot is kept.");
        }
    }
}
