<#
.SYNOPSIS
    Prepare and commit files for the first release.
    
.DESCRIPTION
    This script stages all release pipeline files and creates a commit
    that will trigger the v1.0.0 release when pushed.
    
.PARAMETER Push
    Automatically push to origin/master after committing
    
.EXAMPLE
    .\prepare-first-release.ps1
    
.EXAMPLE
    .\prepare-first-release.ps1 -Push
#>

param(
    [Parameter(Mandatory=$false)]
    [switch]$Push
)

$ErrorActionPreference = "Stop"

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  Prepare First Release (v1.0.0)" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Navigate to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent (Split-Path -Parent $scriptPath)
Set-Location $projectRoot

# Check if we're in a git repository
$isGitRepo = Test-Path ".git"
if (-not $isGitRepo) {
    Write-Error "Not a git repository. Please run this from the project root."
    exit 1
}

# Check current branch
$currentBranch = git rev-parse --abbrev-ref HEAD
Write-Host "Current branch: $currentBranch" -ForegroundColor White

if ($currentBranch -ne "master") {
    Write-Warning "You are not on 'master' branch. The workflow only triggers on master."
    $continue = Read-Host "Continue anyway? (y/n)"
    if ($continue -ne 'y') {
        Write-Host "Cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Files to be committed
$filesToCommit = @(
    ".github\workflows\release.yml",
    "Misc\Docs\RELEASE_PROCESS.md",
    "Misc\Docs\RELEASE_PIPELINE_SETUP.md",
    "Misc\Scripts\test-release.ps1",
    "Misc\Scripts\prepare-first-release.ps1",
    "RBPortKiller.CLI\RBPortKiller.CLI.csproj"
)

Write-Host "Files to be committed:`n" -ForegroundColor Cyan
foreach ($file in $filesToCommit) {
    if (Test-Path $file) {
        Write-Host "  [OK]  $file" -ForegroundColor Green
    } else {
        Write-Host "  [MISSING]  $file" -ForegroundColor Red
    }
}
Write-Host ""

# Check git status
Write-Host "Current git status:" -ForegroundColor Cyan
git status --short

Write-Host "`nThis will:" -ForegroundColor Yellow
Write-Host "  1. Stage all release pipeline files" -ForegroundColor White
Write-Host "  2. Commit with message: 'Add automated release pipeline and prepare v1.0.0 release'" -ForegroundColor White
if ($Push) {
    Write-Host "  3. Push to origin/master (WILL TRIGGER RELEASE BUILD)" -ForegroundColor Red
} else {
    Write-Host "  3. NOT push automatically (you can push manually)" -ForegroundColor White
}
Write-Host ""

$confirm = Read-Host "Proceed? (y/n)"
if ($confirm -ne 'y') {
    Write-Host "Cancelled." -ForegroundColor Yellow
    exit 0
}

try {
    Write-Host "`nStaging files..." -ForegroundColor Cyan
    foreach ($file in $filesToCommit) {
        if (Test-Path $file) {
            git add $file
        }
    }
    
    Write-Host "Creating commit..." -ForegroundColor Cyan
    git commit -m "Add automated release pipeline and prepare v1.0.0 release" -m "- Add GitHub Actions workflow for version-based releases
- Update project file with complete metadata
- Add release documentation and helper scripts
- Release will be created automatically when pushed to master"
    
    if ($Push) {
        Write-Host "Pushing to origin/master..." -ForegroundColor Cyan
        git push origin $currentBranch
        
        Write-Host "`n========================================" -ForegroundColor Green
        Write-Host "  Release Build Triggered!" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Green
        
        Write-Host "Monitor build progress at:" -ForegroundColor Cyan
        Write-Host "https://github.com/rahul-a-bangera/RBPortKiller/actions" -ForegroundColor Blue
        Write-Host "`nRelease will be available at:" -ForegroundColor Cyan
        Write-Host "https://github.com/rahul-a-bangera/RBPortKiller/releases/tag/v1.0.0" -ForegroundColor Blue
        Write-Host ""
        
    } else {
        Write-Host "`n========================================" -ForegroundColor Green
        Write-Host "  Commit Created Successfully!" -ForegroundColor Green
        Write-Host "========================================`n" -ForegroundColor Green
        
        Write-Host "Next step: Push to trigger release" -ForegroundColor Yellow
        Write-Host "  git push origin $currentBranch" -ForegroundColor White
        Write-Host ""
    }
}
catch {
    Write-Host "`nError:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
