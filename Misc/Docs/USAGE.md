# Usage Guide

Complete guide to using RBPortKiller for port and process management.

## Quick Start

### Running the Tool

```powershell
rbportkiller
```

Or in development:
```powershell
dotnet run --project RBPortKiller.CLI
```

### First Launch

On first launch, you will see:
1. RBPortKiller banner
2. Loading spinner while ports are discovered
3. Table of all active network ports
4. Interactive selection menu

## Understanding the Port List

### Table Columns

| Column | Description |
|--------|-------------|
| # | Row number for selection |
| Port | Port number (0-65535) |
| Protocol | TCP, UDP, TCPv6, or UDPv6 |
| PID | Process ID owning the port |
| Process Name | Name of the executable |
| Local Address | Bound IP address and port |
| State | Connection state (TCP only) |
| Created | When the connection was established |

### Connection States

- **LISTENING**: Port is waiting for incoming connections
- **ESTABLISHED**: Active connection to remote host
- **TIME_WAIT**: Connection closing, waiting for final acknowledgment
- **CLOSE_WAIT**: Remote end has closed, waiting for local close
- **N/A**: UDP connections (stateless)

Color coding:
- Green: LISTENING, ESTABLISHED (active)
- Yellow: TIME_WAIT, CLOSE_WAIT (transitional)
- Dim: N/A or other states

### Creation Timestamps

Ports are sorted by creation time (newest first). Display format:

- **Same day**: `14:32:15` (time only)
- **Yesterday**: `Yesterday 14:32`
- **Within 7 days**: `Mon 14:32` (day abbreviation)
- **Older**: `01/28 14:32` (month/day and time)

## Navigation

### Keyboard Controls

| Key | Action |
|-----|--------|
| Up Arrow / Down Arrow | Navigate through ports |
| Page Up / Page Down | Fast scroll (long lists) |
| Enter | Select highlighted port |
| Ctrl+C | Exit immediately |

### Selection Methods

**Arrow Keys**:
1. Use Up/Down arrows to highlight a port
2. Press Enter to select

**Direct Selection**:
1. Type the row number (shown in # column)
2. Press Enter

## Managing Ports

### Viewing Port Details

After selecting a port, you see:
- Port number and protocol
- Process ID and name
- Local and remote addresses
- Connection state
- Process creation time
- Full path to executable (if available)

### Killing a Process

1. Select a port from the list
2. Choose "Kill Process"
3. Confirm the action (default: No)
4. Wait for termination result

**Important**: Killing a process terminates **all ports** owned by that process.

### Process Termination Results

**Success**:
```
? Process 5432 terminated successfully.
```

**Permission Denied**:
```
? Failed to terminate process 5432
Error: Access is denied
Tip: Try running the tool as administrator.
```

**Process Not Found**:
```
? Failed to terminate process 5432
Error: Process has already exited.
```

## Safety Features

### System Process Protection

The following critical Windows processes are automatically hidden:
- System (PID 4)
- svchost.exe (Windows Service Host)
- csrss.exe (Client Server Runtime)
- lsass.exe (Local Security Authority)
- smss.exe (Session Manager)
- wininit.exe (Windows Initialization)
- services.exe (Services Control Manager)
- explorer.exe (Windows Explorer)

These cannot be terminated through RBPortKiller to prevent system instability.

### Confirmation Prompts

Before terminating any process:
1. Process name and PID are displayed
2. Confirmation required (default: No)
3. Permission check performed
4. Warning shown if elevated privileges needed

## Additional Features

### Refreshing the Port List

To reload ports without restarting:
1. Scroll to bottom of port list
2. Select "Refresh Port List"
3. Port list reloads with current data

Use when:
- Started/stopped applications outside RBPortKiller
- Want to verify a process was terminated
- Connection states may have changed

### Quick Exit

Press **Ctrl+C** at any time to exit immediately:
- Works from any menu or screen
- Performs clean shutdown
- No confirmation required

## Administrator Privileges

### When Required

Administrator (elevated) privileges are needed to:
- Terminate processes owned by other users
- Terminate processes running as services
- View some system process details

### Running as Administrator

**PowerShell**:
1. Right-click PowerShell
2. Select "Run as Administrator"
3. Run `rbportkiller`

**Command Prompt**:
1. Right-click Command Prompt
2. Select "Run as Administrator"
3. Run `rbportkiller`

### Permission Errors

If you see "Access is denied":
1. Check if process requires admin privileges
2. Close RBPortKiller
3. Reopen terminal as Administrator
4. Run RBPortKiller again

## Common Workflows

### Finding and Killing a Development Server

```
1. Run rbportkiller
2. Find port 3000, 5000, 8080, etc. in the list
3. Select the port
4. Choose "Kill Process"
5. Confirm
6. Server process terminated
```

### Clearing Stale Connections

```
1. Run rbportkiller
2. Look for TIME_WAIT or CLOSE_WAIT states
3. Note: These typically clear automatically
4. If persistent, kill the owning process
```

### Identifying Port Usage

```
1. Run rbportkiller
2. Find the port number in the list
3. Note the Process Name and PID
4. Select port to see full details including executable path
5. Choose "Back to Port List" to return without killing
```

### Testing Port Availability

```
1. Run rbportkiller
2. Search for specific port number
3. If found: Port is in use (see process)
4. If not found: Port is available
```

## Troubleshooting

### No Ports Found

**Cause**: No active network connections

**Solution**:
- Start some applications (browser, IDE, etc.)
- Verify network connectivity
- Run as Administrator to see system processes

### Cannot Terminate Process

**Cause 1**: Insufficient permissions

**Solution**: Run as Administrator

**Cause 2**: Process is a system process

**Solution**: These are protected and hidden for safety

**Cause 3**: Process has already exited

**Solution**: Refresh the port list

### Tool Runs Slowly

**Cause**: Large number of active connections

**Solution**: This is normal. Discovery takes longer with many ports.

### Missing Process Names

**Cause**: Some processes hide their information

**Solution**: Run as Administrator for complete information

## Tips

- Ports are sorted newest first - recent connections appear at the top
- System processes are hidden by default for safety
- Use Ctrl+C for quick exit from anywhere
- Refresh the list after external changes
- Run as Administrator for full functionality
- Process termination closes all ports owned by that process

## Examples

### Example 1: Kill Node.js Development Server

```
$ rbportkiller

[Port list appears]
Select: 2. TCP:3000 - node (PID: 8432)

[Details appear]
What would you like to do?
> Kill Process

Are you sure you want to kill process node (PID: 8432)? no
> yes

? Process 8432 terminated successfully.
```

### Example 2: Check What's Using Port 8080

```
$ rbportkiller

[Port list appears]
Find port 8080 in the list:
12. TCP:8080 - java (PID: 5532)

Select it to see:
- Process: java.exe
- Path: C:\Program Files\Java\jdk-17\bin\java.exe
- Created: 14:25:30

Choose "Back to Port List" to return without killing
```

### Example 3: Refresh After Closing Application

```
$ rbportkiller

[Port list shows your app on port 5000]

[You close the app in another window]

Scroll down and select "Refresh Port List"

[Port list reloads - port 5000 no longer appears]
```

## Next Steps

- **Architecture**: See `Misc/Docs/ARCHITECTURE.md` for technical details
- **Development**: See `Misc/Docs/DEVELOPMENT.md` for contributing
- **Testing**: See `TestPortOpener/README.md` for testing guide
