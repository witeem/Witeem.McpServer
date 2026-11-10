using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Witeem.McpServer.Domain.Mcp.ToolRegistration;

/// <summary>
/// Discovers MCP tools in assemblies. Splitting <see cref="Scan"/> (pure reflection)
/// from <see cref="EnrichFromDatabaseAsync"/> (persistence) keeps startup resilient:
/// when the database is unavailable, un-enriched descriptors can still be registered.
/// </summary>
public interface IMcpToolScanner
{
    /// <summary>Pure reflection scan; performs no I/O.</summary>
    IReadOnlyList<McpToolDescriptor> Scan(Assembly assembly);

    /// <summary>
    /// Resolves tool ids and enabled flags from persisted entities, upserting
    /// entities for newly discovered tools. Requires an active unit of work.
    /// </summary>
    Task EnrichFromDatabaseAsync(IList<McpToolDescriptor> descriptors);
}
