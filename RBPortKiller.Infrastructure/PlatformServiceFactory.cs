using RBPortKiller.Core.Interfaces;
using RBPortKiller.Infrastructure.Platform;
using RBPortKiller.Infrastructure.Windows;
using System.Runtime.InteropServices;

namespace RBPortKiller.Infrastructure;

/// <summary>
/// Factory for creating platform-specific service implementations.
/// </summary>
public static class PlatformServiceFactory
{
    /// <summary>
    /// Creates a platform-specific port discovery service.
    /// </summary>
    public static IPortDiscoveryService CreatePortDiscoveryService()
    {
        var platform = PlatformDetector.GetCurrentPlatform();

        if (platform == OSPlatform.Windows)
        {
            return new WindowsPortDiscoveryService();
        }

        if (platform == OSPlatform.Linux)
        {
            throw new PlatformNotSupportedException("Linux support is not yet implemented. Contributions welcome!");
        }

        if (platform == OSPlatform.OSX)
        {
            throw new PlatformNotSupportedException("macOS support is not yet implemented. Contributions welcome!");
        }

        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }

    /// <summary>
    /// Creates a platform-specific process management service.
    /// </summary>
    public static IProcessManagementService CreateProcessManagementService()
    {
        var platform = PlatformDetector.GetCurrentPlatform();

        if (platform == OSPlatform.Windows)
        {
            return new WindowsProcessManagementService();
        }

        if (platform == OSPlatform.Linux)
        {
            throw new PlatformNotSupportedException("Linux support is not yet implemented. Contributions welcome!");
        }

        if (platform == OSPlatform.OSX)
        {
            throw new PlatformNotSupportedException("macOS support is not yet implemented. Contributions welcome!");
        }

        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }

    /// <summary>
    /// Gets the current platform name.
    /// </summary>
    public static string GetPlatformName() => PlatformDetector.GetPlatformName();
}
