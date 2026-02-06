using System.Diagnostics;

namespace RBPortKiller.Infrastructure.Windows.Helpers;

/// <summary>
/// Retrieves process information by process ID.
/// Extracted to separate process information gathering from port discovery.
/// </summary>
internal sealed class ProcessInfoProvider
{
    private readonly SystemProcessDetector _systemProcessDetector;
    private readonly ProcessCreationTimeProvider _creationTimeProvider;

    public ProcessInfoProvider()
    {
        _systemProcessDetector = new SystemProcessDetector();
        _creationTimeProvider = new ProcessCreationTimeProvider();
    }

    /// <summary>
    /// Gets comprehensive process information for the specified process ID.
    /// </summary>
    /// <param name="processId">The process ID</param>
    /// <returns>A tuple containing process name, path, creation time, and system process status</returns>
    public (string processName, string? processPath, DateTime? createdDate, bool isSystemProcess) GetProcessInfo(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            var name = process.ProcessName;
            string? path = null;
            DateTime? createdDate = null;

            try
            {
                path = process.MainModule?.FileName;
                createdDate = process.StartTime;
            }
            catch
            {
                // Access denied or process exited - path and creation time remain null
                // We can still try to get creation time separately
                createdDate = _creationTimeProvider.GetProcessCreationTime(processId);
            }

            var isSystemProcess = _systemProcessDetector.IsSystemProcess(processId, name, path);

            return (name, path, createdDate, isSystemProcess);
        }
        catch
        {
            return ("Unknown", null, null, true); // Treat unknown processes as system processes for safety
        }
    }
}

