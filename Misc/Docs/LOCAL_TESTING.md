# Local Testing Guide

Quick guide to run, build, and test RBPortKiller on your local machine.

---

## Prerequisites

- .NET 8 SDK installed ([download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- Administrator/sudo privileges (required for killing processes)

---

## Quick Start

### 1. Clone & Navigate
```bash
cd RBPortKiller
```

### 2. Run Without Building Executable
```bash
dotnet run --project RBPortKiller.CLI
```

---

## Build Executable

### Single-File Executable (Recommended)
```bash
# Windows
dotnet publish RBPortKiller.CLI -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# Linux
dotnet publish RBPortKiller.CLI -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# macOS
dotnet publish RBPortKiller.CLI -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish
```

**Output**: Executable in `./publish/` folder

### Framework-Dependent Build (Smaller Size)
```bash
dotnet publish RBPortKiller.CLI -c Release -o ./publish
```

---

## Testing Locally

### Test Port Discovery
```bash
# From project root
dotnet run --project RBPortKiller.CLI

# Or use the built exe (Windows)
./publish/RBPortKiller.CLI.exe

# Linux/macOS
./publish/RBPortKiller.CLI
```

### Test Specific Port
1. Start a test server:
   ```bash
   # Windows PowerShell
   python -m http.server 8080
   
   # Or use any app on a specific port
   ```

2. Find and kill the port:
   ```bash
   dotnet run --project RBPortKiller.CLI
   # Select the port (8080) from the list
   ```

### Test with Administrator Privileges
```bash
# Windows (PowerShell as Admin)
./publish/RBPortKiller.CLI.exe

# Linux/macOS
sudo ./publish/RBPortKiller.CLI
```

---

## Build & Run All-in-One
```bash
dotnet build && dotnet run --project RBPortKiller.CLI
```

---

## Common Issues

**"Permission Denied"**: Run as administrator/sudo  
**"Command not found"**: Ensure .NET 8 SDK is installed (`dotnet --version`)  
**"Port not found"**: Make sure the port is actually in use (`netstat -ano` on Windows, `lsof -i` on Linux/macOS)

---

## Clean Build
```bash
dotnet clean
dotnet build -c Release
```
