using System;
using System.Collections.Generic;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Thread-safe registry of scanned MCP tools with snapshot semantics.
/// Readers never observe partial updates: <see cref="ReplaceAll"/> swaps an
/// immutable snapshot atomically.
/// </summary>
public interface IMcpToolProvider
{
    /// <summary>Attempts to retrieve a tool descriptor by its exact name.</summary>
    bool TryGetTool(string toolName, out McpToolDescriptor? descriptor);

    /// <summary>Returns an immutable snapshot of all registered tools.</summary>
    IReadOnlyList<McpToolDescriptor> GetSnapshot();

    /// <summary>
    /// Atomically replaces the whole registry. Throws when the batch contains duplicate tool names;
    /// on failure the previous snapshot remains intact.
    /// </summary>
    void ReplaceAll(IEnumerable<McpToolDescriptor> descriptors);

    /// <summary>Raised after a successful snapshot replacement (hook for cache invalidation).</summary>
    event Action? RegistryChanged;
}
