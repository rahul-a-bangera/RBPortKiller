namespace RBPortKiller.CLI.UI;

/// <summary>
/// Provides color mapping for connection states.
/// Extracted to separate presentation concerns from business logic.
/// </summary>
public static class StateColorMapper
{
    /// <summary>
    /// Gets the appropriate color for a connection state.
    /// </summary>
    /// <param name="state">The connection state</param>
    /// <returns>Spectre.Console color name</returns>
    public static string GetColorForState(string? state)
    {
        return state?.ToUpperInvariant() switch
        {
            "ESTABLISHED" => "green",
            "LISTENING" => "cyan",
            "TIME_WAIT" => "yellow",
            "CLOSE_WAIT" => "orange1",
            "CLOSED" => "red",
            _ => "dim"
        };
    }
}
