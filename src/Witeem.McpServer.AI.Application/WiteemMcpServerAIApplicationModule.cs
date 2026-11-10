using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Witeem.McpServer.Application;
using Witeem.McpServer.Application.Contracts;

namespace Witeem.McpServer.AI.Application;

[DependsOn(
    typeof(WiteemMcpServerApplicationModule),
    typeof(WiteemMcpServerApplicationContractsModule),
    typeof(AbpAutoMapperModule)
)]
public class WiteemMcpServerAIApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<WiteemMcpServerAIApplicationModule>();
        });
    }
}
