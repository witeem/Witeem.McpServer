using AutoMapper;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.McpServer.Application.McpTools;

public class McpServerApplicationProfile : Profile
{
    public McpServerApplicationProfile()
    {
        CreateMap<McpToolCall, McpToolCallDto>();
    }
}
