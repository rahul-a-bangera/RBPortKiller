# Contributing to RBPortKiller

Thank you for your interest in contributing to RBPortKiller! This document provides guidelines and instructions for contributing to the project.

## ?? Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Coding Standards](#coding-standards)
- [Making Changes](#making-changes)
- [Submitting Pull Requests](#submitting-pull-requests)
- [Reporting Issues](#reporting-issues)
- [Areas for Contribution](#areas-for-contribution)

## Code of Conduct

This project adheres to the Contributor Covenant [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code. Please report unacceptable behavior to the project maintainers.

## Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/RBPortKiller.git
   cd RBPortKiller
   ```
3. **Add upstream remote**:
   ```bash
   git remote add upstream https://github.com/rahul-a-bangera/RBPortKiller.git
   ```
4. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## Development Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio, VS Code, or JetBrains Rider recommended)
- Git

### Building the Project

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run the CLI locally
dotnet run --project RBPortKiller.CLI
```

### Testing Your Changes

```bash
# Run the test utility to create dummy ports
./run-test-ports.bat      # Windows
./run-test-ports.sh       # Linux/Mac

# In another terminal, run RBPortKiller
cd RBPortKiller.CLI
dotnet run
```

See [TestPortOpener/README.md](TestPortOpener/README.md) for detailed testing instructions.

## Coding Standards

RBPortKiller follows **SOLID principles** and **Clean Architecture**. Please review [.github/copilot-instructions.md](.github/copilot-instructions.md) for comprehensive coding guidelines.

### Key Principles

#### ? DO

- **Follow SOLID principles** - Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Use constructor injection** for all dependencies
- **Validate constructor parameters** with `ArgumentNullException`
- **Mark classes as `sealed`** unless designed for inheritance
- **Use `readonly` fields** for injected dependencies
- **Extract responsibilities** - create new focused classes instead of growing existing ones
- **Separate concerns** - Commands, Parsers, Providers, Builders, Resolvers
- **Write XML documentation** (`///`) for all public classes and methods
- **Use proper namespaces** - organize by layer and purpose
- **Follow naming conventions**:
  - **Commands/Executors**: Classes that perform I/O operations
  - **Parsers**: Classes that transform data (pure functions)
  - **Providers**: Classes that retrieve information
  - **Builders**: Classes that construct complex objects
  - **Mappers**: Classes that map between types
  - **Resolvers**: Classes that make decisions

#### ? DON'T

- **Don't violate SOLID principles**
- **Don't use service locator pattern** - always use constructor injection
- **Don't create large classes** - extract to focused components
- **Don't mix I/O with business logic** - keep them separate for testability
- **Don't add inline comments** unless explaining complex algorithms
- **Don't create .md files in project root or source directories** - use `Misc/Docs/`
- **Don't use static methods** for logic that needs testing

### Code Style

```csharp
// ? GOOD: Constructor injection, validation, readonly fields
public sealed class WindowsPortDiscoveryService : IPortDiscoveryService
{
    private readonly INetstatCommandExecutor _commandExecutor;
    private readonly INetstatOutputParser _parser;

    public WindowsPortDiscoveryService(
        INetstatCommandExecutor commandExecutor,
        INetstatOutputParser parser)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken)
    {
        var output = await _commandExecutor.ExecuteAsync(cancellationToken);
        return _parser.Parse(output);
    }
}

// ? BAD: Service locator, no validation, mixing concerns
public class PortService
{
    public async Task<List<PortInfo>> GetPorts(IServiceProvider services)
    {
        var executor = services.GetRequiredService<INetstatCommandExecutor>();
        var output = await executor.ExecuteAsync(CancellationToken.None);
        
        // Parsing logic mixed with I/O
        var lines = output.Split('\n');
        var ports = new List<PortInfo>();
        foreach (var line in lines)
        {
            // Parsing code...
        }
        return ports;
    }
}
```

### Namespace Organization

- **CLI Layer**: `RBPortKiller.CLI.*`
- **Core Layer**: `RBPortKiller.Core.*`
- **Infrastructure Layer**: `RBPortKiller.Infrastructure.*`

See [.github/copilot-instructions.md](.github/copilot-instructions.md) for detailed namespace structure.

## Making Changes

### Branch Naming

Use descriptive branch names:

- `feature/add-linux-support`
- `bugfix/fix-port-parsing`
- `docs/update-readme`
- `refactor/extract-parser-logic`

### Commit Messages

Write clear, descriptive commit messages:

```
Add Linux port discovery implementation

- Implement LinuxPortDiscoveryService using ss command
- Add parser for ss command output
- Update PlatformServiceFactory for Linux detection
- Add integration tests for Linux platform

Fixes #123
```

### Code Review Checklist

Before submitting, ensure:

- [ ] Code follows SOLID principles
- [ ] All dependencies are injected via constructors
- [ ] Classes have single responsibility
- [ ] Public APIs have XML documentation
- [ ] Code is properly organized in namespaces
- [ ] No hardcoded values or magic strings
- [ ] Error handling is appropriate
- [ ] Changes are manually tested
- [ ] Documentation is updated (if applicable)

## Submitting Pull Requests

1. **Sync with upstream**:
   ```bash
   git fetch upstream
   git rebase upstream/master
   ```

2. **Push your changes**:
   ```bash
   git push origin feature/your-feature-name
   ```

3. **Open a Pull Request** on GitHub with:
   - Clear title and description
   - Reference to related issues (e.g., "Fixes #123")
   - Screenshots or demo (if UI changes)
   - Description of testing performed

4. **Address review comments** if requested

5. **Maintain your PR**:
   - Keep it up to date with master
   - Respond to feedback promptly
   - Make requested changes

### Pull Request Template

We appreciate PRs that include:

- **What**: Brief description of changes
- **Why**: Problem being solved or feature being added
- **How**: Technical approach taken
- **Testing**: How you tested the changes
- **Screenshots**: If applicable

## Reporting Issues

### Bug Reports

Include:

- **Description**: Clear description of the bug
- **Steps to Reproduce**: Detailed steps to reproduce the behavior
- **Expected Behavior**: What you expected to happen
- **Actual Behavior**: What actually happened
- **Environment**: OS, .NET version, terminal type
- **Screenshots**: If applicable
- **Logs**: Any relevant error messages or stack traces

### Feature Requests

Include:

- **Problem**: What problem does this solve?
- **Proposed Solution**: How should it work?
- **Alternatives**: Other solutions you've considered
- **Use Case**: Real-world scenario where this would be useful

## Areas for Contribution

We especially welcome contributions in these areas:

### ?? High Priority

- **Linux Support**: Implement `LinuxPortDiscoveryService` and `LinuxProcessManagementService`
- **macOS Support**: Implement `MacOSPortDiscoveryService` and `MacOSProcessManagementService`
- **Unit Tests**: Add comprehensive test coverage
- **Integration Tests**: Add end-to-end testing

### ?? Features

- Port filtering (by protocol, state, process name)
- Export to CSV/JSON
- Configuration file support
- Watch mode (auto-refresh with change detection)
- Remote port management
- Docker container support

### ?? Documentation

- Architecture diagrams
- API documentation
- Video tutorials
- Translation to other languages

### ?? Bug Fixes

Check the [Issues](https://github.com/rahul-a-bangera/RBPortKiller/issues) page for open bugs.

### ?? Refactoring

- Improve test coverage
- Performance optimizations
- Code quality improvements

## Platform-Specific Contributions

### Adding Linux Support

1. Create `RBPortKiller.Infrastructure/Linux/` folder
2. Implement `LinuxPortDiscoveryService`:
   - Use `ss` command or `/proc/net/tcp` parsing
   - Parse output to `PortInfo` objects
3. Implement `LinuxProcessManagementService`:
   - Use `kill` command or signal handling
   - Handle permission checks
4. Update `PlatformServiceFactory`
5. Test on multiple Linux distributions

### Adding macOS Support

Similar to Linux, but using macOS-specific commands:
- `lsof -i -P -n` for port discovery
- `kill` for process termination

## Development Workflow

```bash
# 1. Sync with upstream
git fetch upstream
git checkout master
git merge upstream/master

# 2. Create feature branch
git checkout -b feature/my-feature

# 3. Make changes and commit
git add .
git commit -m "Add my feature"

# 4. Run tests (when available)
dotnet test

# 5. Push and create PR
git push origin feature/my-feature
```

## Getting Help

- **Documentation**: Check [.github/copilot-instructions.md](.github/copilot-instructions.md)
- **Issues**: Search existing issues or create a new one
- **Discussions**: Use GitHub Discussions for questions

## Recognition

Contributors will be recognized in:
- GitHub contributors list
- Release notes
- Project documentation

Thank you for contributing to RBPortKiller! ??
