---
name: ABP vNext & .NET 9 Full-Stack Development Skill
description: Provides comprehensive full‑stack development expertise using ABP vNext application framework with .NET 9 backend and integrated frontend options. Use this Skill when building enterprise‑grade web applications, modular SaaS platforms, microservices, REST APIs, or applications requiring DDD‑aligned architecture with built‑in infrastructure.
---

\# ABP vNext \& .NET 9 Full-Stack Development Skill



```markdown

| Field | Value |

|-------|-------|

| Identifier | `abp-vnext-dotnet9-fullstack-developer` |

| Version | 1.0.0 |

| Author | ABP vNext Community |

| Category | backend-framework-development |

| Tags | .NET 9, ABP vNext, DDD, Modular Architecture, Enterprise Applications, ASP.NET Core |

| License | MIT |

```



\## Skill Overview



Provides comprehensive full‑stack development expertise using \*\*ABP vNext\*\* application framework with \*\*.NET 9\*\* backend and integrated frontend options. Use this Skill when building enterprise‑grade web applications, modular SaaS platforms, microservices, REST APIs, or applications requiring DDD‑aligned architecture with built‑in infrastructure.



\*\*Core Features:\*\*

\- DDD‑aligned layered architecture (Presentation → Application → Domain → Infrastructure)

\- True modularity with reusable, pluggable modules

\- Built‑in multi‑tenancy support (database‑per‑tenant, shared database with tenant ID)

\- Automatic API layer generation from application services

\- Integrated audit logging, authorization, and exception handling

\- Distributed event bus (RabbitMQ) for microservice communication

\- Localization and virtual file system for resource management

\- Data seeding infrastructure for initialization

\- OpenIddict integration for OAuth2/OpenID Connect



\*\*Technology Stack:\*\*

\- \*\*Backend:\*\* .NET 9 SDK, ASP.NET Core 9, EF Core 9, MongoDB, Redis, RabbitMQ

\- \*\*Frontend:\*\* Blazor (WASM/Server), Angular, MVC/Razor Pages, or Web API only

\- \*\*Testing:\*\* xUnit, ABP integrated test base classes, SQLite in‑memory for EFCore testing

\- \*\*DevOps:\*\* Docker, Kubernetes, CI/CD (GitHub Actions, Azure DevOps)



\*\*Typical Use Cases:\*\*

\- Building SaaS applications with tenant isolation

\- Starting with modular monolith, scaling to microservices when needed

\- Developing reusable application modules for marketplace distribution

\- Enterprise CRUD applications with complex business rules

\- API‑first architectures with OpenAPI/Swagger documentation



\## 🔧 Version Compatibility \& Prerequisites



\### .NET 9 \& ABP 9.x



ABP vNext 9.0+ is upgraded to .NET 9.0—migrate your solutions to .NET 9.0 to use ABP 9.x. However, ABP‘s NuGet packages remain compatible with both .NET 8 and .NET 9, allowing continued use of .NET 8 SDK while still accessing the latest ABP features.



\*\*Minimum Requirements:\*\*

\- .NET 9.0 SDK (for ABP 9.0+)

\- Visual Studio 2022 or JetBrains Rider / VS Code

\- Docker Desktop (optional, for containerized services)



\### ABP CLI Setup



```bash

\# Install ABP CLI

dotnet tool install -g Volo.Abp.Cli



\# Create a new solution

abp new MyProject -u blazor   # or -u angular, -u mvc



\# Add a new module

abp add-module MyModule --new



\# Update ABP packages to latest version

abp update

```



\## 📁 Architecture \& Project Structure



\### Layered Architecture (DDD‑Oriented)



ABP vNext follows DDD principles with clear separation of concerns:



