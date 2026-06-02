using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Witeem.McpServer.EntityFrameworkCore;

[ConnectionStringName("Default")]
public class WiteemMcpServerDbContext : AbpDbContext<WiteemMcpServerDbContext>
{
    public WiteemMcpServerDbContext(DbContextOptions<WiteemMcpServerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}