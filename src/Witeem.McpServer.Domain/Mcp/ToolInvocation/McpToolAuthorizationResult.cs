using System.Threading.Tasks;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Result-based authorization decision. Result semantics (instead of exceptions)
/// distinguish a deliberate denial from a handler fault: handler faults are
/// treated as fail-closed denials with logging.
/// </summary>
public sealed class McpToolAuthorizationResult
{
    public bool Allowed { get; init; }

    /// <summary>Human-readable denial reason; null when allowed.</summary>
    public string? DenialReason { get; init; }

    public static McpToolAuthorizationResult Allow() => new() { Allowed = true };

    public static McpToolAuthorizationResult Deny(string denialReason) =>
        new() { Allowed = false, DenialReason = denialReason };
}
