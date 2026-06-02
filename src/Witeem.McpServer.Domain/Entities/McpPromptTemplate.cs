using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpPromptTemplate : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string TemplateName { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string DisplayName { get; set; }

    [StringLength(500)]
    public virtual string? Description { get; set; }

    [Required]
    [StringLength(20)]
    public virtual string Version { get; set; }

    [Required]
    public virtual string PromptText { get; set; }

    [StringLength(500)]
    public virtual string? VariablesSchema { get; set; }

    [StringLength(100)]
    public virtual string? Category { get; set; }

    [StringLength(500)]
    public virtual string? Tags { get; set; }

    public virtual bool IsDefault { get; set; }

    public virtual bool IsBuiltIn { get; set; }
}
