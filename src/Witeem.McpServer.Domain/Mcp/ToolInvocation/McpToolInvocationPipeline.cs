using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Singleton pipeline that compiles the middleware chain once and executes it
/// per invocation. The compiled chain is a pure function of the context, so a
/// single immutable delegate is safe for concurrent calls. The terminal stage
/// (reflection invocation and result extraction) is always last.
/// </summary>
public class McpToolInvocationPipeline : ISingletonDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly McpToolInvocationOptions _options;
    private readonly Lazy<Func<McpToolInvocationContext, Task>> _compiled;

    public McpToolInvocationPipeline(IServiceProvider serviceProvider, IOptions<McpToolInvocationOptions> options)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _compiled = new Lazy<Func<McpToolInvocationContext, Task>>(
            BuildChain, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>Executes the compiled chain; unwraps <see cref="TargetInvocationException"/> from the terminal stage.</summary>
    public async Task ExecuteAsync(McpToolInvocationContext context)
    {
        try
        {
            await _compiled.Value(context);
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    private Func<McpToolInvocationContext, Task> BuildChain()
    {
        var middlewares = MergeMiddlewares();
        Func<McpToolInvocationContext, Task> chain = InvokeTerminalAsync;
        foreach (var middleware in middlewares.AsEnumerable().Reverse())
        {
            var stage = middleware;
            var inner = chain;
            chain = context => stage.InvokeAsync(context, () => inner(context));
        }
        return chain;
    }

    private IReadOnlyList<IMcpToolInvocationMiddleware> MergeMiddlewares()
    {
        // Options-registered instances take precedence over DI-discovered instances of the same type.
        var merged = new List<IMcpToolInvocationMiddleware>(_options.Middlewares);
        foreach (var middleware in _serviceProvider.GetServices<IMcpToolInvocationMiddleware>())
        {
            if (merged.All(m => m.GetType() != middleware.GetType()))
            {
                merged.Add(middleware);
            }
        }

        return merged.OrderBy(m => m.Order).ToList();
    }

    /// <summary>Terminal stage: resolve the tool target from the invocation scope, invoke, and extract the result.</summary>
    private static async Task InvokeTerminalAsync(McpToolInvocationContext context)
    {
        var serviceInstance = context.ScopedServices.GetRequiredService(context.Descriptor.ServiceType);
        var invocationReturn = context.Descriptor.MethodInfo.Invoke(serviceInstance, context.Arguments);

        var result = await McpToolResultExtractor.ExtractResultAsync(invocationReturn, context.CancellationToken);
        context.Result = result;
        context.ResultJson = result == null
            ? null
            : JsonSerializer.Serialize(result, McpToolJson.Options);
    }
}
