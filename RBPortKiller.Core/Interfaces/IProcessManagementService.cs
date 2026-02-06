using RBPortKiller.Core.Models;

namespace RBPortKiller.Core.Interfaces;

/// <summary>
/// Platform-specific service for process management operations.
/// </summary>
public interface IProcessManagementService
{
    /// <summary>
    /// Attempts to terminate a process by its process ID.
    /// </summary>
    /// <param name="processId">The process ID to terminate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the termination attempt.</returns>
    Task<ProcessTerminationResult> TerminateProcessAsync(int processId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user has permission to terminate the specified process.
    /// </summary>
    /// <param name="processId">The process ID to check.</param>
    /// <returns>True if the user has permission, false otherwise.</returns>
    bool CanTerminateProcess(int processId);
}
