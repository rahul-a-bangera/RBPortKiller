# RBPortKiller - Complete Installation Guide

This guide provides detailed step-by-step instructions for building, installing, running, and uninstalling RBPortKiller.

---

## ?? Table of Contents

1. [Prerequisites](#prerequisites)
2. [Building the Executable](#building-the-executable)
3. [Installing RBPortKiller](#installing-rbportkiller)
4. [Running RBPortKiller](#running-rbportkiller)
5. [Uninstalling RBPortKiller](#uninstalling-rbportkiller)
6. [Troubleshooting](#troubleshooting)
7. [Manual Installation (Alternative)](#manual-installation-alternative)

---

## Prerequisites

Before you begin, ensure you have the following:

### Required

- **Operating System**: Windows 10/11 or Windows Server 2016+
- **PowerShell**: Version 5.1 or later (comes with Windows)
- **.NET 8 SDK**: Required for building from source

### Installing .NET 8 SDK

If you don't have .NET 8 SDK installed:

1. Visit: https://dotnet.microsoft.com/download/dotnet/8.0
2. Download the **.NET 8 SDK** installer (not just the runtime)
3. Run the installer and follow the prompts
4. Verify installation by opening PowerShell and running:
   ```powershell
   dotnet --version
   ```
   You should see `8.0.x` or higher

---

## Building the Executable

### Step 1: Clone or Download the Repository

**Option A - Using Git:**
```powershell
git clone https://github.com/yourusername/RBPortKiller.git
cd RBPortKiller
```

**Option B - Download ZIP:**
1. Download the repository as a ZIP file
2. Extract it to a folder (e.g., `C:\Dev\RBPortKiller`)
3. Open PowerShell and navigate to the folder:
   ```powershell
   cd C:\Dev\RBPortKiller
   ```

### Step 2: Run the Build Script

In PowerShell (from the project root directory):

```powershell
.\build.ps1
```

**What this does:**
- Restores NuGet packages
- Builds the project in Release mode
- Creates a self-contained single-file executable
- Outputs to: `publish\win-x64\rbportkiller.exe`

**Expected output:**
```
========================================
  RBPortKiller Build & Publish Script
========================================

Configuration: Release
Runtime: win-x64
Output Directory: publish\win-x64

Cleaning previous builds...
Restoring dependencies...
Building project...
Publishing self-contained executable...
...
Build completed successfully!
```

### Build Options (Advanced)

Build for different platforms:

```powershell
# Windows (default)
.\build.ps1 -Runtime win-x64

# Linux (requires Linux-specific implementation)
.\build.ps1 -Runtime linux-x64

# macOS Intel
.\build.ps1 -Runtime osx-x64

# macOS Apple Silicon
.\build.ps1 -Runtime osx-arm64

# Debug mode
.\build.ps1 -Configuration Debug
```

---

## Installing RBPortKiller

### Step 3: Run the Install Script

After building successfully, install the tool globally:

```powershell
.\install.ps1
```

**What this does:**
- Copies `rbportkiller.exe` to `%LOCALAPPDATA%\RBPortKiller`
  - Full path: `C:\Users\YourUsername\AppData\Local\RBPortKiller`
- Adds this directory to your user PATH environment variable
- Makes `rbportkiller` command available system-wide

**Expected output:**
```
========================================
  RBPortKiller Installation Script
========================================

Creating installation directory...
Installing rbportkiller to C:\Users\YourUsername\AppData\Local\RBPortKiller...
Adding to PATH...

========================================
  Installation Successful!
========================================

rbportkiller has been installed to:
  C:\Users\YourUsername\AppData\Local\RBPortKiller

IMPORTANT: Please restart your terminal or run:
  $env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')

Then you can use the tool by running:
  rbportkiller
```

### Step 4: Refresh Your Terminal

**Option A - Restart Terminal (Recommended):**
1. Close your current PowerShell/Command Prompt window
2. Open a new PowerShell/Command Prompt window
3. The PATH will be automatically updated

**Option B - Refresh PATH in Current Session:**
```powershell
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
```

### Verify Installation

Check that the tool is accessible:

```powershell
rbportkiller --version
```

Or simply:

```powershell
rbportkiller
```

---

## Running RBPortKiller

### Basic Usage

Open PowerShell or Command Prompt and run:

```powershell
rbportkiller
```

### What You'll See

1. **Port List View** - A table showing all active network ports:
   - Port number
   - Protocol (TCP, UDP, TCPv6, UDPv6)
   - Process ID (PID)
   - Process name
   - Local address
   - Connection state (color-coded: green=Listening, cyan=Established, etc.)

2. **Interactive Menu** - Use keyboard to navigate:
   - **Arrow Keys**: Navigate through the port list
   - **Enter**: Select a port
   - **Type a Number**: Jump directly to a port by its number

3. **Port Details Panel** - Shows detailed information:
   - Full process path
   - Remote address (if applicable)
   - Current state

4. **Actions**:
   - **Kill Process**: Terminate the process using the port
   - **Back to List**: Return to the port list
   - **Exit**: Close the application

### Example Workflow: Kill Process on Port 8080

```powershell
# 1. Run the tool
rbportkiller

# 2. Find the port (e.g., TCP:8080)
#    Navigate with arrow keys or type the selection number

# 3. Press Enter to select

# 4. Choose "Kill Process"

# 5. Confirm with 'y' when prompted

# 6. Process terminated! Press any key to continue
```

### Running as Administrator

Some system processes require elevated privileges:

1. Right-click on PowerShell
2. Select **"Run as Administrator"**
3. Run `rbportkiller`

You'll have permission to terminate system processes.

### Common Use Cases

**Kill process on a specific port:**
```powershell
rbportkiller
# Select the port and kill the process
```

**View all active connections:**
```powershell
rbportkiller
# Browse through all ports and their states
```

**Troubleshoot port conflicts:**
```powershell
rbportkiller
# Find which process is using a port that your app needs
```

---

## Uninstalling RBPortKiller

### Complete Removal

To completely remove RBPortKiller from your system:

```powershell
.\uninstall.ps1
```

**What this does:**
- Removes `rbportkiller.exe` from `%LOCALAPPDATA%\RBPortKiller`
- Removes the installation directory from your PATH
- Deletes the installation folder

**Expected output:**
```
========================================
  RBPortKiller Uninstall Script
========================================

Removing from PATH...
Removing installation directory...

========================================
  Uninstallation Complete!
========================================

rbportkiller has been removed from your system.
```

### Manual Removal (Alternative)

If the uninstall script is not available:

1. **Remove the executable:**
   ```powershell
   Remove-Item -Path "$env:LOCALAPPDATA\RBPortKiller" -Recurse -Force
   ```

2. **Remove from PATH:**
   - Press `Win + R`, type `sysdm.cpl`, press Enter
   - Go to **Advanced** ? **Environment Variables**
   - Under **User variables**, select **Path** ? **Edit**
   - Find and remove: `C:\Users\YourUsername\AppData\Local\RBPortKiller`
   - Click **OK** on all dialogs

3. **Restart your terminal**

---

## Troubleshooting

### Issue: "rbportkiller is not recognized as a command"

**Causes:**
- PATH not refreshed after installation
- Installation failed

**Solutions:**
1. Restart your terminal window
2. Or refresh PATH:
   ```powershell
   $env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
   ```
3. Verify installation:
   ```powershell
   Test-Path "$env:LOCALAPPDATA\RBPortKiller\rbportkiller.exe"
   ```
   Should return `True`

### Issue: "dotnet: command not found" during build

**Cause:** .NET 8 SDK not installed

**Solution:**
1. Download and install .NET 8 SDK from: https://dotnet.microsoft.com/download/dotnet/8.0
2. Restart PowerShell
3. Verify: `dotnet --version`

### Issue: "Access Denied" when killing a process

**Causes:**
- System process requires admin privileges
- Process is protected

**Solutions:**
1. Run PowerShell as Administrator
2. Some processes (e.g., System, svchost) cannot be terminated

### Issue: Build fails with errors

**Solutions:**
1. Ensure you're in the project root directory
2. Clean and rebuild:
   ```powershell
   dotnet clean
   .\build.ps1
   ```
3. Check .NET SDK version: `dotnet --version` (must be 8.0+)
4. Restore packages manually:
   ```powershell
   dotnet restore
   ```

### Issue: "No ports found"

**Causes:**
- No active network connections
- Requires admin rights to see all processes

**Solutions:**
1. Start some network activity (web browser, etc.)
2. Run as Administrator to see system processes

---

## Manual Installation (Alternative)

If you prefer to install manually without using `install.ps1`:

### Step 1: Build the Project

```powershell
dotnet publish RBPortKiller.CLI\RBPortKiller.CLI.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -o publish
```

### Step 2: Copy to a Directory in PATH

**Option A - Create dedicated folder:**

```powershell
# Create folder
New-Item -Path "$env:LOCALAPPDATA\RBPortKiller" -ItemType Directory -Force

# Copy executable
Copy-Item -Path "publish\rbportkiller.exe" -Destination "$env:LOCALAPPDATA\RBPortKiller\"

# Add to PATH (see next step)
```

**Option B - Use existing PATH folder:**

```powershell
# Copy to a folder already in PATH (e.g., if you have a bin folder)
Copy-Item -Path "publish\rbportkiller.exe" -Destination "C:\Users\YourUsername\bin\"
```

### Step 3: Add to PATH (if needed)

1. Press `Win + X` ? **System**
2. Click **Advanced system settings**
3. Click **Environment Variables**
4. Under **User variables**, select **Path** ? **Edit**
5. Click **New** ? Add: `C:\Users\YourUsername\AppData\Local\RBPortKiller`
6. Click **OK** on all dialogs
7. Restart terminal

### Step 4: Verify

```powershell
rbportkiller
```

---

## Quick Reference

### Installation Commands (Quick)

```powershell
# 1. Build
.\build.ps1

# 2. Install
.\install.ps1

# 3. Refresh PATH
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')

# 4. Run
rbportkiller
```

### Uninstallation Command

```powershell
.\uninstall.ps1
```

### File Locations

- **Executable**: `%LOCALAPPDATA%\RBPortKiller\rbportkiller.exe`
- **Full path**: `C:\Users\YourUsername\AppData\Local\RBPortKiller\rbportkiller.exe`
- **Build output**: `publish\win-x64\rbportkiller.exe`

---

## Next Steps

After installation:

1. **Read the Quick Start Guide**: `Misc\Docs\QUICKSTART.md`
2. **Explore features**: Run `rbportkiller` and experiment
3. **Report issues**: Create an issue on GitHub if you encounter problems

---

## Additional Resources

- **Quick Start Guide**: `Misc\Docs\QUICKSTART.md`
- **Contributing Guide**: `Misc\Docs\CONTRIBUTING.md`
- **Project Summary**: `Misc\Docs\PROJECT_SUMMARY.md`
- **Main README**: `README.md`

---

**Need Help?**

If you encounter any issues not covered here:
1. Check the troubleshooting section above
2. Review existing GitHub issues
3. Create a new issue with details about your problem

---

*Last Updated: 2024*
*RBPortKiller - A powerful CLI tool for port management on Windows*
