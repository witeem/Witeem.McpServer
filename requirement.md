以下是您所需的企业级需求分析与实现文档，内容涵盖从业务背景到技术落地的完整方案，基于 \*\*.NET 9\*\* 与 \*\*ABP vNext\*\* 框架，并以 \*\*MCP 服务\*\* 作为核心 AI 能力扩展方式。



```markdown

\# 基于 .NET 9 + ABP vNext 的 MCP 服务扩展平台 – 需求分析与实现方案



\## 1. 引言



\### 1.1 项目背景

随着大语言模型（LLM）在企业中的广泛应用，如何安全、高效地将 AI 能力与现有业务系统集成成为关键挑战。Model Context Protocol（MCP）作为开放标准，允许 AI 客户端（如 GitHub Copilot、自定义 Agent）通过统一协议调用任意工具。本项目的目标是构建一个 \*\*企业级 MCP 服务托管与编排平台\*\*，基于 .NET 9 和 ABP vNext 框架，使业务团队能够以极低成本将现有 .NET 应用服务暴露为 MCP 工具，并借助 AI Workspace 与 Semantic Kernel 实现复杂的 AI 工作流。



\### 1.2 文档范围

本文档覆盖项目的需求分析、系统架构、模块设计、技术选型、实现细节、测试策略及部署方案，适用于项目负责人、架构师、开发及测试人员。



\### 1.3 目标读者

\- 项目经理：理解功能范围与交付内容

\- 系统架构师：评估技术决策与扩展性

\- 开发人员：获取详细实现指引

\- 运维人员：掌握部署与监控要求



\## 2. 需求概述



\### 2.1 业务目标

\- \*\*工具化业务能力\*\*：将现有应用服务（如订单查询、天气信息、文档处理）快速包装为 MCP 工具，供 AI 客户端调用。

\- \*\*AI 工作流编排\*\*：通过 Semantic Kernel 组合多个 MCP 工具，实现智能问答、自动报告生成等复杂任务。

\- \*\*多租户隔离\*\*：不同业务部门拥有独立的 AI 配置与 MCP 工具集，互不干扰。

\- \*\*安全合规\*\*：所有工具调用均通过 ABP 权限系统进行鉴权，敏感数据不直接暴露给 LLM。



\### 2.2 用户角色

| 角色 | 描述 |

|------|------|

| AI 客户端 | 外部系统（如 Copilot、自定义聊天机器人），通过 MCP 协议调用工具 |

| 业务开发人员 | 在代码中添加 `\[McpServerTool]` 注解，将现有服务暴露为工具 |

| 平台管理员 | 管理租户、API 密钥、AI 模型配置，监控工具调用日志 |

| 最终用户 | 通过接入 AI 界面的业务人员，使用自然语言触发工具 |



\## 3. 功能性需求



\### 3.1 MCP 服务托管

| 需求 ID | 描述 | 优先级 |

|---------|------|--------|

| MCP-01 | 支持通过 Stdio 与 SSE/HTTP 两种传输协议暴露 MCP 服务 | 高 |

| MCP-02 | 自动扫描程序集中标记 `\[McpServerTool]` 的方法，注册为工具 | 高 |

| MCP-03 | 支持工具动态发现（`list\_tools`）和调用（`call\_tool`） | 高 |

| MCP-04 | 为每个工具提供 Schema 描述（名称、描述、参数类型） | 高 |

| MCP-05 | 支持工具调用的同步与异步执行 | 中 |



\### 3.2 AI 编排能力

| 需求 ID | 描述 | 优先级 |

|---------|------|--------|

| AI-01 | 集成 ABP AI Workspaces，支持多租户、多模型的配置隔离 | 高 |

| AI-02 | 通过 Semantic Kernel 实现多工具协同工作流 | 高 |

| AI-03 | 提供内置 Prompt 模板库，支持版本化管理 | 中 |

| AI-04 | 支持会话历史管理，实现上下文连续对话 | 中 |



\### 3.3 管理与监控

| 需求 ID | 描述 | 优先级 |

|---------|------|--------|

| OPS-01 | 所有工具调用记录入库（调用时间、参数、结果、耗时） | 高 |

| OPS-02 | 提供管理界面查看调用统计与错误率 | 中 |

| OPS-03 | 支持按租户限流（每分钟调用次数） | 中 |

| OPS-04 | 集成 Serilog 与 OpenTelemetry 实现结构化日志与链路追踪 | 低 |



\## 4. 非功能性需求



| 类别 | 指标 | 目标值 |

|------|------|--------|

| 性能 | 工具调用平均响应时间 | ≤ 200ms (不含 LLM 调用) |

| 可扩展性 | 水平扩展能力 | 支持 Kubernetes 多副本部署 |

| 安全性 | 工具调用鉴权 | 基于 ABP 权限系统，每个工具可绑定策略 |

| 可用性 | SLA | 99.9% |

| 可维护性 | 代码复杂度 | 圈复杂度 ≤ 15 |

| 兼容性 | .NET 版本 | .NET 9 |



\## 5. 系统架构设计



\### 5.1 逻辑架构图



```mermaid

