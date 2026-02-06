# Development Guide

Guide for developers contributing to or building upon RBPortKiller.

## Getting Started

### Prerequisites

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 SDK or later
- Git
- Terminal (PowerShell, Windows Terminal, or Command Prompt)
- IDE (Visual Studio 2022, VS Code, or Rider recommended)

### Initial Setup

```powershell
# Clone the repository
git clone https://github.com/rahul-a-bangera/RBPortKiller.git
cd RBPortKiller

# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run the CLI
dotnet run --project RBPortKiller.CLI
```

## Project Structure

```
RBPortKiller/
??? .github/
?   ??? copilot-instructions.md     # GitHub Copilot guidelines
??? RBPortKiller.Core/              # Core business logic
??? RBPortKiller.Infrastructure/    # Platform implementations
??? RBPortKiller.CLI/               # User interface
??? TestPortOpener/                 # Testing utility
??? Misc/Docs/                      # Documentation
??? README.md                       # Project overview
??? CONTRIBUTING.md                 # Contribution guidelines
??? LICENSE                         # MIT License
```

## Building

### Debug Build

```powershell
dotnet build
```

### Release Build

```powershell
dotnet build -c Release
```

### Single File Publish

```powershell
dotnet publish RBPortKiller.CLI `
    -c Release `
    -r win-x64 `
    --self-contained `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -o publish
```

Output: `publish/rbportkiller.exe`

## Running

### Development Mode

```powershell
dotnet run --project RBPortKiller.CLI
```

### Published Executable

```powershell
.\publish\rbportkiller.exe
```

### With Debugger

**Visual Studio**: F5 or Debug > Start Debugging

**VS Code**: F5 with launch.json configured

## Testing

### Manual Testing with TestPortOpener

TestPortOpener creates dummy TCP/UDP listeners for testing.

**Terminal 1** (run test ports):
```powershell
dotnet run --project TestPortOpener
```

**Terminal 2** (run RBPortKiller):
```powershell
dotnet run --project RBPortKiller.CLI
```

**Verify**:
- TestPortOpener ports appear in the list
- Creation times are accurate
- System processes are filtered out
- Process termination works correctly

See `TestPortOpener/README.md` for detailed testing scenarios.

### Testing Checklist

- [ ] Port discovery shows all active ports
- [ ] Creation times display correctly
- [ ] System processes are hidden
- [ ] Ports sorted by creation time (newest first)
- [ ] Process termination works
- [ ] Permission errors handled gracefully
- [ ] Refresh updates port list
- [ ] Ctrl+C exits cleanly

## Code Standards

### SOLID Principles

Must follow SOLID design principles:

1. **Single Responsibility**: One class, one purpose
2. **Open/Closed**: Extend through new classes, not modifications
3. **Liskov Substitution**: Implementations substitutable for interfaces
4. **Interface Segregation**: Focused, minimal interfaces
5. **Dependency Inversion**: Depend on abstractions

### Naming Conventions

| Type | Convention | Example |
|------|------------|---------|
| Class | PascalCase, descriptive | `PortKillerService` |
| Interface | IPascalCase | `IPortDiscoveryService` |
| Method | PascalCase, verb | `GetActivePortsAsync` |
| Property | PascalCase | `ProcessName` |
| Field (private) | _camelCase | `_portKillerService` |
| Parameter | camelCase | `portInfo` |
| Local variable | camelCase | `activePort` |

### Class Suffixes

Use descriptive suffixes to indicate purpose:

- **Service**: Business logic (`PortKillerService`)
- **Provider**: Data retrieval (`ProcessInfoProvider`)
- **Executor**: Command execution (`NetstatCommandExecutor`)
- **Parser**: Data transformation (`NetstatOutputParser`)
- **Builder**: Object construction (`PortTableBuilder`)
- **Mapper**: Type mapping (`StateColorMapper`)
- **Detector**: Identification (`SystemProcessDetector`)
- **Resolver**: Decision making (`ProtocolResolver`)
- **Formatter**: Presentation (`PortSelectionFormatter`)

### File Organization

- One class per file
- File name matches class name exactly
- Group related classes in subfolders
- Use proper namespaces

### Code Style

```csharp
// Class: sealed unless inheritance intended
public sealed class MyService
{
    // Fields: readonly for dependencies
    private readonly IDependency _dependency;
    
    // Constructor: parameter validation
    public MyService(IDependency dependency)
    {
        _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
    }
    
    // Methods: XML documentation
    /// <summary>
    /// Does something useful.
    /// </summary>
    /// <param name="input">The input value</param>
    /// <returns>The result</returns>
    public async Task<Result> DoSomethingAsync(string input)
    {
        // Implementation
    }
}
```

