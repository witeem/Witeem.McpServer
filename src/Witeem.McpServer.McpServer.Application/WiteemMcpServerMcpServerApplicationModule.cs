using Volo.Abp.Modularity;

namespace Witeem.McpServer.McpServer.Application;

[DependsOn(
    typeof(WiteemMcpServerMcpServerModule)
)]
public class WiteemMcpServerMcpServerApplicationModule : AbpModule
{
}
