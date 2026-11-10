using System;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Witeem.McpServer.Domain.Shared.Mcp;

/// <summary>
/// A request to invoke a registered MCP tool. Used by the application-service
/// entry point and by the executor. <see cref="CallerPrincipal"/> is never bound
/// from client JSON — it is populated by the application layer from the ambient principal.
/// </summary>
public sealed class McpToolInvokeRequest
{
    /// <summary>Name of the tool to invoke.</summary>
    public string ToolName { get; set; } = default!;

    /// <summary>Raw JSON object containing the tool arguments.</summary>
    public string? ParametersJson { get; set; }

    public string? SessionId { get; set; }

    public string? RequestId { get; set; }

    /// <summary>Identity of the caller, injected by the application layer. Ignored on JSON binding.</summary>
    [JsonIgnore]
    public ClaimsPrincipal? CallerPrincipal { get; set; }
}
