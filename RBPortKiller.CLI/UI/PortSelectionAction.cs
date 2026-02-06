namespace RBPortKiller.CLI.UI;

/// <summary>
/// Represents the result of a port selection action.
/// </summary>
public enum PortSelectionAction
{
    /// <summary>
    /// User selected a port to manage.
    /// </summary>
    PortSelected,

    /// <summary>
    /// User chose to refresh the port list.
    /// </summary>
    Refresh,

    /// <summary>
    /// User chose to exit the application.
    /// </summary>
    Exit
}
