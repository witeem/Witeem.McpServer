using System;
using System.Text.Json;
using Shouldly;
using Witeem.McpServer.Domain.Mcp.ToolRegistration;
using Xunit;

namespace Witeem.McpServer.Tests;

/// <summary>Sample tool signatures used to exercise schema generation.</summary>
public class SchemaSampleMethods
{
    public enum Priority { Low, High }

    public void Sample(int count, Guid correlationId, string? label, Priority priority) { }
}

public class McpToolSchemaGeneratorTests
{
    private static JsonDocument GenerateSchema()
    {
        var method = typeof(SchemaSampleMethods).GetMethod(nameof(SchemaSampleMethods.Sample))!;
        return JsonDocument.Parse(McpToolSchemaGenerator.Generate(method));
    }

    [Fact]
    public void Schema_Should_Use_Valid_JsonSchema_Types()
    {
        using var document = GenerateSchema();
        var root = document.RootElement;

        root.GetProperty("type").GetString().ShouldBe("object");
        var properties = root.GetProperty("properties");
        properties.GetProperty("count").GetProperty("type").GetString().ShouldBe("integer");
        properties.GetProperty("correlationId").GetProperty("type").GetString().ShouldBe("string");
        properties.GetProperty("correlationId").GetProperty("format").GetString().ShouldBe("uuid");
        properties.GetProperty("priority").GetProperty("type").GetString().ShouldBe("string");
        properties.GetProperty("priority").GetProperty("enum").GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public void Schema_Should_Mark_Only_NonNullable_Required_Parameters_As_Required()
    {
        using var document = GenerateSchema();
        var required = document.RootElement.GetProperty("required").EnumerateArray();

        required.ShouldContain(x => x.GetString() == "count");
        required.ShouldContain(x => x.GetString() == "correlationId");
        required.ShouldNotContain(x => x.GetString() == "label");
    }
}
