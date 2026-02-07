<#
.SYNOPSIS
    RBPortKiller Installation Script (PowerShell)
    
.DESCRIPTION
    Adds RBPortKiller to the system or user PATH environment variable
    for easy command-line access from any directory.
    
.EXAMPLE
    .\install.ps1
    Run as current user (adds to user PATH)
    
.EXAMPLE
    Run as Administrator
    Right-click PowerShell and "Run as Administrator", then execute script
    (adds to system PATH - available for all users)
#>

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "====================================" -ForegroundColor Cyan
Write-Host "   RBPortKiller Installation" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Get the installation directory (current script location)
$InstallDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Write-Host "Installing from: $InstallDir" -ForegroundColor Yellow
Write-Host ""

# Check if running as administrator
$IsAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)

if ($IsAdmin) {
    Write-Host "Running with administrator privileges..." -ForegroundColor Green
    Write-Host "Adding to SYSTEM PATH (all users)..." -ForegroundColor Yellow
    
    # Get system PATH
    $PathVariable = [Environment]::GetEnvironmentVariable("Path", "Machine")
    
    # Check if already in PATH
    if ($PathVariable -like "*$InstallDir*") {
        Write-Host "RBPortKiller is already in the system PATH." -ForegroundColor Yellow
    } else {
        # Add to system PATH
        [Environment]::SetEnvironmentVariable("Path", "$PathVariable;$InstallDir", "Machine")
        Write-Host "Successfully added to system PATH!" -ForegroundColor Green
        Write-Host ""
        Write-Host "IMPORTANT: Close and reopen your command prompt/PowerShell for changes to take effect." -ForegroundColor Cyan
    }
} else {
    Write-Host "WARNING: Not running as administrator." -ForegroundColor Yellow
    Write-Host "Adding to USER PATH instead..." -ForegroundColor Yellow
    
    # Get user PATH
    $PathVariable = [Environment]::GetEnvironmentVariable("Path", "User")
    
    # Check if already in PATH
    if ($PathVariable -like "*$InstallDir*") {
        Write-Host "RBPortKiller is already in the user PATH." -ForegroundColor Yellow
    } else {
        # Add to user PATH
        [Environment]::SetEnvironmentVariable("Path", "$PathVariable;$InstallDir", "User")
        Write-Host "Successfully added to user PATH!" -ForegroundColor Green
        Write-Host ""
        Write-Host "IMPORTANT: Close and reopen your command prompt/PowerShell for changes to take effect." -ForegroundColor Cyan
    }
}

Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
Write-Host "Type 'rbportkiller' in any command prompt to run the tool." -ForegroundColor White
Write-Host ""

Read-Host "Press Enter to exit"
