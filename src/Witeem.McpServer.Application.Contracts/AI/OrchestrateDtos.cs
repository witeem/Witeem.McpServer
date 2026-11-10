namespace Witeem.McpServer.Application.Contracts.Dtos;

public class OrchestrateRequestDto
{
    public string Prompt { get; set; } = default!;
    public string? WorkspaceName { get; set; }
    public string[]? ToolNames { get; set; }
}

public class OrchestrateResponseDto
{
    public string Response { get; set; } = default!;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
