using System;
using System.Reflection;
using System.Text.Json;
using Volo.Abp.DependencyInjection;

namespace Witeem.McpServer.McpServer.ToolRegistration;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class McpServerToolAttribute : Attribute
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public McpServerToolAttribute() { }

    public McpServerToolAttribute(string name)
    {
        Name = name;
    }
}

public class McpToolDescriptor
{
    public Guid ToolId { get; set; }
    public string ToolName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Type ServiceType { get; set; } = default!;
    public MethodInfo MethodInfo { get; set; } = default!;
    public string? ParametersSchema { get; set; }
    public bool RequiresAuthorization { get; set; }
    public string? RequiredPermissionName { get; set; }
}

public class McpToolProvider : ISingletonDependency
{
    private readonly Dictionary<string, McpToolDescriptor> _tools = new();

    public void RegisterTool(McpToolDescriptor descriptor)
    {
        _tools[descriptor.ToolName] = descriptor;
    }

    public IReadOnlyDictionary<string, McpToolDescriptor> GetTools() => _tools;

    public McpToolDescriptor? GetTool(string toolName) => _tools.GetValueOrDefault(toolName);

    public static string GenerateParametersSchema(MethodInfo method)
    {
        var parameters = method.GetParameters()
            .Where(p => p.ParameterType != typeof(CancellationToken))
            .Select(p => new
            {
                name = p.Name,
                type = p.ParameterType.Name.ToLower(),
                description = p.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>()?.Description
            });

        return JsonSerializer.Serialize(new
        {
            type = "object",
            properties = parameters.ToDictionary(p => p.name!, p => (object)new { type = p.type, description = p.description })
        });
    }
}
