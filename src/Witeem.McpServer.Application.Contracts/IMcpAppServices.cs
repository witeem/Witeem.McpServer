using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Witeem.McpServer.Application.Contracts.Dtos;

namespace Witeem.McpServer.Application.Contracts;

public interface IMcpToolAppService : IApplicationService
{
    Task<PagedResultDto<McpToolDto>> GetListAsync(GetMcpToolsInput input);
    Task<McpToolDto> GetAsync(Guid id);
    Task<McpToolDto> CreateAsync(McpToolCreateUpdateDto input);
    Task<McpToolDto> UpdateAsync(Guid id, McpToolCreateUpdateDto input);
    Task DeleteAsync(Guid id);
}

public interface IMcpToolCallAppService : IApplicationService
{
    Task<PagedResultDto<McpToolCallDto>> GetListAsync(GetMcpToolCallsInput input);
    Task<McpToolCallDto> GetAsync(Guid id);
}
