using RBPortKiller.Core.Models;
using Spectre.Console;

namespace RBPortKiller.CLI.UI;

/// <summary>
/// Builds formatted tables for displaying port information.
/// Extracted to separate table building logic from CLI orchestration.
/// </summary>
public static class PortTableBuilder
{
    /// <summary>
    /// Creates a formatted table displaying port information.
    /// </summary>
    /// <param name="ports">List of ports to display</param>
    /// <returns>Configured Spectre.Console table</returns>
    public static Table BuildPortTable(List<PortInfo> ports)
    {
        var table = CreateBaseTable();
        AddRowsToTable(table, ports);
        return table;
    }

    private static Table CreateBaseTable()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey);

        table.AddColumn(new TableColumn("[cyan]#[/]").Centered());
        table.AddColumn(new TableColumn("[cyan]Port[/]").Centered());
        table.AddColumn(new TableColumn("[cyan]Protocol[/]").Centered());
        table.AddColumn(new TableColumn("[cyan]PID[/]").Centered());
        table.AddColumn(new TableColumn("[cyan]Process Name[/]"));
        table.AddColumn(new TableColumn("[cyan]Local Address[/]"));
        table.AddColumn(new TableColumn("[cyan]State[/]"));
        table.AddColumn(new TableColumn("[cyan]Created[/]"));

        return table;
    }

    private static void AddRowsToTable(Table table, List<PortInfo> ports)
    {
        for (int i = 0; i < ports.Count; i++)
        {
            var port = ports[i];
            var stateColor = StateColorMapper.GetColorForState(port.State);
            var createdDisplay = FormatCreatedDate(port.CreatedDate);

            table.AddRow(
                $"[dim]{i + 1}[/]",
                $"[yellow]{port.Port}[/]",
                $"[blue]{port.Protocol}[/]",
                $"[magenta]{port.ProcessId}[/]",
                $"[green]{port.ProcessName}[/]",
                $"[dim]{port.LocalAddress}[/]",
                $"[{stateColor}]{port.State ?? "N/A"}[/]",
                createdDisplay
            );
        }
    }

    private static string FormatCreatedDate(DateTime? createdDate)
    {
        if (!createdDate.HasValue)
        {
            return "[dim]N/A[/]";
        }

        var date = createdDate.Value;
        var now = DateTime.Now;
        var timeSpan = now - date;

        // If today, show time only
        if (date.Date == now.Date)
        {
            return $"[cyan]{date:HH:mm:ss}[/]";
        }

        // If yesterday
        if (date.Date == now.Date.AddDays(-1))
        {
            return $"[yellow]Yesterday {date:HH:mm}[/]";
        }

        // If within last 7 days
        if (timeSpan.TotalDays < 7)
        {
            return $"[yellow]{date:ddd HH:mm}[/]";
        }

        // Otherwise show full date
        return $"[dim]{date:MM/dd HH:mm}[/]";
    }
}
