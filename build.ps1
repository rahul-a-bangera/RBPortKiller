# RBPortKiller - Build and Publish Script
# This script builds and publishes the CLI tool as a self-contained single-file executable

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('win-x64', 'linux-x64', 'osx-x64', 'osx-arm64')]
    [string]$Runtime = 'win-x64',
    
    [Parameter(Mandatory=$false)]
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RBPortKiller Build & Publish Script  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "RBPortKiller.CLI\RBPortKiller.CLI.csproj"
$outputDir = "publish\$Runtime"

Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Runtime: $Runtime" -ForegroundColor Yellow
Write-Host "Output Directory: $outputDir" -ForegroundColor Yellow
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Green
if (Test-Path $outputDir) {
    Remove-Item -Path $outputDir -Recurse -Force
}

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Green
dotnet restore

# Build the project
Write-Host "Building project..." -ForegroundColor Green
dotnet build $projectPath -c $Configuration

# Publish as self-contained single-file executable
Write-Host "Publishing self-contained executable..." -ForegroundColor Green
dotnet publish $projectPath `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishTrimmed=true `
    -p:TrimMode=partial `
    -p:EnableCompressionInSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -o $outputDir

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Build Successful!                    " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Executable location:" -ForegroundColor Cyan
    
    $exeName = if ($Runtime.StartsWith('win')) { "rbportkiller.exe" } else { "rbportkiller" }
    $exePath = Join-Path $outputDir $exeName
    
    Write-Host "  $exePath" -ForegroundColor Yellow
    Write-Host ""
    
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Host "To install globally, run:" -ForegroundColor Cyan
    Write-Host "  .\install.ps1" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
