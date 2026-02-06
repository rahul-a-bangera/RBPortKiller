using System.Net;
using System.Net.Sockets;

namespace TestPortOpener;

/// <summary>
/// Advanced test scenarios for RBPortKiller testing.
/// Creates ports with different characteristics and timing.
/// </summary>
public class AdvancedPortTester
{
    private static readonly List<TcpListener> _listeners = new();
    private static readonly Random _random = new();

    public static async Task RunAdvancedTests()
    {
        Console.WriteLine("??????????????????????????????????????????????????????????");
        Console.WriteLine("?        Advanced Port Testing Scenarios                ?");
        Console.WriteLine("??????????????????????????????????????????????????????????");
        Console.WriteLine();

        // Scenario 1: Start some ports immediately
        Console.WriteLine("?? Scenario 1: Initial Ports (started now)");
        OpenPort(3000, "Initial Dev Server");
        OpenPort(5000, "Initial API Server");
        Console.WriteLine();

        await Task.Delay(2000);

        // Scenario 2: Start ports after a delay (to test creation time sorting)
        Console.WriteLine("?? Scenario 2: Delayed Ports (started 2 seconds later)");
        OpenPort(8080, "Delayed Web Server");
        OpenPort(9000, "Delayed Service");
        Console.WriteLine();

        await Task.Delay(3000);

        // Scenario 3: Start more ports (to test sorting with multiple creation times)
        Console.WriteLine("?? Scenario 3: More Delayed Ports (started 5 seconds later)");
        OpenPort(7777, "Latest Game Server");
        OpenPort(4200, "Latest Angular App");
        Console.WriteLine();

        // Scenario 4: Ports with high activity
        Console.WriteLine("?? Scenario 4: Active Connection Ports");
        StartActivePort(6000, "Active Service");
        Console.WriteLine();

        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine("Test Scenarios Complete!");
        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine();
        Console.WriteLine("What you should see in RBPortKiller:");
        Console.WriteLine("  • Ports sorted by creation time (newest first)");
        Console.WriteLine("  • Port 4200 and 7777 should be at the top");
        Console.WriteLine("  • Port 8080 and 9000 in the middle");
        Console.WriteLine("  • Port 3000 and 5000 near the bottom");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to stop...");

        await Task.Delay(Timeout.Infinite);
    }

    static void OpenPort(int port, string description)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            _listeners.Add(listener);

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"  ? [{timestamp}] Port {port,5} - {description}");

            // Accept connections silently
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        _ = HandleClient(client);
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ? Port {port,5} - FAILED: {ex.Message}");
        }
    }

    static void StartActivePort(int port, string description)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            _listeners.Add(listener);

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"  ? [{timestamp}] Port {port,5} - {description} (with simulated activity)");

            // Accept and respond to connections
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        _ = Task.Run(async () =>
                        {
                            using (client)
                            using (var stream = client.GetStream())
                            {
                                var message = System.Text.Encoding.UTF8.GetBytes(
                                    $"Test response from port {port}\n");
                                await stream.WriteAsync(message);
                            }
                        });
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ? Port {port,5} - FAILED: {ex.Message}");
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        try
        {
            using (client)
            {
                // Keep connection alive for a random duration
                await Task.Delay(_random.Next(5000, 30000));
            }
        }
        catch
        {
            // Connection closed
        }
    }
}