```

┌─────────────────────────────────────────────────────────┐

│  Presentation Layer (HttpApi / Blazor / Angular / MVC)  │

│  - API Controllers (auto‑generated from App Services)   │

│  - UI Pages / Components                                 │

│  - ViewModels / DTOs                                     │

├─────────────────────────────────────────────────────────┤

│  Application Layer (Application + Application.Contracts)│

│  - Application Services (business process orchestration)│

│  - DTOs (input/output)                                  │

│  - Permissions definition                                │

├─────────────────────────────────────────────────────────┤

│  Domain Layer (Domain + Domain.Shared)                   │

│  - Entities \& Aggregate Roots                            │

│  - Value Objects                                         │

│  - Domain Services                                       │

│  - Repository Interfaces                                 │

│  - Business rules \& invariants                          │

├─────────────────────────────────────────────────────────┤

│  Infrastructure Layer (EntityFrameworkCore / MongoDB)    │

│  - Repository Implementations                            │

│  - DbContext / DbMigrations                             │

│  - External service integrations                        │

└─────────────────────────────────────────────────────────┘

```



\### Standard ABP Module Structure



Each functional module should contain these projects:



| Project | Purpose |

|---------|---------|

| \*\*`MyModule.Domain.Shared`\*\* | Constants, enums, DTOs shared across boundaries. No dependencies, pure contracts. |

| \*\*`MyModule.Domain`\*\* | Entities, value objects, domain services, repository interfaces. Business core. |

| \*\*`MyModule.Application.Contracts`\*\* | Application service interfaces + input/output DTOs (no implementation). |

| \*\*`MyModule.Application`\*\* | Application service implementations. Orchestrates domain objects. |

| \*\*`MyModule.HttpApi`\*\* | API controllers (auto from app services) + Swagger config. |

| \*\*`MyModule.HttpApi.Client`\*\* | C# client proxies for remote service consumption. |

| \*\*`MyModule.EntityFrameworkCore`\*\* | DbContext, repository implementations, migrations, mappings. |



\### Modular Monolith vs Microservices



ABP vNext’s modular design enables starting with a modular monolith and scaling to microservices when needed:



\- \*\*Modular Monolith\*\*: Deploy all modules together as a single application. Modules communicate via direct references (Domain.Shared → Application.Contracts → Application) or local event bus.

\- \*\*Microservices\*\*: Each module deploys independently as a separate service. Modules communicate via HTTP API (HttpApi.Client) and distributed event bus (RabbitMQ).



Microservice repository strategy depends on team size, project complexity, and organizational needs—choose between mono‑repo (all services in one repository) or multiple repos (each microservice has its own repository).



\### Module Definition



Each ABP module is defined by a module class inheriting `AbpModule`:



```csharp

\[DependsOn(

&#x20;   typeof(AbpAspNetCoreMvcModule),

&#x20;   typeof(AbpAutofacModule),

&#x20;   typeof(MyDomainModule),

&#x20;   typeof(MyApplicationModule),

&#x20;   typeof(MyEntityFrameworkCoreModule)

)]

public class MyHttpApiHostModule : AbpModule

{

&#x20;   public override void ConfigureServices(ServiceConfigurationContext context)

&#x20;   {

&#x20;       // Register services, configure options

&#x20;       Configure<AbpLocalizationOptions>(options =>

&#x20;       {

&#x20;           options.Resources.Add<MyResource>("en");

&#x20;       });

&#x20;   }



&#x20;   public override void OnApplicationInitialization(ApplicationInitializationContext context)

&#x20;   {

&#x20;       var app = context.GetApplicationBuilder();

&#x20;       app.UseRouting();

&#x20;       app.UseAuthentication();

&#x20;       app.UseAuthorization();

&#x20;       app.UseConfiguredEndpoints();

&#x20;   }

}

```



\## 👔 Development Standards \& Best Practices



\### Naming Conventions



| Component | Convention | Example |

|-----------|------------|---------|

| Application Service | `{Name}AppService` | `ProductAppService` |

| DTO (Input) | `{Name}CreateDto`, `{Name}UpdateDto`, `{Name}GetListInput` | `ProductCreateDto` |

