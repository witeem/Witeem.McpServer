using System.Threading.Tasks;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// A middleware stage of the MCP tool invocation pipeline. Unlike ABP's list-based
/// contributors (which can neither short-circuit nor wrap), middlewares can abort
/// the invocation (rate limiting, authorization) and wrap it (auditing, timing).
/// </summary>
public interface IMcpToolInvocationMiddleware
{
    /// <summary>Execution order; lower values run first. Use <see cref="McpToolInvocationOrders"/> segments.</summary>
    int Order { get; }

    /// <summary>
    /// Processes the invocation. Call <paramref name="next"/> to hand over to the
    /// following stage; returning without calling it short-circuits the pipeline.
    /// </summary>
    Task InvokeAsync(McpToolInvocationContext context, Func<Task> next);
}
