@echo off
REM Force window to stay open on any error
if not "%1"=="noerror" (
    cmd /c "%~f0" noerror %*
    exit /b
)

REM ============================================================================
REM RBPortKiller Installation Script
REM Adds rbportkiller.exe to Windows PATH for easy command-line access
REM ============================================================================

echo.
echo ====================================
echo    RBPortKiller Installation
echo ====================================
echo.

REM Get installation directory (where this batch file is located)
set "INSTALL_DIR=%~dp0"
REM Remove trailing backslash
if "%INSTALL_DIR:~-1%"=="\" set "INSTALL_DIR=%INSTALL_DIR:~0,-1%"

echo Installing from: %INSTALL_DIR%
echo.

REM Verify rbportkiller.exe exists
if not exist "%INSTALL_DIR%\rbportkiller.exe" (
    echo [ERROR] rbportkiller.exe not found in installation directory!
    echo.
    echo Current directory: %INSTALL_DIR%
    echo.
    echo Files in directory:
    dir "%INSTALL_DIR%" /b
    echo.
    echo Please ensure you extracted the ZIP file correctly.
    echo This installer must be run from the same folder as rbportkiller.exe
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

echo [INFO] Found rbportkiller.exe
echo.

REM Check for administrator privileges
net session >nul 2>&1
set ADMIN_CHECK=%errorLevel%

if "%ADMIN_CHECK%"=="0" (
    set "IS_ADMIN=1"
    echo [INFO] Running with administrator privileges
    echo [INFO] Adding to SYSTEM PATH ^(all users^)...
) else (
    set "IS_ADMIN=0"
    echo [WARN] Not running as administrator
    echo [INFO] Adding to USER PATH ^(current user only^)...
    echo.
    echo To install system-wide: Right-click install.bat and select "Run as administrator"
)
echo.

REM ============================================================================
REM ADD TO PATH
REM ============================================================================

if "%IS_ADMIN%"=="1" goto ADMIN_INSTALL
goto USER_INSTALL

:ADMIN_INSTALL
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

REM Check if already in PATH
echo "%CURRENT_PATH%" | findstr /i /c:"%INSTALL_DIR%" >nul 2>&1
set CHECK_RESULT=%errorLevel%
if "%CHECK_RESULT%"=="0" (
    echo [INFO] RBPortKiller is already in system PATH
    echo [SUCCESS] Installation verified!
    goto INSTALL_COMPLETE
)

REM Add to system PATH
echo Adding to system PATH...
setx PATH "%CURRENT_PATH%;%INSTALL_DIR%" /M >nul 2>&1
set SETX_RESULT=%errorLevel%

if "%SETX_RESULT%"=="0" (
    echo [SUCCESS] Successfully added to system PATH!
    echo.
    echo IMPORTANT: Close and reopen your Command Prompt/PowerShell
    echo            for changes to take effect.
    goto INSTALL_COMPLETE
) else (
    echo [ERROR] Failed to update system PATH
    echo [ERROR] Error code: %SETX_RESULT%
    echo.
    echo Possible causes:
    echo  - PATH variable is too long (max 2047 characters)
    echo  - Insufficient permissions
    echo.
    echo Try manual setup or contact support.
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

:USER_INSTALL
REM ===== USER PATH (NON-ADMIN) =====
echo Checking user PATH...
echo.

REM Read current user PATH from registry
for /f "skip=2 tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path 2^>nul') do set "CURRENT_PATH=%%b"

REM If user PATH doesn't exist, create it empty
if not defined CURRENT_PATH set "CURRENT_PATH="

REM Check if already in PATH
echo "%CURRENT_PATH%" | findstr /i /c:"%INSTALL_DIR%" >nul 2>&1
set CHECK_RESULT=%errorLevel%
if "%CHECK_RESULT%"=="0" (
    echo [INFO] RBPortKiller is already in user PATH
    echo [SUCCESS] Installation verified!
    goto INSTALL_COMPLETE
)

REM Add to user PATH
echo Adding to user PATH...
if defined CURRENT_PATH (
    setx PATH "%CURRENT_PATH%;%INSTALL_DIR%" >nul 2>&1
) else (
    setx PATH "%INSTALL_DIR%" >nul 2>&1
)
set SETX_RESULT=%errorLevel%

if "%SETX_RESULT%"=="0" (
    echo [SUCCESS] Successfully added to user PATH!
    echo.
    echo IMPORTANT: Close and reopen your Command Prompt/PowerShell
    echo            for changes to take effect.
    goto INSTALL_COMPLETE
) else (
    echo [ERROR] Failed to update user PATH
    echo [ERROR] Error code: %SETX_RESULT%
    echo.
    echo Possible causes:
    echo  - PATH variable is too long (max 2047 characters)
    echo  - Registry access issues
    echo.
    echo Try manual setup or contact support.
    echo.
    echo Press any key to exit...
    pause >nul
    exit /b 1
)

:INSTALL_COMPLETE
REM ============================================================================
REM INSTALLATION COMPLETE
REM ============================================================================

echo.
echo ====================================
echo    Installation Complete!
echo ====================================
echo.
echo Press any key to exit...
pause >nul
exit /b 0
