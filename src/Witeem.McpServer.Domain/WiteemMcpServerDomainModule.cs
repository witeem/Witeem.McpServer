using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Witeem.McpServer.Domain.Shared;

namespace Witeem.McpServer.Domain;

[DependsOn(
    typeof(WiteemMcpServerDomainSharedModule)
)]
public class WiteemMcpServerDomainModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // Assembly entry point for the tool registry: scans the assemblies
        // configured via McpToolOptions and populates the registry.
        // Fault-tolerant by design (log-and-continue on database failures).
        var initializer = context.ServiceProvider.GetRequiredService<McpToolRegistryInitializer>();
        await initializer.InitializeAsync();
    }
}
