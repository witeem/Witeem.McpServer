using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Binds raw JSON parameters to method arguments: case-insensitive parameter
/// matching, positional CancellationToken injection, default values for absent
/// parameters, and InvalidParameters short-circuits on malformed input.
/// </summary>
public class McpToolArgumentBindingMiddleware : IMcpToolInvocationMiddleware, ITransientDependency
{
    public int Order => McpToolInvocationOrders.ArgumentBinding;

    public async Task InvokeAsync(McpToolInvocationContext context, Func<Task> next)
    {
        var parameters = context.Descriptor.MethodInfo.GetParameters();
        var arguments = new object?[parameters.Length];
        var jsonValues = ParseParameters(context.RawParameters);

        if (jsonValues == null)
        {
            context.FailureReason = McpToolFailureReason.InvalidParameters;
            context.ErrorMessage = "Tool parameters must be a JSON object.";
            return;
        }

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            if (parameter.ParameterType == typeof(CancellationToken))
            {
                arguments[i] = context.CancellationToken;
                continue;
            }

            if (jsonValues.TryGetValue(parameter.Name!, out var element))
            {
                try
                {
                    arguments[i] = JsonSerializer.Deserialize(element, parameter.ParameterType, McpToolJson.Options);
                }
                catch (JsonException ex)
                {
                    context.FailureReason = McpToolFailureReason.InvalidParameters;
                    context.ErrorMessage = $"Parameter '{parameter.Name}' could not be bound: {ex.Message}";
                    return;
                }
            }
            else if (parameter.HasDefaultValue)
            {
                arguments[i] = parameter.DefaultValue;
            }
        }

        context.Arguments = arguments;
        await next();
    }

    /// <summary>Returns a case-insensitive view of the JSON object; null when the payload is not a valid object.</summary>
    private static Dictionary<string, JsonElement>? ParseParameters(string? rawParameters)
    {
        if (rawParameters.IsNullOrWhiteSpace())
        {
            return new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            using var document = JsonDocument.Parse(rawParameters);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var values = new Dictionary<string, JsonElement>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                values[property.Name] = property.Value.Clone();
            }

            return values;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
