using System.Collections.Generic;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Configures the invocation pipeline following the ABP options convention:
/// modules register custom middlewares via <c>Configure&lt;McpToolInvocationOptions&gt;</c>.
/// Built-in middlewares (auditing, authorization, argument binding) are discovered
/// from DI; options-registered instances of the same type take precedence.
/// </summary>
public class McpToolInvocationOptions
{
    public IList<IMcpToolInvocationMiddleware> Middlewares { get; } = [];

    /// <summary>Maximum persisted length for audit result/parameter/error payloads. Longer values are truncated.</summary>
    public int MaxAuditPayloadLength { get; set; } = 8192;
}
