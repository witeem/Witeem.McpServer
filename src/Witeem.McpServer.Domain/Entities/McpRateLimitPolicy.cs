using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpRateLimitPolicy : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    [Required]
    [StringLength(100)]
    public virtual string PolicyName { get; set; }

    [Required]
    [StringLength(20)]
    public virtual string TargetType { get; set; }

    public virtual Guid? TargetId { get; set; }

    public virtual int LimitRequests { get; set; }

    public virtual int PeriodSeconds { get; set; }

    public virtual bool Enabled { get; set; } = true;

    public virtual int Priority { get; set; }
}
