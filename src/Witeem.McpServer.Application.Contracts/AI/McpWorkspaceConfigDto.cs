using System;
using Volo.Abp.Application.Dtos;

namespace Witeem.McpServer.Application.Contracts.Dtos;

/// <summary>
/// AI workspace configuration DTO. The encrypted API key is deliberately
/// excluded so credentials never travel back to clients.
/// </summary>
public class McpWorkspaceConfigDto : EntityDto<Guid>
{
    public Guid? TenantId { get; set; }
    public string WorkspaceName { get; set; } = default!;
    public string WorkspaceType { get; set; } = default!;
    public string Provider { get; set; } = default!;
    public string ModelName { get; set; } = default!;
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
