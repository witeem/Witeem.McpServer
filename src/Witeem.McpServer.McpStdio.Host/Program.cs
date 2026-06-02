using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.Modularity;
using Witeem.McpServer.McpServer.Application;

namespace Witeem.McpServer;

[DependsOn(
    typeof(WiteemMcpServerMcpServerApplicationModule)
)]
public class McpStdioHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
    }
}

public class McpStdioProgram
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddApplication<McpStdioHostModule>();
        var app = builder.Build();
        await app.InitializeApplicationAsync();
        await app.RunAsync();
    }
}
