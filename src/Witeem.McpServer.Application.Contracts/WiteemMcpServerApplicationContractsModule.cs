using Volo.Abp.Modularity;
using Witeem.McpServer.Domain;
using Witeem.McpServer.Domain.Shared;

namespace Witeem.McpServer.Application.Contracts;

[DependsOn(
    typeof(WiteemMcpServerDomainSharedModule)
)]
public class WiteemMcpServerApplicationContractsModule : AbpModule
{
}
