using System.Runtime.InteropServices;

namespace RBPortKiller.Infrastructure.Platform;

/// <summary>
/// Provides platform detection capabilities.
/// Centralizes platform detection logic to eliminate code duplication.
/// </summary>
public static class PlatformDetector
{
    /// <summary>
    /// Gets the current operating system platform.
    /// </summary>
    public static OSPlatform GetCurrentPlatform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return OSPlatform.Windows;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return OSPlatform.Linux;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return OSPlatform.OSX;

        throw new PlatformNotSupportedException($"Unsupported platform: {RuntimeInformation.OSDescription}");
    }

    /// <summary>
    /// Gets a human-readable name for the current platform.
    /// </summary>
    public static string GetPlatformName()
    {
        return GetCurrentPlatform() switch
        {
            var p when p == OSPlatform.Windows => "Windows",
            var p when p == OSPlatform.Linux => "Linux",
            var p when p == OSPlatform.OSX => "macOS",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Checks if the current platform is Windows.
    /// </summary>
    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Checks if the current platform is Linux.
    /// </summary>
    public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    /// <summary>
    /// Checks if the current platform is macOS.
    /// </summary>
    public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
}
