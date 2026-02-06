using System.Diagnostics;

namespace RBPortKiller.Infrastructure.Windows.Helpers;

/// <summary>
/// Detects whether a process is a system/critical process that should not be terminated.
/// Helps prevent accidental termination of Windows system processes.
/// </summary>
internal sealed class SystemProcessDetector
{
    // Critical Windows system processes that should never be terminated
    private static readonly HashSet<string> CriticalProcessNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "System",
        "Registry",
        "smss",
        "csrss",
        "wininit",
        "services",
        "lsass",
        "winlogon",
        "svchost",
        "dwm",
        "explorer",
        "taskhostw",
        "RuntimeBroker",
        "ApplicationFrameHost",
        "ShellExperienceHost",
        "SearchUI",
        "SearchApp",
        "StartMenuExperienceHost",
        "SystemSettings",
        "dllhost",
        "conhost",
        "fontdrvhost",
        "WUDFHost",
        "Memory Compression",
        "Secure System",
        "ntoskrnl",
        "audiodg"
    };

    // System paths that indicate Windows system processes
    private static readonly string[] SystemPaths =
    {
        @"C:\Windows\System32",
        @"C:\Windows\SysWOW64",
        @"C:\Windows\explorer.exe",
        @"C:\Windows\SystemApps"
    };

    /// <summary>
    /// Determines whether a process is a system process based on its name, path, and characteristics.
    /// </summary>
    /// <param name="processId">The process ID</param>
    /// <param name="processName">The process name</param>
    /// <param name="processPath">The full path to the process executable (if available)</param>
    /// <returns>True if the process is a system process; otherwise, false</returns>
    public bool IsSystemProcess(int processId, string processName, string? processPath)
    {
        // System process (PID 0 or 4)
        if (processId <= 4)
        {
            return true;
        }

        // Check if it's a known critical process name
        if (CriticalProcessNames.Contains(processName))
        {
            return true;
        }

        // If we have the path, check if it's in a system directory
        if (!string.IsNullOrEmpty(processPath))
        {
            if (IsInSystemPath(processPath))
            {
                return true;
            }
        }

        // Additional check: Try to get more process details
        try
        {
            using var process = Process.GetProcessById(processId);
            
            // Check if the process has elevated privileges or is running as SYSTEM
            // Processes we can't access detailed info about are likely system processes
            try
            {
                // If we can't get the main module, it's likely a protected system process
                if (process.MainModule == null)
                {
                    return true;
                }

                var fileName = process.MainModule.FileName;
                if (IsInSystemPath(fileName))
                {
                    return true;
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                // Access denied - likely a system process
                return true;
            }
            catch (InvalidOperationException)
            {
                // Process has exited or is not accessible
                return true;
            }
        }
        catch
        {
            // If we can't access the process, treat it as a system process to be safe
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a path is within a Windows system directory.
    /// </summary>
    private static bool IsInSystemPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        foreach (var systemPath in SystemPaths)
        {
            if (path.StartsWith(systemPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
