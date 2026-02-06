using RBPortKiller.Core.Models;
using System.Net.Sockets;

namespace RBPortKiller.Infrastructure.Windows.Helpers;

/// <summary>
/// Resolves the appropriate protocol version (IPv4/IPv6) based on address family.
/// Eliminates duplicate protocol resolution logic.
/// </summary>
internal static class ProtocolResolver
{
    /// <summary>
    /// Resolves the protocol including IP version based on address family.
    /// </summary>
    /// <param name="baseProtocol">The base protocol (TCP or UDP)</param>
    /// <param name="addressFamily">The address family (InterNetwork or InterNetworkV6)</param>
    /// <returns>The resolved protocol with version (TCP, TCPv6, UDP, or UDPv6)</returns>
    public static Protocol ResolveProtocol(Protocol baseProtocol, AddressFamily addressFamily)
    {
        if (addressFamily == AddressFamily.InterNetworkV6)
        {
            return baseProtocol == Protocol.TCP ? Protocol.TCPv6 : Protocol.UDPv6;
        }

        return baseProtocol;
    }
}
