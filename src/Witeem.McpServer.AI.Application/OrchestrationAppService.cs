using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Witeem.McpServer.AI.Application;

public class OrchestrateRequestDto
{
    public string Prompt { get; set; }
    public string? WorkspaceName { get; set; }
    public string[]? ToolNames { get; set; }
}

public class OrchestrateResponseDto
{
    public string Response { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class OrchestrationAppService : ApplicationService, IOrchestrationAppService
{
    public async Task<OrchestrateResponseDto> OrchestrateAsync(OrchestrateRequestDto request)
    {
        await Task.CompletedTask;
        return new OrchestrateResponseDto
        {
            Response = "AI Orchestration is not yet configured. Please set up a workspace with a valid AI provider.",
            Success = false,
            ErrorMessage = "No AI provider configured"
        };
    }
}
