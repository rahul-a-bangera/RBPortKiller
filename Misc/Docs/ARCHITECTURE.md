# Architecture

Technical overview of RBPortKiller's design, structure, and implementation.

## Overview

RBPortKiller follows clean architecture principles with clear separation between business logic, infrastructure, and presentation layers.

### Core Principles

- **SOLID Design**: Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
- **Dependency Injection**: Constructor injection for loose coupling
- **Platform Abstraction**: OS-specific code isolated from business logic
- **Separation of Concerns**: Clear boundaries between layers

## Project Structure

```
RBPortKiller/
??? RBPortKiller.Core/              # Domain layer (platform-agnostic)
??? RBPortKiller.Infrastructure/    # Infrastructure layer (platform-specific)
??? RBPortKiller.CLI/               # Presentation layer (user interface)
??? TestPortOpener/                 # Testing utility
```

## Layer Details

### RBPortKiller.Core

Platform-agnostic business logic and domain models.

**Namespaces**:
- `RBPortKiller.Core.Models`: Domain entities
- `RBPortKiller.Core.Interfaces`: Service abstractions
- `RBPortKiller.Core.Services`: Business logic services

**Key Components**:

**PortInfo Model**:
```csharp
public sealed class PortInfo
{
    public int Port { get; init; }
    public Protocol Protocol { get; init; }
    public int ProcessId { get; init; }
    public string ProcessName { get; init; }
    public string LocalAddress { get; init; }
    public string? RemoteAddress { get; init; }
    public string? State { get; init; }
    public DateTime? CreatedDate { get; init; }
    public bool IsSystemProcess { get; init; }
}
```

**IPortKillerService Interface**:
- `GetActivePortsAsync()`: Retrieve all active ports
- `TerminateProcessAsync()`: Kill a process
- `CanTerminateProcess()`: Check termination permissions

**PortKillerService Implementation**:
- Orchestrates port discovery and process management
- Delegates platform-specific work to infrastructure layer
- Returns domain models to presentation layer

### RBPortKiller.Infrastructure

Platform-specific implementations.

**Structure**:
```
Infrastructure/
??? Platform/
?   ??? PlatformDetector.cs          # OS detection
??? PlatformServiceFactory.cs        # Factory pattern for platform services
??? Windows/                         # Windows implementation
    ??? WindowsPortDiscoveryService.cs
    ??? WindowsProcessManagementService.cs
    ??? Commands/                    # Command execution
    ?   ??? NetstatCommandExecutor.cs
    ??? Parsers/                     # Output parsing
    ?   ??? NetstatOutputParser.cs
    ??? Helpers/                     # Utility classes
        ??? ProcessInfoProvider.cs
        ??? ProcessCreationTimeProvider.cs
        ??? ProtocolResolver.cs
        ??? SystemProcessDetector.cs
```

**Key Patterns**:

**Command Pattern**: `NetstatCommandExecutor`
- Executes `netstat` command
- Returns raw output string
- Handles errors and edge cases

**Parser Pattern**: `NetstatOutputParser`
- Transforms raw output to structured data
- Pure function (no I/O)
- Returns lists of parsed objects

**Provider Pattern**: `ProcessInfoProvider`
- Retrieves process details (name, path, creation time)
- Wraps `System.Diagnostics.Process` API
- Returns default values on failure

**Detector Pattern**: `SystemProcessDetector`
- Identifies critical system processes
- Returns boolean (is system process)
- Used for filtering

**Resolver Pattern**: `ProtocolResolver`
- Determines protocol from string
- Centralizes protocol detection logic

### RBPortKiller.CLI

User interface and presentation logic.

**Structure**:
```
CLI/
??? Program.cs                       # Entry point, DI setup
??? PortKillerCli.cs                 # Main orchestration
??? Configuration/
?   ??? ServiceConfiguration.cs      # DI container setup
??? UI/                              # UI components
    ??? PortTableBuilder.cs          # Table formatting
    ??? PortDetailsPanelBuilder.cs   # Details panel
    ??? PortSelectionFormatter.cs    # Selection menu
    ??? StateColorMapper.cs          # Color coding
    ??? PortSelectionAction.cs       # Action enum
    ??? PortSelectionResult.cs       # Selection result
```

