以下是针对 MCP 服务平台设计的完整持久化数据表定义文件。



> \*\*说明\*\*：以下脚本基于 \*\*SQL Server\*\* 语法编写，可根据实际需要适配 PostgreSQL / MySQL。所有业务表均继承 ABP vNext 的 IMultiTenant 接口，通过 `TenantId` 字段实现多租户数据隔离。



\---



\## 一、核心业务表



\### 1. McpTools（MCP 工具注册表）



记录系统中所有已注册的 MCP 工具，支持自动扫描和手动管理两种方式。



```sql

\-- MCP 工具注册表

CREATE TABLE \[McpTools] (

&#x20;   \[Id]                       UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]                 UNIQUEIDENTIFIER NULL,              -- 租户ID（支持多租户）

&#x20;   \[ToolName]                 NVARCHAR(100)    NOT NULL,          -- 工具名称（唯一标识）

&#x20;   \[DisplayName]              NVARCHAR(200)    NOT NULL,          -- 显示名称

&#x20;   \[Description]              NVARCHAR(500)    NOT NULL,          -- 工具描述（用于 AI 理解）

&#x20;   \[AssemblyFullName]         NVARCHAR(500)    NOT NULL,          -- 所在程序集全名

&#x20;   \[ClassName]                NVARCHAR(500)    NOT NULL,          -- 工具类的完整名称

&#x20;   \[MethodName]               NVARCHAR(200)    NOT NULL,          -- 方法名称

&#x20;   \[ParametersSchema]         NVARCHAR(MAX)    NULL,              -- 参数 Schema（JSON格式）

&#x20;   \[ReturnSchema]             NVARCHAR(MAX)    NULL,              -- 返回值 Schema（JSON格式）

&#x20;   \[Enabled]                  BIT              NOT NULL DEFAULT 1, -- 是否启用

&#x20;   \[RequiresAuthorization]    BIT              NOT NULL DEFAULT 0, -- 是否需要授权

&#x20;   \[RequiredPermissionName]   NVARCHAR(128)    NULL,              -- 所需权限名称（如：Mcp.Weather.Get）

&#x20;   \[RateLimitPerMinute]       INT              NULL,              -- 每分钟调用次数限制

&#x20;   \[IsBuiltIn]                BIT              NOT NULL DEFAULT 0, -- 是否为框架内置工具

&#x20;   \[ExtraProperties]          NVARCHAR(MAX)    NULL,              -- 扩展属性

&#x20;   \[ConcurrencyStamp]         NVARCHAR(40)     NOT NULL,          -- 并发标记

&#x20;   \[CreationTime]             DATETIME2        NOT NULL,          -- 创建时间

&#x20;   \[CreatorId]                UNIQUEIDENTIFIER NULL,              -- 创建人ID

&#x20;   \[LastModificationTime]     DATETIME2        NULL,              -- 最后修改时间

&#x20;   \[LastModifierId]           UNIQUEIDENTIFIER NULL,              -- 最后修改人ID

&#x20;   \[IsDeleted]                BIT              NOT NULL DEFAULT 0, -- 软删除标记

&#x20;   \[DeleterId]                UNIQUEIDENTIFIER NULL,              -- 删除人ID

&#x20;   \[DeletionTime]             DATETIME2        NULL,              -- 删除时间

&#x20;   CONSTRAINT \[PK\_McpTools] PRIMARY KEY (\[Id])

);



CREATE UNIQUE INDEX \[IX\_McpTools\_ToolName\_TenantId] ON \[McpTools] (\[ToolName], \[TenantId]) WHERE \[IsDeleted] = 0;

CREATE INDEX \[IX\_McpTools\_Enabled] ON \[McpTools] (\[Enabled]) WHERE \[IsDeleted] = 0;

CREATE INDEX \[IX\_McpTools\_TenantId] ON \[McpTools] (\[TenantId]);

```



\### 2. McpToolCalls（MCP 工具调用记录表）



记录每一次 MCP 工具的调用详情，用于审计、监控和故障排查。