### XML Documentation

Required for all public classes and members:

```csharp
/// <summary>
/// Brief description of what this does.
/// </summary>
public class MyClass
{
    /// <summary>
    /// Gets or sets the property description.
    /// </summary>
    public string Property { get; set; }
    
    /// <summary>
    /// Method description.
    /// </summary>
    /// <param name="parameter">Parameter description</param>
    /// <returns>Return value description</returns>
    public string Method(string parameter)
    {
        // Implementation
    }
}
```

## Architecture Guidelines

### Layer Separation

**Core Layer** (platform-agnostic):
- Domain models
- Service interfaces
- Business logic
- No platform-specific code

**Infrastructure Layer** (platform-specific):
- OS API interactions
- Command execution
- Data parsing
- Platform detection

**Presentation Layer** (UI):
- User interaction
- Display formatting
- Input handling
- No business logic

### Dependency Flow

```
CLI (depends on) ? Core (depends on) ? Infrastructure
```

**Never**:
- Infrastructure depends on CLI
- Core depends on Infrastructure

### Creating New Classes

**When to extract**:
- Method exceeds 30 lines
- Multiple responsibilities detected
- Code duplication (DRY violation)
- Difficult to test
- Magic strings or hardcoded values

**Extraction pattern**:
```csharp
// Before: Large method
public void DoEverything()
{
    // I/O operation
    var data = ExecuteCommand();
    
    // Parsing
    var parsed = ParseData(data);
    
    // Business logic
    var result = ProcessData(parsed);
    
    // Formatting
    DisplayResult(result);
}

// After: Extracted to focused classes
private readonly IExecutor _executor;
private readonly IParser _parser;
private readonly IProcessor _processor;
private readonly IFormatter _formatter;

public void DoOneThing()
{
    var data = _executor.Execute();
    var parsed = _parser.Parse(data);
    var result = _processor.Process(parsed);
    _formatter.Display(result);
}
```

### Dependency Injection

**Registration** (`ServiceConfiguration.cs`):
```csharp
services.AddSingleton<IMyService, MyService>();
```

**Consumption** (constructor injection):
```csharp
public class MyClass
{
    private readonly IMyService _service;
    
    public MyClass(IMyService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
}
```

**Avoid**: Service locator pattern in business logic

## Adding Features

### Adding a New Platform

1. Create folder: `RBPortKiller.Infrastructure/{Platform}/`
2. Implement `IPortDiscoveryService`
3. Implement `IProcessManagementService`
4. Update `PlatformServiceFactory`
5. Test on target platform

Example:
```csharp
// Infrastructure/Linux/LinuxPortDiscoveryService.cs
public sealed class LinuxPortDiscoveryService : IPortDiscoveryService
{
    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken ct)
    {
        // Use /proc/net/tcp, /proc/net/udp or ss command
        // Parse output
        // Return PortInfo objects
    }
}

// Update PlatformServiceFactory.cs
public static IPortDiscoveryService CreatePortDiscoveryService()
{
    if (PlatformDetector.IsWindows()) return new WindowsPortDiscoveryService();
    if (PlatformDetector.IsLinux()) return new LinuxPortDiscoveryService();
    // ...
}
```

### Adding UI Components

1. Create in `RBPortKiller.CLI/UI/`
2. Use builder or formatter pattern
3. Keep stateless (static methods)
4. Separate data from presentation

Example:
```csharp
// CLI/UI/MyComponentBuilder.cs
public static class MyComponentBuilder
{
    public static Panel BuildPanel(MyData data)
    {
        var panel = new Panel($"[cyan]{data.Title}[/]");
        // Configure panel
        return panel;
    }
}
```

### Adding Business Logic

1. Update or create service in `RBPortKiller.Core/Services/`
2. Add interface if new service
3. Implement with dependency injection
4. Register in `ServiceConfiguration`

## Error Handling

### Expected Failures

Return default values or result objects:

```csharp
public ProcessInfo GetProcessInfo(int pid)
{
    try
    {
        var process = Process.GetProcessById(pid);
        return new ProcessInfo(process.ProcessName, process.MainModule?.FileName);
    }
    catch (ArgumentException)
    {
        // Process not found
        return new ProcessInfo("Unknown", null);
    }
    catch (Win32Exception)
    {
        // Access denied
        return new ProcessInfo("Access Denied", null);
    }
}
```

