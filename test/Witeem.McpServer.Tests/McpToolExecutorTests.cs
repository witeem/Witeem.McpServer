using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Witeem.McpServer.Domain.Entities;
using Witeem.McpServer.Domain.Mcp.ToolInvocation;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Witeem.McpServer.Domain.Shared.Mcp;
using Xunit;

namespace Witeem.McpServer.Tests;

public class SampleTools
{
    [McpTool("echo")]
    public string Echo(string message) => message;

    [McpTool("sum")]
    public int Sum(int a, int b) => a + b;

    [McpTool("async_echo")]
    public async Task<string> AsyncEcho(string message)
    {
        await Task.Yield();
        return message;
    }
}

public class McpToolExecutorTests
{
    private static McpToolExecutor CreateSut(bool requiresAuthorization = false)
    {
        var provider = new McpToolProvider();
        var scanner = new McpToolScanner(Substitute.For<IRepository<McpTool, Guid>>());
        var descriptors = scanner.Scan(typeof(SampleTools).Assembly)
            .Select(d => d with { RequiresAuthorization = requiresAuthorization })
            .ToList();
        provider.ReplaceAll(descriptors);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton<IMcpToolProvider>(provider);
        services.AddSingleton<McpToolInvocationPipeline>();
        services.AddTransient<IMcpToolInvocationMiddleware, McpToolAuditingMiddleware>();
        services.AddTransient<IMcpToolInvocationMiddleware, McpToolAuthorizationMiddleware>();
        services.AddTransient<IMcpToolInvocationMiddleware, McpToolArgumentBindingMiddleware>();
        services.AddSingleton(Substitute.For<IRepository<McpToolCall, Guid>>());
        services.AddSingleton(Substitute.For<ICurrentTenant>());
        services.AddSingleton<SampleTools>();
        services.AddSingleton<McpToolExecutor>();

        return services.BuildServiceProvider().GetRequiredService<McpToolExecutor>();
    }

    [Fact]
    public async Task Should_Invoke_Synchronous_Tool()
    {
        var result = await CreateSut().InvokeAsync(new McpToolInvokeRequest
        {
            ToolName = "echo",
            ParametersJson = "{\"message\":\"hello\"}"
        });

        result.Success.ShouldBeTrue();
        result.ResultJson.ShouldBe("\"hello\"");
        result.FailureReason.ShouldBe(McpToolFailureReason.None);
    }

    [Fact]
    public async Task Should_Bind_Parameters_Case_Insensitively()
    {
        var result = await CreateSut().InvokeAsync(new McpToolInvokeRequest
        {
            ToolName = "sum",
            ParametersJson = "{\"A\":2,\"B\":3}"
        });

        result.Success.ShouldBeTrue();
        result.ResultJson.ShouldBe("5");
    }

    [Fact]
    public async Task Should_Return_InvalidParameters_For_Malformed_Json()
    {
        var result = await CreateSut().InvokeAsync(new McpToolInvokeRequest
        {
            ToolName = "echo",
            ParametersJson = "{not-json"
        });

        result.Success.ShouldBeFalse();
        result.FailureReason.ShouldBe(McpToolFailureReason.InvalidParameters);
    }

    [Fact]
    public async Task Should_Return_ToolNotFound_For_Unknown_Tool()
    {
        var result = await CreateSut().InvokeAsync(new McpToolInvokeRequest { ToolName = "missing" });

        result.Success.ShouldBeFalse();
        result.FailureReason.ShouldBe(McpToolFailureReason.ToolNotFound);
    }

    [Fact]
    public async Task Should_Deny_Authorized_Tool_Without_Handler()
    {
        // Fail-closed: no authorization handler registered must never widen access.
        var result = await CreateSut(requiresAuthorization: true).InvokeAsync(new McpToolInvokeRequest
        {
            ToolName = "echo",
            ParametersJson = "{\"message\":\"hello\"}"
        });

        result.Success.ShouldBeFalse();
        result.FailureReason.ShouldBe(McpToolFailureReason.AuthorizationDenied);
        result.ResultJson.ShouldBeNull();
    }

    [Fact]
    public async Task Should_Invoke_Generic_Task_Tool()
    {
        var result = await CreateSut().InvokeAsync(new McpToolInvokeRequest
        {
            ToolName = "async_echo",
            ParametersJson = "{\"message\":\"async\"}"
        });

        result.Success.ShouldBeTrue();
        result.ResultJson.ShouldBe("\"async\"");
    }
}
