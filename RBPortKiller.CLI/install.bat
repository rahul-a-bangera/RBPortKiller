@echo off
setlocal enabledelayedexpansion

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
set "INSTALL_DIR=%INSTALL_DIR:~0,-1%"

echo Installing from: %INSTALL_DIR%
echo.

REM Verify rbportkiller.exe exists
if not exist "%INSTALL_DIR%\rbportkiller.exe" (
    echo [ERROR] rbportkiller.exe not found in installation directory!
    echo.
    echo Please ensure you extracted the ZIP file correctly.
    echo.
    pause
    exit /b 1
)

REM Check for administrator privileges
net session >nul 2>&1
if %errorLevel% == 0 (
    set "IS_ADMIN=1"
    echo [INFO] Running with administrator privileges
    echo [INFO] Adding to SYSTEM PATH (all users)...
) else (
    set "IS_ADMIN=0"
    echo [WARN] Not running as administrator
    echo [INFO] Adding to USER PATH (current user only)...
    echo.
    echo To install system-wide: Right-click install.bat ^> Run as administrator
)
echo.

REM ============================================================================
REM ADD TO PATH
REM ============================================================================

if "%IS_ADMIN%"=="1" (
    REM ===== SYSTEM PATH (ADMIN) =====
    echo Checking system PATH...
    
    REM Read current system PATH
    for /f "tokens=2*" %%a in ('reg query "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Environment" /v Path 2^>nul') do set "SYSTEM_PATH=%%b"
    
    REM Check if already in PATH
    echo !SYSTEM_PATH! | findstr /i /c:"%INSTALL_DIR%" >nul
    if !errorLevel! == 0 (
        echo [INFO] RBPortKiller is already in system PATH
        echo [SUCCESS] Installation verified!
    ) else (
        REM Add to system PATH
        echo Adding to system PATH...
        setx PATH "%SYSTEM_PATH%;%INSTALL_DIR%" /M >nul 2>&1
        
        if !errorLevel! == 0 (
            echo [SUCCESS] Successfully added to system PATH!
            echo.
            echo IMPORTANT: Close and reopen your Command Prompt/PowerShell
            echo            for changes to take effect.
        ) else (
            echo [ERROR] Failed to update system PATH
            echo [ERROR] Error code: !errorLevel!
            echo.
            echo Try running as administrator or use manual setup.
            pause
            exit /b 1
        )
    )
) else (
    REM ===== USER PATH (NON-ADMIN) =====
    echo Checking user PATH...
    
    REM Read current user PATH
    for /f "tokens=2*" %%a in ('reg query "HKCU\Environment" /v Path 2^>nul') do set "USER_PATH=%%b"
    
    REM Check if already in PATH
    echo !USER_PATH! | findstr /i /c:"%INSTALL_DIR%" >nul
    if !errorLevel! == 0 (
        echo [INFO] RBPortKiller is already in user PATH
        echo [SUCCESS] Installation verified!
    ) else (
        REM Add to user PATH
        echo Adding to user PATH...
        setx PATH "%USER_PATH%;%INSTALL_DIR%" >nul 2>&1
        
        if !errorLevel! == 0 (
            echo [SUCCESS] Successfully added to user PATH!
            echo.
            echo IMPORTANT: Close and reopen your Command Prompt/PowerShell
            echo            for changes to take effect.
        ) else (
            echo [ERROR] Failed to update user PATH
            echo [ERROR] Error code: !errorLevel!
            echo.
            echo Try manual setup or contact support.
            pause
            exit /b 1
        )
    )
)

REM ============================================================================
REM INSTALLATION COMPLETE
REM ============================================================================

echo.
echo ====================================
echo    Installation Complete!
echo ====================================
echo.
echo Usage: Type 'rbportkiller' in any Command Prompt or PowerShell window
echo.
echo Next steps:
echo   1. Close this window
echo   2. Open a NEW Command Prompt or PowerShell
echo   3. Type: rbportkiller
echo.
echo Support: https://github.com/rahul-a-bangera/RBPortKiller
echo.
pause
exit /b 0
