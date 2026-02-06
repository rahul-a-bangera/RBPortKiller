# Contributing to RBPortKiller

Thank you for your interest in contributing to RBPortKiller! This document provides guidelines and information for contributors.

## 🎯 Areas for Contribution

We welcome contributions in the following areas:

### High Priority

1. **Linux Support** - Implement `IPortDiscoveryService` and `IProcessManagementService` for Linux
2. **macOS Support** - Implement platform-specific services for macOS
3. **Unit Tests** - Add comprehensive unit tests for core services
4. **Integration Tests** - Add end-to-end tests

### Medium Priority

5. **Port Filtering** - Add ability to filter by protocol, port range, or process name
6. **Export Functionality** - Export port list to CSV, JSON, or other formats
7. **Watch Mode** - Auto-refresh port list at intervals
8. **Configuration File** - Support for user preferences and settings
9. **Logging** - Add optional logging for debugging

### Nice to Have

10. **Performance Optimization** - Improve port discovery speed
11. **Additional Protocols** - Support for SCTP, DCCP, etc.
12. **Remote Monitoring** - Monitor ports on remote machines
13. **GUI Version** - Desktop application with the same core logic

## 🏗️ Architecture Overview

### Project Structure

```
RBPortKiller/
├── RBPortKiller.Core/           # Platform-agnostic
│   ├── Models/                  # Domain models
│   ├── Interfaces/              # Abstractions
│   └── Services/                # Business logic
│
├── RBPortKiller.Infrastructure/ # Platform-specific
│   ├── Windows/                 # Windows implementations
│   ├── Linux/                   # (Future) Linux implementations
│   ├── MacOS/                   # (Future) macOS implementations
│   └── PlatformServiceFactory.cs
│
└── RBPortKiller.CLI/            # User interface
    ├── Program.cs               # DI setup
    └── PortKillerCli.cs         # CLI logic
```

### Design Principles

1. **Separation of Concerns**: Core logic is independent of platform and UI
2. **Dependency Injection**: All services are injected, not instantiated
3. **Interface-Based Design**: Platform-specific code implements interfaces
4. **Single Responsibility**: Each class has one clear purpose
5. **Error Handling**: Comprehensive error handling with user-friendly messages

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Git
- A code editor (VS Code, Visual Studio, or Rider)

### Setup Development Environment

1. **Fork and clone the repository:**
   ```bash
   git clone https://github.com/yourusername/RBPortKiller.git
   cd RBPortKiller
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Run the application:**
   ```bash
   dotnet run --project RBPortKiller.CLI
   ```

## 📝 Coding Guidelines

### C# Style

- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods small and focused
- Use `async`/`await` for I/O operations

### Example

```csharp
/// <summary>
/// Retrieves all active network ports on the system.
/// </summary>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>A collection of active port information.</returns>
public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken = default)
{
    // Implementation
}
```

### Naming Conventions

- **Interfaces**: `IServiceName`
- **Classes**: `ServiceName`
- **Methods**: `VerbNoun` (e.g., `GetActivePorts`)
- **Properties**: `NounOrNounPhrase`
- **Private fields**: `_camelCase`

## 🧪 Testing

### Writing Tests

We use xUnit for testing. Create tests in a new `RBPortKiller.Tests` project:

```bash
dotnet new xunit -n RBPortKiller.Tests
dotnet sln add RBPortKiller.Tests
```

### Test Structure

```csharp
public class PortKillerServiceTests
{
    [Fact]
    public async Task GetActivePortsAsync_ReturnsNonEmptyList()
    {
        // Arrange
        var mockDiscovery = new Mock<IPortDiscoveryService>();
        var mockProcess = new Mock<IProcessManagementService>();
        var service = new PortKillerService(mockDiscovery.Object, mockProcess.Object);

        // Act
        var result = await service.GetActivePortsAsync();

        // Assert
        Assert.NotNull(result);
    }
}
```

## 🔧 Adding Platform Support

### Example: Linux Support

1. **Create the implementation:**

```csharp
// RBPortKiller.Infrastructure/Linux/LinuxPortDiscoveryService.cs
public class LinuxPortDiscoveryService : IPortDiscoveryService
{
    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken)
    {
        // Read /proc/net/tcp and /proc/net/udp
        // Or use 'ss' or 'netstat' commands
        // Parse output and create PortInfo objects
    }
}

// RBPortKiller.Infrastructure/Linux/LinuxProcessManagementService.cs
public class LinuxProcessManagementService : IProcessManagementService
{
    public async Task<ProcessTerminationResult> TerminateProcessAsync(int processId, CancellationToken cancellationToken)
    {
        // Use Process.Kill() or send SIGTERM/SIGKILL
    }

    public bool CanTerminateProcess(int processId)
    {
        // Check process ownership and permissions
    }
}
```

2. **Update the factory:**

```csharp
// RBPortKiller.Infrastructure/PlatformServiceFactory.cs
public static IPortDiscoveryService CreatePortDiscoveryService()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return new WindowsPortDiscoveryService();
    
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return new LinuxPortDiscoveryService(); // New!
    
    // ...
}
```

3. **Test on the target platform**

## 📋 Pull Request Process

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes:**
   - Write clean, documented code
   - Add tests if applicable
   - Update documentation

3. **Test your changes:**
   ```bash
   dotnet build
   dotnet test
   dotnet run --project RBPortKiller.CLI
   ```

4. **Commit with clear messages:**
   ```bash
   git commit -m "Add Linux port discovery support"
   ```

5. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request:**
   - Describe what you changed and why
   - Reference any related issues
   - Include screenshots if UI changes

### PR Checklist

- [ ] Code builds without errors
- [ ] Code follows style guidelines
- [ ] XML documentation added for public APIs
- [ ] Tests added/updated (if applicable)
- [ ] README updated (if applicable)
- [ ] No breaking changes (or clearly documented)

## 🐛 Reporting Bugs

### Before Reporting

1. Check if the issue already exists
2. Try the latest version
3. Gather relevant information

### Bug Report Template

```markdown
**Description**
A clear description of the bug.

**To Reproduce**
Steps to reproduce:
1. Run `rbportkiller`
2. Select port X
3. ...

**Expected Behavior**
What you expected to happen.

**Actual Behavior**
What actually happened.

**Environment**
- OS: Windows 11
- .NET Version: 8.0.1
- RBPortKiller Version: 1.0.0

**Additional Context**
Any other relevant information.
```

## 💡 Feature Requests

We welcome feature requests! Please:

1. Check if it's already requested
2. Describe the use case
3. Explain why it would be valuable
4. Suggest a possible implementation (optional)

## 📜 Code of Conduct

### Our Standards

- Be respectful and inclusive
- Accept constructive criticism
- Focus on what's best for the project
- Show empathy towards others

### Unacceptable Behavior

- Harassment or discriminatory language
- Trolling or insulting comments
- Publishing others' private information
- Other unprofessional conduct

## 📞 Getting Help

- **Questions**: Open a GitHub Discussion
- **Bugs**: Open a GitHub Issue
- **Security**: Email the maintainer directly

## 🏆 Recognition

Contributors will be:
- Listed in the README
- Credited in release notes
- Given our eternal gratitude! 🙏

## 📄 License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to RBPortKiller! 🚀
