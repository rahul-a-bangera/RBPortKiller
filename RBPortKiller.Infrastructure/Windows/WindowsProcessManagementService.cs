using RBPortKiller.Core.Interfaces;
using RBPortKiller.Core.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RBPortKiller.Infrastructure.Windows;

/// <summary>
/// Windows-specific implementation of process management using native APIs.
/// </summary>
public sealed class WindowsProcessManagementService : IProcessManagementService
{
    // Windows API constants
    private const int PROCESS_TERMINATE = 0x0001;
    private const int ERROR_ACCESS_DENIED = 5;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CloseHandle(IntPtr hObject);

    public async Task<ProcessTerminationResult> TerminateProcessAsync(int processId, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                // First, try using the managed API
                using var process = Process.GetProcessById(processId);
                
                // Check if process exists
                if (process.HasExited)
                {
                    return ProcessTerminationResult.Failed(
                        processId,
                        "Process has already exited.",
                        false);
                }

                // Attempt graceful termination first
                try
                {
                    process.Kill();
                    process.WaitForExit(5000); // Wait up to 5 seconds

                    if (process.HasExited)
                    {
                        return ProcessTerminationResult.Succeeded(processId);
                    }
                }
                catch (Win32Exception ex) when (ex.NativeErrorCode == ERROR_ACCESS_DENIED)
                {
                    // Try using native API with explicit permissions
                    return TerminateProcessNative(processId);
                }
                catch (InvalidOperationException)
                {
                    // Process already exited
                    return ProcessTerminationResult.Succeeded(processId);
                }

                return ProcessTerminationResult.Failed(
                    processId,
                    "Process did not terminate within the timeout period.",
                    false);
            }
            catch (ArgumentException)
            {
                return ProcessTerminationResult.Failed(
                    processId,
                    "Process not found.",
                    false);
            }
            catch (Win32Exception ex)
            {
                var isAccessDenied = ex.NativeErrorCode == ERROR_ACCESS_DENIED;
                return ProcessTerminationResult.Failed(
                    processId,
                    isAccessDenied
                        ? "Access denied. Try running as administrator."
                        : $"Failed to terminate process: {ex.Message}",
                    isAccessDenied);
            }
            catch (Exception ex)
            {
                return ProcessTerminationResult.Failed(
                    processId,
                    $"Unexpected error: {ex.Message}",
                    false);
            }
        }, cancellationToken);
    }

    public bool CanTerminateProcess(int processId)
    {
        try
        {
            var processHandle = OpenProcess(PROCESS_TERMINATE, false, processId);
            
            if (processHandle == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                return error != ERROR_ACCESS_DENIED;
            }

            CloseHandle(processHandle);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private ProcessTerminationResult TerminateProcessNative(int processId)
    {
        var processHandle = OpenProcess(PROCESS_TERMINATE, false, processId);

        if (processHandle == IntPtr.Zero)
        {
            var error = Marshal.GetLastWin32Error();
            var isAccessDenied = error == ERROR_ACCESS_DENIED;

            return ProcessTerminationResult.Failed(
                processId,
                isAccessDenied
                    ? "Access denied. Try running as administrator."
                    : $"Failed to open process. Error code: {error}",
                isAccessDenied);
        }

        try
        {
            var result = TerminateProcess(processHandle, 1);
            
            if (!result)
            {
                var error = Marshal.GetLastWin32Error();
                var isAccessDenied = error == ERROR_ACCESS_DENIED;

                return ProcessTerminationResult.Failed(
                    processId,
                    isAccessDenied
                        ? "Access denied. Try running as administrator."
                        : $"Failed to terminate process. Error code: {error}",
                    isAccessDenied);
            }

            return ProcessTerminationResult.Succeeded(processId);
        }
        finally
        {
            CloseHandle(processHandle);
        }
    }
}