graph TD

&#x20;   subgraph 客户端层

&#x20;       A\[AI Client<br/>GitHub Copilot等]

&#x20;   end



&#x20;   subgraph 接入层

&#x20;       B\[MCP HTTP/SSE Gateway<br/>(Kestrel)]

&#x20;       C\[MCP Stdio Host]

&#x20;   end



&#x20;   subgraph 应用层

&#x20;       D\[McpServer 模块]

&#x20;       E\[AI Orchestration 模块]

&#x20;       F\[业务应用服务层]

&#x20;   end



&#x20;   subgraph 基础设施层

&#x20;       G\[ABP 权限/租户]

&#x20;       H\[Semantic Kernel]

&#x20;       I\[向量数据库]

&#x20;       J\[日志/监控]

&#x20;   end



&#x20;   A -- MCP协议 --> B

&#x20;   A -- MCP协议 --> C

&#x20;   B --> D

&#x20;   C --> D

&#x20;   D --> F

&#x20;   E --> H

&#x20;   E --> I

&#x20;   E --> D

&#x20;   D --> G

```



\### 5.2 模块划分



| 模块 | 职责 | 对应 ABP 项目 |

|------|------|----------------|

| `McpServerModule` | 注册 MCP 工具、启动协议端点 | `YourCompany.McpServer` |

| `AIWorkspaceModule` | 配置 AI Workspace、集成 Semantic Kernel | `YourCompany.AI` |

| `ApplicationModule` | 业务服务实现（如天气、订单） | `YourCompany.Application` |

| `HttpApi.Host` | 托管 HTTP 网关（SSE），集成 Swagger | `YourCompany.HttpApi.Host` |

| `McpStdio.Host` | 独立控制台宿主，用于 Stdio 模式 | `YourCompany.McpStdioHost` |



\## 6. 详细实现方案



\### 6.1 MCP 工具暴露机制



\*\*核心设计\*\*：利用 `ModelContextProtocol` NuGet 包提供的 `\[McpServerTool]` 特性，结合 ABP 的 `ITransientDependency` 自动注册能力，实现零配置工具发现。



\*\*代码示例\*\*（已在前期对话中给出）。关键技术点：

\- 工具类实现 `ITransientDependency`，保证每次调用可获取全新实例（避免状态污染）。

\- 支持依赖注入：构造函数可注入 `IWeatherAppService`、`ICurrentUser` 等 ABP 服务。

\- 参数自动验证：`\[Description]` 特性生成 OpenAPI 风格参数描述。



\### 6.2 AI Workspace 配置与多租户隔离



\*\*设计\*\*：通过 ABP 的 `AbpAIWorkspaceOptions` 在 `PreConfigureServices` 中为不同租户配置独立的 `Kernel` 实例。



```csharp

PreConfigure<AbpAIWorkspaceOptions>(options =>

{

&#x20;   options.Configure<TenantAWorkspace>(workspace =>

&#x20;   {

&#x20;       workspace.ConfigureKernel(kernel =>

&#x20;       {

&#x20;           kernel.Builder.AddAzureOpenAIChatCompletion(

&#x20;               deploymentName: "gpt-4-tenant-a",

&#x20;               endpoint: "https://tenant-a.openai.azure.com/",

&#x20;               apiKey: "key-a"

&#x20;           );

&#x20;       });

&#x20;   });

});

```



在应用服务中使用泛型 `IKernelAccessor<TWorkspace>` 获得对应的 Kernel，实现租户数据隔离。



\### 6.3 MCP 服务两种托管模式



| 模式 | 适用场景 | 实现方式 |

|------|----------|----------|

| Stdio 模式 | 本地 CLI 工具、IDE 插件（如 Copilot） | 独立控制台应用，使用 `StdioServerTransport` |

| SSE/HTTP 模式 | 云端部署、多客户端共享 | ASP.NET Core 端点，使用 `SseServerTransport` |



\*\*HTTP 模式配置\*\*（在 `HttpApi.Host` 中）：

```csharp

app.MapMcpSseEndpoint("/mcp", serviceProvider => 

{

&#x20;   var toolProvider = serviceProvider.GetRequiredService<IMcpToolProvider>();

&#x20;   return new McpServer("MyMCP", toolProvider);

});

```



\### 6.4 安全与鉴权



\- \*\*工具级别权限\*\*：在工具方法上使用 `\[Authorize]` 特性，ABP 自动拦截未授权调用。

\- \*\*API Key 认证\*\*：对于 HTTP SSE 模式，可通过自定义 `IAuthenticationHandler` 验证请求头中的 `X-API-Key`。

\- \*\*数据脱敏\*\*：在工具返回前，使用 `IDataMaskingService` 对敏感字段（如身份证号）进行掩码处理。



\### 6.5 调用链路追踪



集成 OpenTelemetry 与 Jaeger：

```csharp

