using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Application.Contracts.Dtos;

namespace Witeem.McpServer.AI.Application;

/// <summary>
/// Placeholder orchestration service. DTOs and the interface now live in
/// Application.Contracts, keeping the AI layer free of transport contracts.
/// </summary>
public class OrchestrationAppService : ApplicationService, IOrchestrationAppService
{
    public Task<OrchestrateResponseDto> OrchestrateAsync(OrchestrateRequestDto request)
    {
        return Task.FromResult(new OrchestrateResponseDto
        {
            Response = "AI Orchestration is not yet configured. Please set up a workspace with a valid AI provider.",
            Success = false,
            ErrorMessage = "No AI provider configured"
        });
    }
}
