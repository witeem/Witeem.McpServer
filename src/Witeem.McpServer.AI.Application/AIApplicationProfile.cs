using AutoMapper;
using Witeem.McpServer.Application.Contracts.Dtos;
using Witeem.McpServer.Domain.Entities;

namespace Witeem.McpServer.AI.Application;

public class AIApplicationProfile : Profile
{
    public AIApplicationProfile()
    {
        CreateMap<McpWorkspaceConfig, McpWorkspaceConfigDto>();
    }
}
