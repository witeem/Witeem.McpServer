using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Witeem.McpServer.Domain.Entities;

public class McpToolCall : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid ToolId { get; set; }

    [StringLength(100)]
    public virtual string ToolName { get; set; }

    [StringLength(500)]
    public virtual string? CallerInfo { get; set; }

    public virtual Guid? UserId { get; set; }

    [StringLength(256)]
    public virtual string? UserName { get; set; }

    [StringLength(100)]
    public virtual string? SessionId { get; set; }

    [StringLength(100)]
    public virtual string? RequestId { get; set; }

    public virtual string? Parameters { get; set; }

    public virtual string? Result { get; set; }

    public virtual string? ErrorMessage { get; set; }

    public virtual int ExecutionDurationMs { get; set; }

    public virtual int? StatusCode { get; set; }

    public virtual bool Success { get; set; }

    public virtual DateTime CallTime { get; set; }
}
