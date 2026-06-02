using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpTool : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string ToolName { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string DisplayName { get; set; }

    [Required]
    [StringLength(500)]
    public virtual string Description { get; set; }

    [Required]
    [StringLength(500)]
    public virtual string AssemblyFullName { get; set; }

    [Required]
    [StringLength(500)]
    public virtual string ClassName { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string MethodName { get; set; }

    [StringLength(500)]
    public virtual string? ParametersSchema { get; set; }

    [StringLength(500)]
    public virtual string? ReturnSchema { get; set; }

    public virtual bool Enabled { get; set; } = true;

    public virtual bool RequiresAuthorization { get; set; }

    [StringLength(128)]
    public virtual string? RequiredPermissionName { get; set; }

    public virtual int? RateLimitPerMinute { get; set; }

    public virtual bool IsBuiltIn { get; set; }
}