**Key Patterns**:

**Builder Pattern**: `PortTableBuilder`, `PortDetailsPanelBuilder`
- Construct complex UI elements
- Static methods for stateless building
- Separation of data from presentation

**Mapper Pattern**: `StateColorMapper`
- Maps connection states to colors
- Centralized color scheme
- Easy to modify visual theme

**Formatter Pattern**: `PortSelectionFormatter`
- Formats data for selection menus
- Handles choice creation and parsing
- Decouples formatting from business logic

**Result Pattern**: `PortSelectionResult`
- Encapsulates user action
- Type-safe action handling
- Factory methods for different results

## Data Flow

### Port Discovery Flow

```
User Request
    ?
PortKillerCli.RunAsync()
    ?
PortKillerService.GetActivePortsAsync()
    ?
WindowsPortDiscoveryService.GetActivePortsAsync()
    ?
.NET IPGlobalProperties API
    ?
ProcessInfoProvider (enrich with process details)
    ?
SystemProcessDetector (filter system processes)
    ?
Sort by CreatedDate (newest first)
    ?
PortTableBuilder.BuildPortTable()
    ?
Display to User
```

### Process Termination Flow

```
User Selects Port
    ?
PortKillerCli.HandlePortActionAsync()
    ?
Display Confirmation Prompt
    ?
PortKillerService.TerminateProcessAsync()
    ?
WindowsProcessManagementService.TerminateProcessAsync()
    ?
System.Diagnostics.Process.Kill()
    ?
Return ProcessTerminationResult
    ?
Display Success/Error Message
```

## Dependency Injection

### Service Registration

**ServiceConfiguration.cs**:
```csharp
public static IServiceCollection AddRBPortKillerServices(this IServiceCollection services)
{
    // Core services
    services.AddSingleton<IPortKillerService, PortKillerService>();
    
    // Platform-specific services (factory pattern)
    services.AddSingleton<IPortDiscoveryService>(sp => 
        PlatformServiceFactory.CreatePortDiscoveryService());
    services.AddSingleton<IProcessManagementService>(sp => 
        PlatformServiceFactory.CreateProcessManagementService());
    
    // CLI
    services.AddTransient<PortKillerCli>();
    
    return services;
}
```

**Benefits**:
- Testable: Easy to mock dependencies
- Flexible: Swap implementations without changing consumers
- Maintainable: Single registration point

## Design Patterns

### Creational Patterns

- **Factory Pattern**: `PlatformServiceFactory` creates platform-specific services
- **Builder Pattern**: `PortTableBuilder` constructs complex tables

### Structural Patterns

- **Facade Pattern**: `PortKillerService` simplifies complex subsystems
- **Adapter Pattern**: Infrastructure layer adapts OS APIs to domain interfaces

### Behavioral Patterns

- **Strategy Pattern**: Different implementations for `IPortDiscoveryService` per platform
- **Command Pattern**: `NetstatCommandExecutor` encapsulates command execution

## Error Handling

### Philosophy

- **Expected failures**: Return default values or result objects
- **Unexpected errors**: Throw exceptions
- **User-facing errors**: Friendly messages with actionable suggestions

### Implementation

**Process Termination**:
```csharp
public async Task<ProcessTerminationResult> TerminateProcessAsync(PortInfo portInfo)
{
    try
    {
        // Attempt termination
        process.Kill();
        return ProcessTerminationResult.Success();
    }
    catch (Win32Exception ex) when (ex.Message.Contains("Access is denied"))
    {
        return ProcessTerminationResult.Failure("Access denied", isPermissionDenied: true);
    }
    catch (InvalidOperationException)
    {
        return ProcessTerminationResult.Failure("Process has already exited");
    }
}
```

**Benefits**:
- No exception propagation to UI layer
- Type-safe result objects
- Context-specific error messages

## Threading and Async

### Async Operations

- Port discovery: I/O-bound (network queries)
- Process termination: I/O-bound (OS calls)
- UI operations: Synchronous (Spectre.Console)

### Cancellation Support

All async methods accept `CancellationToken`:
- Ctrl+C triggers cancellation
- Graceful shutdown on user interrupt
- No resource leaks

## Platform Abstraction

