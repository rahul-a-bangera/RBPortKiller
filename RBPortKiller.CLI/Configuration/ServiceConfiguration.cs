using Microsoft.Extensions.DependencyInjection;
using RBPortKiller.Core.Interfaces;
using RBPortKiller.Core.Services;
using RBPortKiller.Infrastructure;

namespace RBPortKiller.CLI.Configuration;

/// <summary>
/// Configures dependency injection for the application.
/// Centralizes service registration for improved maintainability.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Registers all application services including platform-specific and core services.
    /// </summary>
    public static IServiceCollection AddRBPortKillerServices(this IServiceCollection services)
    {
        // Register platform-specific services
        services.AddSingleton<IPortDiscoveryService>(sp => PlatformServiceFactory.CreatePortDiscoveryService());
        services.AddSingleton<IProcessManagementService>(sp => PlatformServiceFactory.CreateProcessManagementService());

        // Register core services
        services.AddSingleton<IPortKillerService, PortKillerService>();

        // Register CLI
        services.AddSingleton<PortKillerCli>();

        return services;
    }
}