```sql

\-- MCP 工具调用记录表

CREATE TABLE \[McpToolCalls] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID

&#x20;   \[ToolId]                 UNIQUEIDENTIFIER NOT NULL,          -- 工具ID（关联 McpTools）

&#x20;   \[ToolName]               NVARCHAR(100)    NOT NULL,          -- 工具名称（冗余字段，便于查询）

&#x20;   \[CallerInfo]             NVARCHAR(500)    NULL,              -- 调用方信息（IP/客户端标识）

&#x20;   \[UserId]                 UNIQUEIDENTIFIER NULL,              -- 调用用户ID

&#x20;   \[UserName]               NVARCHAR(256)    NULL,              -- 调用用户名

&#x20;   \[SessionId]              NVARCHAR(100)    NULL,              -- MCP会话ID

&#x20;   \[RequestId]              NVARCHAR(100)    NULL,              -- 请求追踪ID（用于分布式追踪）

&#x20;   \[Parameters]             NVARCHAR(MAX)    NULL,              -- 调用参数（JSON格式）

&#x20;   \[Result]                 NVARCHAR(MAX)    NULL,              -- 调用结果（JSON格式）

&#x20;   \[ErrorMessage]           NVARCHAR(MAX)    NULL,              -- 错误信息（调用失败时）

&#x20;   \[ExecutionDurationMs]    INT              NOT NULL,          -- 执行耗时（毫秒）

&#x20;   \[StatusCode]             INT              NULL,              -- 状态码（200/400/500等）

&#x20;   \[Success]                BIT              NOT NULL,          -- 是否成功

&#x20;   \[CallTime]               DATETIME2        NOT NULL,          -- 调用时间

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,              -- 扩展属性

&#x20;   CONSTRAINT \[PK\_McpToolCalls] PRIMARY KEY (\[Id])

);



CREATE INDEX \[IX\_McpToolCalls\_ToolId\_CallTime] ON \[McpToolCalls] (\[ToolId], \[CallTime] DESC);

CREATE INDEX \[IX\_McpToolCalls\_TenantId\_CallTime] ON \[McpToolCalls] (\[TenantId], \[CallTime] DESC);

CREATE INDEX \[IX\_McpToolCalls\_Success\_CallTime] ON \[McpToolCalls] (\[Success], \[CallTime] DESC);

CREATE INDEX \[IX\_McpToolCalls\_UserId\_CallTime] ON \[McpToolCalls] (\[UserId], \[CallTime] DESC);

```



\### 3. McpToolCallStats（MCP 工具调用统计表）



按小时/天聚合的工具调用统计，用于性能监控和容量规划。



```sql

\-- MCP 工具调用统计表

CREATE TABLE \[McpToolCallStats] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID

&#x20;   \[ToolId]                 UNIQUEIDENTIFIER NOT NULL,          -- 工具ID

&#x20;   \[ToolName]               NVARCHAR(100)    NOT NULL,          -- 工具名称（冗余字段）

&#x20;   \[StatDate]               DATE             NOT NULL,          -- 统计日期

&#x20;   \[StatHour]               TINYINT          NULL,              -- 统计小时（0-23，NULL表示按日统计）

&#x20;   \[TotalCalls]             INT              NOT NULL DEFAULT 0, -- 总调用次数

&#x20;   \[SuccessCount]           INT              NOT NULL DEFAULT 0, -- 成功次数

&#x20;   \[FailureCount]           INT              NOT NULL DEFAULT 0, -- 失败次数

&#x20;   \[TotalDurationMs]        BIGINT           NOT NULL DEFAULT 0, -- 总耗时（毫秒）

&#x20;   \[MaxDurationMs]          INT              NOT NULL DEFAULT 0, -- 最大耗时（毫秒）

&#x20;   \[AvgDurationMs]          AS (CASE WHEN \[TotalCalls] = 0 THEN 0 ELSE \[TotalDurationMs] / \[TotalCalls] END) PERSISTED,

&#x20;   \[P50DurationMs]          INT              NULL,              -- P50耗时

&#x20;   \[P95DurationMs]          INT              NULL,              -- P95耗时

&#x20;   \[P99DurationMs]          INT              NULL,              -- P99耗时

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,

&#x20;   \[CreationTime]           DATETIME2        NOT NULL,          -- 创建时间

&#x20;   \[LastModificationTime]   DATETIME2        NULL,              -- 最后修改时间

&#x20;   CONSTRAINT \[PK\_McpToolCallStats] PRIMARY KEY (\[Id])

);



CREATE UNIQUE INDEX \[IX\_McpToolCallStats\_Unique] ON \[McpToolCallStats] (\[ToolId], \[StatDate], \[StatHour]) WHERE \[StatHour] IS NOT NULL;

CREATE UNIQUE INDEX \[IX\_McpToolCallStats\_Unique\_Daily] ON \[McpToolCallStats] (\[ToolId], \[StatDate]) WHERE \[StatHour] IS NULL;

CREATE INDEX \[IX\_McpToolCallStats\_StatDate] ON \[McpToolCallStats] (\[StatDate]);

```



