# RBPortKiller

A powerful, production-ready command-line interface (CLI) tool for managing network ports and terminating processes on Windows. Built with C# and .NET 8.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)

## Features

✨ **Interactive Terminal Interface** - Beautiful, user-friendly CLI powered by Spectre.Console  
🔍 **Real-time Port Discovery** - Lists all active network ports with detailed information  
⚡ **Process Management** - Safely terminate processes by PID with proper permission handling  
🎯 **Smart Selection** - Keyboard navigation and numeric selection for easy port management  
🛡️ **Safety First** - Confirmation prompts and graceful error handling  
🏗️ **Clean Architecture** - Separation of concerns with platform abstraction layer  
🌐 **Cross-platform Ready** - Designed for easy extension to Linux and macOS  
📦 **Single Binary** - Self-contained executable with no dependencies  

## Screenshots

When you run `rbportkiller`, you'll see:

1. **Port List View** - A beautiful table showing all active ports with:
   - Port number
   - Protocol (TCP, UDP, TCPv6, UDPv6)
   - Process ID (PID)
   - Process name
   - Local address
   - Connection state (color-coded)

2. **Action Menu** - Select a port to:
   - View detailed information
   - Terminate the owning process
   - Return to the port list

3. **Confirmation & Feedback** - Clear success/error messages with helpful tips

## Installation

### Prerequisites

- Windows 10/11 or Windows Server 2016+
- .NET 8.0 SDK (for building from source)

### Quick Install

1. **Clone the repository:**
   ```powershell
   git clone https://github.com/yourusername/RBPortKiller.git
   cd RBPortKiller
   ```

2. **Build the application:**
   ```powershell
   .\build.ps1
   ```

3. **Install globally:**
   ```powershell
   .\install.ps1
   ```

4. **Restart your terminal** or refresh your PATH:
   ```powershell
   $env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
   ```

5. **Run the tool:**
   ```powershell
   rbportkiller
   ```

📖 **For detailed step-by-step instructions, troubleshooting, and uninstallation guide, see:**  
**[Complete Installation Guide](Misc/Docs/INSTALLATION.md)**

### Manual Installation

If you prefer manual installation:

1. Build the project:
   ```powershell
   dotnet publish RBPortKiller.CLI\RBPortKiller.CLI.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
   ```

2. Copy `rbportkiller.exe` to a directory in your PATH

## Usage

### Basic Usage

Simply run the command:

```powershell
rbportkiller
```

This will:
1. Display all active network ports in a table
2. Allow you to select a port using arrow keys or by number
3. Show detailed information about the selected port
4. Offer options to kill the process or return to the list

### Administrator Privileges

Some processes require administrator privileges to terminate. If you encounter permission errors:

```powershell
# Run PowerShell as Administrator, then:
rbportkiller
```

### Example Workflow

```
1. Run rbportkiller
2. See a list of all active ports
3. Select port 8080 (used by a Node.js process)
4. Choose "Kill Process"
5. Confirm the action
6. Process terminated successfully
7. Return to the port list (refreshed)
```

## Architecture

The project follows clean architecture principles with clear separation of concerns:

```
RBPortKiller/
├── RBPortKiller.Core/           # Platform-agnostic business logic
│   ├── Models/                  # Domain models (PortInfo, Protocol, etc.)
│   ├── Interfaces/              # Abstraction interfaces
│   └── Services/                # Core business services
│
├── RBPortKiller.Infrastructure/ # Platform-specific implementations
│   ├── Windows/                 # Windows-specific services
│   │   ├── WindowsPortDiscoveryService.cs
│   │   └── WindowsProcessManagementService.cs
│   └── PlatformServiceFactory.cs
│
└── RBPortKiller.CLI/            # Interactive CLI interface
    ├── Program.cs               # Entry point & DI setup
    └── PortKillerCli.cs         # CLI logic & UI
```

### Key Design Decisions

1. **Platform Abstraction**: Core logic is separated from OS-specific code, making it easy to add Linux/macOS support
2. **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection for loose coupling
3. **Rich UI**: Spectre.Console provides a modern, interactive terminal experience
4. **Error Handling**: Comprehensive error handling with user-friendly messages
5. **Safety**: Confirmation prompts and permission checks before destructive operations

## Development

### Building from Source

```powershell
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests (when available)
dotnet test

# Run the CLI (Debug mode)
dotnet run --project RBPortKiller.CLI
```

### Project Structure

- **RBPortKiller.Core**: Contains domain models, interfaces, and core business logic
- **RBPortKiller.Infrastructure**: Platform-specific implementations (currently Windows)
- **RBPortKiller.CLI**: Interactive command-line interface

### Adding Platform Support

To add Linux or macOS support:

1. Create `LinuxPortDiscoveryService.cs` or `MacOSPortDiscoveryService.cs` in `Infrastructure/`
2. Implement `IPortDiscoveryService` and `IProcessManagementService`
3. Update `PlatformServiceFactory.cs` to return the new implementations
4. Test on the target platform

Example for Linux:

```csharp
// Infrastructure/Linux/LinuxPortDiscoveryService.cs
public class LinuxPortDiscoveryService : IPortDiscoveryService
{
    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken)
    {
        // Use /proc/net/tcp, /proc/net/udp, or ss/netstat commands
        // Parse output and return PortInfo objects
    }
}
```

## Technical Details

### Port Discovery (Windows)

The tool uses multiple approaches for accurate port-to-process mapping:

1. **.NET NetworkInformation APIs**: For getting TCP/UDP connections and listeners
2. **netstat Command**: For reliable PID resolution
3. **Process APIs**: For retrieving process names and paths

### Process Termination (Windows)

Two-tier approach for maximum compatibility:

1. **Managed API**: Uses `Process.Kill()` for standard termination
2. **Native Win32 API**: Falls back to `TerminateProcess()` for stubborn processes

Both approaches include:
- Permission checking before attempting termination
- Graceful error handling
- Clear feedback on access denied errors

### Dependencies

- **Spectre.Console** (0.49.1): Rich terminal UI framework
- **Microsoft.Extensions.DependencyInjection** (10.0.2): Dependency injection container

## Uninstallation

To remove RBPortKiller from your system:

```powershell
.\uninstall.ps1
```

This will:
- Remove the executable from the installation directory
- Remove the installation path from your PATH environment variable

## Troubleshooting

### "Access Denied" Errors

**Problem**: Cannot terminate certain processes  
**Solution**: Run PowerShell as Administrator before running `rbportkiller`

### "Command not found"

**Problem**: `rbportkiller` command not recognized  
**Solution**: Restart your terminal or refresh PATH:
```powershell
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
```

### No Ports Displayed

**Problem**: The port list is empty  
**Solution**: This is normal if no applications are listening on network ports. Try running a web server or other network application first.

## Contributing

Contributions are welcome! Areas for improvement:

- [ ] Linux support
- [ ] macOS support
- [ ] Unit tests
- [ ] Integration tests
- [ ] Filter ports by protocol or process name
- [ ] Export port list to CSV/JSON
- [ ] Watch mode (auto-refresh)
- [ ] Configuration file support

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with [.NET 8](https://dotnet.microsoft.com/)
- UI powered by [Spectre.Console](https://spectreconsole.net/)
- Inspired by tools like `netstat`, `lsof`, and `ss`

## Author

**Rahul**

---

**Note**: This tool is designed for system administration and development purposes. Always exercise caution when terminating processes, as it may result in data loss or system instability.
