# Port List Refresh Feature

## Overview
The refresh feature allows users to reload the active ports list without restarting the application, making it easy to see newly opened or recently closed ports.

## How to Use

### From the Port Selection Menu
1. Run RBPortKiller
2. View the list of active ports
3. Scroll to the bottom of the port selection menu
4. Select **"Refresh Port List"** (appears just before "Exit")
5. The screen clears and reloads with updated port information

### Visual Flow
```
Port List ? Select "Refresh Port List" ? Screen Clears ? Banner Redisplays ? New Port List
```

## When to Use Refresh

### Scenario 1: New Application Started
You've just started a new application (like a web server) and want to see its ports without exiting RBPortKiller.

**Action**: Select refresh to see the newly opened ports.

### Scenario 2: Application Closed
You closed an application outside of RBPortKiller and want to verify its ports are no longer active.

**Action**: Select refresh to confirm ports are gone.

### Scenario 3: Port Status Changed
A connection state has changed (e.g., from ESTABLISHED to TIME_WAIT) and you want to see the current status.

**Action**: Select refresh to see updated connection states.

### Scenario 4: Verifying Kill Action
After killing a process, you want to confirm all its ports are closed.

**Action**: Return to main menu and select refresh to verify.

## What Happens During Refresh

1. **Screen Clears**: Console is cleared for a fresh view
2. **Banner Redisplays**: RBPortKiller banner is shown again
3. **Status Message**: "Refreshing port list..." is displayed
4. **Port Discovery**: System queries active ports (same as initial load)
5. **Filtering**: System processes are filtered out
6. **Sorting**: Ports are sorted by creation time (newest first)
7. **Display**: Updated port table is shown
8. **Menu Returns**: Selection menu appears with refreshed data

## Technical Details

### Implementation
- Uses the same `LoadPortsAsync()` method as initial load
- Maintains all filtering (system processes are still hidden)
- Preserves sorting (newest ports first)
- No state is cached - always queries fresh data

### Performance
- **Typical refresh time**: <1 second
- Depends on number of active connections
- No performance degradation with multiple refreshes

### Thread Safety
- Refresh uses async/await pattern
- Safe to use repeatedly
- Cancellation token support (Ctrl+C)

## Architecture

### Classes Involved

**PortSelectionAction** (Enum)
```csharp
public enum PortSelectionAction
{
    PortSelected,  // User selected a port
    Refresh,       // User chose refresh
    Exit           // User chose exit
}
```

**PortSelectionResult** (Class)
```csharp
public sealed class PortSelectionResult
{
    public PortSelectionAction Action { get; init; }
    public PortInfo? SelectedPort { get; init; }
    
    public static PortSelectionResult Refresh();
}
```

**PortSelectionFormatter** (Updated)
- Added "Refresh Port List" to menu choices
- Returns -2 index for refresh selection
- Maintains backward compatibility

**PortKillerCli** (Updated)
- Handles `PortSelectionAction.Refresh` case
- Clears screen and redisplays banner
- Loops back to reload ports

### Code Flow
```
RunAsync() 
  ?
LoadPortsAsync() ? Display ports
  ?
DisplayPortSelectionMenu()
  ?
User selects "Refresh"
  ?
ParseSelectionIndex() returns -2
  ?
PortSelectionResult.Refresh() created
  ?
Action == Refresh detected
  ?
Clear screen + Display banner
  ?
Continue loop (back to LoadPortsAsync)
```

## User Experience

### Before Refresh
```
Select a port to manage:
? 1. Port 3000 (TCP) - TestPortOpener [PID: 5432]
  2. Port 5000 (TCP) - TestPortOpener [PID: 5432]
  Refresh Port List
  Exit
```

### During Refresh
```
[Screen clears]

    ____  ____  ____             __   __ __ _ ____            
   / __ \/ __ )/ __ \____  _____/ /_ / //_/(_) / /__  _____  
  / /_/ / __  / /_/ / __ \/ ___/ __// ,<  / / / / _ \/ ___/  
 / _, _/ /_/ / ____/ /_/ / /  / /_ / /| |/ / / /  __/ /      
/_/ |_/_____/_/    \____/_/   \__//_/ |_/_/_/_/\___/_/       

A powerful CLI tool for managing network ports

Refreshing port list...

? Loading active ports...
```