\### 4. McpServers（MCP 服务端点配置表）



用于管理多个 MCP 服务器实例，支持集群部署场景。



```sql

\-- MCP 服务端点配置表

CREATE TABLE \[McpServers] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID

&#x20;   \[ServerName]             NVARCHAR(100)    NOT NULL,          -- 服务名称

&#x20;   \[DisplayName]            NVARCHAR(200)    NOT NULL,          -- 显示名称

&#x20;   \[Description]            NVARCHAR(500)    NULL,              -- 服务描述

&#x20;   \[Endpoint]               NVARCHAR(500)    NOT NULL,          -- 服务端点（HTTP/SSE或Stdio命令行）

&#x20;   \[TransportType]          NVARCHAR(20)     NOT NULL,          -- 传输类型：Sse, Stdio

&#x20;   \[AuthType]               NVARCHAR(50)     NOT NULL DEFAULT 'None', -- 认证类型：None, ApiKey, Jwt

&#x20;   \[ApiKey]                 NVARCHAR(256)    NULL,              -- API密钥（AuthType为ApiKey时使用）

&#x20;   \[Enabled]                BIT              NOT NULL DEFAULT 1, -- 是否启用

&#x20;   \[HealthCheckEndpoint]    NVARCHAR(500)    NULL,              -- 健康检查端点

&#x20;   \[Tags]                   NVARCHAR(500)    NULL,              -- 标签（用于分类，逗号分隔）

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,

&#x20;   \[ConcurrencyStamp]       NVARCHAR(40)     NOT NULL,

&#x20;   \[CreationTime]           DATETIME2        NOT NULL,

&#x20;   \[CreatorId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[LastModificationTime]   DATETIME2        NULL,

&#x20;   \[LastModifierId]         UNIQUEIDENTIFIER NULL,

&#x20;   \[IsDeleted]              BIT              NOT NULL DEFAULT 0,

&#x20;   \[DeleterId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[DeletionTime]           DATETIME2        NULL,

&#x20;   CONSTRAINT \[PK\_McpServers] PRIMARY KEY (\[Id])

);



CREATE UNIQUE INDEX \[IX\_McpServers\_ServerName\_TenantId] ON \[McpServers] (\[ServerName], \[TenantId]) WHERE \[IsDeleted] = 0;

CREATE INDEX \[IX\_McpServers\_Enabled] ON \[McpServers] (\[Enabled]) WHERE \[IsDeleted] = 0;

```



\### 5. McpWorkspaceConfigs（AI Workspace 配置表）



存储各租户/用户的 AI Workspace 配置，支持多租户隔离的模型配置。



