# Quick Start Guide - RBPortKiller

> 📖 **Need detailed instructions, troubleshooting, or uninstallation steps?**  
> See the **[Complete Installation Guide](INSTALLATION.md)** for comprehensive documentation.

---

## 🚀 Get Started in 3 Steps

### Step 1: Build the Application

Open PowerShell in the project directory and run:

```powershell
.\build.ps1
```

This will:
- Restore all dependencies
- Build the project in Release mode
- Create a self-contained single-file executable in `publish\win-x64\`

### Step 2: Install Globally

```powershell
.\install.ps1
```

This will:
- Copy the executable to `%LOCALAPPDATA%\RBPortKiller`
- Add the installation directory to your PATH
- Make `rbportkiller` available system-wide

### Step 3: Run the Tool

**Option A: Restart your terminal**, then run:

```powershell
rbportkiller
```

**Option B: Refresh PATH in current session**, then run:

```powershell
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
rbportkiller
```

## 📖 Using the Tool

### Basic Workflow

1. **Launch**: Run `rbportkiller` in any terminal
2. **Browse**: See all active network ports in a table
3. **Select**: Use arrow keys or type a number to select a port
4. **Action**: Choose to kill the process or go back
5. **Confirm**: Confirm the action when prompted

### Navigation

- **Arrow Keys**: Navigate through the port list
- **Enter**: Select a port or confirm an action
- **Type Number**: Quickly jump to a specific port
- **Select "Exit"**: Close the application

### Example: Kill a Process on Port 8080

```
1. Run: rbportkiller
2. Find: TCP:8080 - node (PID: 12345)
3. Select: Press Enter on that row
4. Choose: "Kill Process"
5. Confirm: Press 'y' when asked
6. Done: Process terminated!
```

## 🔧 Advanced Usage

### Running as Administrator

Some system processes require admin privileges:

```powershell
# Right-click PowerShell → "Run as Administrator"
rbportkiller
```

### Building for Different Platforms

```powershell
# For Linux
.\build.ps1 -Runtime linux-x64

# For macOS (Intel)
.\build.ps1 -Runtime osx-x64

# For macOS (Apple Silicon)
.\build.ps1 -Runtime osx-arm64
```

**Note**: Linux and macOS support requires implementing platform-specific services first.

### Debug Mode

For development and troubleshooting:

```powershell
dotnet run --project RBPortKiller.CLI\RBPortKiller.CLI.csproj
```

## 🛠️ Troubleshooting

### Issue: "rbportkiller" not recognized

**Solution 1**: Restart your terminal

**Solution 2**: Refresh PATH manually:
```powershell
$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')
```

**Solution 3**: Run directly:
```powershell
& "$env:LOCALAPPDATA\RBPortKiller\rbportkiller.exe"
```

### Issue: "Access Denied" when killing process

**Cause**: Insufficient permissions

**Solution**: Run PowerShell as Administrator

### Issue: Build fails

**Check**:
1. .NET 8 SDK is installed: `dotnet --version`
2. All files are present
3. No antivirus blocking the build

**Fix**: Clean and rebuild:
```powershell
dotnet clean
.\build.ps1
```

## 📦 Distribution

### Share the Executable

After building, share just the executable:

```
publish\win-x64\rbportkiller.exe
```

Recipients can:
1. Copy it to any directory
2. Add that directory to PATH
3. Run `rbportkiller`

### Create a Portable Version

```powershell
# Build
.\build.ps1

# Create a portable folder
New-Item -ItemType Directory -Path "RBPortKiller-Portable"
Copy-Item "publish\win-x64\rbportkiller.exe" -Destination "RBPortKiller-Portable\"
Copy-Item "README.md" -Destination "RBPortKiller-Portable\"

# Zip it
Compress-Archive -Path "RBPortKiller-Portable" -DestinationPath "RBPortKiller-Portable.zip"
```

## 🔄 Updating

To update to a new version:

```powershell
# Pull latest changes (if using git)
git pull

# Rebuild
.\build.ps1

# Reinstall
.\install.ps1
```

## 🗑️ Uninstalling

To remove RBPortKiller:

```powershell
.\uninstall.ps1
```

This removes:
- The executable from `%LOCALAPPDATA%\RBPortKiller`
- The PATH entry

## 💡 Tips

1. **Bookmark the command**: Add an alias in your PowerShell profile:
   ```powershell
   Set-Alias pk rbportkiller
   ```

2. **Check what's using a port**: Look for the port number in the list

3. **Be careful**: Always confirm before killing processes - some are critical system processes

4. **Use filters**: The interactive menu supports searching (type to filter)

## 🆘 Getting Help

- **Documentation**: See `README.md` for full documentation
- **Issues**: Check the GitHub issues page
- **Architecture**: See `README.md` → Architecture section

---

**Ready to go?** Run `.\build.ps1` now! 🚀
