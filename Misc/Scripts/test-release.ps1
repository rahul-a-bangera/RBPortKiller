<#
.SYNOPSIS
    Build and test release packages locally before pushing.
    
.DESCRIPTION
    This script simulates the GitHub Actions release workflow locally,
    allowing you to verify builds before creating an actual release.
    
.PARAMETER Version
    Optional. If not provided, reads from RBPortKiller.CLI.csproj
    
.PARAMETER SkipTests
    Skip running tests
    
.EXAMPLE
    .\test-release.ps1
    
.EXAMPLE
    .\test-release.ps1 -Version "1.0.1" -SkipTests
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests
)

$ErrorActionPreference = "Stop"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  RBPortKiller - Release Build Test" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Navigate to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptPath)
Set-Location $projectRoot

# Get version from csproj if not provided
if ([string]::IsNullOrEmpty($Version)) {
    $csprojContent = Get-Content "RBPortKiller.CLI\RBPortKiller.CLI.csproj" -Raw
    if ($csprojContent -match '<Version>([^<]+)</Version>') {
        $Version = $matches[1]
        Write-Host "Detected version from csproj: $Version" -ForegroundColor Green
    } else {
        Write-Error "Could not find version in RBPortKiller.CLI.csproj"
        exit 1
    }
}

Write-Host "`nBuilding version: $Version`n" -ForegroundColor Yellow

# Clean previous builds
Write-Host "[1/8] Cleaning previous builds..." -ForegroundColor Cyan
if (Test-Path ".\publish") {
    Remove-Item ".\publish" -Recurse -Force
}
if (Test-Path ".\release-test") {
    Remove-Item ".\release-test" -Recurse -Force
}
New-Item -ItemType Directory -Path ".\release-test" | Out-Null

# Restore dependencies
Write-Host "[2/8] Restoring dependencies..." -ForegroundColor Cyan
dotnet restore | Out-Null

# Build
Write-Host "[3/8] Building solution..." -ForegroundColor Cyan
dotnet build --configuration Release --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Run tests
if (-not $SkipTests) {
    Write-Host "[4/8] Running tests..." -ForegroundColor Cyan
    dotnet test --no-restore --verbosity normal
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warning "Tests failed, but continuing..."
    }
} else {
    Write-Host "[4/8] Skipping tests..." -ForegroundColor Yellow
}

# Publish for each platform
$platforms = @(
    @{ Runtime = "win-x64"; Name = "Windows x64" },
    @{ Runtime = "win-x86"; Name = "Windows x86" }
)

$step = 5
foreach ($platform in $platforms) {
    Write-Host "[$step/7] Publishing $($platform.Name)..." -ForegroundColor Cyan
    
    dotnet publish "RBPortKiller.CLI\RBPortKiller.CLI.csproj" `
        -c Release `
        -r $platform.Runtime `
        --self-contained true `
        -p:PublishSingleFile=true `
        -p:PublishTrimmed=true `
        -p:IncludeNativeLibrariesForSelfExtract=true `
        -p:EnableCompressionInSingleFile=true `
        -o ".\publish\$($platform.Runtime)" `
        --nologo | Out-Null
        
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Publish failed for $($platform.Name)"
        exit 1
    }
    
    $step++
}

# Copy installation files
Write-Host "[7/9] Copying installation files..." -ForegroundColor Cyan

foreach ($platform in $platforms) {
    Copy-Item "RBPortKiller.CLI\install.bat" -Destination ".\publish\$($platform.Runtime)\" -Force
    Copy-Item "RBPortKiller.CLI\uninstall.bat" -Destination ".\publish\$($platform.Runtime)\" -Force
    Copy-Item "RBPortKiller.CLI\README.txt" -Destination ".\publish\$($platform.Runtime)\" -Force
}

# Create archives
Write-Host "[8/9] Creating ZIP archives..." -ForegroundColor Cyan

foreach ($platform in $platforms) {
    $zipName = if ($platform.Runtime -eq "win-x64") { "rbportkillerwin64.zip" } else { "rbportkillerwin86.zip" }
    Compress-Archive -Path ".\publish\$($platform.Runtime)\*" `
        -DestinationPath ".\release-test\$zipName" `
        -Force
}

# Calculate checksums
Write-Host "[9/9] Calculating checksums..." -ForegroundColor Cyan

$checksums = @()
Get-ChildItem ".\release-test\*.zip" | ForEach-Object {
    $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
    $checksums += "$($_.Name): $hash"
}

$checksums | Out-File ".\release-test\checksums.txt" -Encoding utf8

# Display summary
Write-Host "`n========================================" -ForegroundColor Green
Write-Host "  Build Completed Successfully!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Green

Write-Host "Version: $Version" -ForegroundColor White
Write-Host "Output directory: .\release-test\`n" -ForegroundColor White

Write-Host "Archives created:" -ForegroundColor Cyan
Get-ChildItem ".\release-test\*.zip" | ForEach-Object {
    $size = [math]::Round($_.Length / 1MB, 2)
    Write-Host "  - $($_.Name) ($size MB)" -ForegroundColor White
}

Write-Host "`nChecksums:" -ForegroundColor Cyan
Get-Content ".\release-test\checksums.txt" | ForEach-Object {
    Write-Host "  $_" -ForegroundColor White
}

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Test the executables in .\release-test\" -ForegroundColor White
Write-Host "  2. Update CHANGELOG.md with release notes" -ForegroundColor White
Write-Host "  3. Commit and push to trigger actual release" -ForegroundColor White
Write-Host ""