```sql

\-- AI Workspace 配置表

CREATE TABLE \[McpWorkspaceConfigs] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID

&#x20;   \[WorkspaceName]          NVARCHAR(100)    NOT NULL,          -- Workspace名称（如：DefaultWorkspace）

&#x20;   \[WorkspaceType]          NVARCHAR(200)    NOT NULL,          -- Workspace类型（类的全名）

&#x20;   \[Provider]               NVARCHAR(50)     NOT NULL,          -- AI提供商：AzureOpenAI, OpenAI, Ollama

&#x20;   \[ModelName]              NVARCHAR(100)    NOT NULL,          -- 模型名称

&#x20;   \[Endpoint]               NVARCHAR(500)    NULL,              -- API端点

&#x20;   \[ApiKeyEncrypted]        NVARCHAR(500)    NOT NULL,          -- 加密存储的API密钥

&#x20;   \[DeploymentName]         NVARCHAR(100)    NULL,              -- 部署名称（Azure专用）

&#x20;   \[ApiVersion]             NVARCHAR(50)     NULL,              -- API版本

&#x20;   \[MaxTokens]              INT              NULL,              -- 最大Token数

&#x20;   \[Temperature]            DECIMAL(3,2)     NULL,              -- 温度参数

&#x20;   \[TopP]                   DECIMAL(3,2)     NULL,              -- Top-P参数

&#x20;   \[SystemPrompt]           NVARCHAR(MAX)    NULL,              -- 系统提示词

&#x20;   \[IsDefault]              BIT              NOT NULL DEFAULT 0, -- 是否为租户默认Workspace

&#x20;   \[Enabled]                BIT              NOT NULL DEFAULT 1, -- 是否启用

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,

&#x20;   \[ConcurrencyStamp]       NVARCHAR(40)     NOT NULL,

&#x20;   \[CreationTime]           DATETIME2        NOT NULL,

&#x20;   \[CreatorId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[LastModificationTime]   DATETIME2        NULL,

&#x20;   \[LastModifierId]         UNIQUEIDENTIFIER NULL,

&#x20;   CONSTRAINT \[PK\_McpWorkspaceConfigs] PRIMARY KEY (\[Id])

);



CREATE INDEX \[IX\_McpWorkspaceConfigs\_TenantId\_WorkspaceName] ON \[McpWorkspaceConfigs] (\[TenantId], \[WorkspaceName]);

CREATE INDEX \[IX\_McpWorkspaceConfigs\_IsDefault] ON \[McpWorkspaceConfigs] (\[IsDefault]);

```



\### 6. McpPromptTemplates（Prompt 模板表）



存储可复用的 Prompt 模板，支持版本管理和变量替换。



```sql

\-- Prompt 模板表

CREATE TABLE \[McpPromptTemplates] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID

&#x20;   \[TemplateName]           NVARCHAR(100)    NOT NULL,          -- 模板名称

&#x20;   \[DisplayName]            NVARCHAR(200)    NOT NULL,          -- 显示名称

&#x20;   \[Description]            NVARCHAR(500)    NULL,              -- 模板描述

&#x20;   \[Version]                NVARCHAR(20)     NOT NULL,          -- 版本号（如：1.0.0）

&#x20;   \[PromptText]             NVARCHAR(MAX)    NOT NULL,          -- Prompt文本（支持变量：{{变量名}}）

&#x20;   \[VariablesSchema]        NVARCHAR(MAX)    NULL,              -- 变量定义Schema（JSON格式）

&#x20;   \[Category]               NVARCHAR(100)    NULL,              -- 模板分类

&#x20;   \[Tags]                   NVARCHAR(500)    NULL,              -- 标签（逗号分隔）

&#x20;   \[IsDefault]              BIT              NOT NULL DEFAULT 0, -- 是否为默认版本

&#x20;   \[IsBuiltIn]              BIT              NOT NULL DEFAULT 0, -- 是否为内置模板

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,

&#x20;   \[ConcurrencyStamp]       NVARCHAR(40)     NOT NULL,

&#x20;   \[CreationTime]           DATETIME2        NOT NULL,

&#x20;   \[CreatorId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[LastModificationTime]   DATETIME2        NULL,

&#x20;   \[LastModifierId]         UNIQUEIDENTIFIER NULL,

&#x20;   \[IsDeleted]              BIT              NOT NULL DEFAULT 0,

&#x20;   \[DeleterId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[DeletionTime]           DATETIME2        NULL,

&#x20;   CONSTRAINT \[PK\_McpPromptTemplates] PRIMARY KEY (\[Id])

);



CREATE UNIQUE INDEX \[IX\_McpPromptTemplates\_TemplateName\_Version\_TenantId] ON \[McpPromptTemplates] (\[TemplateName], \[Version], \[TenantId]) WHERE \[IsDeleted] = 0;

CREATE INDEX \[IX\_McpPromptTemplates\_Category\_IsDefault] ON \[McpPromptTemplates] (\[Category], \[IsDefault]);

```



