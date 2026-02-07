# RBPortKiller

A command-line tool for managing network ports and terminating processes on Windows. Built with .NET 8 and clean architecture principles.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)

## Features

- Interactive terminal interface powered by Spectre.Console
- Real-time port discovery with detailed connection information on Windows
- Process termination with permission validation
- System process protection prevents accidental termination of critical processes
- Port creation timestamps with intelligent sorting (newest first)
- Keyboard navigation with Ctrl+C quick exit support
- Port list refresh without restarting the application
- Color-coded connection states for easy visualization
- Clean architecture with SOLID principles
- Self-contained single binary executable for Windows

## What You Get

When you run `rbportkiller`:

- **Port List View**: Table showing all active ports
  - Port number and protocol (TCP/UDP/TCPv6/UDPv6)
  - Process ID (PID) and process name
  - Local address and connection state
  - Creation timestamp with intelligent formatting

- **Interactive Selection**: Navigate with arrow keys or type port number
  - Select port to view details and manage
  - Refresh port list without restarting
  - Ctrl+C anytime to exit quickly

- **Safe Process Termination**: Confirmation prompts and permission checks
  - System processes automatically filtered out
  - Clear error messages with actionable suggestions

## Installation

### Prerequisites

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 SDK (for building from source)

### Quick Install

```powershell
# Clone the repository
git clone https://github.com/rahul-a-bangera/RBPortKiller.git
cd RBPortKiller

# Build the application
dotnet build -c Release

# Run the tool
dotnet run --project RBPortKiller.CLI
```

### Install as Global Tool

```powershell
# Build and publish
dotnet publish RBPortKiller.CLI -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o publish

# Add to PATH (manual)
# Copy publish\rbportkiller.exe to a directory in your PATH
```

## Usage

### Basic Usage

Run the tool:
```powershell
rbportkiller
```

The application will:
1. Display all active network ports in a table
2. Allow selection of a port using arrow keys or numeric input
3. Show detailed information about the selected port
4. Offer options to terminate the process or return to the list

### Keyboard Shortcuts

- **Arrow Keys / Number Keys**: Navigate and select ports
- **Ctrl+C**: Exit the application at any time
- **Enter**: Confirm selection
- **Escape**: Cancel or go back (in prompts)

### Example Workflow

```
1. Run rbportkiller
2. View list of active ports sorted by creation time (newest first)
3. Select port 8080 (used by a development server)
4. Choose "Kill Process"
5. Confirm the action
6. Process terminated successfully
7. Port list refreshes automatically
```

### Admin Privileges

Some processes require administrator privileges to terminate. If you encounter permission errors:

```powershell
# Run PowerShell as Administrator, then:
rbportkiller
```

## Architecture

The project follows clean architecture principles with clear separation of concerns:

```
RBPortKiller/
├── RBPortKiller.Core/              # Business logic and abstractions
│   ├── Models/                     # Domain models (PortInfo, Protocol)
│   ├── Interfaces/                 # Service abstractions
│   └── Services/                   # Core business services
│
├── RBPortKiller.Infrastructure/    # Windows-specific implementations
│   ├── Windows/                    # Windows implementation
│   │   ├── Commands/               # Command executors
│   │   ├── Parsers/                # Output parsers
│   │   └── Helpers/                # Platform helpers
│   └── PlatformServiceFactory.cs   # Platform detection & factory
│
└── RBPortKiller.CLI/               # Interactive CLI interface
    ├── UI/                         # UI components and builders
    ├── Configuration/              # DI configuration
    └── PortKillerCli.cs            # Main CLI orchestration
```

### Key Design Principles

- **SOLID Principles**: Each class has a single responsibility and depends on abstractions
- **Platform Abstraction**: Core logic is separated from OS-specific code
- **Dependency Injection**: Loose coupling through constructor injection
- **Separation of Concerns**: UI, business logic, and infrastructure are clearly separated
- **Testability**: Pure functions and interface-based design enable easy testing

## Development

### Building from Source

```powershell
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run in development mode
dotnet run --project RBPortKiller.CLI
```

### Project Structure

- **RBPortKiller.Core**: Domain models, interfaces, and core business logic
- **RBPortKiller.Infrastructure**: Platform-specific implementations (Windows)
- **RBPortKiller.CLI**: Interactive command-line interface with Spectre.Console
- **TestPortOpener**: Testing utility for opening dummy ports

### Testing the Tool

Use the included TestPortOpener utility:

```powershell
# Run test port opener
dotnet run --project TestPortOpener

# In another terminal, run RBPortKiller
dotnet run --project RBPortKiller.CLI
```

See `TestPortOpener/README.md` for detailed testing instructions.

## Contributing

Contributions are welcome! Please read the contributing guidelines before submitting pull requests.

### Key Guidelines

- Follow SOLID principles and clean architecture patterns
- Use proper namespaces and file organization
- Add XML documentation for public APIs
- Separate I/O operations from business logic
- Test your changes thoroughly

See `CONTRIBUTING.md` for detailed guidelines.

## Documentation

- **Usage Guide**: `Misc/Docs/USAGE.md` (detailed usage instructions)
- **Architecture**: `Misc/Docs/ARCHITECTURE.md` (detailed architecture documentation)
- **Development**: `Misc/Docs/DEVELOPMENT.md` (development setup and guidelines)
- **Testing**: `TestPortOpener/README.md` (testing utility documentation)

## License

This project is licensed under the MIT License. See the LICENSE file for details.

## Platform Support

**Currently supports:** Windows 10/11 and Windows Server 2016+

This tool is designed specifically for Windows and uses Windows-specific commands (netstat) for port discovery and process management.

## Acknowledgments

Built with:
- [.NET 8](https://dotnet.microsoft.com/)
- [Spectre.Console](https://spectreconsole.net/) for rich terminal UI
- Clean architecture principles and SOLID design patterns
