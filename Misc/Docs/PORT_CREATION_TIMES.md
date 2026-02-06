# Understanding Port Creation Times

## What You'll See

When you run RBPortKiller, the port list now includes a "Created" column showing when each connection was established.

### Time Format Examples

| Display | Meaning | Example Scenario |
|---------|---------|------------------|
| `14:32:15` | Today at this time | Application started at 2:32:15 PM today |
| `Yesterday 18:45` | Yesterday at this time | Connection from yesterday evening |
| `Mon 09:30` | Recent weekday | Connection from earlier this week |
| `12/25 14:00` | Older date | Connection from a specific past date |
| `N/A` | Unknown | System couldn't determine creation time |

## What Ports Are Shown

### ? You WILL See
- User applications (browsers, IDEs, custom servers)
- Development tools (Node.js, Python servers, databases)
- Third-party applications (Spotify, Discord, Steam)
- Services you installed (Docker, MongoDB, PostgreSQL)

### ? You WON'T See
- Windows system processes
- Critical OS services (like `svchost`, `System`, `lsass`)
- Protected Windows components (`explorer`, `dwm`)
- Core networking services

## Why This Matters

### Safety
By hiding system processes, RBPortKiller prevents you from accidentally terminating critical Windows services that could:
- Crash your system
- Corrupt files
- Require a restart to recover

### Convenience
- **Cleaner List**: Only see ports you actually care about
- **No Errors**: Won't encounter "Access Denied" when trying to kill protected processes
- **Faster Navigation**: Fewer items to scroll through

### Better Troubleshooting
- **Recent First**: Newest connections appear at the top
- **Quick Identification**: See when a port was opened
- **Historical Context**: Identify long-running vs. recent connections

## Sorting Behavior

Ports are displayed in this order:
1. **Newest connections first** (by creation date)
2. **Port number** (if creation times are the same)

This means if you just started an application, its ports will appear at the top of the list.

## Example Scenarios

### Scenario 1: Development Server
```
You run: npm start
Result: Your dev server's port appears at the top with today's time
Action: Easy to find and kill when you're done
```

### Scenario 2: Stuck Process
```
Problem: Port 3000 is already in use
Solution: Check RBPortKiller - see when the port was opened
Action: Identify if it's from an old session you forgot to close
```

### Scenario 3: Multiple Applications
```
You have: Chrome, VS Code, local servers all running
Result: All their ports are listed, sorted by when they started
Action: Easy to identify which one you want to manage
```

## Limitations

### Creation Time May Show "N/A" When:
- Process started before Windows tracking began
- Process has restricted access (requires elevation)
- Process terminated between listing and display
- Running in limited permission mode

### System Processes Are Hidden Even If:
- They're using common ports (80, 443, etc.)
- You have administrator privileges
- They're part of Windows services

This is by design for your safety.

## Tips

### Finding Recent Connections
- Look at the top of the list
- Check for today's time stamps
- Recent connections are highlighted in cyan

### Identifying Old Connections
- Scroll down to find older dates
- Look for dates beyond "Yesterday"
- Consider closing long-running idle connections

### If You Need to See System Processes
*Current version does not support showing system processes*

Future versions may include:
- Toggle to show/hide system processes
- "Advanced Mode" with warnings
- Read-only view of all processes

## Frequently Asked Questions

### Q: Why don't I see `svchost.exe`?
**A**: It's a critical Windows service. RBPortKiller hides it for safety.

### Q: Can I show system processes anyway?
**A**: Not in the current version. This is a safety feature.

### Q: What if creation time shows "N/A"?
**A**: The process info wasn't available. The port is still manageable.

### Q: Why is the newest port at the top?
**A**: Recent connections are usually what you want to manage first.

### Q: Is it safe to kill any process shown?
**A**: Yes! Only non-system processes are shown. However, always verify it's not something you need before killing.

### Q: What happens if I kill the wrong process?
**A**: You can restart the application. Unlike system processes, user applications are safe to restart.

## Need More Information?

- See `SYSTEM_PROCESS_PROTECTION.md` for technical details
- See `INSTALLATION.md` for setup instructions
- See `LOCAL_TESTING.md` for development information
