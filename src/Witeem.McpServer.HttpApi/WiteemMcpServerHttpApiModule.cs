using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Witeem.McpServer.Application.Contracts;

namespace Witeem.McpServer.HttpApi;

[DependsOn(
    typeof(WiteemMcpServerApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class WiteemMcpServerHttpApiModule : AbpModule
{
}
