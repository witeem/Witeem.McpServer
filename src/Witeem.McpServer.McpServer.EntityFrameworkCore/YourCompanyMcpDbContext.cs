using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.McpServer.EntityFrameworkCore;

[ConnectionStringName("McpServer")]
public class YourCompanyMcpDbContext : AbpDbContext<YourCompanyMcpDbContext>
{
    public DbSet<McpTool> McpTools { get; set; }
    public DbSet<McpToolCall> McpToolCalls { get; set; }
    public DbSet<McpToolCallStat> McpToolCallStats { get; set; }
    public DbSet<McpPromptTemplate> McpPromptTemplates { get; set; }
    public DbSet<McpRateLimitPolicy> McpRateLimitPolicies { get; set; }
    public DbSet<McpServerEntity> McpServers { get; set; }
    public DbSet<McpWorkspaceConfig> McpWorkspaceConfigs { get; set; }

    public YourCompanyMcpDbContext(DbContextOptions<YourCompanyMcpDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureMcpServer();
    }
}
