using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Modularity;
using Witeem.McpServer.Domain;
using Witeem.McpServer.EntityFrameworkCore;
using Witeem.McpServer.McpServer;

namespace Witeem.McpServer.McpServer.EntityFrameworkCore;

[DependsOn(
    typeof(WiteemMcpServerEntityFrameworkCoreModule),
    typeof(WiteemMcpServerMcpServerModule)
)]
public class WiteemMcpServerMcpServerEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<YourCompanyMcpDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}
