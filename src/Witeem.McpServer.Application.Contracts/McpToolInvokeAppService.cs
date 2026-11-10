using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Application.Contracts;

/// <summary>
/// Execution entry point for MCP tools. Consolidates tool invocation behind the
/// application layer so HTTP/stdio hosts depend only on contracts.
/// </summary>
public interface IMcpToolInvokeAppService : IApplicationService
{
    Task<McpToolInvokeResult> InvokeAsync(McpToolInvokeRequest request);
}
