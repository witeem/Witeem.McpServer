using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.AI.Application;

/// <summary>
/// Moved from the former Witeem.McpServer.AI module: an application service now
/// lives in the application layer, with its interface and DTO in Contracts.
/// </summary>
public class McpWorkspaceConfigAppService : ApplicationService, IMcpWorkspaceConfigAppService
{
    private readonly IRepository<McpWorkspaceConfig, Guid> _repository;

    public McpWorkspaceConfigAppService(IRepository<McpWorkspaceConfig, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<McpWorkspaceConfigDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable.Where(x => x.TenantId == CurrentTenant.Id);
        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(
            query.OrderBy(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));
        return new PagedResultDto<McpWorkspaceConfigDto>(totalCount,
            ObjectMapper.Map<List<McpWorkspaceConfig>, List<McpWorkspaceConfigDto>>(items));
    }
}