| DTO (Output) | `{Name}Dto` | `ProductDto` |

| Repository Interface | `I{Name}Repository` | `IProductRepository` |

| Entity/Aggregate Root | `{Name}` (no suffix) | `Product` |

| Module Class | `{Name}Module` | `MyProductModule` |

| Permission | `{Module}:{Action}` | `ProductManagement:Create` |



\### Application Service Pattern



```csharp

public class ProductAppService : ApplicationService, IProductAppService

{

&#x20;   private readonly IRepository<Product, Guid> \_productRepository;

&#x20;   private readonly IProductManager \_productManager;  // Domain service



&#x20;   public ProductAppService(

&#x20;       IRepository<Product, Guid> productRepository,

&#x20;       IProductManager productManager)

&#x20;   {

&#x20;       \_productRepository = productRepository;

&#x20;       \_productManager = productManager;

&#x20;   }



&#x20;   // Auto API endpoint: GET /api/app/product/{id}

&#x20;   public virtual async Task<ProductDto> GetAsync(Guid id)

&#x20;   {

&#x20;       var product = await \_productRepository.GetAsync(id);

&#x20;       return ObjectMapper.Map<Product, ProductDto>(product);

&#x20;   }



&#x20;   // Auto API endpoint: POST /api/app/product

&#x20;   public virtual async Task<ProductDto> CreateAsync(ProductCreateDto input)

&#x20;   {

&#x20;       // Use domain service for business rules

&#x20;       var product = await \_productManager.CreateAsync(

&#x20;           input.Name, input.Price, input.CategoryId

&#x20;       );

&#x20;       await \_productRepository.InsertAsync(product);

&#x20;       return ObjectMapper.Map<Product, ProductDto>(product);

&#x20;   }

}

```



\*\*Guidelines for Application Services:\*\*

\- Keep them thin—coordinate domain objects, don’t implement business rules

\- Use DTOs for input/output, never expose entities directly to clients

\- Apply `\[Authorize]` attributes for permission control

\- Leverage ABP’s automatic validation (IValidatableObject or DataAnnotations)

\- Use `ObjectMapper` (AutoMapper configured by ABP) for entity ↔ DTO mapping



\### Repository Best Practices



\*\*DO’s \& DON’Ts for Repositories\*\*:



| ✅ DO | ❌ DON’T |

|-------|----------|

| Define repository interfaces in Domain layer | Expose `IQueryable<TEntity>` to application layer |

| Create custom repository interface per aggregate root | Use generic `IRepository<TEntity, TKey>` in application code |

| Make all repository methods async | Create composite classes (e.g., `UserWithRoles`) |

| Add `CancellationToken` parameter to every method | Create projection classes for entities |

| Add `includeDetails = true` for single‑entity methods | Define repositories for non‑aggregate‑root entities |

| Add `includeDetails = false` for list‑returning methods | |



```csharp

// Domain layer: Repository interface

public interface IProductRepository : IBasicRepository<Product, Guid>

{

&#x20;   Task<Product> FindByNameAsync(string name, CancellationToken cancellationToken = default);

&#x20;   Task<List<Product>> GetListByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

}



// Infrastructure layer: Implementation (EF Core)

public class EfCoreProductRepository : EfCoreRepository<MyDbContext, Product, Guid>, IProductRepository

{

&#x20;   public EfCoreProductRepository(IDbContextProvider<MyDbContext> dbContextProvider)

&#x20;       : base(dbContextProvider) { }



&#x20;   public async Task<Product> FindByNameAsync(string name, CancellationToken cancellationToken = default)

&#x20;   {

&#x20;       var dbContext = await GetDbContextAsync();

&#x20;       return await dbContext.Set<Product>()

&#x20;           .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);

&#x20;   }

}

```



\### Permission \& Authorization



