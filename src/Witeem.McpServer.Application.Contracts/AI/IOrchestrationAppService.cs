using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Witeem.McpServer.Application.Contracts.Dtos;

namespace Witeem.McpServer.Application.Contracts;

/// <summary>AI orchestration entry point (Semantic Kernel workflows).</summary>
public interface IOrchestrationAppService : IApplicationService
{
    Task<OrchestrateResponseDto> OrchestrateAsync(OrchestrateRequestDto request);
}
