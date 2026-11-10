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

namespace Witeem.McpServer.Application;

/// <summary>
/// Consolidated tool-call query service. Previously duplicated in both the main
/// Application project and the nested McpServer.Application project.
/// </summary>
public class McpToolCallAppService : ApplicationService, IMcpToolCallAppService
{
    private readonly IRepository<McpToolCall, Guid> _repository;

    public McpToolCallAppService(IRepository<McpToolCall, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<McpToolCallDto>> GetListAsync(GetMcpToolCallsInput input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable
            .WhereIf(input.ToolId.HasValue, x => x.ToolId == input.ToolId!.Value)
            .WhereIf(input.TenantId.HasValue, x => x.TenantId == input.TenantId!.Value)
            .WhereIf(input.UserId.HasValue, x => x.UserId == input.UserId!.Value)
            .WhereIf(input.Success.HasValue, x => x.Success == input.Success!.Value)
            .WhereIf(input.StartTime.HasValue, x => x.CallTime >= input.StartTime!.Value)
            .WhereIf(input.EndTime.HasValue, x => x.CallTime <= input.EndTime!.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(
            query.OrderByDescending(x => x.CallTime).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<McpToolCallDto>(totalCount,
            ObjectMapper.Map<List<McpToolCall>, List<McpToolCallDto>>(items));
    }

    public async Task<McpToolCallDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<McpToolCall, McpToolCallDto>(entity);
    }
}
