using RBPortKiller.Core.Interfaces;
using RBPortKiller.Core.Models;

namespace RBPortKiller.Core.Services;

/// <summary>
/// Implementation of the port killer service that coordinates port discovery and process termination.
/// </summary>
public sealed class PortKillerService : IPortKillerService
{
    private readonly IPortDiscoveryService _portDiscoveryService;
    private readonly IProcessManagementService _processManagementService;

    public PortKillerService(
        IPortDiscoveryService portDiscoveryService,
        IProcessManagementService processManagementService)
    {
        _portDiscoveryService = portDiscoveryService ?? throw new ArgumentNullException(nameof(portDiscoveryService));
        _processManagementService = processManagementService ?? throw new ArgumentNullException(nameof(processManagementService));
    }

    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken = default)
    {
        var ports = await _portDiscoveryService.GetActivePortsAsync(cancellationToken);
        
        // Sort by port number for consistent display
        return ports.OrderBy(p => p.Port).ThenBy(p => p.Protocol).ToList();
    }

    public async Task<ProcessTerminationResult> TerminateProcessAsync(PortInfo portInfo, CancellationToken cancellationToken = default)
    {
        if (portInfo == null)
        {
            throw new ArgumentNullException(nameof(portInfo));
        }

        return await _processManagementService.TerminateProcessAsync(portInfo.ProcessId, cancellationToken);
    }

    public bool CanTerminateProcess(PortInfo portInfo)
    {
        if (portInfo == null)
        {
            throw new ArgumentNullException(nameof(portInfo));
        }

        return _processManagementService.CanTerminateProcess(portInfo.ProcessId);
    }
}