\### 7. McpRateLimitPolicies（限流策略表）



配置各租户/工具的调用限流策略。



```sql

\-- 限流策略表

CREATE TABLE \[McpRateLimitPolicies] (

&#x20;   \[Id]                     UNIQUEIDENTIFIER NOT NULL,          -- 主键

&#x20;   \[TenantId]               UNIQUEIDENTIFIER NULL,              -- 租户ID（NULL表示全局策略）

&#x20;   \[PolicyName]             NVARCHAR(100)    NOT NULL,          -- 策略名称

&#x20;   \[TargetType]             NVARCHAR(20)     NOT NULL,          -- 目标类型：Tenant, User, Tool, ApiKey

&#x20;   \[TargetId]               UNIQUEIDENTIFIER NULL,              -- 目标ID（根据TargetType对应不同表）

&#x20;   \[LimitRequests]          INT              NOT NULL,          -- 限制请求次数

&#x20;   \[PeriodSeconds]          INT              NOT NULL,          -- 限制周期（秒）

&#x20;   \[Enabled]                BIT              NOT NULL DEFAULT 1, -- 是否启用

&#x20;   \[Priority]               INT              NOT NULL DEFAULT 0, -- 优先级（数字越大优先级越高）

&#x20;   \[ExtraProperties]        NVARCHAR(MAX)    NULL,

&#x20;   \[CreationTime]           DATETIME2        NOT NULL,

&#x20;   \[CreatorId]              UNIQUEIDENTIFIER NULL,

&#x20;   \[LastModificationTime]   DATETIME2        NULL,

&#x20;   \[LastModifierId]         UNIQUEIDENTIFIER NULL,

&#x20;   CONSTRAINT \[PK\_McpRateLimitPolicies] PRIMARY KEY (\[Id])

);



CREATE INDEX \[IX\_McpRateLimitPolicies\_TargetType\_TargetId] ON \[McpRateLimitPolicies] (\[TargetType], \[TargetId]) WHERE \[Enabled] = 1;

```



\---



\## 二、ABP 框架基础表（框架自动生成）



以下为 ABP vNext 框架内置的表，执行数据库迁移后会自动创建。



| 表名 | 用途 | 与本项目的关系 |

|------|------|----------------|

| `AbpUsers` | 用户信息表 | 记录 MCP 工具调用者的用户信息 |

| `AbpTenants` | 租户信息表 | 支撑多租户数据隔离 |

| `AbpAuditLogs` | 审计日志主表 | 记录 Web 请求级审计信息 |

| `AbpAuditLogActions` | 审计操作明细表 | 记录操作参数、返回值等 |

| `AbpSettings` | 系统设置表 | 存储 Workspace 等模块配置 |

| `AbpPermissionGrants` | 权限授权表 | 控制 MCP 工具调用权限 |

| `AbpBackgroundJobs` | 后台任务表 | 执行异步 AI 任务编排 |

| `AbpUserDelegations` | 用户委派表 | 支持用户代理调用场景 |



\---



\## 三、种子数据



以下为系统初始化时需要插入的种子数据（`McpTools` 表的示例配置）：



