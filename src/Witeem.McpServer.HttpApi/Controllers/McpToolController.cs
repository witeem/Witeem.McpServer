using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.McpServer.ToolInvocation;

namespace Witeem.McpServer.HttpApi.Controllers;

public class McpToolController : AbpControllerBase
{
    private readonly IMcpToolAppService _toolAppService;

    public McpToolController(IMcpToolAppService toolAppService)
    {
        _toolAppService = toolAppService;
    }

    [HttpGet("api/mcp/tools")]
    public Task<PagedResultDto<McpToolDto>> GetListAsync(GetMcpToolsInput input)
    {
        return _toolAppService.GetListAsync(input);
    }

    [HttpGet("api/mcp/tools/{id}")]
    public Task<McpToolDto> GetAsync(Guid id)
    {
        return _toolAppService.GetAsync(id);
    }

    [HttpPost("api/mcp/tools")]
    public Task<McpToolDto> CreateAsync(McpToolCreateUpdateDto input)
    {
        return _toolAppService.CreateAsync(input);
    }

    [HttpPut("api/mcp/tools/{id}")]
    public Task<McpToolDto> UpdateAsync(Guid id, McpToolCreateUpdateDto input)
    {
        return _toolAppService.UpdateAsync(id, input);
    }

    [HttpDelete("api/mcp/tools/{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _toolAppService.DeleteAsync(id);
    }
}

public class McpToolCallController : AbpControllerBase
{
    private readonly IMcpToolCallAppService _toolCallAppService;
    private readonly McpToolExecutor _toolExecutor;

    public McpToolCallController(IMcpToolCallAppService toolCallAppService, McpToolExecutor toolExecutor)
    {
        _toolCallAppService = toolCallAppService;
        _toolExecutor = toolExecutor;
    }

    [HttpGet("api/mcp/tool-calls")]
    public Task<PagedResultDto<McpToolCallDto>> GetListAsync(GetMcpToolCallsInput input)
    {
        return _toolCallAppService.GetListAsync(input);
    }

    [HttpPost("api/mcp/execute/{toolName}")]
    public async Task<McpToolCallResult> ExecuteAsync(string toolName, [FromBody] ExecuteToolRequest request)
    {
        return await _toolExecutor.ExecuteAsync(toolName, request.Parameters, request.SessionId, request.RequestId);
    }
}

public class ExecuteToolRequest
{
    public string Parameters { get; set; }
    public string? SessionId { get; set; }
    public string? RequestId { get; set; }
}
