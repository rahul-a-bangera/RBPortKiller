# Testing Guide for RBPortKiller

This guide walks you through testing the new system process protection and creation time features.

## Prerequisites

- .NET 8 SDK installed
- Windows OS (for full testing)
- Two terminal windows

## Step-by-Step Testing

### 1. Build Everything

```bash
# Build RBPortKiller
dotnet build

# Build TestPortOpener
dotnet build TestPortOpener\TestPortOpener.csproj
```

### 2. Test System Process Filtering

**Goal**: Verify that Windows system processes are hidden from the port list.

**Steps**:
1. Open Terminal 1, run RBPortKiller:
   ```bash
   cd RBPortKiller.CLI
   dotnet run
   ```

2. Look at the port list - you should **NOT** see:
   - `svchost` (Windows Service Host)
   - `System` (Kernel)
   - `csrss` (Client Server Runtime)
   - `lsass` (Local Security Authority)
   - `explorer` (Windows Explorer)
   
3. You **SHOULD** see user applications like:
   - Your IDE (Visual Studio, VS Code)
   - Browsers (Chrome, Edge, Firefox)
   - Development tools (Node.js, Docker Desktop)

? **Success Criteria**: No Windows system processes appear in the list.

### 3. Test Creation Time Display (Simple)

**Goal**: Verify port creation times are displayed correctly.

**Steps**:
1. Open Terminal 1, run TestPortOpener:
   ```bash
   run-test-ports.bat
   # Choose option: 1 (Simple Mode)
   ```

2. Note the timestamps shown when ports open (e.g., `[14:32:15]`)

3. Open Terminal 2, run RBPortKiller:
   ```bash
   cd RBPortKiller.CLI
   dotnet run
   ```

4. Find `TestPortOpener` in the list

5. Check the "Created" column - should show times matching when you started TestPortOpener

? **Success Criteria**: 
   - All TestPortOpener ports show creation times
   - Times match approximately when you started the app
   - Today's ports show time only (e.g., `14:32:15`)

### 4. Test Creation Time Sorting (Advanced)

**Goal**: Verify ports are sorted by creation time (newest first).

**Steps**:
1. **STOP** TestPortOpener if it's running (Ctrl+C)

2. Open Terminal 1, run TestPortOpener in Advanced Mode:
   ```bash
   run-test-ports.bat
   # Choose option: 2 (Advanced Mode)
   ```

3. Watch the stages:
   - Stage 1: Ports 3000, 5000 open
   - Wait 2 seconds...
   - Stage 2: Ports 8080, 9000 open
   - Wait 3 more seconds...
   - Stage 3: Ports 7777, 4200, 6000 open

4. Open Terminal 2, run RBPortKiller:
   ```bash
   cd RBPortKiller.CLI
   dotnet run
   ```

5. Find TestPortOpener ports in the list

6. Verify the **ORDER** (top to bottom):
   ```
   Position 1: Port 7777 or 4200 or 6000 (Stage 3 - newest)
   Position 2: Port 7777 or 4200 or 6000 (Stage 3)
   Position 3: Port 7777 or 4200 or 6000 (Stage 3)
   Position 4: Port 8080 or 9000 (Stage 2)
   Position 5: Port 8080 or 9000 (Stage 2)
   Position 6: Port 3000 or 5000 (Stage 1 - oldest)
   Position 7: Port 3000 or 5000 (Stage 1)
   ```

? **Success Criteria**: 
   - Newest ports (Stage 3) appear at the **top**
   - Oldest ports (Stage 1) appear at the **bottom**
   - Sorting is clearly visible

### 5. Test Port Termination

**Goal**: Verify you can successfully kill TestPortOpener.

**Steps**:
1. Ensure TestPortOpener is running (Terminal 1)

2. In Terminal 2, run RBPortKiller and select any TestPortOpener port

3. View port details - should show:
   - Process Name: `TestPortOpener`
   - PID: (some number)
   - Creation time: (recent timestamp)
   - Process Path: (path to TestPortOpener.dll)

4. Choose "Kill Process"

5. Confirm the action

6. Watch Terminal 1 - TestPortOpener should exit

