# RBPortKiller - Installation Script
# This script installs the rbportkiller command globally on your system

param(
    [Parameter(Mandatory=$false)]
    [string]$InstallPath = "$env:LOCALAPPDATA\RBPortKiller"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RBPortKiller Installation Script     " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if the executable exists
$publishDir = "publish\win-x64"
$exePath = Join-Path $publishDir "rbportkiller.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "Error: Executable not found at $exePath" -ForegroundColor Red
    Write-Host "Please run .\build.ps1 first to build the application." -ForegroundColor Yellow
    exit 1
}

# Create installation directory
Write-Host "Creating installation directory..." -ForegroundColor Green
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath -Force | Out-Null
}

# Copy executable
Write-Host "Installing rbportkiller to $InstallPath..." -ForegroundColor Green
Copy-Item -Path $exePath -Destination $InstallPath -Force

# Add to PATH if not already present
$currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
if ($currentPath -notlike "*$InstallPath*") {
    Write-Host "Adding to PATH..." -ForegroundColor Green
    $newPath = "$currentPath;$InstallPath"
    [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Installation Successful!             " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "rbportkiller has been installed to:" -ForegroundColor Cyan
    Write-Host "  $InstallPath" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "IMPORTANT: Please restart your terminal or run:" -ForegroundColor Yellow
    Write-Host "  `$env:Path = [System.Environment]::GetEnvironmentVariable('Path','User')" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Then you can use the tool by running:" -ForegroundColor Cyan
    Write-Host "  rbportkiller" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Installation Successful!             " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "rbportkiller has been updated at:" -ForegroundColor Cyan
    Write-Host "  $InstallPath" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "You can now use the tool by running:" -ForegroundColor Cyan
    Write-Host "  rbportkiller" -ForegroundColor Yellow
}

Write-Host ""
