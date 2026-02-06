# Changelog

All notable changes to RBPortKiller will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-06

### Added
- Initial release of RBPortKiller
- Interactive CLI for managing network ports
- Real-time port discovery using OS-level APIs
- Process identification and termination
- Beautiful terminal UI with Spectre.Console
- Support for TCP, UDP, TCPv6, and UDPv6 protocols
- Color-coded connection states
- Confirmation prompts for safety
- Permission checking before process termination
- Comprehensive error handling
- Windows platform support (fully functional)
- Clean architecture with platform abstraction
- Single-file self-contained executable
- Build script for automated publishing
- Installation script for global availability
- Uninstallation script
- Comprehensive documentation (README, QUICKSTART, CONTRIBUTING)
- MIT License

### Features
- Display all active network ports in a table
- Show port number, protocol, PID, process name, local address, and state
- Interactive selection with keyboard navigation
- Detailed port information view
- Safe process termination with Win32 API fallback
- Administrator privilege detection
- User-friendly error messages
- Loading indicators and status feedback

### Technical
- .NET 8.0 target framework
- Dependency injection with Microsoft.Extensions.DependencyInjection
- Platform detection and factory pattern
- Async/await throughout
- XML documentation for public APIs
- Partial trimming for optimized executable size (~11 MB)

### Documentation
- README.md - Comprehensive documentation
- QUICKSTART.md - Quick start guide
- CONTRIBUTING.md - Contribution guidelines
- PROJECT_SUMMARY.md - Technical overview
- LICENSE - MIT License

## [Unreleased]

### Planned
- Linux support
- macOS support
- Port filtering functionality
- Export to CSV/JSON
- Watch mode with auto-refresh
- Configuration file support
- Unit tests
- Integration tests
- CI/CD pipeline

---

## Version History

- **1.0.0** (2026-02-06) - Initial release with Windows support

## Migration Guide

### From Nothing to 1.0.0

This is the initial release. To install:

1. Run `.\build.ps1` to build the application
2. Run `.\install.ps1` to install globally
3. Use `rbportkiller` command in any terminal

## Breaking Changes

None (initial release)

## Deprecations

None (initial release)

## Security

### 1.0.0
- Implements permission checking before process termination
- Requires user confirmation for destructive operations
- Handles access denied errors gracefully
- Recommends administrator privileges when needed

## Known Issues

### 1.0.0
- Windows only (Linux/macOS support planned)
- No filtering or search functionality yet
- Some system processes may not show full path due to permissions

## Support

For issues, questions, or contributions, please see:
- GitHub Issues for bug reports
- CONTRIBUTING.md for contribution guidelines
- README.md for usage documentation