```sql

\-- 插入内置 MCP 工具示例（用于演示）

INSERT INTO \[McpTools] (

&#x20;   \[Id], \[TenantId], \[ToolName], \[DisplayName], \[Description],

&#x20;   \[AssemblyFullName], \[ClassName], \[MethodName],

&#x20;   \[ParametersSchema], \[ReturnSchema], \[Enabled], \[RequiresAuthorization],

&#x20;   \[RequiredPermissionName], \[IsBuiltIn], \[ConcurrencyStamp],

&#x20;   \[CreationTime], \[IsDeleted]

) VALUES (

&#x20;   NEWID(), NULL, 'get\_weather', '获取天气信息',

&#x20;   '获取指定城市的实时天气信息',

&#x20;   'YourCompany.McpServer.Tools', 'YourCompany.McpServer.Tools.WeatherTool',

&#x20;   'GetWeatherAsync',

&#x20;   '{"type":"object","properties":{"city":{"type":"string","description":"城市名称"}},"required":\["city"]}',

&#x20;   '{"type":"string","description":"天气信息描述"}',

&#x20;   1, 1, 'Mcp.Weather.Get', 1, NEWID(), GETUTCDATE(), 0

);

```



\---



\## 四、与 ABP vNext 集成的实体类示例



以下为对应的 .NET 实体类示例：



\*\*McpTool.cs\*\*



```csharp

using System;

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Domain.Entities.Auditing;

using Volo.Abp.MultiTenancy;



namespace YourCompany.McpServer.Entities;



/// <summary>

/// MCP 工具注册实体

/// </summary>

public class McpTool : FullAuditedAggregateRoot<Guid>, IMultiTenant

{

&#x20;   public virtual Guid? TenantId { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(100)]

&#x20;   public virtual string ToolName { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(200)]

&#x20;   public virtual string DisplayName { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(500)]

&#x20;   public virtual string Description { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(500)]

&#x20;   public virtual string AssemblyFullName { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(500)]

&#x20;   public virtual string ClassName { get; set; }



&#x20;   \[Required]

&#x20;   \[StringLength(200)]

&#x20;   public virtual string MethodName { get; set; }



&#x20;   \[StringLength(500)]

&#x20;   public virtual string ParametersSchema { get; set; }



&#x20;   \[StringLength(500)]

&#x20;   public virtual string ReturnSchema { get; set; }



&#x20;   public virtual bool Enabled { get; set; } = true;



&#x20;   public virtual bool RequiresAuthorization { get; set; }



&#x20;   \[StringLength(128)]

&#x20;   public virtual string RequiredPermissionName { get; set; }



&#x20;   public virtual int? RateLimitPerMinute { get; set; }



&#x20;   public virtual bool IsBuiltIn { get; set; }

}

```



\*\*McpToolCall.cs\*\*



```csharp

using System;

using Volo.Abp.Domain.Entities;

using Volo.Abp.MultiTenancy;



namespace YourCompany.McpServer.Entities;



/// <summary>

/// MCP 工具调用记录实体

/// </summary>

public class McpToolCall : Entity<Guid>, IMultiTenant

{

&#x20;   public virtual Guid? TenantId { get; set; }



&#x20;   public virtual Guid ToolId { get; set; }



&#x20;   \[StringLength(100)]

&#x20;   public virtual string ToolName { get; set; }



&#x20;   \[StringLength(500)]

&#x20;   public virtual string CallerInfo { get; set; }



&#x20;   public virtual Guid? UserId { get; set; }



&#x20;   \[StringLength(256)]

&#x20;   public virtual string UserName { get; set; }



&#x20;   \[StringLength(100)]

&#x20;   public virtual string SessionId { get; set; }



&#x20;   \[StringLength(100)]

&#x20;   public virtual string RequestId { get; set; }



&#x20;   public virtual string Parameters { get; set; }



&#x20;   public virtual string Result { get; set; }



&#x20;   public virtual string ErrorMessage { get; set; }



&#x20;   public virtual int ExecutionDurationMs { get; set; }



&#x20;   public virtual int? StatusCode { get; set; }



&#x20;   public virtual bool Success { get; set; }



&#x20;   public virtual DateTime CallTime { get; set; }

}

```



\---



\## 五、数据库上下文（DbContext）配置



在 `YourCompanyMcpDbContext` 中注册实体：



