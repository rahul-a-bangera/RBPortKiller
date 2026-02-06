# GitHub Copilot Instructions for RBPortKiller

## Project Overview
RBPortKiller is a cross-platform CLI tool for managing network ports and processes, built with .NET 8 and following SOLID principles.

---

## Documentation Standards

**IMPORTANT: Do not create .md files in the project root or source directories.**

- ? All documentation files (.md) MUST be placed in: `Misc/Docs/`
- ? Examples: `Misc/Docs/REFACTORING_SUMMARY.md`, `Misc/Docs/ARCHITECTURE.md`
- ? Never create documentation in: project root, `RBPortKiller.CLI/`, `RBPortKiller.Core/`, `RBPortKiller.Infrastructure/`
- Exceptions: `README.md` in project root only

---

## Code Style & Architecture

### Namespace Organization
- **CLI Layer**: `RBPortKiller.CLI`
  - Configuration: `RBPortKiller.CLI.Configuration`
  - UI Components: `RBPortKiller.CLI.UI`
- **Core Layer**: `RBPortKiller.Core`
  - Models: `RBPortKiller.Core.Models`
  - Interfaces: `RBPortKiller.Core.Interfaces`
  - Services: `RBPortKiller.Core.Services`
- **Infrastructure Layer**: `RBPortKiller.Infrastructure`
  - Platform: `RBPortKiller.Infrastructure.Platform`
  - Windows: `RBPortKiller.Infrastructure.Windows`
    - Commands: `RBPortKiller.Infrastructure.Windows.Commands`
    - Parsers: `RBPortKiller.Infrastructure.Windows.Parsers`
    - Helpers: `RBPortKiller.Infrastructure.Windows.Helpers`

### SOLID Principles (MANDATORY)
1. **Single Responsibility**: Each class should have ONE clear purpose
2. **Open/Closed**: Extend functionality through new classes, not modifying existing ones
3. **Liskov Substitution**: Implementations must be substitutable for their interfaces
4. **Interface Segregation**: Keep interfaces focused and minimal
5. **Dependency Inversion**: Depend on abstractions, inject dependencies via constructors

### Class Design
- ? Use `sealed` for classes not intended for inheritance
- ? Mark infrastructure helpers as `internal` unless needed externally
- ? Constructor injection for all dependencies
- ? Validate constructor parameters with `ArgumentNullException`
- ? Use `readonly` fields for injected dependencies

### Separation of Concerns
- **Extractors**: Separate I/O operations (file, process, network) from business logic
- **Parsers**: Pure functions for data transformation
- **Formatters**: UI/presentation logic separate from business logic
- **Providers**: Encapsulate data retrieval
- **Resolvers**: Centralize decision-making logic

### Naming Conventions
- **Commands/Executors**: Classes that perform I/O operations (e.g., `NetstatCommandExecutor`)
- **Parsers**: Classes that transform data (e.g., `NetstatOutputParser`)
- **Providers**: Classes that retrieve information (e.g., `ProcessInfoProvider`)
- **Builders**: Classes that construct complex objects (e.g., `PortTableBuilder`)
- **Mappers**: Classes that map between types (e.g., `StateColorMapper`)
- **Resolvers**: Classes that make decisions (e.g., `ProtocolResolver`)

### Error Handling
- ? Use try-catch for expected failures (permissions, process not found)
- ? Return default values (null, empty, 0) instead of throwing for expected failures
- ? Throw exceptions for invalid arguments and programming errors
- ? Log errors appropriately (when logging is implemented)

### Code Comments
- ? XML documentation (`///`) for all public classes and methods
- ? Use `<summary>`, `<param>`, `<returns>` tags
- ? Avoid inline comments unless explaining complex algorithms
- ? Code should be self-documenting through clear naming

---

## Refactoring Guidelines

### When to Extract a New Class
1. Method exceeds 30 lines
2. Multiple responsibilities detected in one class
3. Code duplication (DRY violation)
4. Hard to test due to tight coupling
5. Magic strings or hardcoded values

