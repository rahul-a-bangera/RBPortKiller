# TestPortOpener

Testing utility for RBPortKiller that opens dummy TCP and UDP ports.

## Purpose

TestPortOpener helps verify RBPortKiller functionality by:
- Creating multiple network listeners on specified ports
- Testing port discovery and display
- Validating creation time sorting
- Verifying system process filtering
- Confirming process termination works correctly

## Running the Tool

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

### Mode 1: Simple Port Opening

Opens 7 ports simultaneously:

| Port | Protocol | Description |
|------|----------|-------------|
| 3000 | TCP | Development server |
| 5000 | TCP | API server |
| 8080 | TCP | Web server |
| 9000 | TCP | Test service |
| 7777 | TCP | Game server |
| 6000 | UDP | UDP service 1 |
| 6001 | UDP | UDP service 2 |

**Use for**:
- Basic functionality testing
- Verifying port discovery
- Testing process termination
- Quick validation

### Mode 2: Advanced Staged Opening

Opens ports in three stages with delays:

**Stage 1** (T+0s):
- Port 3000 (TCP)
- Port 5000 (TCP)

**Wait 2 seconds**

**Stage 2** (T+2s):
- Port 8080 (TCP)
- Port 9000 (TCP)

**Wait 3 seconds**

**Stage 3** (T+5s):
- Port 7777 (TCP)
- Port 4200 (TCP)
- Port 6000 (UDP)

**Use for**:
- Testing creation time sorting
- Verifying newest-first ordering
- Validating timestamp display

## Testing Scenarios

### Test 1: Port Discovery

**Objective**: Verify all ports are discovered

**Steps**:
1. Run TestPortOpener (any mode)
2. Run RBPortKiller
3. Verify all TestPortOpener ports appear in the list

**Expected**:
- All opened ports visible
- Protocol types correct (TCP/UDP)
- Process name shows "TestPortOpener"

### Test 2: System Process Filtering

**Objective**: Confirm TestPortOpener is not treated as system process

**Steps**:
1. Run TestPortOpener
2. Run RBPortKiller
3. Check port list

**Expected**:
- TestPortOpener appears in list
- No critical system processes visible (svchost, System, csrss, lsass)

### Test 3: Creation Time Display

**Objective**: Verify creation timestamps are accurate

**Steps**:
1. Run TestPortOpener (Simple Mode)
2. Note the timestamps shown in TestPortOpener output
3. Run RBPortKiller
4. Check "Created" column for TestPortOpener ports

**Expected**:
- All ports show creation times
- Times match approximately when TestPortOpener started
- Same-day ports show time only (HH:mm:ss format)

### Test 4: Creation Time Sorting

**Objective**: Confirm ports are sorted newest first

**Steps**:
1. Run TestPortOpener (Advanced Mode)
2. Wait for all three stages to complete (~6 seconds)
3. Run RBPortKiller
4. Check port order

**Expected Ordering** (top to bottom):
1. Stage 3 ports (4200, 7777, 6000) - newest
2. Stage 2 ports (8080, 9000) - middle
3. Stage 1 ports (3000, 5000) - oldest

### Test 5: Process Termination

**Objective**: Verify process termination works correctly

**Steps**:
1. Run TestPortOpener
2. Run RBPortKiller
3. Select any TestPortOpener port
4. Choose "Kill Process"
5. Confirm action

**Expected**:
- Process terminates successfully
- All TestPortOpener ports close
- TestPortOpener console window closes
- No permission errors

### Test 6: Port Refresh

**Objective**: Test port list refresh functionality

**Steps**:
1. Run RBPortKiller first
2. Note current port list
3. Run TestPortOpener
4. In RBPortKiller, select "Refresh Port List"
5. Verify TestPortOpener ports now appear

**Expected**:
- Port list updates with new ports
- TestPortOpener ports visible after refresh
- Creation times show recent timestamps

### Test 7: Multiple Instances

**Objective**: Test with multiple TestPortOpener instances

**Steps**:
1. Run TestPortOpener instance 1
2. Run TestPortOpener instance 2 (different ports or same)
3. Run RBPortKiller

**Expected**:
- All ports from both instances visible
- Separate process IDs
- Can terminate each instance independently

## Interpreting Output

### TestPortOpener Console Output

```
TestPortOpener - Port Testing Utility
========================================

Choose a mode:
1. Simple Mode: Open all ports at once
2. Advanced Mode: Open ports in stages (for testing time sorting)
3. Exit

Selection: 1

Opening ports...
[14:32:15] TCP Listener on port 3000 started
[14:32:15] TCP Listener on port 5000 started
[14:32:15] TCP Listener on port 8080 started
[14:32:15] TCP Listener on port 9000 started
[14:32:15] TCP Listener on port 7777 started
[14:32:15] UDP Listener on port 6000 started
[14:32:15] UDP Listener on port 6001 started

All ports are now open. Press Ctrl+C or close window to stop.
```

**Note**: Timestamps in brackets show when each port opened.

### RBPortKiller Display

Look for TestPortOpener entries in the port table:

```
#  Port  Protocol  PID    Process Name    Local Address    State        Created
1  7777  TCP       5432   TestPortOpener  0.0.0.0:7777    LISTENING    14:32:15
2  9000  TCP       5432   TestPortOpener  0.0.0.0:9000    LISTENING    14:32:15
...
```

## Common Issues

### Port Already in Use

**Error**: `Port 3000 is already in use`

**Solution**:
- Another application is using that port
- Close the other application first
- Or use RBPortKiller to kill it

### Permission Denied (Linux/Mac)

**Error**: `Permission denied` when opening ports <1024

**Solution**:
```bash
sudo dotnet run --project TestPortOpener
```

### TestPortOpener Not Appearing in RBPortKiller

**Cause**: TestPortOpener not running or no ports opened

**Solution**:
- Verify TestPortOpener console shows "All ports are now open"
- Check for error messages in TestPortOpener
- Refresh port list in RBPortKiller

## Advanced Usage

### Custom Ports

Edit `TestPortOpener/Program.cs` to use different ports:

```csharp
// Simple mode ports
var simplePorts = new[]
{
    (3000, ProtocolType.Tcp),
    (5000, ProtocolType.Tcp),
    // Add your ports here
};
```

### Custom Timing

Edit staging delays in `AdvancedPortTester.cs`:

```csharp
await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);  // Change delay
```

## Integration with CI/CD

### Automated Testing Script

```powershell
# Start TestPortOpener in background
Start-Process dotnet -ArgumentList "run --project TestPortOpener" -NoNewWindow

# Wait for ports to open
Start-Sleep -Seconds 2

# Run RBPortKiller tests
# (when test suite exists)

# Cleanup: Kill TestPortOpener
Get-Process -Name TestPortOpener | Stop-Process
```

## Files

- `Program.cs`: Main entry point, mode selection
- `AdvancedPortTester.cs`: Staged port opening logic
- `README.md`: This file

## Tips

- Use **Simple Mode** for quick functional testing
- Use **Advanced Mode** specifically for creation time sorting tests
- Keep TestPortOpener running while testing RBPortKiller
- TestPortOpener can run multiple instances for stress testing
- Press Ctrl+C in TestPortOpener to cleanly close all ports

## See Also

- Main documentation: `README.md`
- Usage guide: `Misc/Docs/USAGE.md`
- Development guide: `Misc/Docs/DEVELOPMENT.md`
- Architecture: `Misc/Docs/ARCHITECTURE.md`
