# Release Process

## How It Works

Releases are triggered automatically when the version in `RBPortKiller.CLI/RBPortKiller.CLI.csproj` is changed and pushed to `master`.

## Create a Release

1. Update version in `RBPortKiller.CLI/RBPortKiller.CLI.csproj`:
   ```xml
   <Version>1.0.1</Version>
   <AssemblyVersion>1.0.1</AssemblyVersion>
   <FileVersion>1.0.1</FileVersion>
   ```

2. Commit and push:
   ```bash
   git add RBPortKiller.CLI/RBPortKiller.CLI.csproj
   git commit -m "Release v1.0.1"
   git push origin master
   ```

3. Monitor: https://github.com/rahul-a-bangera/RBPortKiller/actions

## Test Locally (Optional)

```powershell
.\Misc\Scripts\test-release.ps1
```

## What Gets Built

- `rbportkillerwin64.zip` (Windows 64-bit)
- `rbportkillerwin86.zip` (Windows 32-bit)
- `checksums.txt` (SHA256)

## Troubleshooting

**Release not created?**
- Check if tag exists: `git tag -l`
- Review GitHub Actions logs

**Wrong version released?**
- Delete tag: `git push origin --delete vX.Y.Z`
- Delete release from GitHub UI
- Fix version and recommit
