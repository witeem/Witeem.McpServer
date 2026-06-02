using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpToolCallStat : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid ToolId { get; set; }

    public virtual string ToolName { get; set; }

    public virtual DateTime StatDate { get; set; }

    public virtual int? StatHour { get; set; }

    public virtual int TotalCalls { get; set; }

    public virtual int SuccessCount { get; set; }

    public virtual int FailureCount { get; set; }

    public virtual long TotalDurationMs { get; set; }

    public virtual int MaxDurationMs { get; set; }

    public virtual int? P50DurationMs { get; set; }

    public virtual int? P95DurationMs { get; set; }

    public virtual int? P99DurationMs { get; set; }

    public virtual DateTime CreationTime { get; set; }

    public virtual DateTime? LastModificationTime { get; set; }
}
