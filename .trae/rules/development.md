# Development & Coding Standards (.NET 9 + ABP vNext)

This document outlines the coding standards, async patterns, naming conventions, and validation frameworks required for development. Trae AI must ensure all generated code conforms to these practices.

## 1. C# 13 & .NET 9 Modern Features

Leverage cutting-edge C# and .NET 9 features to ensure optimal memory allocation, readability, and performance.

* **Primary Constructors**: Use primary constructors for dependency injection in classes, domain services, and application services to reduce boilerplate.
* **Collection Expressions**: Use `[]` syntax for initializing arrays, lists, and spans instead of `new List<T>()` or `new T[] {}`.
* **Implicit Backing Fields**: Use `field` keyword where applicable for properties requiring encapsulated logic without defining explicit backing variables.
* **Frozen Collections**: Use `System.Collections.Immutable` and modern .NET 9 frozen structures (`ToFrozenDictionary`, `ToFrozenSet`) for lookup data that remains read-only after startup.

---

## 2. Naming Conventions & Code Style

* **Interfaces**: Must be prefixed with `I` (e.g., `IProductAppService`).
* **DTOs**: Append `Dto`, `CreateUpdateDto`, or `GetListInput` to the class name (e.g., `ProductDto`, `ProductCreateDto`).
* **Async Methods**: Every asynchronous method must append the `Async` suffix and accept an optional `CancellationToken cancellationToken = default`.
* **Dependency Injection**: Name injected private fields with a camelCase prefix matching the interface name without the `I` (e.g., `IProductRepository` becomes `productRepository`).

---

## 3. Exception Handling & Validation

* **Validation**: Use `FluentValidation` or Data Annotations within the `.Application.Contracts` DTO models. Do not write manual `if` validation checks inside Application Service implementations.
* **Domain Exceptions**: Throw exceptions deriving from `UserFriendlyException` or custom `BusinessException` inside the Domain Layer. Let the ABP global exception filter catch them and convert them to secure JSON responses automatically. Do not use generic `try-catch` blocks that swallow exceptions.

---

## 4. Async & Threading Best Practices

* **Task Performance**: Always append `.ConfigureAwait(false)` in infrastructure libraries or non-UI bound processes if applicable, though ASP.NET Core does not have a `SynchronizationContext`.
* **LINQ Operations**: Always use EF Core async extensions (e.g., `ToListAsync()`, `FirstOrDefaultAsync()`) when fetching data from repositories to prevent blocking thread pools.

---

## 5. Performance & Query Optimization

* **No-Tracking Queries**: Use `IRepository.GetQueryableAsync()` and apply `.AsNoTracking()` for read-only dashboards, lists, and search lookups to save EF Core change tracker overhead.
* **Pagination**: All list endpoints must inherit from `PagedAndSortedResultRequestDto` and strictly enforce maximum page limits (`MaxResultCount`) to prevent memory exhaustion from massive SQL payloads.