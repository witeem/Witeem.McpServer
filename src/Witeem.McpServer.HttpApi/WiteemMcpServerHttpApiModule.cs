using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.McpServer;

namespace Witeem.McpServer.HttpApi;

[DependsOn(
    typeof(WiteemMcpServerApplicationContractsModule),
    typeof(WiteemMcpServerMcpServerModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class WiteemMcpServerHttpApiModule : AbpModule
{
}