```csharp

// Define permissions in Application.Contracts

public static class MyPermissions

{

&#x20;   public const string GroupName = "MyModule";

&#x20;   

&#x20;   public static class Products

&#x20;   {

&#x20;       public const string Default = GroupName + ".Products";

&#x20;       public const string Create = Default + ".Create";

&#x20;       public const string Update = Default + ".Update";

&#x20;       public const string Delete = Default + ".Delete";

&#x20;   }

}



// Permission provider

public class MyPermissionDefinitionProvider : PermissionDefinitionProvider

{

&#x20;   public override void Define(IPermissionDefinitionContext context)

&#x20;   {

&#x20;       var myGroup = context.AddGroup(MyPermissions.GroupName);

&#x20;       var products = myGroup.AddPermission(MyPermissions.Products.Default);

&#x20;       products.AddChild(MyPermissions.Products.Create);

&#x20;       products.AddChild(MyPermissions.Products.Update);

&#x20;       products.AddChild(MyPermissions.Products.Delete);

&#x20;   }

}

```



\### Audit Logging



ABP automatically logs API calls with execution time, parameters, exceptions, and request IP. Control audit behavior with attributes:



```csharp

// Enable audit for specific method

\[Audited]

public class ProductAppService : ApplicationService

{

&#x20;   \[Audited]

&#x20;   public virtual async Task<ProductDto> CreateAsync(ProductCreateDto input)

&#x20;   {

&#x20;       // This method will be audited

&#x20;   }

}



// Disable audit for performance‑critical operations

\[DisableAuditing]

public virtual async Task PingAsync()

{

&#x20;   // Skip audit logging for this method

}

```



\### Data Seeding



Implement `IDataSeedContributor` for automatic data seeding through `DbMigrator` project:



```csharp

public class DefaultDataSeederContributor : IDataSeedContributor, ITransientDependency

{

&#x20;   private readonly IRepository<Product, Guid> \_productRepository;



&#x20;   public DefaultDataSeederContributor(IRepository<Product, Guid> productRepository)

&#x20;   {

&#x20;       \_productRepository = productRepository;

&#x20;   }



&#x20;   public async Task SeedAsync(DataSeedContext context)

&#x20;   {

&#x20;       if (await \_productRepository.GetCountAsync() > 0) return;

&#x20;       

&#x20;       await \_productRepository.InsertAsync(new Product("Sample Product", 99.99m));

&#x20;   }

}

```



\## 🧠 Design Patterns in ABP vNext



\### Repository Pattern

ABP implements the Repository pattern to abstract data access. Always create custom repository interfaces per aggregate root and never expose `IQueryable` to higher layers.



\### Unit of Work Pattern

ABP automatically wraps application service methods in a Unit of Work. Use `IUnitOfWorkManager` for manual control over transaction boundaries.



\### Factory Pattern

Use dependency injection for service resolution (ABP is built on Microsoft.Extensions.DependencyInjection). ABP vNext relies on ASP.NET Core‘s DI container, eliminating legacy Castle Windsor dependencies.



\### Strategy Pattern

Implement via dependency injection—register multiple implementations of an interface and use `IEnumerable<T>` or factory service to select appropriate strategy at runtime.



\### Domain Events / Event Bus

ABP provides two event bus types:



\*\*Local Event Bus\*\* (in‑process messaging):

```csharp

public class ProductCreatedHandler : ILocalEventHandler<ProductCreatedEto>, ITransientDependency

{

&#x20;   public async Task HandleEventAsync(ProductCreatedEto eventData)

&#x20;   {

&#x20;       // Handle event within same process

&#x20;   }

}



// Publish event

await \_localEventBus.PublishAsync(new ProductCreatedEto { ProductId = product.Id });

```



\*\*Distributed Event Bus\*\* (cross‑process, microservices)—configure RabbitMQ for production use:

```csharp

public class ProductCreatedHandler : IDistributedEventHandler<ProductCreatedEto>, ITransientDependency

{

&#x20;   public async Task HandleEventAsync(ProductCreatedEto eventData)

&#x20;   {

&#x20;       // Handle event across service boundaries

&#x20;   }

}



await \_distributedEventBus.PublishAsync(new ProductCreatedEto { ProductId = product.Id });

```



