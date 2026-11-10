using System.Threading;
using System.Threading.Tasks;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Orchestrates a single MCP tool invocation: registry lookup, scoped service
/// resolution, pipeline execution, failure mapping. Never throws for expected
/// failures — those are reported through <see cref="McpToolInvokeResult"/>.
/// </summary>
public interface IMcpToolExecutor
{
    Task<McpToolInvokeResult> InvokeAsync(McpToolInvokeRequest request, CancellationToken cancellationToken = default);
}
