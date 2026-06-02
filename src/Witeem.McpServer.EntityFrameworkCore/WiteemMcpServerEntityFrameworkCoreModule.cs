using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Witeem.McpServer.Domain;

namespace Witeem.McpServer.EntityFrameworkCore;

[DependsOn(
    typeof(WiteemMcpServerDomainModule),
    typeof(AbpEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule)
)]
public class WiteemMcpServerEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<WiteemMcpServerDbContext>();
    }
}