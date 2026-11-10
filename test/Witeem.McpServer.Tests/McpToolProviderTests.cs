using System;
using Shouldly;
using Volo.Abp;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Xunit;

namespace Witeem.McpServer.Tests;

public class McpToolProviderTests
{
    private static McpToolDescriptor Descriptor(string name) => new()
    {
        ToolName = name,
        Description = $"Tool: {name}",
        ServiceType = typeof(object),
        MethodInfo = typeof(object).GetMethod(nameof(ToString))!
    };

    [Fact]
    public void ReplaceAll_Should_Publish_Atomic_Snapshot()
    {
        var provider = new McpToolProvider();
        var changed = 0;
        provider.RegistryChanged += () => changed++;

        provider.ReplaceAll([Descriptor("first"), Descriptor("second")]);

        provider.GetSnapshot().Count.ShouldBe(2);
        provider.TryGetTool("first", out var found).ShouldBeTrue();
        found!.ToolName.ShouldBe("first");
        changed.ShouldBe(1);
    }

    [Fact]
    public void ReplaceAll_Duplicate_Names_Should_Keep_Previous_Snapshot()
    {
        var provider = new McpToolProvider();
        provider.ReplaceAll([Descriptor("stable")]);

        Should.Throw<AbpException>(() => provider.ReplaceAll([Descriptor("dup"), Descriptor("dup")]));

        provider.GetSnapshot().Count.ShouldBe(1);
        provider.TryGetTool("stable", out _).ShouldBeTrue();
    }

    [Fact]
    public void TryGetTool_Should_Return_False_For_Unknown_Tool()
    {
        var provider = new McpToolProvider();
        provider.TryGetTool("missing", out var descriptor).ShouldBeFalse();
        descriptor.ShouldBeNull();
    }
}
