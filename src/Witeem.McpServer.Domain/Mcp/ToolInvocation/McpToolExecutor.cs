using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Default executor: resolves the tool from the registry, opens a per-call
/// service scope (so scoped dependencies never root into the singleton
/// pipeline), runs the middleware chain, and maps outcomes to
/// <see cref="McpToolInvokeResult"/> with a categorized failure reason.
/// </summary>
public class McpToolExecutor : IMcpToolExecutor, ITransientDependency
{
    private readonly IMcpToolProvider _toolProvider;
    private readonly McpToolInvocationPipeline _pipeline;
    private readonly ICurrentTenant _currentTenant;
    private readonly IServiceProvider _rootServiceProvider;

    public McpToolExecutor(
        IMcpToolProvider toolProvider,
        McpToolInvocationPipeline pipeline,
        ICurrentTenant currentTenant,
        IServiceProvider rootServiceProvider)
    {
        _toolProvider = toolProvider;
        _pipeline = pipeline;
        _currentTenant = currentTenant;
        _rootServiceProvider = rootServiceProvider;
    }

    public async Task<McpToolInvokeResult> InvokeAsync(McpToolInvokeRequest request, CancellationToken cancellationToken = default)
    {
        if (!_toolProvider.TryGetTool(request.ToolName, out var descriptor) || descriptor == null)
        {
            return McpToolInvokeResult.Failed(
                McpToolFailureReason.ToolNotFound,
                Guid.Empty,
                $"Tool '{request.ToolName}' not found.");
        }

        if (!descriptor.Enabled)
        {
            return McpToolInvokeResult.Failed(
                McpToolFailureReason.Disabled,
                descriptor.ToolId,
                $"Tool '{request.ToolName}' is disabled.");
        }

        var stopwatch = Stopwatch.StartNew();
        using var scope = _rootServiceProvider.CreateScope();
        var context = new McpToolInvocationContext(
            scope.ServiceProvider,
            descriptor,
            request,
            cancellationToken,
            _currentTenant.Id);

        try
        {
            await _pipeline.ExecuteAsync(context);
        }
        catch (OperationCanceledException)
        {
            context.FailureReason = McpToolFailureReason.Timeout;
            context.ErrorMessage = "The tool invocation was canceled.";
        }
        catch (Exception ex)
        {
            context.FailureReason = McpToolFailureReason.ExecutionError;
            context.ErrorMessage = ex.Message;
        }

        // The auditing middleware (outermost) normally owns timing; fall back when absent.
        if (context.ExecutionDurationMs == 0)
        {
            stopwatch.Stop();
            context.ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds;
        }

        return new McpToolInvokeResult
        {
            Success = context.FailureReason is null,
            FailureReason = context.FailureReason ?? McpToolFailureReason.None,
            ToolId = descriptor.ToolId,
            ResultJson = context.ResultJson,
            ErrorMessage = context.ErrorMessage,
            ExecutionDurationMs = context.ExecutionDurationMs
        };
    }
}