\### Specification Pattern

Use `ISpecification<T>` for complex query logic—reusable, composable business rules.



\### Builder Pattern

ABP’s configuration APIs extensively use the Builder pattern (e.g., `Configure<T>`, `PreConfigure<T>`).



\### Observer Pattern

ABP’s event system implements the Observer pattern for loose coupling between components.



\### Decorator Pattern

Dynamic proxies for audit logging, authorization, and Unit of Work use the Decorator pattern.



\## 🧪 Testing Strategy



\### Unit \& Integration Testing



ABP vNext uses xUnit with integrated test base classes:



```csharp

public class ProductAppServiceTests : AbpIntegratedTest<ProductAppServiceTestModule>

{

&#x20;   private readonly IProductAppService \_productAppService;



&#x20;   protected override void AfterAddApplication(IServiceCollection services)

&#x20;   {

&#x20;       // Replace real services with mocks if needed

&#x20;       services.AddTransient<IMyExternalService, MockExternalService>();

&#x20;   }



&#x20;   \[Fact]

&#x20;   public async Task Create\_Product\_Should\_Persist\_To\_Database()

&#x20;   {

&#x20;       var input = new ProductCreateDto { Name = "Test Product", Price = 99.99m };

&#x20;       var result = await \_productAppService.CreateAsync(input);

&#x20;       

&#x20;       Assert.NotNull(result);

&#x20;       Assert.Equal("Test Product", result.Name);

&#x20;   }

}

```



\*\*Best Practices:\*\*

\- Use `AbpIntegratedTest<TStartupModule>` for integration tests with full ABP module initialization

\- Use `AbpAspNetCoreIntegratedTestBase` for testing API controllers with HTTP client

\- Simulate EF Core with SQLite in‑memory for isolated database testing

\- Prefer integration tests over unit tests for repository and application service layers



\## 🌍 Multi‑Tenancy \& SaaS



\### Tenant Isolation Strategies



ABP supports multiple data isolation strategies:



| Strategy | Use Case | Implementation |

|----------|----------|----------------|

| \*\*Shared database, shared schema\*\* | Small teams, low isolation requirements | Tenant ID column + global query filter |

| \*\*Shared database, separate schemas\*\* | Medium isolation with moderate data volume | Schema per tenant |

| \*\*Separate databases\*\* | High security, large tenants | Database connection string per tenant |

| \*\*Separate databases, separate deployments\*\* | Maximum isolation | Independent infrastructure per tenant |



\### Tenant Configuration



```csharp

// Configure multi-tenancy in module

Configure<AbpMultiTenancyOptions>(options =>

{

&#x20;   options.IsEnabled = true;

});



// Resolve current tenant

public class MyService

{

&#x20;   private readonly ICurrentTenant \_currentTenant;



&#x20;   public MyService(ICurrentTenant currentTenant)

&#x20;   {

&#x20;       \_currentTenant = currentTenant;

&#x20;   }



&#x20;   public async Task ExecuteAsync()

&#x20;   {

&#x20;       var tenantId = \_currentTenant.Id;

&#x20;       var tenantName = \_currentTenant.Name;

&#x20;       

&#x20;       // Switch tenant context manually (e.g., for background jobs)

&#x20;       using (\_currentTenant.Change(tenantId))

&#x20;       {

&#x20;           // Code executes in tenant context

&#x20;       }

&#x20;   }

}

```



\## 🌐 Localization \& Internationalization



\### JSON Resource Files



Structure localization resources per module:



```

Localization/

└── MyModule/

&#x20;   ├── en.json

&#x20;   ├── zh-Hans.json

&#x20;   └── zh-Hant.json

```



Example `en.json`:

