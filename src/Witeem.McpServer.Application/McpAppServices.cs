using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.Application;

public class McpToolAppService : ApplicationService, IMcpToolAppService
{
    private readonly IRepository<McpTool, Guid> _repository;

    public McpToolAppService(IRepository<McpTool, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PagedResultDto<McpToolDto>> GetListAsync(GetMcpToolsInput input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable
            .WhereIf(!input.ToolName.IsNullOrWhiteSpace(), x => x.ToolName.Contains(input.ToolName!))
            .WhereIf(input.Enabled.HasValue, x => x.Enabled == input.Enabled!.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);
        var items = await AsyncExecuter.ToListAsync(
            query.OrderBy(x => x.CreationTime).PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<McpToolDto>(totalCount, ObjectMapper.Map<System.Collections.Generic.List<McpTool>, System.Collections.Generic.List<McpToolDto>>(items));
    }

    public async Task<McpToolDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<McpTool, McpToolDto>(entity);
    }

    public async Task<McpToolDto> CreateAsync(McpToolCreateUpdateDto input)
    {
        var entity = ObjectMapper.Map<McpToolCreateUpdateDto, McpTool>(input);
        await _repository.InsertAsync(entity);
        return ObjectMapper.Map<McpTool, McpToolDto>(entity);
    }

    public async Task<McpToolDto> UpdateAsync(Guid id, McpToolCreateUpdateDto input)
    {
        var entity = await _repository.GetAsync(id);
        ObjectMapper.Map(input, entity);
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<McpTool, McpToolDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}

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

        return new PagedResultDto<McpToolCallDto>(totalCount, ObjectMapper.Map<System.Collections.Generic.List<McpToolCall>, System.Collections.Generic.List<McpToolCallDto>>(items));
    }

    public async Task<McpToolCallDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<McpToolCall, McpToolCallDto>(entity);
    }
}
