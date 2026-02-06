# System Process Protection & Port Creation Tracking

## Overview
This document describes the safety features implemented to protect system processes and the new functionality to track and display port creation times.

## Features Implemented

### 1. System Process Protection

#### Purpose
Prevents users from accidentally terminating critical Windows system processes that could cause system instability or crashes.

#### Implementation

**SystemProcessDetector** (`RBPortKiller.Infrastructure.Windows.Helpers.SystemProcessDetector.cs`)
- Identifies critical Windows system processes
- Uses multiple detection strategies:
  - **Process ID filtering**: PIDs 0-4 are system-critical
  - **Known process names**: Maintains a list of critical process names (System, csrss, lsass, etc.)
  - **Path-based detection**: Checks if process is located in system directories
  - **Access control**: Processes that can't be accessed are treated as system processes for safety

**Protected Process List**:
- System core: `System`, `Registry`, `smss`, `csrss`, `wininit`, `services`, `lsass`, `winlogon`
- Windows components: `svchost`, `dwm`, `explorer`, `RuntimeBroker`
- System apps: `SearchUI`, `StartMenuExperienceHost`, `SystemSettings`
- Drivers and hosts: `audiodg`, `fontdrvhost`, `WUDFHost`, `conhost`, `dllhost`

**Protected Paths**:
- `C:\Windows\System32`
- `C:\Windows\SysWOW64`
- `C:\Windows\explorer.exe`
- `C:\Windows\SystemApps`

#### Behavior
- System processes are **automatically filtered out** from the port list
- Users only see ports opened by user-installed applications
- This ensures that only killable processes (non-system) are shown

### 2. Port Creation Time Tracking

#### Purpose
Helps users identify and sort ports by when they were created, making it easier to find recently opened connections.

#### Implementation

**ProcessCreationTimeProvider** (`RBPortKiller.Infrastructure.Windows.Helpers.ProcessCreationTimeProvider.cs`)
- Retrieves process start time from Windows Process API
- Returns `null` if unavailable due to permissions or process termination
- Handles access-denied scenarios gracefully

**PortInfo Model Updates** (`RBPortKiller.Core.Models.PortInfo.cs`)
- Added `CreatedDate` property (DateTime?)
- Added `IsSystemProcess` property (bool)

#### Display Format

**Port Table**:
- **Today**: Shows time only (e.g., `14:32:15`)
- **Yesterday**: Shows "Yesterday HH:mm" (e.g., `Yesterday 18:45`)
- **Last 7 days**: Shows day and time (e.g., `Mon 09:30`)
- **Older**: Shows month/day and time (e.g., `12/25 14:00`)
- **N/A**: Displayed when creation time is unavailable

**Port Details Panel**:
- Shows full timestamp: `yyyy-MM-dd HH:mm:ss` (e.g., `2024-01-15 14:32:15`)

#### Sorting
Ports are sorted by:
1. **Creation date** (newest first)
2. **Port number** (as secondary sort)

### 3. Enhanced Process Information

**ProcessInfoProvider Updates** (`RBPortKiller.Infrastructure.Windows.Helpers.ProcessInfoProvider.cs`)
- Now returns comprehensive tuple: `(processName, processPath, createdDate, isSystemProcess)`
- Integrates system process detection
- Includes creation time retrieval
- Gracefully handles access-denied scenarios

## Architecture Adherence

### SOLID Principles
? **Single Responsibility**:
- `SystemProcessDetector`: Only detects system processes
- `ProcessCreationTimeProvider`: Only retrieves creation times
- `ProcessInfoProvider`: Orchestrates process information gathering

? **Open/Closed**:
- System process detection can be extended by adding more names/paths
- No modification to existing filtering logic required

? **Dependency Inversion**:
- All helpers use constructor injection
- Core interfaces remain unchanged

### Naming Conventions
- **Detector**: `SystemProcessDetector` - makes decisions about process type
- **Provider**: `ProcessInfoProvider`, `ProcessCreationTimeProvider` - retrieve information
- **Builder**: `PortTableBuilder` - constructs UI components

### Error Handling
- Graceful degradation when process info unavailable
- Returns default values (null, false) instead of throwing
- System processes treated as unsafe by default when uncertain

## Security Considerations

### Safety First Approach
1. **Unknown = System**: If we can't determine if a process is safe, we treat it as a system process
2. **Access Denied = Protected**: Processes we can't access are assumed to be system-critical
3. **No Surprises**: Users only see processes they can safely terminate

### User Experience
- **Cleaner List**: Only shows user-controllable processes
- **No Errors**: Eliminates "Access Denied" errors when trying to kill system processes
- **Better Context**: Creation time helps identify recent/relevant connections

## Testing Recommendations

### Manual Testing
1. **System Process Filtering**:
   - Run the tool and verify no `svchost.exe`, `System`, `csrss.exe` appear
   - Check that `explorer.exe` is filtered out
   
2. **Creation Time Display**:
   - Open a new application (e.g., browser)
   - Refresh port list and verify it appears at the top
   - Check time format matches expectations

3. **User Application Ports**:
   - Start a web server (e.g., `python -m http.server`)
   - Verify it appears in the list with creation time
   - Confirm you can kill it

### Edge Cases
- Processes that start and stop quickly
- Processes with restricted access
- Applications running as administrator
- Multiple ports from the same process

## Future Enhancements

### Potential Additions
1. **Configurable Filter**: Allow users to show/hide system processes
2. **Process Trust Level**: Display security context (User, Admin, System)
3. **Connection Duration**: Show how long the connection has been active
4. **Process Tree**: Show parent-child relationships
5. **Custom Process Lists**: Allow users to define their own "protected" processes

## Related Files

### Core Layer
- `RBPortKiller.Core\Models\PortInfo.cs` - Enhanced model

### Infrastructure Layer
- `RBPortKiller.Infrastructure\Windows\WindowsPortDiscoveryService.cs` - Updated discovery
- `RBPortKiller.Infrastructure\Windows\Helpers\SystemProcessDetector.cs` - NEW
- `RBPortKiller.Infrastructure\Windows\Helpers\ProcessCreationTimeProvider.cs` - NEW
- `RBPortKiller.Infrastructure\Windows\Helpers\ProcessInfoProvider.cs` - Enhanced

### CLI Layer
- `RBPortKiller.CLI\UI\PortTableBuilder.cs` - Updated display
- `RBPortKiller.CLI\UI\PortDetailsPanelBuilder.cs` - Updated details

## Changelog

### Version: Current
**Added**:
- System process detection and filtering
- Port creation time tracking and display
- Enhanced process information retrieval
- User-friendly time formatting

**Changed**:
- `PortInfo` model now includes `CreatedDate` and `IsSystemProcess`
- `ProcessInfoProvider` returns additional information
- Port list automatically filters system processes
- Port list sorted by creation date (newest first)

**Security**:
- Users cannot see or attempt to kill system-critical processes
- Reduced risk of system instability
- Better user experience with relevant ports only
