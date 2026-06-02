using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Witeem.McpServer.Application;
using Witeem.McpServer.EntityFrameworkCore;
using Witeem.McpServer.HttpApi;
using Witeem.McpServer.McpServer.EntityFrameworkCore;
using Witeem.McpServer.McpServer.Application;
using Witeem.McpServer.AI.Application;

namespace Witeem.McpServer;

[DependsOn(
    typeof(WiteemMcpServerApplicationModule),
    typeof(WiteemMcpServerHttpApiModule),
    typeof(WiteemMcpServerMcpServerApplicationModule),
    typeof(WiteemMcpServerAIApplicationModule),
    typeof(WiteemMcpServerMcpServerEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule)
)]
public class WiteemMcpServerHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(WiteemMcpServerApplicationModule).Assembly, opts =>
            {
                opts.RootPath = "api/app";
            });
            options.ConventionalControllers.Create(typeof(WiteemMcpServerMcpServerApplicationModule).Assembly, opts =>
            {
                opts.RootPath = "api/mcp";
            });
            options.ConventionalControllers.Create(typeof(WiteemMcpServerAIApplicationModule).Assembly, opts =>
            {
                opts.RootPath = "api/ai";
            });
        });

        context.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Witeem MCP Server API", Version = "v1" });
            options.DocInclusionPredicate((docName, description) => true);
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(configuration["App:CorsOrigins"]?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.RemovePostFix("/")).ToArray() ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Witeem MCP Server API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine("Logs", "logs.txt"))
            .CreateLogger();

        try
        {
            Log.Information("Starting Witeem.McpServer.HttpApi.Host");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseAutofac();
            builder.Services.AddApplication<WiteemMcpServerHttpApiHostModule>();
            var app = builder.Build();
            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
