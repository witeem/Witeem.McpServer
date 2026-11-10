using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Enforces tool authorization requirements, fail-closed:
/// tools without an authorization requirement pass through;
/// required tools without any handler are denied;
/// handler faults are logged and treated as denials.
/// </summary>
public class McpToolAuthorizationMiddleware : IMcpToolInvocationMiddleware, ITransientDependency
{
    public int Order => McpToolInvocationOrders.Authorization;

    private readonly IEnumerable<IMcpToolAuthorizationHandler> _handlers;
    private readonly ILogger<McpToolAuthorizationMiddleware> _logger;

    public McpToolAuthorizationMiddleware(
        IEnumerable<IMcpToolAuthorizationHandler> handlers,
        ILogger<McpToolAuthorizationMiddleware> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    public async Task InvokeAsync(McpToolInvocationContext context, Func<Task> next)
    {
        if (!context.Descriptor.RequiresAuthorization)
        {
            await next();
            return;
        }

        var handlers = _handlers.OrderBy(h => h.Order).ToList();
        if (handlers.Count == 0)
        {
            Deny(context, $"Tool '{context.Descriptor.ToolName}' requires authorization but no authorization handler is configured.");
            return;
        }

        foreach (var handler in handlers)
        {
            McpToolAuthorizationResult result;
            try
            {
                result = await handler.CheckAsync(context);
            }
            catch (Exception ex)
            {
                // Fail-closed: an authorization fault must never widen access.
                _logger.LogError(ex, "Authorization handler {HandlerType} faulted while checking tool {ToolName}.",
                    handler.GetType().Name, context.Descriptor.ToolName);
                Deny(context, "Authorization check failed.");
                return;
            }

            if (!result.Allowed)
            {
                Deny(context, result.DenialReason ?? $"Caller is not allowed to invoke tool '{context.Descriptor.ToolName}'.");
                return;
            }
        }

        await next();
    }

    private static void Deny(McpToolInvocationContext context, string reason)
    {
        context.FailureReason = McpToolFailureReason.AuthorizationDenied;
        context.ErrorMessage = reason;
    }
}
