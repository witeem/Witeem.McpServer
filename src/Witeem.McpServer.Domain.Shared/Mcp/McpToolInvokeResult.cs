using System;

namespace Witeem.McpServer.Domain.Shared.Mcp;

/// <summary>
/// The result of invoking an MCP tool. A shared contract consumed by
/// the executor, application services and HTTP/stdio entry points.
/// </summary>
public sealed class McpToolInvokeResult
{
    public bool Success { get; init; }

    /// <summary><see cref="McpToolFailureReason.None"/> when successful.</summary>
    public McpToolFailureReason FailureReason { get; init; }

    /// <summary>Identifier of the tool entity backing the invocation (empty when the tool is unknown).</summary>
    public Guid ToolId { get; init; }

    /// <summary>Serialized tool output (camelCase JSON), or null when nothing was returned.</summary>
    public string? ResultJson { get; init; }

    public string? ErrorMessage { get; init; }

    public int ExecutionDurationMs { get; init; }

    public static McpToolInvokeResult Failed(McpToolFailureReason reason, Guid toolId, string errorMessage) =>
        new()
        {
            Success = false,
            FailureReason = reason,
            ToolId = toolId,
            ErrorMessage = errorMessage
        };

    public static McpToolInvokeResult Succeeded(Guid toolId, string? resultJson, int durationMs) =>
        new()
        {
            Success = true,
            FailureReason = McpToolFailureReason.None,
            ToolId = toolId,
            ResultJson = resultJson,
            ExecutionDurationMs = durationMs
        };
}
