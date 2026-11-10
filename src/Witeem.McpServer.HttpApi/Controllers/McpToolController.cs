using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Witeem.McpServer.Application.Contracts;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.Domain.Shared.Mcp;

namespace Witeem.McpServer.HttpApi.Controllers;

public class McpToolController : AbpControllerBase
{
    private readonly IMcpToolAppService _toolAppService;

    public McpToolController(IMcpToolAppService toolAppService)
    {
        _toolAppService = toolAppService;
    }

    [HttpGet("api/mcp/tools")]
    public Task<PagedResultDto<McpToolDto>> GetListAsync(GetMcpToolsInput input) =>
        _toolAppService.GetListAsync(input);

    [HttpGet("api/mcp/tools/{id}")]
    public Task<McpToolDto> GetAsync(Guid id) =>
        _toolAppService.GetAsync(id);

    [HttpPost("api/mcp/tools")]
    public Task<McpToolDto> CreateAsync(McpToolCreateUpdateDto input) =>
        _toolAppService.CreateAsync(input);

    [HttpPut("api/mcp/tools/{id}")]
    public Task<McpToolDto> UpdateAsync(Guid id, McpToolCreateUpdateDto input) =>
        _toolAppService.UpdateAsync(id, input);

    [HttpDelete("api/mcp/tools/{id}")]
    public Task DeleteAsync(Guid id) =>
        _toolAppService.DeleteAsync(id);
}

public class McpToolCallController : AbpControllerBase
{
    private readonly IMcpToolCallAppService _toolCallAppService;
    private readonly IMcpToolInvokeAppService _toolInvokeAppService;

    public McpToolCallController(
        IMcpToolCallAppService toolCallAppService,
        IMcpToolInvokeAppService toolInvokeAppService)
    {
        _toolCallAppService = toolCallAppService;
        _toolInvokeAppService = toolInvokeAppService;
    }

    [HttpGet("api/mcp/tool-calls")]
    public Task<PagedResultDto<McpToolCallDto>> GetListAsync(GetMcpToolCallsInput input) =>
        _toolCallAppService.GetListAsync(input);

    [HttpPost("api/mcp/execute")]
    public Task<McpToolInvokeResult> ExecuteAsync([FromBody] McpToolInvokeRequest request) =>
        _toolInvokeAppService.InvokeAsync(request);
}
