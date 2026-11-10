using System;
using Volo.Abp.Application.Dtos;

namespace Witeem.McpServer.Application.Contracts.Dtos;

public class McpToolDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string ToolName { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string AssemblyFullName { get; set; } = default!;
    public string ClassName { get; set; } = default!;
    public string MethodName { get; set; } = default!;
    public string? ParametersSchema { get; set; }
    public string? ReturnSchema { get; set; }
    public bool Enabled { get; set; }
    public bool RequiresAuthorization { get; set; }
    public string? RequiredPermissionName { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public bool IsBuiltIn { get; set; }
}

public class McpToolCreateUpdateDto
{
    public string ToolName { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string AssemblyFullName { get; set; } = default!;
    public string ClassName { get; set; } = default!;
    public string MethodName { get; set; } = default!;
    public string? ParametersSchema { get; set; }
    public string? ReturnSchema { get; set; }
    public bool Enabled { get; set; } = true;
    public bool RequiresAuthorization { get; set; }
    public string? RequiredPermissionName { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public bool IsBuiltIn { get; set; }
}

public class GetMcpToolsInput : PagedAndSortedResultRequestDto
{
    public string? ToolName { get; set; }
    public bool? Enabled { get; set; }
}
