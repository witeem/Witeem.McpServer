using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.AI;

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
            ObjectMapper.Map<System.Collections.Generic.List<McpWorkspaceConfig>, System.Collections.Generic.List<McpWorkspaceConfigDto>>(items));
    }
}

public interface IMcpWorkspaceConfigAppService : IApplicationService
{
    Task<PagedResultDto<McpWorkspaceConfigDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}

public class McpWorkspaceConfigDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string WorkspaceName { get; set; }
    public string WorkspaceType { get; set; }
    public string Provider { get; set; }
    public string ModelName { get; set; }
    public string? Endpoint { get; set; }
    public string? DeploymentName { get; set; }
    public string? ApiVersion { get; set; }
    public int? MaxTokens { get; set; }
    public decimal? Temperature { get; set; }
    public decimal? TopP { get; set; }
    public string? SystemPrompt { get; set; }
    public bool IsDefault { get; set; }
    public bool Enabled { get; set; }
}
