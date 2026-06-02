using Microsoft.EntityFrameworkCore;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.McpServer.EntityFrameworkCore;

public static class McpServerModelBuilderExtensions
{
    public static void ConfigureMcpServer(this ModelBuilder builder)
    {
        builder.Entity<McpTool>(b =>
        {
            b.ToTable("McpTools");
            b.Property(x => x.ToolName).HasMaxLength(100).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500).IsRequired();
            b.Property(x => x.AssemblyFullName).HasMaxLength(500).IsRequired();
            b.Property(x => x.ClassName).HasMaxLength(500).IsRequired();
            b.Property(x => x.MethodName).HasMaxLength(200).IsRequired();
            b.Property(x => x.ParametersSchema).HasMaxLength(500);
            b.Property(x => x.ReturnSchema).HasMaxLength(500);
            b.Property(x => x.RequiredPermissionName).HasMaxLength(128);
            b.HasIndex(x => new { x.ToolName, x.TenantId }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.Enabled).HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.TenantId);
        });

        builder.Entity<McpToolCall>(b =>
        {
            b.ToTable("McpToolCalls");
            b.Property(x => x.ToolName).HasMaxLength(100).IsRequired();
            b.Property(x => x.CallerInfo).HasMaxLength(500);
            b.Property(x => x.UserName).HasMaxLength(256);
            b.Property(x => x.SessionId).HasMaxLength(100);
            b.Property(x => x.RequestId).HasMaxLength(100);
            b.HasIndex(x => new { x.ToolId, x.CallTime });
            b.HasIndex(x => new { x.TenantId, x.CallTime });
            b.HasIndex(x => new { x.Success, x.CallTime });
            b.HasIndex(x => new { x.UserId, x.CallTime });
        });

        builder.Entity<McpToolCallStat>(b =>
        {
            b.ToTable("McpToolCallStats");
            b.Property(x => x.ToolName).HasMaxLength(100).IsRequired();
            b.HasIndex(x => new { x.ToolId, x.StatDate, x.StatHour }).IsUnique().HasFilter("[StatHour] IS NOT NULL");
            b.HasIndex(x => new { x.ToolId, x.StatDate }).IsUnique().HasFilter("[StatHour] IS NULL");
            b.HasIndex(x => x.StatDate);
        });

        builder.Entity<McpPromptTemplate>(b =>
        {
            b.ToTable("McpPromptTemplates");
            b.Property(x => x.TemplateName).HasMaxLength(100).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.Version).HasMaxLength(20).IsRequired();
            b.Property(x => x.VariablesSchema).HasMaxLength(500);
            b.Property(x => x.Category).HasMaxLength(100);
            b.Property(x => x.Tags).HasMaxLength(500);
            b.HasIndex(x => new { x.TemplateName, x.Version, x.TenantId }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => new { x.Category, x.IsDefault });
        });

        builder.Entity<McpRateLimitPolicy>(b =>
        {
            b.ToTable("McpRateLimitPolicies");
            b.Property(x => x.PolicyName).HasMaxLength(100).IsRequired();
            b.Property(x => x.TargetType).HasMaxLength(20).IsRequired();
            b.HasIndex(x => new { x.TargetType, x.TargetId }).HasFilter("[Enabled] = 1");
        });

        builder.Entity<McpServerEntity>(b =>
        {
            b.ToTable("McpServers");
            b.Property(x => x.ServerName).HasMaxLength(100).IsRequired();
            b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.Endpoint).HasMaxLength(500).IsRequired();
            b.Property(x => x.TransportType).HasMaxLength(20).IsRequired();
            b.Property(x => x.AuthType).HasMaxLength(50).HasDefaultValue("None");
            b.Property(x => x.ApiKey).HasMaxLength(256);
            b.Property(x => x.HealthCheckEndpoint).HasMaxLength(500);
            b.Property(x => x.Tags).HasMaxLength(500);
            b.HasIndex(x => new { x.ServerName, x.TenantId }).IsUnique().HasFilter("[IsDeleted] = 0");
            b.HasIndex(x => x.Enabled).HasFilter("[IsDeleted] = 0");
        });

        builder.Entity<McpWorkspaceConfig>(b =>
        {
            b.ToTable("McpWorkspaceConfigs");
            b.Property(x => x.WorkspaceName).HasMaxLength(100).IsRequired();
            b.Property(x => x.WorkspaceType).HasMaxLength(200).IsRequired();
            b.Property(x => x.Provider).HasMaxLength(50).IsRequired();
            b.Property(x => x.ModelName).HasMaxLength(100).IsRequired();
            b.Property(x => x.Endpoint).HasMaxLength(500);
            b.Property(x => x.ApiKeyEncrypted).HasMaxLength(500).IsRequired();
            b.Property(x => x.DeploymentName).HasMaxLength(100);
            b.Property(x => x.ApiVersion).HasMaxLength(50);
            b.HasIndex(x => new { x.TenantId, x.WorkspaceName });
            b.HasIndex(x => x.IsDefault);
        });
    }
}