7. Check RBPortKiller again - TestPortOpener ports should be gone

? **Success Criteria**:
   - Process terminates successfully
   - No "Access Denied" errors
   - All ports close immediately
   - Success message displayed

### 6. Test Port List Refresh

**Goal**: Verify the refresh functionality reloads ports correctly.

**Steps**:
1. Start TestPortOpener in Terminal 1

2. Run RBPortKiller in Terminal 2 - note the ports shown

3. While RBPortKiller is showing the menu, go to Terminal 1

4. **Without closing TestPortOpener**, start another application that opens ports:
   ```bash
   # Example: Start a simple Python HTTP server in a new terminal
   python -m http.server 8888
   ```

5. Go back to RBPortKiller (Terminal 2)

6. Select "Refresh Port List" from the menu

7. Observe the screen clears and shows "Refreshing port list..."

8. Verify the new Python server port (8888) now appears in the list

9. Test removing a port: Close the Python server (Ctrl+C)

10. Select "Refresh Port List" again

11. Verify port 8888 is no longer in the list

? **Success Criteria**:
   - Refresh option appears before "Exit" in the menu
   - Screen clears when refresh is selected
   - New ports appear after refresh
   - Closed ports disappear after refresh
   - Creation times update correctly
   - Banner is redisplayed after refresh
   - No errors or crashes

### 7. Test Quick Exit (Ctrl+C)

**Goal**: Verify that Ctrl+C provides a quick way to exit the application.

**Steps**:
1. Start TestPortOpener in Terminal 1

2. Run RBPortKiller in Terminal 2

3. When the port selection menu appears, note the tip message below the table:
   - Should display: `Tip: Press Ctrl+C anytime to exit quickly`

4. Press **Ctrl+C** on your keyboard

5. Observe the behavior:
   - Application should immediately exit
   - Should display: `Exiting...` in yellow
   - Should display: `Thank you for using RBPortKiller!` in green

6. Verify clean exit (no errors or stack traces)

7. Restart RBPortKiller and select a port to view details

8. Press **Ctrl+C** while in the port details/action menu

9. Verify it exits immediately from any screen

? **Success Criteria**:
   - Tip message is visible below port table
   - Ctrl+C works from port selection menu
   - Ctrl+C works from port action menu
   - Clean exit with "Exiting..." message
   - No error messages or stack traces
   - Graceful shutdown

### 8. Test Edge Cases

#### Test 8a: Process Already Terminated

1. Start TestPortOpener
2. Run RBPortKiller - see the ports
3. Manually close TestPortOpener (Ctrl+C in Terminal 1)
4. Select "Refresh Port List"
5. TestPortOpener ports should disappear

? **Success**: Ports are no longer listed after process exits.

#### Test 8b: No Ports Available

1. Close all user applications
2. Run RBPortKiller
3. You might see few or no ports

? **Success**: Message "No active ports found" or minimal list.

#### Test 8c: Unknown Creation Time

Some older system services might show:
- Creation time: `N/A`

? **Success**: Application doesn't crash, just shows N/A.

### 9. Visual Verification Checklist

When viewing the port table, verify:

- [ ] **Column Headers**:
  - `#` (Index)
  - `Port`
  - `Protocol`
  - `PID`
  - `Process Name`
  - `Local Address`
  - `State`
  - `Created` ? **NEW**

- [ ] **Color Coding**:
  - Port numbers: Yellow
  - Protocols: Blue
  - PIDs: Magenta
  - Process names: Green
  - Creation times: Cyan (recent) / Yellow (older) / Dim (N/A)

- [ ] **Creation Time Formats**:
  - Today: `HH:mm:ss` (e.g., `14:32:15`)
  - Yesterday: `Yesterday HH:mm`
  - This week: `DDD HH:mm` (e.g., `Mon 09:30`)
  - Older: `MM/dd HH:mm` (e.g., `12/25 14:00`)

- [ ] **Menu Options**:
  - Port selections (numbered 1, 2, 3, ...)
  - "Refresh Port List" option <- **NEW**
  - "Exit" option (last item)

- [ ] **User Hints**:
  - "Tip: Press Ctrl+C anytime to exit quickly" <- **NEW**