```csharp

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;

using Volo.Abp.EntityFrameworkCore;

using YourCompany.McpServer.Entities;



namespace YourCompany.McpServer.EntityFrameworkCore;



\[ConnectionStringName("McpServer")]

public class YourCompanyMcpDbContext : AbpDbContext<YourCompanyMcpDbContext>

{

&#x20;   public DbSet<McpTool> McpTools { get; set; }

&#x20;   public DbSet<McpToolCall> McpToolCalls { get; set; }

&#x20;   public DbSet<McpToolCallStat> McpToolCallStats { get; set; }

&#x20;   public DbSet<McpServer> McpServers { get; set; }

&#x20;   public DbSet<McpWorkspaceConfig> McpWorkspaceConfigs { get; set; }

&#x20;   public DbSet<McpPromptTemplate> McpPromptTemplates { get; set; }

&#x20;   public DbSet<McpRateLimitPolicy> McpRateLimitPolicies { get; set; }



&#x20;   public YourCompanyMcpDbContext(DbContextOptions<YourCompanyMcpDbContext> options)

&#x20;       : base(options)

&#x20;   {

&#x20;   }



&#x20;   protected override void OnModelCreating(ModelBuilder builder)

&#x20;   {

&#x20;       base.OnModelCreating(builder);

&#x20;       builder.ConfigureMcpServer();

&#x20;   }

}

```



对应的 EntityFramework Core 映射配置文件 `McpServerDbContextModelCreatingExtensions.cs`：



```csharp

using Microsoft.EntityFrameworkCore;

using YourCompany.McpServer.Entities;



namespace YourCompany.McpServer.EntityFrameworkCore;



public static class McpServerDbContextModelCreatingExtensions

{

&#x20;   public static void ConfigureMcpServer(this ModelBuilder builder)

&#x20;   {

&#x20;       builder.Entity<McpTool>(b =>

&#x20;       {

&#x20;           b.ToTable("McpTools");

&#x20;           b.Property(x => x.ToolName).IsRequired().HasMaxLength(100);

&#x20;           b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);

&#x20;           b.Property(x => x.Description).IsRequired().HasMaxLength(500);

&#x20;           b.Property(x => x.AssemblyFullName).IsRequired().HasMaxLength(500);

&#x20;           b.Property(x => x.ClassName).IsRequired().HasMaxLength(500);

&#x20;           b.Property(x => x.MethodName).IsRequired().HasMaxLength(200);

&#x20;           b.Property(x => x.RequiredPermissionName).HasMaxLength(128);

&#x20;           b.HasIndex(x => new { x.ToolName, x.TenantId }).IsUnique();

&#x20;       });



&#x20;       builder.Entity<McpToolCall>(b =>

&#x20;       {

&#x20;           b.ToTable("McpToolCalls");

&#x20;           b.Property(x => x.ToolName).IsRequired().HasMaxLength(100);

&#x20;           b.Property(x => x.CallerInfo).HasMaxLength(500);

&#x20;           b.Property(x => x.UserName).HasMaxLength(256);

&#x20;           b.Property(x => x.SessionId).HasMaxLength(100);

&#x20;           b.Property(x => x.RequestId).HasMaxLength(100);

&#x20;           b.HasIndex(x => new { x.ToolId, x.CallTime });

&#x20;           b.HasIndex(x => new { x.TenantId, x.CallTime });

&#x20;       });



&#x20;       // 其他实体的映射配置...

&#x20;   }

}

```



\---



\## 六、迁移与部署



使用 ABP CLI 或 `dotnet ef` 命令进行数据库迁移：



```bash

\# 添加迁移

dotnet ef migrations add "Initial\_McpServer\_20241201" --project src/YourCompany.McpServer.EntityFrameworkCore



\# 更新数据库

dotnet ef database update --project src/YourCompany.McpServer.EntityFrameworkCore

```



执行迁移后，EF Core 会自动创建 `\_\_EFMigrationsHistory` 表记录迁移历史，同时 ABP vNext 框架也会自动创建 `AbpSettings`、`AbpAuditLogs` 等框架表。



\---

> 📌 \*\*说明\*\*：以上表结构可根据实际业务需求进行调整，如增加 `McpAgentConfigs`（Agent 配置表）、`McpConversationHistories`（对话历史表）等用于更复杂的多 Agent 编排和 RAG 场景。如需针对特定数据库类型优化，可进一步适配。

