using RBPortKiller.Core.Models;

namespace RBPortKiller.CLI.UI;

/// <summary>
/// Represents the result of a port selection from the menu.
/// </summary>
public sealed class PortSelectionResult
{
    /// <summary>
    /// The action selected by the user.
    /// </summary>
    public PortSelectionAction Action { get; init; }

    /// <summary>
    /// The selected port (only valid when Action is PortSelected).
    /// </summary>
    public PortInfo? SelectedPort { get; init; }

    /// <summary>
    /// Creates a result for when a port is selected.
    /// </summary>
    public static PortSelectionResult PortSelected(PortInfo port) => new()
    {
        Action = PortSelectionAction.PortSelected,
        SelectedPort = port
    };

    /// <summary>
    /// Creates a result for when refresh is selected.
    /// </summary>
    public static PortSelectionResult Refresh() => new()
    {
        Action = PortSelectionAction.Refresh,
        SelectedPort = null
    };

    /// <summary>
    /// Creates a result for when exit is selected.
    /// </summary>
    public static PortSelectionResult Exit() => new()
    {
        Action = PortSelectionAction.Exit,
        SelectedPort = null
    };
}
