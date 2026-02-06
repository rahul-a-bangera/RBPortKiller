# Test Port Opener

A utility application for testing **RBPortKiller** functionality by opening multiple dummy network ports.

## Purpose

This tool helps you test RBPortKiller by:
- Creating multiple TCP and UDP listeners on various ports
- Simulating real-world port usage scenarios
- Testing port creation time sorting
- Verifying system process filtering works correctly

## Quick Start

### Windows
```cmd
run-test-ports.bat
```

### Linux/Mac
```bash
chmod +x run-test-ports.sh
./run-test-ports.sh
```

### Manual
```bash
dotnet run --project TestPortOpener
```

## Test Modes

### 1. Simple Mode (Option 1)

Opens 7 ports immediately:

| Port | Protocol | Description |
|------|----------|-------------|
| 3000 | TCP | Development Server |
| 5000 | TCP | API Server |
| 8080 | TCP | Web Server |
| 9000 | TCP | Test Service |
| 7777 | TCP | Game Server |
| 6000 | UDP | UDP Service 1 |
| 6001 | UDP | UDP Service 2 |

**Use Case**: Quick testing of basic RBPortKiller functionality.

### 2. Advanced Mode (Option 2)

Opens ports in stages with delays to test creation time sorting:

**Stage 1** (T+0s):
- Port 3000 - Initial Dev Server
- Port 5000 - Initial API Server

**Stage 2** (T+2s):
- Port 8080 - Delayed Web Server
- Port 9000 - Delayed Service

**Stage 3** (T+5s):
- Port 7777 - Latest Game Server
- Port 4200 - Latest Angular App
- Port 6000 - Active Service (with simulated connections)

**Use Case**: Testing creation time sorting, verifying newest ports appear first.

## What to Test

