using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Witeem.McpServer.Application.Contracts.Dtos;

namespace Witeem.McpServer.Application.Contracts;

/// <summary>Manages AI workspace configurations (multi-tenant).</summary>
public interface IMcpWorkspaceConfigAppService : IApplicationService
{
    Task<PagedResultDto<McpWorkspaceConfigDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
