using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Generates valid JSON Schema documents for tool parameters.
/// Fixes the previous generator that emitted non-schema type names ("int32", "guid").
/// </summary>
public static class McpToolSchemaGenerator
{
    private const int MaxDepth = 3;

    public static string Generate(MethodInfo method)
    {
        var properties = new Dictionary<string, object>(StringComparer.Ordinal);
        var required = new List<string>();

        foreach (var parameter in method.GetParameters())
        {
            if (parameter.ParameterType == typeof(CancellationToken))
            {
                continue;
            }

            var schema = BuildSchema(parameter.ParameterType, depth: 0);
            var description = parameter.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description;
            if (description != null)
            {
                schema = new Dictionary<string, object>(schema) { ["description"] = description };
            }

            properties[parameter.Name!] = schema;

            if (!parameter.HasDefaultValue && !IsNullableReference(parameter))
            {
                required.Add(parameter.Name!);
            }
        }

        var document = new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = properties
        };
        if (required.Count > 0)
        {
            document["required"] = required;
        }

        return JsonSerializer.Serialize(document);
    }

    private static bool IsNullableReference(ParameterInfo parameter) =>
        new NullabilityInfoContext().Create(parameter).WriteState == NullabilityState.Nullable;

    private static Dictionary<string, object> BuildSchema(Type type, int depth)
    {
        var underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            var inner = BuildSchema(underlying, depth);
            inner["nullable"] = true;
            return inner;
        }

        if (type.IsEnum)
        {
            return new Dictionary<string, object>
            {
                ["type"] = "string",
                ["enum"] = Enum.GetNames(type)
            };
        }

        if (type == typeof(Guid) || type == typeof(DateTime) || type == typeof(DateTimeOffset) ||
            type == typeof(TimeSpan) || type == typeof(DateOnly) || type == typeof(TimeOnly))
        {
            var format = type == typeof(Guid) ? "uuid"
                : type == typeof(TimeSpan) ? "duration"
                : type == typeof(DateOnly) ? "date"
                : type == typeof(TimeOnly) ? "time"
                : "date-time";
            return new Dictionary<string, object>
            {
                ["type"] = "string",
                ["format"] = format
            };
        }

        if (type == typeof(string) || type == typeof(char))
        {
            return new Dictionary<string, object> { ["type"] = "string" };
        }

        if (type == typeof(bool))
        {
            return new Dictionary<string, object> { ["type"] = "boolean" };
        }

        if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) ||
            type == typeof(ushort) || type == typeof(int) || type == typeof(uint) ||
            type == typeof(long) || type == typeof(ulong))
        {
            var format = type == typeof(int) ? "int32"
                : type == typeof(long) ? "int64"
                : (type == typeof(byte) || type == typeof(sbyte)) ? "int8"
                : "int16";
            return new Dictionary<string, object>
            {
                ["type"] = "integer",
                ["format"] = format
            };
        }

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            var format = type == typeof(float) ? "float"
                : type == typeof(decimal) ? "decimal"
                : "double";
            return new Dictionary<string, object>
            {
                ["type"] = "number",
                ["format"] = format
            };
        }

        if (IsArrayLike(type, out var elementType))
        {
            return new Dictionary<string, object>
            {
                ["type"] = "array",
                ["items"] = depth >= MaxDepth
                    ? new Dictionary<string, object> { ["type"] = "object" }
                    : BuildSchema(elementType, depth + 1)
            };
        }

        // Complex object: recurse with a depth guard to avoid runaway schemas.
        if (depth >= MaxDepth)
        {
            return new Dictionary<string, object> { ["type"] = "object" };
        }

        var properties = new Dictionary<string, object>(StringComparer.Ordinal);
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
        {
            properties[property.Name] = BuildSchema(property.PropertyType, depth + 1);
        }

        return new Dictionary<string, object>
        {
            ["type"] = "object",
            ["properties"] = properties
        };
    }

    private static bool IsArrayLike(Type type, out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        if (type == typeof(string) || !typeof(IEnumerable).IsAssignableFrom(type))
        {
            elementType = typeof(object);
            return false;
        }

        if (type.IsGenericType && type.GetGenericArguments().Length == 1)
        {
            elementType = type.GetGenericArguments()[0];
            return true;
        }

        elementType = typeof(object);
        return false;
    }
}