### After Refresh
```
[Updated port table with new data]

Select a port to manage:
? 1. Port 8888 (TCP) - python [PID: 9999]        ? NEW
  2. Port 3000 (TCP) - TestPortOpener [PID: 5432]
  3. Port 5000 (TCP) - TestPortOpener [PID: 5432]
  Refresh Port List
  Exit
```

## Benefits

### For Users
- No need to restart the application
- Quick way to see new ports
- Verify kill operations worked
- Monitor port changes in real-time
- Reduced context switching

### For Developers
- Clean separation of concerns
- Reuses existing load logic (DRY principle)
- Type-safe action handling (enum-based)
- Easy to test
- No additional dependencies

## Testing Recommendations

### Manual Testing
1. **Basic Refresh**: Start app, select refresh, verify it reloads
2. **New Port**: Start new app while RBPortKiller running, refresh, verify new port appears
3. **Closed Port**: Close app while RBPortKiller running, refresh, verify port disappears
4. **Multiple Refreshes**: Refresh multiple times, verify no errors
5. **Empty List**: Close all apps, refresh, verify "No ports found" message

### Edge Cases
- Refresh when no ports are active
- Refresh after killing a process
- Rapid multiple refreshes
- Refresh with hundreds of ports

### Performance Testing
- Measure refresh time with 10, 50, 100+ ports
- Verify no memory leaks with repeated refreshes
- Check CPU usage during refresh

## Limitations

### Current Version
- Refresh is manual (no auto-refresh)
- No indication of what changed (all ports are reloaded)
- No "diff" view showing added/removed ports

### Future Enhancements
- **Auto-refresh mode**: Reload every N seconds
- **Change highlighting**: Show new/removed ports in different colors
- **Refresh interval config**: User-configurable refresh rate
- **Background monitoring**: Detect changes and prompt to refresh
- **Diff view**: Show side-by-side before/after comparison

## Keyboard Shortcuts (Future)
Consider adding:
- `F5` - Refresh (like browsers)
- `Ctrl+R` - Refresh (common convention)
- `Space` - Quick refresh from any menu

## Comparison with Other Tools

### netstat
- Must re-run command manually
- No persistent UI
- **RBPortKiller advantage**: Single command to refresh

### Task Manager
- Auto-refreshes but can't be controlled
- No manual refresh option
- **RBPortKiller advantage**: User-controlled timing

### TCPView (Sysinternals)
- Auto-refreshes continuously
- No manual refresh control
- **RBPortKiller advantage**: Both manual and (future) auto modes

## FAQ

**Q: How often can I refresh?**  
A: As often as you want. There's no cooldown or limit.

**Q: Does refresh show system processes?**  
A: No, system process filtering is maintained during refresh.

**Q: Will refresh show processes I don't have permission for?**  
A: No, permissions are checked on each refresh.

**Q: Does refresh clear my selection?**  
A: Yes, you'll return to the port selection menu.

**Q: Can I undo a refresh?**  
A: No, but you can navigate the previous list in your scroll buffer.

**Q: Why not auto-refresh?**  
A: Manual refresh gives you control. Auto-refresh may be added as an option in the future.

## Related Documentation

- `TESTING_GUIDE.md` - Test section 6: Port List Refresh
- `QUICK_TEST.md` - Refresh feature testing
- `PortSelectionAction.cs` - Action enum definition
- `PortSelectionResult.cs` - Result class
- `PortKillerCli.cs` - Main CLI implementation

## Version History

### v1.1.0 (Current)
- Added manual refresh functionality
- Refresh option appears before Exit
- Screen clears on refresh
- Banner redisplays after refresh
- All filtering and sorting maintained

### Future Versions
- [Future] Auto-refresh mode
- [Future] Change highlighting
- [Future] Keyboard shortcuts
- [Future] Diff view

---

**The refresh feature makes RBPortKiller more dynamic and user-friendly by allowing real-time monitoring without application restart!**