### Unexpected Errors

Throw exceptions with context:

```csharp
public void ValidatePort(int port)
{
    if (port < 0 || port > 65535)
    {
        throw new ArgumentOutOfRangeException(nameof(port), 
            "Port must be between 0 and 65535");
    }
}
```

### User-Facing Errors

Provide actionable messages:

```csharp
if (result.IsPermissionDenied)
{
    AnsiConsole.MarkupLine("[red]Error:[/] Access denied");
    AnsiConsole.MarkupLine("[yellow]Tip:[/] Try running as administrator");
}
```

## Performance

### Async/Await

Use for I/O-bound operations:

```csharp
public async Task<List<PortInfo>> GetPortsAsync(CancellationToken ct)
{
    return await Task.Run(() =>
    {
        // I/O operation
        var ports = DiscoverPorts();
        return ports;
    }, ct);
}
```

### Cancellation Tokens

Always accept and respect:

```csharp
public async Task<Data> GetDataAsync(CancellationToken ct = default)
{
    foreach (var item in items)
    {
        ct.ThrowIfCancellationRequested();
        // Process item
    }
}
```

### Resource Management

Dispose of resources:

```csharp
using var process = Process.GetProcessById(pid);
// Use process
```

## Debugging

### Debug Configuration

Launch settings (`Properties/launchSettings.json`):
```json
{
  "profiles": {
    "RBPortKiller.CLI": {
      "commandName": "Project",
      "commandLineArgs": ""
    }
  }
}
```

### Common Debug Scenarios

**Port discovery not working**:
- Check `WindowsPortDiscoveryService.GetActivePortsAsync`
- Verify `ProcessInfoProvider` enrichment
- Confirm `SystemProcessDetector` filtering

**Process termination failing**:
- Check `WindowsProcessManagementService.TerminateProcessAsync`
- Verify permissions
- Confirm process exists

**UI not displaying correctly**:
- Check `PortTableBuilder` formatting
- Verify `StateColorMapper` colors
- Confirm data passed correctly

## Git Workflow

### Branch Naming

- `feature/description`: New features
- `fix/description`: Bug fixes
- `refactor/description`: Code improvements
- `docs/description`: Documentation updates

### Commit Messages

Format: `<type>: <description>`

Types:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code refactoring
- `docs`: Documentation
- `style`: Formatting
- `test`: Tests
- `chore`: Maintenance

Examples:
- `feat: add Linux platform support`
- `fix: handle process access denied error`
- `refactor: extract parser to separate class`
- `docs: update architecture documentation`

### Pull Request Process

1. Fork the repository
2. Create feature branch
3. Make changes following code standards
4. Test thoroughly
5. Commit with clear messages
6. Push to your fork
7. Create pull request
8. Address review feedback

## Documentation

### Documentation Files

All documentation goes in `Misc/Docs/`:
- **Never** create .md files in project root (except README.md)
- **Never** create .md files in source directories
- Group related docs in Misc/Docs

### Updating Documentation

When adding features:
1. Update `README.md` (overview)
2. Update `Misc/Docs/USAGE.md` (user guide)
3. Update `Misc/Docs/ARCHITECTURE.md` (if architecture changes)
4. Update `Misc/Docs/DEVELOPMENT.md` (if development process changes)

## Troubleshooting

### Build Errors

**Missing SDK**:
```powershell
dotnet --version  # Verify 8.0+
```

**Restore issues**:
```powershell
dotnet clean
dotnet restore
dotnet build
```

### Runtime Errors

**PlatformNotSupportedException**:
- Check `PlatformDetector` logic
- Verify platform implementations exist

**DI Resolution Errors**:
- Check `ServiceConfiguration` registrations
- Verify constructor parameters match registrations

## Resources

### Internal Documentation

- `README.md`: Project overview
- `Misc/Docs/USAGE.md`: User guide
- `Misc/Docs/ARCHITECTURE.md`: Technical details
- `.github/copilot-instructions.md`: Copilot coding standards
- `TestPortOpener/README.md`: Testing guide

### External Resources

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Spectre.Console](https://spectreconsole.net/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

## Getting Help

- Review existing code for patterns
- Check documentation files
- Refer to `.github/copilot-instructions.md` for standards
- Create GitHub issue for bugs or questions

## Next Steps

- Read `Misc/Docs/ARCHITECTURE.md` for technical details
- Review existing implementations as examples
- Start with small changes to familiarize yourself
- Follow code standards and SOLID principles
- Test your changes thoroughly before submitting
