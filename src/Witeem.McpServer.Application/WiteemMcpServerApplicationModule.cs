using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Domain;

namespace Witeem.McpServer.Application;

[DependsOn(
    typeof(WiteemMcpServerDomainModule),
    typeof(WiteemMcpServerApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class WiteemMcpServerApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WiteemMcpServerApplicationModule>();
        });
    }
}
