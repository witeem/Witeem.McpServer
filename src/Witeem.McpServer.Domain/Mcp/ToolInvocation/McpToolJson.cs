using System.Text.Json;

namespace Witeem.McpServer.Domain.Mcp.ToolInvocation;

/// <summary>
/// Shared JSON serializer settings for MCP tool payloads: camelCase properties
/// and case-insensitive binding, matching MCP client expectations.
/// </summary>
public static class McpToolJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
