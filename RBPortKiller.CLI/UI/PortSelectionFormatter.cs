using RBPortKiller.Core.Models;

namespace RBPortKiller.CLI.UI;

/// <summary>
/// Formats port information for selection menus.
/// Extracted to centralize formatting logic and improve maintainability.
/// </summary>
public static class PortSelectionFormatter
{
    /// <summary>
    /// Formats a port as a selection choice string.
    /// </summary>
    /// <param name="index">The index of the port (1-based)</param>
    /// <param name="port">The port information</param>
    /// <returns>Formatted selection string</returns>
    public static string FormatPortChoice(int index, PortInfo port)
    {
        return $"{index}. {port.Protocol}:{port.Port} - {port.ProcessName} (PID: {port.ProcessId})";
    }

    /// <summary>
    /// Creates a list of formatted port choices with refresh and exit options.
    /// </summary>
    /// <param name="ports">List of ports to format</param>
    /// <returns>List of formatted choices</returns>
    public static List<string> CreatePortChoices(List<PortInfo> ports)
    {
        var choices = new List<string>(ports.Count + 2);
        
        for (int i = 0; i < ports.Count; i++)
        {
            choices.Add(FormatPortChoice(i + 1, ports[i]));
        }
        
        choices.Add("Refresh Port List");
        choices.Add("Exit");
        
        return choices;
    }

    /// <summary>
    /// Extracts the index from a formatted port choice string.
    /// </summary>
    /// <param name="selection">The selected choice string</param>
    /// <returns>The 0-based index, -1 for Exit, or -2 for Refresh</returns>
    public static int ParseSelectionIndex(string selection)
    {
        if (selection == "Exit")
        {
            return -1;
        }

        if (selection == "Refresh Port List")
        {
            return -2;
        }

        var indexPart = selection.Split('.')[0];
        return int.Parse(indexPart) - 1;
    }
}
