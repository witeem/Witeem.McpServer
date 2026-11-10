using System.Collections.Generic;
using System.Security.Claims;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Per-invocation pipeline context. One instance per tool call, flowing through
/// the middleware chain on a single logical flow — deliberately NOT thread-safe.
/// Carries the caller principal so authorization works in non-HTTP hosts (stdio).
/// </summary>
public sealed class McpToolInvocationContext
{
    /// <summary>Scoped service provider created for this invocation; use it to resolve tool targets.</summary>
    public IServiceProvider ScopedServices { get; }

    public McpToolDescriptor Descriptor { get; }

    /// <summary>Raw JSON parameters as received (null when absent).</summary>
    public string? RawParameters { get; }

    public string? SessionId { get; }
    public string? RequestId { get; }

    /// <summary>Caller identity supplied by the host (null for anonymous transports).</summary>
    public ClaimsPrincipal? CallerPrincipal { get; }

    public Guid? TenantId { get; }

    /// <summary>Bound method arguments, filled by the argument-binding middleware.</summary>
    public object?[]? Arguments { get; set; }

    /// <summary>Untyped tool result, filled by the terminal stage only.</summary>
    public object? Result { get; set; }

    /// <summary>Serialized tool output (camelCase JSON).</summary>
    public string? ResultJson { get; set; }

    /// <summary>Set by short-circuiting middlewares or the executor's error handler.</summary>
    public McpToolFailureReason? FailureReason { get; set; }

    public string? ErrorMessage { get; set; }

    /// <summary>Elapsed execution time; set by the auditing middleware (outermost stage).</summary>
    public int ExecutionDurationMs { get; set; }

    public CancellationToken CancellationToken { get; set; }

    /// <summary>Strongly-typed extensible bag for cross-middleware data (rate-limit keys, authorization decisions).</summary>
    public IDictionary<object, object?> Items { get; } = new Dictionary<object, object?>();

    public McpToolInvocationContext(
        IServiceProvider scopedServices,
        McpToolDescriptor descriptor,
        McpToolInvokeRequest request,
        CancellationToken cancellationToken,
        Guid? tenantId)
    {
        ScopedServices = scopedServices;
        Descriptor = descriptor;
        RawParameters = request.ParametersJson;
        SessionId = request.SessionId;
        RequestId = request.RequestId;
        CallerPrincipal = request.CallerPrincipal;
        TenantId = tenantId;
        CancellationToken = cancellationToken;
    }
}
