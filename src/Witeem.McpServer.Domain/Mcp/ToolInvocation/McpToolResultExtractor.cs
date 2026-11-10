using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Normalizes tool method return values into plain objects.
/// Supports sync methods, Task, Task&lt;T&gt;, ValueTask and ValueTask&lt;T&gt;,
/// and unwraps the completed value for generic task types.
/// </summary>
public static class McpToolResultExtractor
{
    public static async Task<object?> ExtractResultAsync(object? invocationReturn, CancellationToken cancellationToken)
    {
        if (invocationReturn == null)
        {
            return null;
        }

        if (invocationReturn is Task task)
        {
            await task.ConfigureAwait(false);
            return GetTaskResult(task);
        }

        if (invocationReturn is ValueTask valueTask)
        {
            await valueTask.ConfigureAwait(false);
            return null;
        }

        var returnType = invocationReturn.GetType();
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var asTask = (Task)returnType.GetMethod("AsTask")!.Invoke(invocationReturn, null)!;
            await asTask.ConfigureAwait(false);
            return GetTaskResult(asTask);
        }

        // Synchronous return value.
        return invocationReturn;
    }

    private static object? GetTaskResult(Task task)
    {
        // Async methods return an internal state-machine box that derives from
        // Task<T>, so inspect the whole base-type chain instead of only the
        // concrete runtime type.
        var taskType = task.GetType();
        while (taskType != null && taskType != typeof(Task))
        {
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                return taskType.GetProperty("Result")!.GetValue(task);
            }

            taskType = taskType.BaseType;
        }

        return null;
    }
}
