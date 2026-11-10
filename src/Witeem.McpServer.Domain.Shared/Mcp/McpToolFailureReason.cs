namespace Witeem.McpServer.Domain.Shared.Mcp;

/// <summary>
/// Categorizes why an MCP tool invocation failed, so protocol layers can map
/// failure reasons to MCP/HTTP error codes instead of a bare message.
/// </summary>
public enum McpToolFailureReason
{
    /// <summary>The invocation succeeded.</summary>
    None = 0,

    /// <summary>No tool is registered under the requested name.</summary>
    ToolNotFound = 1,

    /// <summary>The tool exists but is disabled in configuration.</summary>
    Disabled = 2,

    /// <summary>The caller is not authorized to invoke the tool.</summary>
    AuthorizationDenied = 3,

    /// <summary>The invocation was rejected by a rate-limit middleware.</summary>
    RateLimited = 4,

    /// <summary>The supplied parameters could not be bound or validated.</summary>
    InvalidParameters = 5,

    /// <summary>The invocation was canceled or exceeded its budget.</summary>
    Timeout = 6,

    /// <summary>The tool executed but threw an unexpected error.</summary>
    ExecutionError = 7
}
