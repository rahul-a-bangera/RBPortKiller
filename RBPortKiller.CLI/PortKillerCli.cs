using RBPortKiller.Core.Models;
using RBPortKiller.Core.Services;
using RBPortKiller.CLI.UI;
using Spectre.Console;

namespace RBPortKiller.CLI;

/// <summary>
/// Interactive CLI for managing network ports and processes.
/// </summary>
public sealed class PortKillerCli
{
    private readonly IPortKillerService _portKillerService;

    public PortKillerCli(IPortKillerService portKillerService)
    {
        _portKillerService = portKillerService ?? throw new ArgumentNullException(nameof(portKillerService));
    }

    public async Task RunAsync()
    {
        DisplayBanner();

        while (true)
        {
            try
            {
                var ports = await LoadPortsAsync();
                
                if (!ports.Any())
                {
                    AnsiConsole.MarkupLine("[yellow]No active ports found.[/]");
                    
                    if (!PromptToContinue())
                    {
                        break;
                    }
                    continue;
                }

                var selectionResult = DisplayPortSelectionMenu(ports);
                
                if (selectionResult.Action == PortSelectionAction.Exit)
                {
                    // User chose to exit
                    break;
                }

                if (selectionResult.Action == PortSelectionAction.Refresh)
                {
                    // User chose to refresh - loop will reload ports
                    AnsiConsole.Clear();
                    DisplayBanner();
                    AnsiConsole.MarkupLine("[cyan]Refreshing port list...[/]\n");
                    continue;
                }

                // User selected a port to manage
                if (selectionResult.SelectedPort != null)
                {
                    await HandlePortActionAsync(selectionResult.SelectedPort);
                }
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[yellow]Exiting...[/]");
                break;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                
                if (!PromptToContinue())
                {
                    break;
                }
            }
        }

        AnsiConsole.MarkupLine("\n[green]Thank you for using RBPortKiller![/]");
    }

    private void DisplayBanner()
    {
        var banner = new FigletText("RBPortKiller")
            .Centered()
            .Color(Color.Cyan1);

        AnsiConsole.Write(banner);
        AnsiConsole.MarkupLine("[dim]View and terminate processes by network port.[/]\n");
    }

    private async Task<List<PortInfo>> LoadPortsAsync()
    {
        return await AnsiConsole.Status()
            .StartAsync("Loading active ports...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("cyan"));

                var ports = await _portKillerService.GetActivePortsAsync();
                return ports.ToList();
            });
    }

    private PortSelectionResult DisplayPortSelectionMenu(List<PortInfo> ports)
    {
        var table = PortTableBuilder.BuildPortTable(ports);
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim] Tip: Press Ctrl+C anytime to exit quickly[/]\n");

        var choices = PortSelectionFormatter.CreatePortChoices(ports);

        var selection = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Select a port to manage:[/]")
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down to reveal more ports)[/]")
                .AddChoices(choices));

        var index = PortSelectionFormatter.ParseSelectionIndex(selection);
        
        if (index == -1)
        {
            return PortSelectionResult.Exit();
        }

        if (index == -2)
        {
            return PortSelectionResult.Refresh();
        }

        return PortSelectionResult.PortSelected(ports[index]);
    }

    private async Task HandlePortActionAsync(PortInfo portInfo)
    {
        AnsiConsole.WriteLine();
        
        var panel = PortDetailsPanelBuilder.BuildDetailsPanel(portInfo);
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();

        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]What would you like to do?[/]")
                .AddChoices(new[]
                {
                    "Kill Process",
                    "Back to Port List"
                }));

        if (action == "Kill Process")
        {
            await TerminateProcessAsync(portInfo);
        }
    }

    private async Task TerminateProcessAsync(PortInfo portInfo)
    {
        // Check permissions first
        if (!_portKillerService.CanTerminateProcess(portInfo))
        {
            AnsiConsole.MarkupLine("[yellow]Warning:[/] You may not have permission to terminate this process.");
            AnsiConsole.MarkupLine("[dim]Try running as administrator.[/]\n");
        }

        var confirm = AnsiConsole.Confirm(
            $"[red]Are you sure you want to kill process {portInfo.ProcessName} (PID: {portInfo.ProcessId})?[/]",
            false);

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[yellow]Operation cancelled.[/]\n");
            return;
        }

        var result = await AnsiConsole.Status()
            .StartAsync("Terminating process...", async ctx =>
            {
                ctx.Spinner(Spinner.Known.Dots);
                ctx.SpinnerStyle(Style.Parse("red"));
                return await _portKillerService.TerminateProcessAsync(portInfo);
            });

        AnsiConsole.WriteLine();

        if (result.Success)
        {
            AnsiConsole.MarkupLine($"[green]✓[/] Process {portInfo.ProcessId} terminated successfully.");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]✗[/] Failed to terminate process {portInfo.ProcessId}");
            AnsiConsole.MarkupLine($"[red]Error:[/] {result.ErrorMessage}");

            if (result.IsPermissionDenied)
            {
                AnsiConsole.MarkupLine("[yellow]Tip:[/] Try running the tool as administrator.");
            }
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("[dim]Press any key to continue...[/]");
        Console.ReadKey(true);
    }

    private bool PromptToContinue()
    {
        return AnsiConsole.Confirm("Would you like to refresh and try again?", true);
    }
}
