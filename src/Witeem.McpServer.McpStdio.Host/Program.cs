using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Witeem.McpServer.Domain;
using Witeem.McpServer.EntityFrameworkCore;

namespace Witeem.McpServer;

/// <summary>
/// Stdio host module. Depends on the domain engine (tool registry + invocation
/// pipeline) and the consolidated EF Core module (repositories/audit), without
/// pulling in the full HTTP application layer.
/// </summary>
[DependsOn(
    typeof(WiteemMcpServerDomainModule),
    typeof(WiteemMcpServerEntityFrameworkCoreModule),
    typeof(AbpAutofacModule)
)]
public class McpStdioHostModule : AbpModule
{
}

public class McpStdioProgram
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseAutofac();
        builder.Services.AddApplication<McpStdioHostModule>();
        var app = builder.Build();
        await app.InitializeApplicationAsync();
        await app.RunAsync();
    }
}
