using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Witeem.McpServer.AI.Application;

public interface IOrchestrationAppService : IApplicationService
{
    Task<OrchestrateResponseDto> OrchestrateAsync(OrchestrateRequestDto request);
}
