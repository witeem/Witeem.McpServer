using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Domain.Entities;
using Witeem.McpServer.McpServer.Application.McpTools.Dtos;

namespace Witeem.McpServer.McpServer.Application.McpTools;

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
            ObjectMapper.Map<System.Collections.Generic.List<McpToolCall>, System.Collections.Generic.List<McpToolCallDto>>(items));
    }
}

public interface IMcpToolCallAppService : IApplicationService
{
    Task<PagedResultDto<McpToolCallDto>> GetListAsync(GetMcpToolCallsInput input);
}

public class McpToolCallDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public Guid ToolId { get; set; }
    public string ToolName { get; set; }
    public string? CallerInfo { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string? SessionId { get; set; }
    public string? RequestId { get; set; }
    public string? Parameters { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public int ExecutionDurationMs { get; set; }
    public int? StatusCode { get; set; }
    public bool Success { get; set; }
    public DateTime CallTime { get; set; }
}
