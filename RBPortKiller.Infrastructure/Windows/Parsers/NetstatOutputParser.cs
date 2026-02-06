using RBPortKiller.Core.Models;

namespace RBPortKiller.Infrastructure.Windows.Parsers;

/// <summary>
/// Parses netstat command output to extract process IDs for specific ports.
/// Extracted for testability and single responsibility.
/// </summary>
internal sealed class NetstatOutputParser
{
    private const string TcpProtocolPrefix = "TCP";
    private const string UdpProtocolPrefix = "UDP";

    /// <summary>
    /// Parses netstat output to find the process ID for a specific port and protocol.
    /// </summary>
    /// <param name="netstatOutput">Raw output from netstat -ano command</param>
    /// <param name="port">The port number to find</param>
    /// <param name="protocol">The protocol (TCP or UDP)</param>
    /// <returns>Process ID if found, otherwise 0</returns>
    public int ParseProcessIdForPort(string netstatOutput, int port, Protocol protocol)
    {
        if (string.IsNullOrWhiteSpace(netstatOutput))
            return 0;

        var protocolPrefix = GetProtocolPrefix(protocol);
        var portPattern = $":{port} ";

        var lines = netstatOutput.Split('\n');

        foreach (var line in lines)
        {
            if (IsMatchingLine(line, portPattern, protocolPrefix))
            {
                var processId = ExtractProcessId(line);
                if (processId > 0)
                    return processId;
            }
        }

        return 0;
    }

    private static string GetProtocolPrefix(Protocol protocol)
    {
        return protocol switch
        {
            Protocol.UDP or Protocol.UDPv6 => UdpProtocolPrefix,
            Protocol.TCP or Protocol.TCPv6 => TcpProtocolPrefix,
            _ => TcpProtocolPrefix
        };
    }

    private static bool IsMatchingLine(string line, string portPattern, string protocolPrefix)
    {
        return line.Contains(portPattern, StringComparison.OrdinalIgnoreCase) &&
               line.Contains(protocolPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static int ExtractProcessId(string line)
    {
        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length >= 5 && int.TryParse(parts[^1], out var pid))
        {
            return pid;
        }

        return 0;
    }
}
