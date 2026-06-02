using System;
using Volo.Abp.Application.Dtos;

namespace Witeem.McpServer.Application.Contracts.Dtos;

public class McpToolCallDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ToolId { get; set; }
    public string ToolName { get; set; }
    public string? CallerInfo { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? SessionId { get; set; }
    public string? RequestId { get; set; }
    public string? Parameters { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int ExecutionDurationMs { get; set; }
    public int? StatusCode { get; set; }
    public bool Success { get; set; }
    public DateTime CallTime { get; set; }
}

public class GetMcpToolCallsInput : PagedAndSortedResultRequestDto
{
    public Guid? ToolId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public bool? Success { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
