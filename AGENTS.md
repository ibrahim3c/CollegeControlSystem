# AGENTS.md - College Control System

Coding guidelines for .NET 9.0 ASP.NET Core Web API with Clean Architecture.

## Table of Contents

1. [Build & Test Commands](#build--test-commands)
2. [Migrations](#migrations)
3. [Project Structure](#project-structure)
4. [Code Style](#code-style)
5. [Naming Conventions](#naming-conventions)
6. [Patterns](#patterns)
7. [File Organization](#file-organization)
8. [Controller Conventions](#controller-conventions)
9. [Domain-Driven Design](#domain-driven-design)
10. [Validation](#validation)
11. [Error Handling](#error-handling)
12. [Database & EF Core](#database--ef-core)
13. [Dependencies](#dependencies)

---

## Build & Test Commands

```bash
# Build solution
dotnet build

# Build Release configuration
dotnet build --configuration Release

# Run all tests
dotnet test

# Run single test by class and method name
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Run tests with specific project
dotnet test CollegeControlSystem.Tests --filter "FullyQualifiedName~StudentServiceTests"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Build and test specific project
dotnet build CollegeControlSystem.Application
```

---

## Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project CollegeControlSystem.Infrastructure --startup-project CollegeControlSystem.Presentation

# Apply migrations to database
dotnet ef database update --project CollegeControlSystem.Infrastructure --startup-project CollegeControlSystem.Presentation

# Remove last migration (if not applied)
dotnet ef migrations remove --project CollegeControlSystem.Infrastructure --startup-project CollegeControlSystem.Presentation

# Generate SQL script
dotnet ef migrations script --project CollegeControlSystem.Infrastructure --startup-project CollegeControlSystem.Presentation --output migrations.sql
```

---

## Project Structure

```
CollegeControlSystem.sln
├── src/
│   ├── CollegeControlSystem.Domain/        # Entities, Value Objects, Domain Events, Errors
│   ├── CollegeControlSystem.Application/   # Commands, Queries, Validators, Services
│   ├── CollegeControlSystem.Infrastructure/ # EF Core, Repositories, External Services
│   └── CollegeControlSystem.Presentation/ # Controllers, Program.cs, Middlewares
└── tests/
    └── CollegeControlSystem.Tests/         # Unit & Integration Tests
```

---

## Code Style

### File Headers
- No header comments or XML documentation required
- Only namespace and using statements at top

### Using Statements
- Place `System` namespace imports first, then third-party, then project
- Use file-scoped namespaces (`namespace X.Y;`)
- No redundant `using` statements (IDE will manage)

### Type Declarations
- **Records** for: Commands, Queries, Responses/DTOs
- **Sealed classes** for: Handlers, Validators, Entities, Repositories
- **Partial classes** only when needed for source generators

### Property Modifiers
```csharp
// Immutable identifier
public Guid Id { get; private init; }

// Controlled mutation (domain logic only)
public string Name { get; private set; }

// Navigation properties
public Program Program { get; private set; }

// Collection navigation
public List<Registration> Registrations { get; private set; } = new();
```

### Implicit Usings
- `ImplicitUsings` is enabled in all projects
- `Nullable` reference types is enabled
- Do NOT add explicit `using System;` statements

---

## Naming Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Namespaces | PascalCase | `CollegeControlSystem.Domain.Students` |
| Classes/Records | PascalCase | `CreateStudentCommand` |
| Sealed Classes | PascalCase | `StudentRepository` |
| Interfaces | `I` Prefix | `IStudentRepository` |
| Methods | PascalCase | `GetByIdAsync` |
| Properties | PascalCase | `StudentName` |
| Private Fields | camelCase | `_unitOfWork`, `userManager` |
| Error Classes | `*Errors` | `StudentErrors` |
| Constants | PascalCase | `MaxCreditsWarning` |
| Files | Match Class | `CreateStudentCommand.cs` |

### Async Methods
- Always suffix with `Async`
- Never use `Async` suffix for CancellationToken parameters

---

## Patterns

### CQRS with MediatR

Commands and Queries return wrapped Result types:

```csharp
// Command interface (returns Result<T>)
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

// Query interface (returns Result<T>)
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

// Command handler
internal sealed class CreateStudentCommandHandler
    : ICommandHandler<CreateStudentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateStudentCommand request,
        CancellationToken cancellationToken)
    {
        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);
        
        return Result<Guid>.Success(student.Id);
    }
}

// Query handler
internal sealed class GetStudentProfileQueryHandler
    : IQueryHandler<GetStudentProfileQuery, StudentResponse>
{
}
```

### Result Pattern

```csharp
// Success
return Result.Success();
return Result<T>.Success(value);

// Failure
return Result.Failure(Error);
return Result<T>.Failure(error);

// Propagate failure
if (result.IsFailure)
    return Result<T>.Failure(result.Error);
```

### Entity Factory Method

```csharp
public sealed class Student : Entity
{
    private Student() { } // EF Core

    private Student(Guid id, string name, ...) : base(id)
    {
        Name = name;
    }

    public static Result<Student> Create(string name, ...)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Student>.Failure(Error.NullValue);

        return Result<Student>.Success(new Student(Guid.NewGuid(), name, ...));
    }
}
```

### Repository Pattern

```csharp
// Interface in Domain layer
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Student student);
    void Update(Student student);
}

// Implementation in Infrastructure layer
internal sealed class StudentRepository : IStudentRepository
{
}
```

### Unit of Work

```csharp
public interface IUnitOfWork
{
    IStudentRepository StudentRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

---

## File Organization

### Application Layer

```
Application/
├── FeatureName/
│   ├── FeatureNameCommand.cs
│   ├── FeatureNameCommandHandler.cs
│   ├── FeatureNameCommandValidator.cs
│   ├── FeatureNameQuery.cs
│   ├── FeatureNameQueryHandler.cs
│   └── FeatureNameResponse.cs
├── Abstractions/
│   ├── Behaviors/
│   ├── Exceptions/
│   ├── Messaging/
│   └── Services/
└── DependencyInjection.cs
```

### Domain Layer

```
Domain/
├── Entities/         # Domain entities
├── ValueObjects/     # Value objects (CourseCode, etc.)
├── Events/           # Domain events
├── Errors/           # *Errors static classes
└── Abstractions/     # Base classes (Entity, Result, IUnitOfWork)
```

---

## Controller Conventions

```csharp
[ApiController]
[Route("api/[controller]")]  // or custom route like "api/courses"
public sealed class EntityController : ControllerBase
{
    private readonly ISender _sender;

    public EntityController(ISender sender)
    {
        _sender = sender;
    }
}
```

### HTTP Status Codes

| Operation | Status Code |
|-----------|-----------|
| Create success | 201 Created (`CreatedAtAction`) |
| Read success | 200 OK (`Ok`) |
| Update success | 204 No Content (`NoContent`) |
| Delete success | 204 No Content (`NoContent`) |
| Validation error | 400 Bad Request (`BadRequest`) |
| Not found | 404 Not Found (`NotFound`) |
| Conflict | 409 Conflict (`Conflict`) |

### Return Patterns

```csharp
// Create - returns 201 with Location header
[HttpPost]
public async Task<IActionResult> Create(
    [FromBody] CreateCommand command,
    CancellationToken ct)
{
    var result = await _sender.Send(command, ct);

    if (result.IsFailure)
        return BadRequest(result.Error);

    return CreatedAtAction(
        nameof(GetById),
        new { id = result.Value },
        result.Value);
}

// Read - returns 200
[HttpGet("{id:guid}")]
public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
{
    var result = await _sender.Send(new GetByIdQuery(id), ct);

    if (result.IsFailure)
        return NotFound(result.Error);

    return Ok(result.Value);
}

// Update - returns 204
[HttpPut("{id:guid}")]
public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateCommand command,
    CancellationToken ct)
{
    var result = await _sender.Send(command with { Id = id }, ct);

    if (result.IsFailure)
        return BadRequest(result.Error);

    return NoContent();
}
```

### Authorization

```csharp
[Authorize(Roles = Roles.AdminRole)]
[Authorize(Roles = $"{Roles.AdminRole},{Roles.AdvisorRole}")]
[Authorize(Policy = "RequireEmailConfirmed")]
```

---

## Domain-Driven Design

### Value Objects

```csharp
public sealed record CourseCode
{
    private static readonly Regex Pattern = new(@"^([A-Z]{3})\s?([0-9]{3})$");

    public string Value { get; }
    public string DepartmentPart { get; }
    public int LevelPart { get; }

    private CourseCode(string value, string department, int level)
    {
        Value = value;
        DepartmentPart = department;
        LevelPart = level;
    }

    public static Result<CourseCode> Create(string code)
    {
        // Validation logic returning Result<CourseCode>
    }

    public override string ToString() => Value;
}
```

### Domain Events

```csharp
// Event class
public sealed record StudentDismissedDomainEvent : IDomainEvent
{
    public Guid StudentId { get; init; }
    public DateTime OccurredOn { get; init; }
}

// Raise from aggregate root
public class Student : Entity
{
    public void Dismiss(string reason)
    {
        Status = AcademicStatus.Dismissed;
        RaiseDomainEvent(new StudentDismissedDomainEvent(
            StudentId: Id,
            OccurredOn: DateTime.UtcNow));
    }
}
```

### Invariants & Validation

- Validate in factory methods (Entity.Create)
- Use `Result<T>` for all factory returns
- Keep invariants in single aggregate
- Cross-aggregate validation goes in Application layer

---

## Validation

### FluentValidation

```csharp
public sealed class CreateStudentCommandValidator
    : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(150)
            .MinimumLength(3)
            .WithMessage("Username must be between 3 and 150 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.ProgramId)
            .NotEmpty()
            .WithMessage("Program is required");
    }
}
```

### Validation Rules
- Validate collection sizes before accessing elements
- Use `.When()` for conditional validation
- Chain `.WithMessage()` for custom error messages

---

## Error Handling

### Domain Errors (Errors folder)

```csharp
public static class StudentErrors
{
    public static readonly Error NotFound = new("Student.NotFound", "Student not found.");
    public static readonly Error DuplicateAcademicNumber = new(
        "Student.DuplicateAcademicNumber",
        "A student with this academic number already exists.");
}
```

### Application Exceptions

```csharp
// Custom exceptions in Abstractions/Exceptions
public sealed class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }
}
```

### Middleware

```csharp
// Global exception handling middleware
public sealed class ExceptionHandlingMiddleware
{
    // Map exceptions to HTTP status codes
    // Log errors
    // ReturnProblemDetails response
}
```

---

## Database & EF Core

### Entity Configuration

```csharp
public sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.AcademicNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(s => s.AcademicNumber).IsUnique();

        builder.HasOne(s => s.Program)
            .WithMany()
            .HasForeignKey(s => s.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### Value Object Conversion

```csharp
builder.Property(c => c.Code)
    .HasConversion(
        code => code.Value,
        value => CourseCode.Create(value).Value)
    .HasColumnName("Code")
    .HasMaxLength(10);
```

### Migrations
- Always specify project flags
- Use descriptive migration names
- Review generated SQL before applying to production

---

## Dependencies

| Project | Key Packages |
|---------|-------------|
| Domain | MediatR, Microsoft.AspNetCore.Identity.EntityFrameworkCore |
| Application | FluentValidation, MediatR, Serilog.AspNetCore |
| Infrastructure | Npgsql.EntityFrameworkCore.PostgreSQL, brevo_csharp, JWT Bearer |
| Presentation | Swashbuckle.AspNetCore, Serilog.Sinks.Console, HealthChecks |

---

## Notes

- `ImplicitUsings` and `Nullable` are enabled in all projects
- No explicit `using System;` statements needed
- Handlers are `internal sealed` by default
- Controllers are `public sealed`
- Use `cancellationToken` parameter on all async methods
- Prefix async fields with underscore (`_userManager`)