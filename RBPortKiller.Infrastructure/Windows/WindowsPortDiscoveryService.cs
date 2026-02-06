using RBPortKiller.Core.Interfaces;
using RBPortKiller.Core.Models;
using RBPortKiller.Infrastructure.Windows.Commands;
using RBPortKiller.Infrastructure.Windows.Helpers;
using RBPortKiller.Infrastructure.Windows.Parsers;
using System.Net.NetworkInformation;

namespace RBPortKiller.Infrastructure.Windows;

/// <summary>
/// Windows-specific implementation of port discovery using .NET APIs and native calls.
/// </summary>
public sealed class WindowsPortDiscoveryService : IPortDiscoveryService
{
    private readonly NetstatCommandExecutor _netstatExecutor;
    private readonly NetstatOutputParser _netstatParser;
    private readonly ProcessInfoProvider _processInfoProvider;

    public WindowsPortDiscoveryService()
    {
        _netstatExecutor = new NetstatCommandExecutor();
        _netstatParser = new NetstatOutputParser();
        _processInfoProvider = new ProcessInfoProvider();
    }
    public async Task<IEnumerable<PortInfo>> GetActivePortsAsync(CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var portInfoList = new List<PortInfo>();

            // Get TCP connections
            var tcpConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
            foreach (var connection in tcpConnections)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var portInfo = CreatePortInfoFromTcpConnection(connection);
                if (portInfo != null)
                {
                    portInfoList.Add(portInfo);
                }
            }

            // Get TCP listeners
            var tcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (var listener in tcpListeners)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var portInfo = CreatePortInfoFromEndPoint(listener, Protocol.TCP, "LISTENING");
                if (portInfo != null)
                {
                    portInfoList.Add(portInfo);
                }
            }

            // Get UDP listeners
            var udpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
            foreach (var listener in udpListeners)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var portInfo = CreatePortInfoFromEndPoint(listener, Protocol.UDP, null);
                if (portInfo != null)
                {
                    portInfoList.Add(portInfo);
                }
            }

            // Sort by creation date (newest first), then by port number
            return portInfoList
                .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
                .ThenBy(p => p.Port)
                .AsEnumerable();
        }, cancellationToken);
    }

    private PortInfo? CreatePortInfoFromTcpConnection(TcpConnectionInformation connection)
    {
        try
        {
            var processId = GetProcessIdForPort(connection.LocalEndPoint.Port, Protocol.TCP);
            if (processId == 0)
            {
                return null;
            }

            var (processName, processPath, createdDate, isSystemProcess) = _processInfoProvider.GetProcessInfo(processId);
            
            // Filter out system processes
            if (isSystemProcess)
            {
                return null;
            }

            var protocol = ProtocolResolver.ResolveProtocol(Protocol.TCP, connection.LocalEndPoint.AddressFamily);

            return new PortInfo
            {
                Port = connection.LocalEndPoint.Port,
                Protocol = protocol,
                ProcessId = processId,
                ProcessName = processName,
                LocalAddress = connection.LocalEndPoint.Address.ToString(),
                RemoteAddress = connection.RemoteEndPoint.Address.ToString() + ":" + connection.RemoteEndPoint.Port,
                State = connection.State.ToString(),
                ProcessPath = processPath,
                CreatedDate = createdDate,
                IsSystemProcess = isSystemProcess
            };
        }
        catch
        {
            return null;
        }
    }

    private PortInfo? CreatePortInfoFromEndPoint(System.Net.IPEndPoint endPoint, Protocol baseProtocol, string? state)
    {
        try
        {
            var processId = GetProcessIdForPort(endPoint.Port, baseProtocol);
            if (processId == 0)
            {
                return null;
            }

            var (processName, processPath, createdDate, isSystemProcess) = _processInfoProvider.GetProcessInfo(processId);
            
            // Filter out system processes
            if (isSystemProcess)
            {
                return null;
            }

            var protocol = ProtocolResolver.ResolveProtocol(baseProtocol, endPoint.AddressFamily);

            return new PortInfo
            {
                Port = endPoint.Port,
                Protocol = protocol,
                ProcessId = processId,
                ProcessName = processName,
                LocalAddress = endPoint.Address.ToString(),
                State = state,
                ProcessPath = processPath,
                CreatedDate = createdDate,
                IsSystemProcess = isSystemProcess
            };
        }
        catch
        {
            return null;
        }
    }

    private int GetProcessIdForPort(int port, Protocol protocol)
    {
        try
        {
            var output = _netstatExecutor.Execute();
            return _netstatParser.ParseProcessIdForPort(output, port, protocol);
        }
        catch
        {
            return 0;
        }
    }
}
