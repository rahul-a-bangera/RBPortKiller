using System.Diagnostics;

namespace RBPortKiller.Infrastructure.Windows.Helpers;

/// <summary>
/// Retrieves process creation time information.
/// Extracted to separate creation time retrieval from other process information gathering.
/// </summary>
internal sealed class ProcessCreationTimeProvider
{
    /// <summary>
    /// Gets the creation time for the specified process ID.
    /// </summary>
    /// <param name="processId">The process ID</param>
    /// <returns>The process creation time, or null if unavailable</returns>
    public DateTime? GetProcessCreationTime(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return process.StartTime;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            // Access denied or not found
            return null;
        }
        catch (InvalidOperationException)
        {
            // Process has exited
            return null;
        }
        catch
        {
            // Other errors
            return null;
        }
    }
}