```json

{

&#x20; "MyModule:ProductName": "Product Name",

&#x20; "MyModule:Price": "Price",

&#x20; "MyModule:Validation:Required": "{0} is required"

}

```



\### Register Resources



```csharp

Configure<AbpLocalizationOptions>(options =>

{

&#x20;   options.Resources

&#x20;       .Add<MyModuleResource>("en")

&#x20;       .AddVirtualJson("/Localization/MyModule");

&#x20;   

&#x20;   options.DefaultResourceType = typeof(MyModuleResource);

});

```



\### Use in Code



```csharp

public class MyService

{

&#x20;   private readonly IStringLocalizer<MyModuleResource> \_localizer;



&#x20;   public MyService(IStringLocalizer<MyModuleResource> localizer)

&#x20;   {

&#x20;       \_localizer = localizer;

&#x20;   }



&#x20;   public string GetLocalizedString() => \_localizer\["MyModule:ProductName"];

}

```



\## 💻 .NET 9 Integration Points



\### Static Asset Delivery Optimization



Use `MapStaticAssets()` for optimized static asset delivery in Blazor, Razor Pages, and MVC apps—a drop‑in replacement for `UseStaticFiles()` that automatically handles compression, caching, and fingerprinting.



\### Performance Improvements



.NET 9 brings >1,000 performance improvements across the runtime, including enhanced JSON support and C# 13 features.



\### Security Enhancements



\- Stronger HTTPS defaults

\- Enhanced authentication support with OpenIddict

\- Improved rate limiting

\- Automatic encryption key rotation support



\### Native AOT Support



Improved Native AOT compilation for reduced memory footprint and faster startup in cloud‑native deployments.



\## 📦 Virtual File System (VFS)



Embed views, JS, CSS, images, and other files into assemblies for reuse across modules:



```csharp

// Configure virtual file system

Configure<AbpVirtualFileSystemOptions>(options =>

{

&#x20;   options.FileSets.AddEmbedded<MyModule>("MyModule");

});



// Read embedded file

public class MyService

{

&#x20;   private readonly IVirtualFileProvider \_virtualFileProvider;



&#x20;   public MyService(IVirtualFileProvider virtualFileProvider)

&#x20;   {

&#x20;       \_virtualFileProvider = virtualFileProvider;

&#x20;   }



&#x20;   public string GetEmbeddedFile()

&#x20;   {

&#x20;       var file = \_virtualFileProvider.GetFileInfo("/MyResources/config.json");

&#x20;       return file.ReadAsString();  // ABP extension method

&#x20;   }

}

```



\## 🚀 Getting Started Commands



```bash

\# Create new ABP solution with .NET 9

abp new MySaaSApp -t app -u blazor --database-provider ef --dotnet-version 9.0



\# Add module

cd MySaaSApp

abp add-module OrderManagement --new



\# Run database migrations (creates DB + seeds data)

cd aspnet-core/src/MySaaSApp.DbMigrator

dotnet run



\# Run the application

cd ../MySaaSApp.Web

dotnet run

```



\## 📚 Reference Documentation



\- ABP Framework Official Docs: https://abp.io/docs/latest

\- .NET 9 Release Notes: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9

\- ASP.NET Core 9 What‘s New: https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-9.0

\- ABP GitHub Repository: https://github.com/abpframework/abp

\- Repository Best Practices: https://abp.io/docs/latest/framework/architecture/best-practices/repositories

\- Event Bus Documentation: https://abp.io/docs/latest/framework/infrastructure/event-bus



\## 📝 Skill Application Triggers



Use this Skill when:

\- User mentions “ABP Framework”, “ABP vNext”, “ASP.NET Boilerplate”, or “ABP”

\- Building enterprise applications requiring DDD architecture

\- Developing modular monolith or microservices with .NET

\- Implementing multi‑tenant SaaS applications

\- User needs guidance on ABP project structure, modules, or best practices

\- Questions about .NET 9 integration with ABP Framework

\- Requests for repository patterns, application services, or permission management in ABP

```