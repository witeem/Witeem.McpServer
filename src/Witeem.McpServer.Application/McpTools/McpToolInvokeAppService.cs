using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Security.Claims;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Domain.Mcp.ToolInvocation;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Application;

/// <summary>
/// Application-layer entry point for invoking MCP tools. Injects the ambient
/// caller principal into the request so authorization works for both HTTP and
/// non-HTTP transports.
/// </summary>
public class McpToolInvokeAppService : ApplicationService, IMcpToolInvokeAppService
{
    private readonly IMcpToolExecutor _executor;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public McpToolInvokeAppService(
        IMcpToolExecutor executor,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        _executor = executor;
        _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public async Task<McpToolInvokeResult> InvokeAsync(McpToolInvokeRequest request)
    {
        request.CallerPrincipal = _currentPrincipalAccessor.Principal;
        return await _executor.InvokeAsync(request);
    }
}
