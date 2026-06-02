using Volo.Abp.Modularity;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Domain;
using Witeem.McpServer.McpServer;

namespace Witeem.McpServer.AI;

[DependsOn(
    typeof(WiteemMcpServerDomainModule),
    typeof(WiteemMcpServerMcpServerModule),
    typeof(WiteemMcpServerApplicationContractsModule)
)]
public class WiteemMcpServerAIModule : AbpModule
{
}
