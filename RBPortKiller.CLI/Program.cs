using Microsoft.Extensions.DependencyInjection;
using RBPortKiller.CLI;
using RBPortKiller.CLI.Configuration;
using Spectre.Console;

// Setup dependency injection
var services = new ServiceCollection();
services.AddRBPortKillerServices();

var serviceProvider = services.BuildServiceProvider();

// Run the CLI
try
{
    var cli = serviceProvider.GetRequiredService<PortKillerCli>();
    await cli.RunAsync();
}
catch (PlatformNotSupportedException ex)
{
    AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
    return 1;
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Unexpected error:[/] {ex.Message}");
    return 1;
}

return 0;
