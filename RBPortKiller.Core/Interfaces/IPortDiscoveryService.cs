using RBPortKiller.Core.Models;

namespace RBPortKiller.Core.Interfaces;

/// <summary>
/// Platform-specific service for discovering active network ports.
/// </summary>
public interface IPortDiscoveryService
{
    /// <summary>
    /// Retrieves all active network ports on the system.
    /// </summary>
    /// <returns>A collection of active port information.</returns>
    Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken = default);
}
