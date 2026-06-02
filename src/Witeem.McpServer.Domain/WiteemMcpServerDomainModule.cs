using Volo.Abp.Modularity;
using Witeem.McpServer.Domain.Shared;

namespace Witeem.McpServer.Domain;

[DependsOn(
    typeof(WiteemMcpServerDomainSharedModule)
)]
public class WiteemMcpServerDomainModule : AbpModule
{
}
