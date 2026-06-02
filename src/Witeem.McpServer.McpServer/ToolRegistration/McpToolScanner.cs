using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.McpServer.ToolRegistration;

public class McpToolScanner : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly McpToolProvider _toolProvider;
    private readonly IRepository<McpTool, Guid> _toolRepository;

    public McpToolScanner(IServiceProvider serviceProvider, McpToolProvider toolProvider, IRepository<McpTool, Guid> toolRepository)
    {
        _serviceProvider = serviceProvider;
        _toolProvider = toolProvider;
        _toolRepository = toolRepository;
    }

    public async Task ScanAndRegisterAsync(Assembly assembly)
    {
        var toolMethods = assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(m => m.GetCustomAttribute<McpServerToolAttribute>() != null);

        foreach (var method in toolMethods)
        {
            var attribute = method.GetCustomAttribute<McpServerToolAttribute>()!;
            var classType = method.DeclaringType!;

            var toolName = attribute.Name ?? $"{classType.Name.Replace("Tool", "").ToLower()}_{method.Name.Replace("Async", "")}";
            var description = attribute.Description ?? $"Tool: {method.Name}";

            var toolEntity = await _toolRepository.FirstOrDefaultAsync(t => t.ToolName == toolName);
            var toolId = toolEntity?.Id ?? Guid.Empty;

            var descriptor = new McpToolDescriptor
            {
                ToolId = toolId,
                ToolName = toolName,
                Description = description,
                ServiceType = classType,
                MethodInfo = method,
                ParametersSchema = McpToolProvider.GenerateParametersSchema(method),
                RequiresAuthorization = method.GetCustomAttribute<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>() != null,
                RequiredPermissionName = method.GetCustomAttribute<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>()?.Policy
            };

            _toolProvider.RegisterTool(descriptor);
        }
    }
}
