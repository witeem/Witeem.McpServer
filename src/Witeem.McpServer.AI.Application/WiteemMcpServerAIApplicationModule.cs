using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Witeem.McpServer.AI;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.McpServer.Application;

namespace Witeem.McpServer.AI.Application;

[DependsOn(
    typeof(WiteemMcpServerAIModule),
    typeof(WiteemMcpServerApplicationContractsModule),
    typeof(WiteemMcpServerMcpServerApplicationModule),
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
