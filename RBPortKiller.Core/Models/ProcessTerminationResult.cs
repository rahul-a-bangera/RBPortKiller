namespace RBPortKiller.Core.Models;

/// <summary>
/// Result of attempting to terminate a process.
/// </summary>
public sealed class ProcessTerminationResult
{
    /// <summary>
    /// Whether the termination was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The process ID that was targeted.
    /// </summary>
    public int ProcessId { get; init; }

    /// <summary>
    /// Error message if the termination failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Whether the failure was due to insufficient permissions.
    /// </summary>
    public bool IsPermissionDenied { get; init; }

    public static ProcessTerminationResult Succeeded(int processId)
    {
        return new ProcessTerminationResult
        {
            Success = true,
            ProcessId = processId
        };
    }

    public static ProcessTerminationResult Failed(int processId, string errorMessage, bool isPermissionDenied = false)
    {
        return new ProcessTerminationResult
        {
            Success = false,
            ProcessId = processId,
            ErrorMessage = errorMessage,
            IsPermissionDenied = isPermissionDenied
        };
    }
}
