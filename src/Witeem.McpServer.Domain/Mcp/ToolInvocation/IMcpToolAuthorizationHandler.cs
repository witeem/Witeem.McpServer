using System.Threading.Tasks;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Evaluates whether the caller may invoke a tool. Implementations are composed:
/// the authorization middleware requires every handler to allow (any denial or
/// fault short-circuits the invocation). The default domain layer registers no
/// handler — tools marked with authorization requirements are then denied
/// (fail-closed); hosts that need real checks provide handlers (e.g. the
/// permission-based one in the application layer).
/// </summary>
public interface IMcpToolAuthorizationHandler
{
    /// <summary>Order in which handlers are consulted.</summary>
    int Order { get; }

    /// <summary>Checks the invocation against the handler's policy.</summary>
    Task<McpToolAuthorizationResult> CheckAsync(McpToolInvocationContext context);
}
