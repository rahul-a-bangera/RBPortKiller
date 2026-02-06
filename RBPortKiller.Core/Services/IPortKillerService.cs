using RBPortKiller.Core.Models;

namespace RBPortKiller.Core.Services;

/// <summary>
/// Orchestrates port management operations.
/// </summary>
public interface IPortKillerService
{
    /// <summary>
    /// Gets all active ports on the system.
    /// </summary>
    Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates the process associated with a specific port.
    /// </summary>
    /// <param name="portInfo">The port information containing the process to terminate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the termination attempt.</returns>
    Task<ProcessTerminationResult> TerminateProcessAsync(PortInfo portInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user can terminate the process associated with the port.
    /// </summary>
    /// <param name="portInfo">The port information containing the process to check.</param>
    /// <returns>True if the user has permission, false otherwise.</returns>
    bool CanTerminateProcess(PortInfo portInfo);
}
