using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Domain.Entities;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Scans an assembly for <see cref="McpToolAttribute"/>-annotated methods and
/// produces <see cref="McpToolDescriptor"/>s. Database enrichment (tool ids,
/// enabled flags, entity upsert) is a separate step so scanning stays resilient
/// to database outages.
/// </summary>
public class McpToolScanner : ITransientDependency, IMcpToolScanner
{
    private readonly IRepository<McpTool, Guid> _toolRepository;

    public McpToolScanner(IRepository<McpTool, Guid> toolRepository)
    {
        _toolRepository = toolRepository;
    }

    public IReadOnlyList<McpToolDescriptor> Scan(Assembly assembly)
    {
        Check.NotNull(assembly, nameof(assembly));

        var toolMethods = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Select(m => (Method: m, Attribute: m.GetCustomAttribute<McpToolAttribute>()))
            .Where(x => x.Attribute != null);

        var descriptors = new List<McpToolDescriptor>();
        foreach (var (method, attribute) in toolMethods)
        {
            var classType = method.DeclaringType!;
            var toolName = attribute!.Name ?? $"{classType.Name.RemovePostFix("Tool").ToLowerInvariant()}_{method.Name.RemovePostFix("Async")}";
            var description = attribute.Description ?? $"Tool: {method.Name}";

            var authorize = method.GetCustomAttribute<AuthorizeAttribute>();
            descriptors.Add(new McpToolDescriptor
            {
                ToolName = toolName,
                Description = description,
                ServiceType = classType,
                MethodInfo = method,
                ParametersSchema = McpToolSchemaGenerator.Generate(method),
                RequiresAuthorization = authorize != null,
                RequiredPermissionName = authorize?.Policy,
                Enabled = true
            });
        }

        return descriptors;
    }

    public async Task EnrichFromDatabaseAsync(IList<McpToolDescriptor> descriptors)
    {
        if (descriptors.Count == 0)
        {
            return;
        }

        var names = descriptors.Select(d => d.ToolName).Distinct().ToList();
        var entities = await _toolRepository.GetListAsync(t => names.Contains(t.ToolName));
        var byName = entities.ToDictionary(e => e.ToolName, StringComparer.Ordinal);

        for (var i = 0; i < descriptors.Count; i++)
        {
            var descriptor = descriptors[i];
            if (!byName.TryGetValue(descriptor.ToolName, out var entity))
            {
                entity = new McpTool
                {
                    ToolName = descriptor.ToolName,
                    DisplayName = descriptor.ToolName,
                    Description = descriptor.Description.Truncate(500)!,
                    AssemblyFullName = descriptor.ServiceType.Assembly.FullName!,
                    ClassName = descriptor.ServiceType.FullName!,
                    MethodName = descriptor.MethodInfo.Name,
                    ParametersSchema = descriptor.ParametersSchema.Truncate(500),
                    RequiresAuthorization = descriptor.RequiresAuthorization,
                    RequiredPermissionName = descriptor.RequiredPermissionName,
                    IsBuiltIn = true,
                    Enabled = true
                };
                await _toolRepository.InsertAsync(entity);
                byName.Add(descriptor.ToolName, entity);
            }

            descriptors[i] = descriptor with
            {
                ToolId = entity.Id,
                Enabled = entity.Enabled
            };
        }
    }
}
