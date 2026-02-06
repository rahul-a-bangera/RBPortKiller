using RBPortKiller.Core.Models;
using Spectre.Console;

namespace RBPortKiller.CLI.UI;

/// <summary>
/// Builds formatted panels for displaying detailed port information.
/// Extracted to separate panel formatting from CLI orchestration.
/// </summary>
public static class PortDetailsPanelBuilder
{
    /// <summary>
    /// Creates a formatted panel displaying detailed port information.
    /// </summary>
    /// <param name="portInfo">The port information to display</param>
    /// <returns>Configured Spectre.Console panel</returns>
    public static Panel BuildDetailsPanel(PortInfo portInfo)
    {
        var content = FormatPortDetails(portInfo);

        return new Panel(content)
            .Header("[yellow]Selected Port Details[/]")
            .BorderColor(Color.Yellow)
            .Padding(1, 1);
    }

    private static string FormatPortDetails(PortInfo portInfo)
    {
        var details = $"""
            [cyan]Port:[/] [yellow]{portInfo.Port}[/]
            [cyan]Protocol:[/] [blue]{portInfo.Protocol}[/]
            [cyan]Process ID:[/] [magenta]{portInfo.ProcessId}[/]
            [cyan]Process Name:[/] [green]{portInfo.ProcessName}[/]
            [cyan]Local Address:[/] {portInfo.LocalAddress}
            [cyan]State:[/] {portInfo.State ?? "N/A"}
            """;

        if (portInfo.CreatedDate.HasValue)
        {
            details += $"\n[cyan]Created:[/] {portInfo.CreatedDate.Value:yyyy-MM-dd HH:mm:ss}";
        }

        if (portInfo.ProcessPath != null)
        {
            details += $"\n[cyan]Process Path:[/] [dim]{portInfo.ProcessPath}[/]";
        }

        return details;
    }
}
