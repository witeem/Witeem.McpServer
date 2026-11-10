using System;
using System.Collections.Generic;
using System.Reflection;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Configures which assemblies are scanned for <see cref="McpToolAttribute"/>-annotated
/// methods at startup. Hosts register their tool assemblies via
/// <c>Configure&lt;McpToolOptions&gt;</c>; nothing is scanned when the list is empty.
/// </summary>
public class McpToolOptions
{
    public IList<Assembly> ToolAssemblies { get; } = [];
}
