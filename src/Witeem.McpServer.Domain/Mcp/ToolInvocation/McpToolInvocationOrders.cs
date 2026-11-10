namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Canonical order segments for invocation middlewares. The pipeline executes in
/// ascending order; the terminal (reflection invocation) sits at the very end.
/// Segment gaps leave room for user middlewares between built-in stages.
/// </summary>
public static class McpToolInvocationOrders
{
    /// <summary>Outermost stage: persists the audit record in a finally block, capturing every short-circuit.</summary>
    public const int Auditing = -100;

    /// <summary>Reserved: coarse-grained anonymous rate limiting (DoS protection, runs before authorization).</summary>
    public const int GlobalRateLimit = 100;

    /// <summary>Authorization stage: fail-closed enforcement of tool permission requirements.</summary>
    public const int Authorization = 200;

    /// <summary>Reserved: tenant/user scoped rate limiting (runs after authorization, when identity is known).</summary>
    public const int ScopedRateLimit = 300;

    /// <summary>Built-in argument binding (fills <see cref="McpToolInvocationContext.Arguments"/>).</summary>
    public const int ArgumentBinding = 500;

    /// <summary>Reserved: schema-based parameter validation (runs after binding).</summary>
    public const int Validation = 600;

    /// <summary>Reserved: invocation timeout enforcement.</summary>
    public const int Timeout = 700;

    /// <summary>Reserved: explicit unit-of-work boundary for non-AppService tool targets.</summary>
    public const int UnitOfWork = 800;
}
