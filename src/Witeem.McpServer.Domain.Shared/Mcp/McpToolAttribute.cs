using System;

namespace Witeem.McpServer.Domain.Shared.Mcp;

/// <summary>
/// Marks a method as an MCP tool. Scanned at startup and registered into the tool registry.
/// Renamed from <c>McpServerToolAttribute</c> to avoid clashing with the official
/// ModelContextProtocol SDK attribute of the same name.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class McpToolAttribute : Attribute
{
    /// <summary>
    /// Explicit tool name. When omitted, a name is derived from the declaring type and method name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Tool description exposed to MCP clients. When omitted, a generic description is generated.
    /// </summary>
    public string? Description { get; }

    public McpToolAttribute(string? name = null, string? description = null)
    {
        Name = name;
        Description = description;
    }
}
