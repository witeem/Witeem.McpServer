using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Outermost pipeline stage: persists an <see cref="McpToolCall"/> audit record
/// for every invocation of a known tool — including rejected and failed ones —
/// in a finally block. Persistence is fault-tolerant (log-and-continue) and runs
/// in its own unit of work so tool failures never roll back audit records.
/// </summary>
public class McpToolAuditingMiddleware : IMcpToolInvocationMiddleware, ITransientDependency
{
    public int Order => McpToolInvocationOrders.Auditing;

    private readonly IRepository<McpToolCall, Guid> _toolCallRepository;
    private readonly IOptions<McpToolInvocationOptions> _options;
    private readonly ILogger<McpToolAuditingMiddleware> _logger;

    public McpToolAuditingMiddleware(
        IRepository<McpToolCall, Guid> toolCallRepository,
        IOptions<McpToolInvocationOptions> options,
        ILogger<McpToolAuditingMiddleware> logger)
    {
        _toolCallRepository = toolCallRepository;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(McpToolInvocationContext context, Func<Task> next)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next();
        }
        finally
        {
            stopwatch.Stop();
            context.ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds;
            await PersistAuditAsync(context);
        }
    }

    private async Task PersistAuditAsync(McpToolInvocationContext context)
    {
        try
        {
            var maxLength = _options.Value.MaxAuditPayloadLength;
            var call = new McpToolCall
            {
                ToolId = context.Descriptor.ToolId,
                ToolName = context.Descriptor.ToolName,
                Parameters = context.RawParameters?.Truncate(maxLength),
                Result = context.ResultJson?.Truncate(maxLength),
                ErrorMessage = context.ErrorMessage?.Truncate(maxLength),
                ExecutionDurationMs = context.ExecutionDurationMs,
                Success = context.FailureReason is null,
                CallTime = DateTime.UtcNow,
                SessionId = context.SessionId,
                RequestId = context.RequestId,
                UserId = ResolveUserId(context.CallerPrincipal),
                UserName = context.CallerPrincipal?.Identity?.Name
            };

            // Audit must survive tool failures: run in a fresh, independent unit of work.
            var unitOfWorkManager = context.ScopedServices.GetService<IUnitOfWorkManager>();
            if (unitOfWorkManager != null)
            {
                using (var unitOfWork = unitOfWorkManager.Begin(requiresNew: true))
                {
                    await _toolCallRepository.InsertAsync(call);
                    await unitOfWork.CompleteAsync();
                }
            }
            else
            {
                await _toolCallRepository.InsertAsync(call);
            }
        }
        catch (Exception ex)
        {
            // Never let audit persistence break or invalidate the tool result.
            _logger.LogError(ex, "Persisting the MCP tool-call audit failed for tool {ToolName}.", context.Descriptor.ToolName);
        }
    }

    private static Guid? ResolveUserId(ClaimsPrincipal? principal)
    {
        var value = principal?.FindFirst(AbpClaimTypes.UserId)?.Value;
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
