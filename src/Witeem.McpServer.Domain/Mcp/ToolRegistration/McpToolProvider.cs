using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Default <see cref="IMcpToolProvider"/> backed by an immutable snapshot.
/// The volatile field guarantees that readers observe a fully populated registry.
/// </summary>
public class McpToolProvider : IMcpToolProvider, ISingletonDependency
{
    private volatile ImmutableDictionary<string, McpToolDescriptor> _tools =
        ImmutableDictionary<string, McpToolDescriptor>.Empty;

    public event Action? RegistryChanged;

    public bool TryGetTool(string toolName, out McpToolDescriptor? descriptor)
    {
        var snapshot = _tools;
        return snapshot.TryGetValue(toolName, out descriptor);
    }

    public IReadOnlyList<McpToolDescriptor> GetSnapshot() => _tools.Values.ToImmutableList();

    public void ReplaceAll(IEnumerable<McpToolDescriptor> descriptors)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, McpToolDescriptor>(StringComparer.Ordinal);
        foreach (var descriptor in descriptors)
        {
            if (!builder.TryAdd(descriptor.ToolName, descriptor))
            {
                throw new AbpException(
                    $"Duplicate MCP tool name '{descriptor.ToolName}' detected during registration. " +
                    "Tool names must be unique across all scanned assemblies.");
            }
        }

        _tools = builder.ToImmutable();
        RegistryChanged?.Invoke();
    }
}
