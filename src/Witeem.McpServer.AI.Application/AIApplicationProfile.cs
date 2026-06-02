using AutoMapper;
using Witeem.McpServer.AI;

namespace Witeem.McpServer.AI.Application;

public class AIApplicationProfile : Profile
{
    public AIApplicationProfile()
    {
        CreateMap<Domain.Entities.McpWorkspaceConfig, McpWorkspaceConfigDto>();
    }
}
