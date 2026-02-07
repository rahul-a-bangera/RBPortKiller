@echo off
REM Force window to stay open on any error
if not "%1"=="noerror" (
    cmd /c "%~f0" noerror %*
    exit /b
)

REM ============================================================================
REM RBPortKiller Uninstallation Script
REM Removes rbportkiller.exe from Windows PATH
REM ============================================================================

echo.
echo ====================================
echo    RBPortKiller Uninstallation
echo ====================================
echo.

REM Get installation directory (where this batch file is located)
set "INSTALL_DIR=%~dp0"
REM Remove trailing backslash
if "%INSTALL_DIR:~-1%"=="\" set "INSTALL_DIR=%INSTALL_DIR:~0,-1%"

echo Uninstalling from: %INSTALL_DIR%
echo.

REM Check for administrator privileges
net session >nul 2>&1
set ADMIN_CHECK=%errorLevel%

if "%ADMIN_CHECK%"=="0" (
    set "IS_ADMIN=1"
    echo [INFO] Running with administrator privileges
    echo [INFO] Removing from SYSTEM PATH ^(all users^)...
) else (
    set "IS_ADMIN=0"
    echo [WARN] Not running as administrator
    echo [INFO] Removing from USER PATH ^(current user only^)...
    echo.
    echo To uninstall system-wide: Right-click uninstall.bat and select "Run as administrator"
)
echo.

REM ============================================================================
REM REMOVE FROM PATH
REM ============================================================================

if "%IS_ADMIN%"=="1" goto ADMIN_UNINSTALL
goto USER_UNINSTALL

:ADMIN_UNINSTALL
REM ===== SYSTEM PATH (ADMIN) =====
echo Checking system PATH...
echo.

REM Read current system PATH from registry
for /f "skip=2 tokens=2*" %%a in ('reg query "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v Path 2^>nul') do set "CURRENT_PATH=%%b"

