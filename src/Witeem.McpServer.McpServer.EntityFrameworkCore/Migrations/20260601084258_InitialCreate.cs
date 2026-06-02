using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Witeem.McpServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "McpPromptTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TemplateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PromptText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VariablesSchema = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpPromptTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpRateLimitPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PolicyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LimitRequests = table.Column<int>(type: "int", nullable: false),
                    PeriodSeconds = table.Column<int>(type: "int", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpRateLimitPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpServers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AuthType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "None"),
                    ApiKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    HealthCheckEndpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpToolCalls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CallerInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutionDurationMs = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    CallTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpToolCalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpToolCallStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatHour = table.Column<int>(type: "int", nullable: true),
                    TotalCalls = table.Column<int>(type: "int", nullable: false),
                    SuccessCount = table.Column<int>(type: "int", nullable: false),
                    FailureCount = table.Column<int>(type: "int", nullable: false),
                    TotalDurationMs = table.Column<long>(type: "bigint", nullable: false),
                    MaxDurationMs = table.Column<int>(type: "int", nullable: false),
                    P50DurationMs = table.Column<int>(type: "int", nullable: true),
                    P95DurationMs = table.Column<int>(type: "int", nullable: true),
                    P99DurationMs = table.Column<int>(type: "int", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpToolCallStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpTools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToolName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AssemblyFullName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ClassName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MethodName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParametersSchema = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReturnSchema = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    RequiresAuthorization = table.Column<bool>(type: "bit", nullable: false),
                    RequiredPermissionName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RateLimitPerMinute = table.Column<int>(type: "int", nullable: true),
                    IsBuiltIn = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpTools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "McpWorkspaceConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkspaceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WorkspaceType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApiKeyEncrypted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DeploymentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApiVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaxTokens = table.Column<int>(type: "int", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TopP = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SystemPrompt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpWorkspaceConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_McpPromptTemplates_Category_IsDefault",
                table: "McpPromptTemplates",
                columns: new[] { "Category", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_McpPromptTemplates_TemplateName_Version_TenantId",
                table: "McpPromptTemplates",
                columns: new[] { "TemplateName", "Version", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_McpRateLimitPolicies_TargetType_TargetId",
                table: "McpRateLimitPolicies",
                columns: new[] { "TargetType", "TargetId" },
                filter: "[Enabled] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_McpServers_Enabled",
                table: "McpServers",
                column: "Enabled",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_McpServers_ServerName_TenantId",
                table: "McpServers",
                columns: new[] { "ServerName", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCalls_Success_CallTime",
                table: "McpToolCalls",
                columns: new[] { "Success", "CallTime" });

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCalls_TenantId_CallTime",
                table: "McpToolCalls",
                columns: new[] { "TenantId", "CallTime" });

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCalls_ToolId_CallTime",
                table: "McpToolCalls",
                columns: new[] { "ToolId", "CallTime" });

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCalls_UserId_CallTime",
                table: "McpToolCalls",
                columns: new[] { "UserId", "CallTime" });

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCallStats_StatDate",
                table: "McpToolCallStats",
                column: "StatDate");

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCallStats_ToolId_StatDate",
                table: "McpToolCallStats",
                columns: new[] { "ToolId", "StatDate" },
                unique: true,
                filter: "[StatHour] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_McpToolCallStats_ToolId_StatDate_StatHour",
                table: "McpToolCallStats",
                columns: new[] { "ToolId", "StatDate", "StatHour" },
                unique: true,
                filter: "[StatHour] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_McpTools_Enabled",
                table: "McpTools",
                column: "Enabled",
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_McpTools_TenantId",
                table: "McpTools",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_McpTools_ToolName_TenantId",
                table: "McpTools",
                columns: new[] { "ToolName", "TenantId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_McpWorkspaceConfigs_IsDefault",
                table: "McpWorkspaceConfigs",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_McpWorkspaceConfigs_TenantId_WorkspaceName",
                table: "McpWorkspaceConfigs",
                columns: new[] { "TenantId", "WorkspaceName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "McpPromptTemplates");

            migrationBuilder.DropTable(
                name: "McpRateLimitPolicies");

            migrationBuilder.DropTable(
                name: "McpServers");

            migrationBuilder.DropTable(
                name: "McpToolCalls");

            migrationBuilder.DropTable(
                name: "McpToolCallStats");

            migrationBuilder.DropTable(
                name: "McpTools");

            migrationBuilder.DropTable(
                name: "McpWorkspaceConfigs");
        }
    }
}