### Extraction Pattern
```csharp
// Before: Large method with multiple concerns
public void DoEverything() 
{
    // I/O operation
    // Parsing logic
    // Business logic
    // Formatting
}

// After: Extracted to focused classes
private readonly IExecutor _executor;
private readonly IParser _parser;
private readonly IFormatter _formatter;

public void DoOneThing()
{
    var data = _executor.Execute();
    var parsed = _parser.Parse(data);
    return _formatter.Format(parsed);
}
```

### Testing Considerations
- Extract pure functions for easy testing
- Use interfaces for mockable dependencies
- Avoid static methods for logic that needs testing
- Keep I/O operations separate from business logic

---

## Platform-Specific Code

### Adding New Platform Support
1. Create new folder: `RBPortKiller.Infrastructure/{Platform}/`
2. Implement interfaces: `IPortDiscoveryService`, `IProcessManagementService`
3. Update `PlatformServiceFactory` with new platform detection
4. Follow Windows implementation pattern (Commands, Parsers, Helpers)

### Platform Detection
- ? Always use `PlatformDetector` class
- ? Never use `RuntimeInformation.IsOSPlatform()` directly
- ? Use helper methods: `PlatformDetector.IsWindows()`, `IsLinux()`, `IsMacOS()`

---

## Dependency Injection

### Service Registration
- ? All service registration in `ServiceConfiguration.cs`
- ? Use extension method pattern: `services.AddRBPortKillerServices()`
- ? Register interfaces, not concrete types (where applicable)
- ? Avoid service locator pattern in business logic

### DI Container Usage
```csharp
// ? Correct: Constructor injection
public class MyService
{
    private readonly IDependency _dependency;
    
    public MyService(IDependency dependency)
    {
        _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
    }
}

// ? Incorrect: Service locator
public class MyService
{
    public void DoWork(IServiceProvider services)
    {
        var dependency = services.GetRequiredService<IDependency>();
    }
}
```

---

## UI/CLI Guidelines

### Spectre.Console Usage
- ? Extract UI components to `RBPortKiller.CLI.UI` namespace
- ? Separate builders for tables, panels, prompts
- ? Centralize color schemes in mappers
- ? Format data before passing to UI components

### User Experience
- Always show loading indicators for async operations
- Provide clear error messages with actionable suggestions
- Use colors consistently (green=success, red=error, yellow=warning, cyan=info)
- Include "Press any key to continue" after important operations

---

## File Creation Rules

### New Class Checklist
- [ ] Placed in correct namespace/folder
- [ ] XML documentation added
- [ ] Constructor validates parameters
- [ ] Dependencies injected via constructor
- [ ] Follows single responsibility principle
- [ ] Marked as `sealed` or `internal` if appropriate
- [ ] Unit tests created (when testing infrastructure exists)

### Naming
- Use descriptive names that indicate purpose
- Avoid abbreviations unless widely recognized
- Match file name to class name exactly
- Group related classes in subfolders

---

## Performance Considerations

- ? Use `async`/`await` for I/O operations
- ? Pass `CancellationToken` for long-running operations
- ? Use `Task.Run()` for CPU-bound work on background threads
- ? Dispose of `Process` objects and other `IDisposable` resources
- ? Use `string.Contains()` with `StringComparison` for case-insensitive comparisons

---

## Security Considerations

- ? Handle permission-denied scenarios gracefully
- ? Validate process IDs before terminating processes
- ? Show warnings for elevated privilege requirements
- ? Avoid displaying sensitive information (full file paths minimized)

---

## Version Targeting

- **Framework**: .NET 8
- **C# Version**: Latest (implicit with .NET 8)
- Use modern C# features: records, file-scoped namespaces, pattern matching, string interpolation

---

## Summary

**Key Rules to Remember:**
1. ?? **Documentation goes in `Misc/Docs/` ONLY**
2. ??? **Follow SOLID principles religiously**
3. ?? **Extract, don't expand** - create new classes instead of growing existing ones
4. ?? **Design for testability** - separate I/O from logic
5. ?? **Use proper namespaces** - organize by layer and purpose
6. ?? **Constructor injection always** - no service locator pattern
7. ?? **Separate UI from logic** - CLI layer components only for presentation

When in doubt, look at existing refactored code (WindowsPortDiscoveryService, PortKillerCli) as examples of proper architecture.