if not defined CURRENT_PATH (
    echo [ERROR] Could not read system PATH from registry
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

REM Check if installation directory is in PATH
echo "%CURRENT_PATH%" | findstr /i /c:"%INSTALL_DIR%" >nul 2>&1
set CHECK_RESULT=%errorLevel%
if "%CHECK_RESULT%" neq "0" (
    echo [INFO] RBPortKiller is not in system PATH
    echo [INFO] Nothing to uninstall from system PATH
    goto CHECK_USER_PATH
)

REM Remove from system PATH
echo Removing from system PATH...

REM Create new PATH without the installation directory
set "NEW_PATH="
setlocal enabledelayedexpansion

for %%p in ("%CURRENT_PATH:;=" "%") do (
    set "ENTRY=%%~p"
    REM Remove quotes and compare
    if /i not "!ENTRY!"=="%INSTALL_DIR%" (
        if defined NEW_PATH (
            set "NEW_PATH=!NEW_PATH!;!ENTRY!"
        ) else (
            set "NEW_PATH=!ENTRY!"
        )
    )
)

REM Update registry
endlocal & set "NEW_PATH=%NEW_PATH%"

REM Check if new PATH would exceed setx limit (1024 chars)
call :strlen NEW_PATH NEW_PATH_LEN

if %NEW_PATH_LEN% GTR 1024 (
    echo [INFO] Using PowerShell method for long PATH...
    powershell -Command "[Environment]::SetEnvironmentVariable('Path', '%NEW_PATH%', 'Machine')" >nul 2>&1
    set SETX_RESULT=%errorLevel%
) else (
    setx PATH "%NEW_PATH%" /M >nul 2>&1
    set SETX_RESULT=%errorLevel%
)

if "%SETX_RESULT%"=="0" (
    echo [SUCCESS] Successfully removed from system PATH!
    echo.
    REM Refresh PATH for current session
    set "PATH=%NEW_PATH%"
    echo [INFO] PATH refreshed for current session
    echo.
    echo To apply in NEW windows: Close and reopen your Command Prompt/PowerShell
    echo In THIS window: The PATH has been updated for this session
    goto UNINSTALL_COMPLETE
) else (
    echo [ERROR] Failed to update system PATH
    echo [ERROR] Error code: %SETX_RESULT%
    echo.
    echo Possible causes:
    echo  - PATH variable is too long
    echo  - Insufficient permissions
    echo  - Registry access issues
    echo.
    echo Try manual removal or contact support.
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

:USER_UNINSTALL
REM ===== USER PATH (NON-ADMIN) =====
echo Checking user PATH...
echo.

REM Read current user PATH from registry
for /f "skip=2 tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path 2^>nul') do set "CURRENT_PATH=%%b"

REM If user PATH doesn't exist
if not defined CURRENT_PATH (
    echo [INFO] User PATH is empty or not set
    echo [INFO] Nothing to uninstall from user PATH
    goto CHECK_SYSTEM_PATH
)

REM Check if installation directory is in PATH
echo "%CURRENT_PATH%" | findstr /i /c:"%INSTALL_DIR%" >nul 2>&1
set CHECK_RESULT=%errorLevel%
if "%CHECK_RESULT%" neq "0" (
    echo [INFO] RBPortKiller is not in user PATH
    echo [INFO] Nothing to uninstall from user PATH
    goto CHECK_SYSTEM_PATH
)

REM Remove from user PATH
echo Removing from user PATH...

REM Create new PATH without the installation directory
set "NEW_PATH="
setlocal enabledelayedexpansion

for %%p in ("%CURRENT_PATH:;=" "%") do (
    set "ENTRY=%%~p"
    REM Remove quotes and compare
    if /i not "!ENTRY!"=="%INSTALL_DIR%" (
        if defined NEW_PATH (
            set "NEW_PATH=!NEW_PATH!;!ENTRY!"
        ) else (
            set "NEW_PATH=!ENTRY!"
        )
    )
)

REM Update registry
endlocal & set "NEW_PATH=%NEW_PATH%"

REM Check if new PATH would exceed setx limit (1024 chars)
call :strlen NEW_PATH NEW_PATH_LEN

if defined NEW_PATH (
    if %NEW_PATH_LEN% GTR 1024 (
        echo [INFO] Using PowerShell method for long PATH...
        powershell -Command "[Environment]::SetEnvironmentVariable('Path', '%NEW_PATH%', 'User')" >nul 2>&1
        set SETX_RESULT=%errorLevel%
    ) else (
        setx PATH "%NEW_PATH%" >nul 2>&1
        set SETX_RESULT=%errorLevel%
    )
) else (
    REM If PATH would be empty, set it to empty string
    setx PATH "" >nul 2>&1
    set SETX_RESULT=%errorLevel%
)

if "%SETX_RESULT%"=="0" (
    echo [SUCCESS] Successfully removed from user PATH!
    echo.
    REM Refresh PATH for current session
    if defined NEW_PATH (
        set "PATH=%NEW_PATH%"
    ) else (
        set "PATH="
    )
    echo [INFO] PATH refreshed for current session
    echo.
    echo To apply in NEW windows: Close and reopen your Command Prompt/PowerShell
    echo In THIS window: The PATH has been updated for this session
    goto UNINSTALL_COMPLETE
) else (
    echo [ERROR] Failed to update user PATH
    echo [ERROR] Error code: %SETX_RESULT%
    echo.
    echo Possible causes:
    echo  - PATH variable is too long
    echo  - Registry access issues
    echo.
    echo Try manual removal or contact support.
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

:CHECK_USER_PATH
REM If not found in system PATH (for admin), check user PATH as fallback
echo.
echo [INFO] Not found in system PATH
echo [INFO] Checking user PATH as fallback...
echo.

for /f "skip=2 tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path 2^>nul') do set "CURRENT_PATH=%%b"

if not defined CURRENT_PATH (
    echo [INFO] User PATH is empty or not set
    echo [SUCCESS] RBPortKiller is not installed
    goto UNINSTALL_COMPLETE
)

echo "%CURRENT_PATH%" | findstr /i /c:"%INSTALL_DIR%" >nul 2>&1
set CHECK_RESULT=%errorLevel%
if "%CHECK_RESULT%" neq "0" (
    echo [INFO] Not found in user PATH either
    echo [SUCCESS] RBPortKiller is not installed
    goto UNINSTALL_COMPLETE
)

echo [WARN] Found in user PATH
echo [INFO] Removing from user PATH...
goto USER_UNINSTALL_DIRECT

:CHECK_SYSTEM_PATH
REM If not found in user PATH (for non-admin), inform about system PATH
echo.
echo [INFO] Not found in user PATH
echo [WARN] RBPortKiller may be installed in system PATH
echo.
echo To remove from system PATH:
echo   1. Right-click uninstall.bat
echo   2. Select "Run as administrator"
echo.
goto UNINSTALL_COMPLETE

:USER_UNINSTALL_DIRECT
REM Same logic as USER_UNINSTALL but jumped to from CHECK_USER_PATH
set "NEW_PATH="
setlocal enabledelayedexpansion

for %%p in ("%CURRENT_PATH:;=" "%") do (
    set "ENTRY=%%~p"
    if /i not "!ENTRY!"=="%INSTALL_DIR%" (
        if defined NEW_PATH (
            set "NEW_PATH=!NEW_PATH!;!ENTRY!"
        ) else (
            set "NEW_PATH=!ENTRY!"
        )
    )
)

endlocal & set "NEW_PATH=%NEW_PATH%"

REM Check if new PATH would exceed setx limit (1024 chars)
call :strlen NEW_PATH NEW_PATH_LEN

if defined NEW_PATH (
    if %NEW_PATH_LEN% GTR 1024 (
        echo [INFO] Using PowerShell method for long PATH...
        powershell -Command "[Environment]::SetEnvironmentVariable('Path', '%NEW_PATH%', 'User')" >nul 2>&1
        set SETX_RESULT=%errorLevel%
    ) else (
        setx PATH "%NEW_PATH%" >nul 2>&1
        set SETX_RESULT=%errorLevel%
    )
) else (
    setx PATH "" >nul 2>&1
    set SETX_RESULT=%errorLevel%
)

if "%SETX_RESULT%"=="0" (
echo [SUCCESS] Successfully removed from user PATH!
echo.
REM Refresh PATH for current session
if defined NEW_PATH (
    set "PATH=%NEW_PATH%"
) else (
    set "PATH="
)
echo [INFO] PATH refreshed for current session
echo.
echo To apply in NEW windows: Close and reopen your Command Prompt/PowerShell
echo In THIS window: The PATH has been updated for this session
goto UNINSTALL_COMPLETE
) else (
    echo [ERROR] Failed to update user PATH
    echo [ERROR] Error code: %SETX_RESULT%
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

:UNINSTALL_COMPLETE
REM ============================================================================
REM UNINSTALLATION COMPLETE
REM ============================================================================

echo.
echo ====================================
echo    Uninstallation Complete!
echo ====================================
echo.
echo Press any key to exit...
pause >nul
exit /b 0

:strlen
REM Helper function to get string length
REM Usage: call :strlen VAR_NAME RESULT_VAR
setlocal enabledelayedexpansion
set "str=!%~1!"
set "len=0"
:strlen_loop
if defined str (
    set "str=!str:~1!"
    set /a len+=1
    goto :strlen_loop
)
endlocal & set "%~2=%len%"
exit /b