### Current Implementation: Windows

Uses .NET's `System.Net.NetworkInformation.IPGlobalProperties`:
- `GetActiveTcpConnections()`: Active TCP connections
- `GetActiveTcpListeners()`: Listening TCP ports
- `GetActiveUdpListeners()`: Listening UDP ports

### Adding New Platforms

To support Linux or macOS:

1. Create new directory: `Infrastructure/{Platform}/`
2. Implement interfaces:
   - `IPortDiscoveryService`
   - `IProcessManagementService`
3. Update `PlatformServiceFactory`:
   ```csharp
   public static IPortDiscoveryService CreatePortDiscoveryService()
   {
       if (PlatformDetector.IsWindows()) return new WindowsPortDiscoveryService();
       if (PlatformDetector.IsLinux()) return new LinuxPortDiscoveryService();
       if (PlatformDetector.IsMacOS()) return new MacOSPortDiscoveryService();
       throw new PlatformNotSupportedException();
   }
   ```

## Code Organization

### Namespace Conventions

| Namespace | Purpose |
|-----------|---------|
| `RBPortKiller.Core.Models` | Domain entities |
| `RBPortKiller.Core.Interfaces` | Service abstractions |
| `RBPortKiller.Core.Services` | Business logic |
| `RBPortKiller.Infrastructure.Platform` | OS detection |
| `RBPortKiller.Infrastructure.Windows` | Windows-specific code |
| `RBPortKiller.Infrastructure.Windows.Commands` | Command executors |
| `RBPortKiller.Infrastructure.Windows.Parsers` | Output parsers |
| `RBPortKiller.Infrastructure.Windows.Helpers` | Utility classes |
| `RBPortKiller.CLI.UI` | UI components |
| `RBPortKiller.CLI.Configuration` | DI setup |

### File Naming

- Class name matches file name exactly
- One class per file
- Related classes grouped in folders

### Access Modifiers

- **public**: Domain models, service interfaces
- **internal**: Infrastructure helpers, parsers
- **sealed**: Classes not designed for inheritance
- **readonly**: Injected dependencies

## Testing Strategy

### Testability Features

- **Interface-based design**: Easy to mock
- **Pure functions**: Parsers, formatters (no state)
- **Dependency injection**: Swap real for test implementations
- **Separation of I/O and logic**: Test business logic independently

### Test Utility: TestPortOpener

- Opens dummy TCP/UDP ports
- Tests port discovery
- Validates creation time sorting
- Verifies system process filtering

## Performance Considerations

### Port Discovery

- **Typical time**: <1 second for ~100 ports
- **Bottleneck**: Process info enrichment (requires per-process queries)
- **Optimization**: Parallel processing considered but adds complexity

### Memory

- **Port list**: ~1KB per PortInfo object
- **Typical usage**: <1MB for 1000 ports
- **No caching**: Fresh data on each discovery

## Security

### System Process Protection

Critical processes are filtered out:
- Prevents accidental termination
- Hardcoded list of known system processes
- Based on process name (case-insensitive)

### Permission Handling

- Pre-flight checks before termination
- Clear error messages on permission denial
- No privilege elevation by the tool itself

## Dependencies

### External Libraries

- **Microsoft.Extensions.DependencyInjection**: DI container
- **Spectre.Console**: Rich terminal UI

### Framework

- **.NET 8**: LTS release
- **C# 12**: Modern language features

## Future Enhancements

### Potential Improvements

- Linux/macOS support
- Port filtering (by port number, protocol, process name)
- Process details view (command line arguments, environment variables)
- Export port list (CSV, JSON)
- Watch mode (continuous monitoring)
- Port range scanning

### Architecture Readiness

The current design supports these enhancements:
- New platforms: Add implementations, no core changes
- Filtering: Add to service layer
- Export: New formatters in CLI layer
- Watch mode: Add observable pattern to service layer

## Conclusion

RBPortKiller's architecture prioritizes:
- **Maintainability**: Clear structure, single responsibilities
- **Extensibility**: Easy to add platforms and features
- **Testability**: Interface-based, separated concerns
- **Usability**: Rich UI with safety features

The clean architecture approach ensures the codebase remains organized and adaptable as requirements evolve.
