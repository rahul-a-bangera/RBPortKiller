namespace RBPortKiller.Core.Models;

/// <summary>
/// Represents information about an active network port on the system.
/// </summary>
public sealed class PortInfo
{
    /// <summary>
    /// The port number (0-65535).
    /// </summary>
    public int Port { get; init; }

    /// <summary>
    /// The protocol used (TCP, UDP, etc.).
    /// </summary>
    public Protocol Protocol { get; init; }

    /// <summary>
    /// The process ID that owns this port.
    /// </summary>
    public int ProcessId { get; init; }

    /// <summary>
    /// The name of the process that owns this port.
    /// </summary>
    public string ProcessName { get; init; } = string.Empty;

    /// <summary>
    /// The local address bound to this port.
    /// </summary>
    public string LocalAddress { get; init; } = string.Empty;

    /// <summary>
    /// The remote address (if applicable).
    /// </summary>
    public string? RemoteAddress { get; init; }

    /// <summary>
    /// The connection state (for TCP connections).
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// Full path to the process executable (if available).
    /// </summary>
    public string? ProcessPath { get; init; }

    /// <summary>
    /// The date and time when the connection was created (if available).
    /// </summary>
    public DateTime? CreatedDate { get; init; }

    /// <summary>
    /// Indicates whether this process is a system/critical process that should not be terminated.
    /// </summary>
    public bool IsSystemProcess { get; init; }

    public override string ToString()
    {
        return $"{Protocol}:{Port} - {ProcessName} (PID: {ProcessId})";
    }
}
