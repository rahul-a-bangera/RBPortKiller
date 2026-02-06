# RBPortKiller - Uninstallation Script
# This script removes rbportkiller from your system

param(
    [Parameter(Mandatory=$false)]
    [string]$InstallPath = "$env:LOCALAPPDATA\RBPortKiller"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RBPortKiller Uninstall Script        " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Remove from PATH
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($currentPath -like "*$InstallPath*") {
    Write-Host "Removing from PATH..." -ForegroundColor Yellow
    $newPath = ($currentPath -split ';' | Where-Object { $_ -ne $InstallPath }) -join ';'
    [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
}

# Remove installation directory
if (Test-Path $InstallPath) {
    Write-Host "Removing installation directory..." -ForegroundColor Yellow
    Remove-Item -Path $InstallPath -Recurse -Force
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  Uninstallation Complete!             " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "rbportkiller has been removed from your system." -ForegroundColor Cyan
Write-Host ""
