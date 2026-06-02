using System;
using Volo.Abp.Application.Dtos;

namespace Witeem.McpServer.McpServer.Application.McpTools.Dtos;

public class GetMcpToolCallsInput : PagedAndSortedResultRequestDto
{
    public Guid? ToolId { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? UserId { get; set; }
    public bool? Success { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
