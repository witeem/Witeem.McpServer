# Architecture Guidelines (.NET 9 + ABP vNext)

This document defines the architectural patterns, structural design, and domain boundaries for this project. Trae AI Agent must strictly follow these rules when creating or refactoring code.

## 1. Domain-Driven Design (DDD) Layers

The solution is divided into four standard DDD layers as prescribed by the ABP Framework.

```
+--------------------------------------------------------+
|               Distributed Services Layer               |
|         (HttpApi, HttpApi.Client, Blazor/Mvc)          |
+--------------------------+-----------------------------+
                           |
                           v
+--------------------------------------------------------+
|                    Application Layer                   |
|          (Application, Application.Contracts)          |
+--------------------------+-----------------------------+
                           |
                           v
+--------------------------------------------------------+
|                      Domain Layer                      |
|              (Domain, Domain.Shared)                   |
+--------------------------+-----------------------------+
                           |
                           v
+--------------------------------------------------------+
|                  Infrastructure Layer                  |
|          (EntityFrameworkCore, MongoDB, etc.)          |
+--------------------------------------------------------+
```

### 1.1 Domain Layer
* **`.Domain.Shared`**: Contains constants, enums, localization resources, and multi-tenant exceptions. It must not depend on any other project in the solution.
* **`.Domain`**: Core business layer. Contains **Aggregate Roots**, **Entities**, **Value Objects**, **Domain Services**, and **Repository Interfaces**.
    * *Rule:* Never inject Application Services or Repositories directly into Entities. Use Domain Services for cross-aggregate business logic.

### 1.2 Application Layer
* **`.Application.Contracts`**: Contains **DTOs (Data Transfer Objects)**, **Interfaces**, and **Permissions**.
    * *Rule:* All public endpoints must accept and return DTOs, never domain entities.
* **`.Application`**: Implements the interfaces defined in `.Application.Contracts`. Responsible for application workflow, transaction boundaries, and mapping via `ObjectMapper`.

### 1.3 Infrastructure Layer
* **`.EntityFrameworkCore`**: Contains `DbContext`, EF Core Migrations, and **Custom Repository Implementations**.
    * *Rule:* Do not write SQL or EF-specific code inside the Domain or Application layers. Keep LINQ queries inside repositories if they require complex optimizations or database-specific features.

### 1.4 Distributed Services / Presentation Layer
* **`.HttpApi`**: Contains ASP.NET Core MVC Controllers deriving from `AbpController`.
* **`.HttpApi.Client`**: Provides C# client proxies for remote service invocation.

---

## 2. Dependency Injection & Modularity

* **Module Classes**: Every project must contain an ABP Module class (inheriting from `AbpModule`) that configures its dependencies via `ConfigureServices`.
* **Lifetime Registration**:
    * Implement `ITransientDependency` for transient services (e.g., Application Services, Domain Services).
    * Implement `ISingletonDependency` for stateless, reusable engines or configurations.
    * Implement `IScopedDependency` for unit-of-work or request-bound contexts.

---

## 3. Data Consistency & Aggregate Boundaries

* **Aggregate Roots**: Modify data only through Aggregate Roots (`AggregateRoot<TKey>`).
* **Repositories**: One repository interface (`IRepository<TAggregate, TKey>`) per Aggregate Root. Do not create separate repositories for child entities; access them through their parent aggregate root.
* **Transactions**: ABP automatically manages ambient transactions via the `[UnitOfWork]` attribute on application services. Ensure any operation modifying multiple aggregates is marked as transactional if automatic interceptors are bypassed.