services.AddOpenTelemetry()

&#x20;   .WithTracing(tracing => tracing

&#x20;       .AddSource("MCP")

&#x20;       .AddAspNetCoreInstrumentation()

&#x20;       .AddHttpClientInstrumentation()

&#x20;       .AddJaegerExporter());

```

每个工具调用生成一个 Span，记录参数、结果大小、异常信息。



\## 7. 技术选型总结



| 类别 | 技术 | 理由 |

|------|------|------|

| 框架 | .NET 9 | 最新 LTS，性能提升，原生支持 JSON 处理 |

| 应用框架 | ABP vNext 9.x | 模块化、多租户、DDD、权限系统开箱即用 |

| MCP SDK | `ModelContextProtocol` (预览版) | 官方支持，与 .NET 深度集成 |

| AI 编排 | Semantic Kernel | 微软主导，插件生态丰富，与 ABP AI Workspace 无缝配合 |

| AI 抽象 | `Microsoft.Extensions.AI` | 统一接口，便于更换模型提供商 |

| 向量数据库 | Qdrant (或 SQLite + vector) | 轻量级，支持 RAG 场景 |

| 日志/监控 | Serilog + Seq, OpenTelemetry | 结构化日志，符合云原生标准 |

| 容器化 | Docker + K8s | 便于部署与弹性伸缩 |



\## 8. 测试策略



\### 8.1 单元测试

\- 使用 `xUnit` + `NSubstitute` 模拟依赖

\- 测试 MCP 工具方法的输入输出正确性

\- 测试 AI Workspace 配置加载逻辑



\### 8.2 集成测试

\- 使用 `Microsoft.AspNetCore.TestHost` 模拟 HTTP 端点

\- 编写测试夹具启动真实的 MCP Server，通过 MCP 客户端调用工具

\- 验证多租户 Kernel 隔离效果



\### 8.3 性能测试

\- 使用 `NBomber` 模拟高并发工具调用

\- 测试目标：1000 并发下平均延迟 ≤ 500ms



\### 8.4 AI 效果评估

\- 对典型 Prompt 构建回归测试集，评估 Semantic Kernel 工作流输出质量

\- 记录每次变更后的 BLEU / ROUGE 指标



\## 9. 部署方案



\### 9.1 开发环境

\- 使用 `dotnet run` 启动 `HttpApi.Host`（默认端口 5000）

\- 可选 `McpStdioHost` 通过命令行调试



\### 9.2 生产环境（Kubernetes）



```yaml

\# mcp-server-deployment.yaml

apiVersion: apps/v1

kind: Deployment

metadata:

&#x20; name: mcp-server-http

spec:

&#x20; replicas: 3

&#x20; selector:

&#x20;   matchLabels:

&#x20;     app: mcp-server

&#x20; template:

&#x20;   metadata:

&#x20;     labels:

&#x20;       app: mcp-server

&#x20;   spec:

&#x20;     containers:

&#x20;     - name: mcp-server

&#x20;       image: your-registry/mcp-server:latest

&#x20;       ports:

&#x20;       - containerPort: 8080

&#x20;       env:

&#x20;       - name: ConnectionStrings\_\_Default

&#x20;         valueFrom:

&#x20;           secretKeyRef:

&#x20;             name: db-secret

&#x20;             key: connection-string

&#x20;       - name: AI\_\_OpenAI\_\_Key

&#x20;         valueFrom:

&#x20;           secretKeyRef:

&#x20;             name: ai-secret

&#x20;             key: openai-key

```



暴露 Service 类型为 `LoadBalancer` 或 `Ingress`。



\### 9.3 监控告警

\- Prometheus 采集自定义指标：`mcp\_tool\_calls\_total`、`mcp\_tool\_duration\_seconds`

\- Grafana 仪表盘展示 QPS、错误率、P99 延迟

\- Alertmanager 配置：错误率 > 5% 持续 5 分钟触发报警



\## 10. 附录



\### 10.1 术语表



| 术语 | 解释 |

|------|------|

| MCP | Model Context Protocol – 模型上下文协议，用于 AI 客户端与工具服务器通信 |

| ABP vNext | 开源企业级 ASP.NET Core 应用框架 |

| Workspace | ABP AI 中的配置单元，拥有独立的 Kernel 与设置 |

| Semantic Kernel | 微软出品的轻量级 AI 编排 SDK |



\### 12.2 参考文档

\- \[MCP for .NET 官方快速入门](https://devblogs.microsoft.com/dotnet/mcp-server-dotnet-nuget-quickstart)

\- \[ABP AI 集成文档](https://abp.io/docs/latest/framework/infrastructure/artificial-intelligence)

\- \[Semantic Kernel 文档](https://learn.microsoft.com/semantic-kernel/)



\---