### ? System Process Filtering
1. Run TestPortOpener
2. Run RBPortKiller
3. **Verify**: TestPortOpener appears in the list (it's NOT a system process)
4. **Verify**: No Windows system processes appear (svchost, System, etc.)

### ? Creation Time Display
1. Run TestPortOpener in **Simple Mode**
2. Wait a few seconds
3. Run RBPortKiller
4. **Verify**: Creation times are displayed correctly
5. **Check**: Times should be very recent (within last minute)

### ? Creation Time Sorting
1. Run TestPortOpener in **Advanced Mode**
2. Wait for all stages to complete (about 6 seconds)
3. Run RBPortKiller
4. **Verify**: Port 4200 and 7777 should be at the **top** (newest)
5. **Verify**: Port 8080 and 9000 should be in the **middle**
6. **Verify**: Port 3000 and 5000 should be near the **bottom** (oldest)

### ? Port Termination
1. Run TestPortOpener
2. Run RBPortKiller
3. Select any port owned by "TestPortOpener"
4. Choose "Kill Process"
5. **Verify**: Process terminates successfully
6. **Verify**: All ports close (TestPortOpener window closes)
7. **Verify**: No error messages about access denied

### ? Multiple Instances
1. Run TestPortOpener (first instance)
2. Change port numbers in code or run from different terminal
3. Run another instance (if ports don't conflict)
4. **Verify**: Both show up separately in RBPortKiller
5. **Verify**: You can kill them individually

## Expected Behavior in RBPortKiller

When you open RBPortKiller while TestPortOpener is running, you should see:

```
#   Port   Protocol   PID    Process Name      Local Address   State       Created
??????????????????????????????????????????????????????????????????????????????????
1   7777   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:15
2   4200   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:15
3   9000   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:13
4   8080   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:13
5   5000   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:11
6   3000   TCP        1234   TestPortOpener    0.0.0.0        LISTENING   14:32:11
7   6000   UDP        1234   TestPortOpener    0.0.0.0        N/A         14:32:15
```

Key observations:
- ? All ports have the same PID (same process)
- ? Process name is "TestPortOpener" (not a system process)
- ? Creation times are sorted (newest first)
- ? Both TCP and UDP protocols appear
- ? Process can be safely killed

## Troubleshooting

### Port Already in Use
```
? TCP Port  3000 - FAILED: Only one usage of each socket address...
```

**Solution**: 
- Close any other applications using these ports (Node.js, Docker, etc.)
- Or modify the port numbers in the code

### Permission Denied
```
Access to the path is denied
```

**Solution**:
- Run as Administrator (Windows)
- Use `sudo` on Linux/Mac if ports < 1024

### Ports Not Showing in RBPortKiller
**Possible Causes**:
1. TestPortOpener crashed - check if it's still running
2. System process filter is working (good!) - verify "TestPortOpener" is the process name
3. Run RBPortKiller as Administrator to see all process info

### Can't Kill TestPortOpener
**Check**:
1. Are you running RBPortKiller as Administrator?
2. Is TestPortOpener still running?
3. Try pressing Ctrl+C in TestPortOpener window first

## Customization

### Add More Ports
Edit `Program.cs` and add more listeners:

```csharp
StartTcpListener(YOUR_PORT, "Your Description");
StartUdpListener(YOUR_PORT, "Your Description");
```

### Change Port Numbers
Modify the port numbers in the `StartTcpListener()` and `StartUdpListener()` calls.

### Add Delays
In Advanced Mode, adjust the `Task.Delay()` values:

```csharp
await Task.Delay(2000); // Wait 2 seconds
await Task.Delay(5000); // Wait 5 seconds
```

## Clean Up

### Stop the Application
- Press **Ctrl+C** in the terminal
- Or kill it using RBPortKiller (that's the point!)

### Verify Ports Are Closed
```bash
# Windows
netstat -ano | findstr "3000 5000 8080"

# Linux/Mac
netstat -tuln | grep "3000\|5000\|8080"
```

Should return no results when TestPortOpener is stopped.

## Notes for Developers

### Why This Tool?

Testing RBPortKiller requires:
1. **Non-system processes** - TestPortOpener qualifies
2. **Known port numbers** - Easy to identify
3. **Persistent connections** - Stays open until killed
4. **Safe to terminate** - No data loss risk

### Architecture

```
TestPortOpener/
??? Program.cs                  # Main entry, simple mode
??? AdvancedPortTester.cs      # Advanced scenarios
??? TestPortOpener.csproj      # .NET 8 project
```

### Adding Test Scenarios

Create a new test method in `AdvancedPortTester.cs`:

```csharp
public static async Task RunCustomScenario()
{
    OpenPort(YOUR_PORT, "Custom Test");
    await Task.Delay(YOUR_DELAY);
    // More ports...
}
```

Then call it from `Program.cs`.

## Safety Notice

?? **This is a test tool - not for production use!**

- Opens ports without authentication
- Accepts connections but doesn't validate them
- No error recovery or logging
- Designed to be killed easily

Perfect for testing, **not** for real applications!

## Integration with RBPortKiller

This tool was designed specifically to test these RBPortKiller features:

? **System Process Filtering** - TestPortOpener is a user app, not a system process  
? **Creation Time Tracking** - Advanced mode creates ports at different times  
? **Creation Time Sorting** - Verifies newest ports appear first  
? **Process Termination** - Safe to kill, tests the core functionality  
? **Multi-Protocol Support** - Tests both TCP and UDP  
? **Port Discovery** - Creates various listener types  

## Example Test Session

```bash
# Terminal 1: Start test ports
> run-test-ports.bat
Select test mode:
  1. Simple Mode
  2. Advanced Mode
Enter choice (1 or 2): 2

[Advanced test runs, opens ports in stages]

# Terminal 2: Test RBPortKiller
> cd RBPortKiller.CLI
> dotnet run

[See TestPortOpener ports in the list]
[Verify creation times]
[Verify sorting]
[Kill the process]
[Verify success]

# Terminal 1: TestPortOpener should exit
[Application terminated by RBPortKiller]
```

Perfect test! ?
