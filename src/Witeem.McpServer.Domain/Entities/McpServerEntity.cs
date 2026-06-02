using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpServerEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string ServerName { get; set; }

    [Required]
    [StringLength(200)]
    public virtual string DisplayName { get; set; }

    [StringLength(500)]
    public virtual string? Description { get; set; }

    [Required]
    [StringLength(500)]
    public virtual string Endpoint { get; set; }

    [Required]
    [StringLength(20)]
    public virtual string TransportType { get; set; }

    [StringLength(50)]
    public virtual string AuthType { get; set; } = "None";

    [StringLength(256)]
    public virtual string? ApiKey { get; set; }

    public virtual bool Enabled { get; set; } = true;

    [StringLength(500)]
    public virtual string? HealthCheckEndpoint { get; set; }

    [StringLength(500)]
    public virtual string? Tags { get; set; }
}