### 10. Port Details Panel Verification

When selecting a port:

```
????????????????????????????????????????
?       Selected Port Details          ?
????????????????????????????????????????
? Port: 3000                           ?
? Protocol: TCP                        ?
? Process ID: 12345                    ?
? Process Name: TestPortOpener         ?
? Local Address: 0.0.0.0              ?
? State: LISTENING                     ?
? Created: 2024-01-15 14:32:15  ? NEW ?
? Process Path: C:\...\TestPortOpener.dll ?
????????????????????????????????????????
```

? Verify "Created" line is present with full timestamp.

## Expected vs Actual

### What You Should See ?

```
#  Port  Protocol  PID    Process Name      State       Created
?????????????????????????????????????????????????????????????????
1  7777  TCP       5432   TestPortOpener    LISTENING   14:35:20
2  4200  TCP       5432   TestPortOpener    LISTENING   14:35:20
3  8080  TCP       5432   TestPortOpener    LISTENING   14:35:18
4  3000  TCP       5432   TestPortOpener    LISTENING   14:35:16
5  4000  TCP       8765   node              LISTENING   12:30:45
6  8000  TCP       9123   python            LISTENING   Yesterday 18:30
```

### What You Should NOT See ?

```
# NO system processes:
?  445   TCP       4      System            LISTENING   N/A
?  135   TCP       1024   svchost           LISTENING   N/A
?  139   TCP       4      System            LISTENING   N/A
```

## Troubleshooting

### Issue: "No ports found"
**Solution**: Start TestPortOpener first, then run RBPortKiller.

### Issue: "Access Denied" when killing
**Solution**: Run RBPortKiller as Administrator.

### Issue: TestPortOpener not in list
**Possible Causes**:
1. It crashed - check Terminal 1
2. Ports are in use by another app - check error messages
3. System process filter bug - check if process name is correct

### Issue: Creation times all show "N/A"
**Possible Causes**:
1. Permission issue - run as Administrator
2. Process started before tracking began
3. Bug in ProcessCreationTimeProvider

### Issue: Sorting doesn't work
**Check**:
1. Are creation times different? (Use Advanced Mode)
2. Is sorting logic implemented correctly?
3. Check WindowsPortDiscoveryService.GetActivePortsAsync

## Performance Testing

### Large Number of Ports

Modify TestPortOpener to open many ports:

```csharp
for (int port = 5000; port < 5100; port++)
{
    StartTcpListener(port, $"Test Port {port}");
}
```

Run RBPortKiller and verify:
- [ ] All ports load
- [ ] Sorting still works
- [ ] UI remains responsive
- [ ] No crashes or timeouts

## Security Testing

### Verify System Process Protection

Try to manually find system processes:

```bash
# Windows - show all ports
netstat -ano

# Look for PID 4 (System)
# Look for PID 0 
# Look for svchost PIDs
```

Then run RBPortKiller - these should NOT appear in the list.

? **Success**: System processes are completely hidden.

## Completion Checklist

Test complete when you can confirm:

- [x] System processes are filtered out
- [x] TestPortOpener appears in the list
- [x] Creation times are displayed
- [x] Times are formatted correctly
- [x] Ports are sorted by creation time (newest first)
- [x] Can successfully kill TestPortOpener
- [x] Port details panel shows creation time
- [x] No crashes or errors
- [x] UI is responsive and clear

## Next Steps

After successful testing:
1. Document any bugs found
2. Create GitHub issues for improvements
3. Consider adding more test scenarios
4. Update user documentation
5. Prepare for production deployment

## Getting Help

If tests fail:
1. Check build logs: `dotnet build`
2. Review error messages in console
3. Verify .NET 8 is installed: `dotnet --version`
4. Check documentation in `Misc/Docs/`
5. Review code changes in git diff

## Summary

This testing verifies:
? **Safety**: System processes protected from termination  
? **Usability**: Creation times help identify relevant ports  
? **Sorting**: Newest connections appear first  
? **Reliability**: Graceful handling of edge cases  
? **UI/UX**: Clear, informative display  

Happy testing! ??
