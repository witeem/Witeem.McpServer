using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpWorkspaceConfig : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string WorkspaceName { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string WorkspaceType { get; set; }

    [Required]
    [StringLength(50)]
    public virtual string Provider { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string ModelName { get; set; }

    [StringLength(500)]
    public virtual string? Endpoint { get; set; }

    [Required]
    [StringLength(500)]
    public virtual string ApiKeyEncrypted { get; set; }

    [StringLength(100)]
    public virtual string? DeploymentName { get; set; }

    [StringLength(50)]
    public virtual string? ApiVersion { get; set; }

    public virtual int? MaxTokens { get; set; }

    public virtual decimal? Temperature { get; set; }

    public virtual decimal? TopP { get; set; }

    public virtual string? SystemPrompt { get; set; }

    public virtual bool IsDefault { get; set; }

    public virtual bool Enabled { get; set; } = true;
}
