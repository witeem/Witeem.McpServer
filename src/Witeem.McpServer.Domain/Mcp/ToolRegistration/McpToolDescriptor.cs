using System;
using System.Reflection;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Immutable description of a discoverable MCP tool. Instances are shared across
/// threads from the singleton registry, therefore all properties are init-only.
/// </summary>
public sealed record McpToolDescriptor
{
    /// <summary>Identifier of the persisted tool entity (empty when the entity is missing).</summary>
    public Guid ToolId { get; init; }

    public string ToolName { get; init; } = default!;

    public string Description { get; init; } = default!;

    /// <summary>Service type resolved from DI to obtain the target instance.</summary>
    public Type ServiceType { get; init; } = default!;

    public MethodInfo MethodInfo { get; init; } = default!;

    /// <summary>JSON schema describing tool parameters (MCP clients rely on it).</summary>
    public string? ParametersSchema { get; init; }

    public bool RequiresAuthorization { get; init; }

    public string? RequiredPermissionName { get; init; }

    /// <summary>Runtime switch synced from the tool entity; disabled tools are rejected.</summary>
    public bool Enabled { get; init; } = true;
}
