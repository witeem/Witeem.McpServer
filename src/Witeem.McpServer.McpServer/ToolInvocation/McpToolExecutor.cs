using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Domain.Entities;
using Witeem.McpServer.McpServer.ToolRegistration;

namespace Witeem.McpServer.McpServer.ToolInvocation;

public class McpToolExecutor : ITransientDependency
{
    private readonly IRepository<McpToolCall, Guid> _toolCallRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly McpToolProvider _toolProvider;

    public McpToolExecutor(
        IRepository<McpToolCall, Guid> toolCallRepository,
        IServiceProvider serviceProvider,
        McpToolProvider toolProvider)
    {
        _toolCallRepository = toolCallRepository;
        _serviceProvider = serviceProvider;
        _toolProvider = toolProvider;
    }

    public async Task<McpToolCallResult> ExecuteAsync(string toolName, string parameters, string? sessionId = null, string? requestId = null)
    {
        var descriptor = _toolProvider.GetTool(toolName);
        if (descriptor == null)
        {
            return new McpToolCallResult { Success = false, ErrorMessage = $"Tool '{toolName}' not found" };
        }

        var stopwatch = Stopwatch.StartNew();
        string? result = null;
        string? errorMessage = null;
        bool success = false;

        try
        {
            var serviceInstance = _serviceProvider.GetService(descriptor.ServiceType);
            if (serviceInstance == null)
            {
                return new McpToolCallResult { Success = false, ErrorMessage = $"Service '{descriptor.ServiceType.Name}' not resolved" };
            }

            var methodParams = descriptor.MethodInfo.GetParameters();
            var args = new object?[methodParams.Length];
            var jsonDoc = System.Text.Json.JsonDocument.Parse(parameters ?? "{}");

            for (int i = 0; i < methodParams.Length; i++)
            {
                if (methodParams[i].ParameterType == typeof(CancellationToken))
                {
                    args[i] = CancellationToken.None;
                }
                else if (jsonDoc.RootElement.TryGetProperty(methodParams[i].Name!, out var element))
                {
                    args[i] = System.Text.Json.JsonSerializer.Deserialize(element, methodParams[i].ParameterType);
                }
                else
                {
                    args[i] = methodParams[i].HasDefaultValue ? methodParams[i].DefaultValue : null;
                }
            }

            var invokeResult = descriptor.MethodInfo.Invoke(serviceInstance, args);

            if (invokeResult is Task task)
            {
                await task;
                var resultProperty = task.GetType().GetProperty("Result");
                result = resultProperty != null ? System.Text.Json.JsonSerializer.Serialize(resultProperty.GetValue(task)) : null;
            }
            else
            {
                result = invokeResult != null ? System.Text.Json.JsonSerializer.Serialize(invokeResult) : null;
            }

            success = true;
        }
        catch (Exception ex)
        {
            errorMessage = ex.InnerException?.Message ?? ex.Message;
        }

        stopwatch.Stop();

        var toolCall = new McpToolCall
        {
            ToolId = descriptor.ToolId,
            ToolName = toolName,
            Parameters = parameters,
            Result = result,
            ErrorMessage = errorMessage,
            ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds,
            Success = success,
            CallTime = DateTime.UtcNow,
            SessionId = sessionId,
            RequestId = requestId
        };

        await _toolCallRepository.InsertAsync(toolCall);

        return new McpToolCallResult
        {
            Success = success,
            Result = result,
            ErrorMessage = errorMessage,
            ExecutionDurationMs = (int)stopwatch.ElapsedMilliseconds
        };
    }
}

public class McpToolCallResult
{
    public bool Success { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int ExecutionDurationMs { get; set; }
}
