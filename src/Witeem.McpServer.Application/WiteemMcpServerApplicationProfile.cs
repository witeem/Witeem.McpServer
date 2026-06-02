using AutoMapper;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.Application;

public class WiteemMcpServerApplicationProfile : Profile
{
    public WiteemMcpServerApplicationProfile()
    {
        CreateMap<McpTool, McpToolDto>();
        CreateMap<McpToolCreateUpdateDto, McpTool>();
        CreateMap<McpToolCall, McpToolCallDto>();
    }
}
