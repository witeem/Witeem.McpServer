using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;
using Witeem.McpServer.Domain.Mcp.ToolInvocation;

namespace Witeem.McpServer.Application;

/// <summary>
/// Default authorization handler for the application layer. Evaluates the
/// caller carried by the invocation context (not the HTTP-only ICurrentUser),
/// and wraps permission checks with the ambient principal so stdio-style hosts
/// still get correct decisions.
/// </summary>
public class PermissionMcpToolAuthorizationHandler : IMcpToolAuthorizationHandler, ITransientDependency
{
    public int Order => 0;

    private readonly IPermissionChecker _permissionChecker;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;

    public PermissionMcpToolAuthorizationHandler(
        IPermissionChecker permissionChecker,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        _permissionChecker = permissionChecker;
        _currentPrincipalAccessor = currentPrincipalAccessor;
    }

    public async Task<McpToolAuthorizationResult> CheckAsync(McpToolInvocationContext context)
    {
        var principal = context.CallerPrincipal;
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return McpToolAuthorizationResult.Deny("An authenticated caller is required to invoke this tool.");
        }

        var permissionName = context.Descriptor.RequiredPermissionName;
        if (permissionName.IsNullOrWhiteSpace())
        {
            return McpToolAuthorizationResult.Allow();
        }

        using (_currentPrincipalAccessor.Change(principal))
        {
            return await _permissionChecker.IsGrantedAsync(permissionName)
                ? McpToolAuthorizationResult.Allow()
                : McpToolAuthorizationResult.Deny($"Permission '{permissionName}' is required to invoke this tool.");
        }
    }
}
