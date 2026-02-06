# Quick Start: Testing RBPortKiller

## In 3 Simple Steps

### Step 1: Open Test Ports (Terminal 1)
```bash
run-test-ports.bat
```
Choose option **1** for simple testing or **2** for advanced sorting tests.

### Step 2: Run RBPortKiller (Terminal 2)
```bash
cd RBPortKiller.CLI
dotnet run
```

### Step 3: Verify Features
Look for these in the port list:
- **TestPortOpener** appears (user process)
- **svchost, System, explorer** don't appear (system processes)
- **Created column** shows times (e.g., `14:32:15`)
- **Newest ports are at the top**
- **Tip shown** about pressing Ctrl+C to exit

## What to Test

1. **System Process Protection**
   - No Windows system processes in the list
   - Only user applications visible

2. **Creation Time Display**
   - "Created" column shows timestamps
   - Recent ports show time (e.g., `14:32:15`)
   - Older ports show date+time (e.g., `Yesterday 18:30`)

3. **Creation Time Sorting**
   - Newest connections at the **top**
   - Oldest connections at the **bottom**
   - Use Advanced Mode (option 2) to test this

4. **Port List Refresh**
- Select "Refresh Port List" from the menu
   - Port list reloads automatically
   - New ports appear if any were opened
   - Removed ports disappear if any were closed

5. **Quick Exit (Ctrl+C)**
   - Press Ctrl+C at any point in the menu
   - Application exits immediately
   - Clean exit message displayed

6. **Process Termination**
   - Select TestPortOpener port
   - Choose "Kill Process"
   - Verify it terminates successfully
   - No "Access Denied" errors

## Expected Output

### TestPortOpener (Terminal 1)
```
??????????????????????????????????????????????????????????
?          Test Port Opener for RBPortKiller            ?
??????????????????????????????????????????????????????????

? [14:32:11] TCP Port  3000 - Development Server
? [14:32:11] TCP Port  5000 - API Server
? [14:32:11] TCP Port  8080 - Web Server
? [14:32:11] TCP Port  9000 - Test Service
? [14:32:11] TCP Port  7777 - Game Server
? [14:32:11] UDP Port  6000 - UDP Service 1
? [14:32:11] UDP Port  6001 - UDP Service 2

All ports are now OPEN and ready for testing!
Press Ctrl+C to stop...
```

### RBPortKiller (Terminal 2)
```
    ____  ____  ____             __   __ __ _ ____            
   / __ \/ __ )/ __ \____  _____/ /_ / //_/(_) / /__  _____  
  / /_/ / __  / /_/ / __ \/ ___/ __// ,<  / / / / _ \/ ___/  
 / _, _/ /_/ / ____/ /_/ / /  / /_ / /| |/ / / /  __/ /      
/_/ |_/_____/_/    \____/_/   \__//_/ |_/_/_/_/\___/_/       

A powerful CLI tool for managing network ports

????????????????????????????????????????????????????????????????????????
? #  ? Port ? Protocol ? PID  ? Process Name    ? State     ? Created  ?
????????????????????????????????????????????????????????????????????????
? 1  ? 3000 ? TCP      ? 5432 ? TestPortOpener  ? LISTENING ? 14:32:11 ?
? 2  ? 5000 ? TCP      ? 5432 ? TestPortOpener  ? LISTENING ? 14:32:11 ?
? 3  ? 8080 ? TCP      ? 5432 ? TestPortOpener  ? LISTENING ? 14:32:11 ?
? 4  ? 9000 ? TCP      ? 5432 ? TestPortOpener  ? LISTENING ? 14:32:11 ?
? 5  ? 7777 ? TCP      ? 5432 ? TestPortOpener  ? LISTENING ? 14:32:11 ?
? 6  ? 6000 ? UDP      ? 5432 ? TestPortOpener  ? N/A       ? 14:32:11 ?
? 7  ? 6001 ? UDP      ? 5432 ? TestPortOpener  ? N/A       ? 14:32:11 ?
????????????????????????????????????????????????????????????????????????

Tip: Press Ctrl+C anytime to exit quickly

Select a port to manage:
? 1. Port 3000 (TCP) - TestPortOpener [PID: 5432]
  2. Port 5000 (TCP) - TestPortOpener [PID: 5432]
  ...
  7. Port 6001 (UDP) - TestPortOpener [PID: 5432]
  Refresh Port List
  Exit
```

## Success Criteria ?

After testing, you should verify:

- [ ] TestPortOpener visible in RBPortKiller
- [ ] No system processes (svchost, System, etc.) visible
- [ ] "Created" column shows accurate timestamps
- [ ] Ports sorted by creation time (newest first)
- [ ] "Tip: Press Ctrl+C anytime to exit quickly" displayed below table
- [ ] "Refresh Port List" option appears before "Exit"
- [ ] Refresh reloads ports correctly
- [ ] Ctrl+C exits immediately with "Exiting..." message
- [ ] Can kill TestPortOpener successfully
- [ ] All TestPortOpener ports close when killed
- [ ] No error messages or crashes

## Quick Commands Reference

| Action | Command |
|--------|---------|
| Build all | `dotnet build` |
| Open test ports | `run-test-ports.bat` |
| Run RBPortKiller | `cd RBPortKiller.CLI && dotnet run` |
| Stop test ports | Press `Ctrl+C` in TestPortOpener window |
| Check if ports open | `netstat -ano \| findstr "3000 5000 8080"` |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Port in use | Close other apps using that port |
| Access denied | Run as Administrator |
| TestPortOpener not showing | Restart it, check Terminal 1 for errors |
| No creation times | Run RBPortKiller as Administrator |

## Learn More

- **Full Testing Guide**: `Misc\Docs\TESTING_GUIDE.md`
- **Technical Details**: `Misc\Docs\SYSTEM_PROCESS_PROTECTION.md`
- **User Guide**: `Misc\Docs\PORT_CREATION_TIMES.md`
- **TestPortOpener Docs**: `TestPortOpener\README.md`

## Support

If something doesn't work:
1. Check Terminal 1 for TestPortOpener errors
2. Check Terminal 2 for RBPortKiller errors
3. Verify .NET 8 is installed: `dotnet --version`
4. Review the detailed testing guide
5. Check the documentation files

---

**Ready to test?** 

1. Open Terminal 1 ? `run-test-ports.bat` ? Choose mode
2. Open Terminal 2 ? `cd RBPortKiller.CLI && dotnet run`
3. Find TestPortOpener in the list
4. Verify creation times are shown
5. Kill the process
6. Success! ??
