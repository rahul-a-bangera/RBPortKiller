using System.Net;
using System.Net.Sockets;

namespace TestPortOpener;

/// <summary>
/// Test application that opens multiple dummy ports for testing RBPortKiller.
/// This is a non-system process that can be safely terminated.
/// </summary>
class Program
{
    private static readonly List<TcpListener> _listeners = new();
    private static readonly List<UdpClient> _udpClients = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("??????????????????????????????????????????????????????????");
        Console.WriteLine("?          Test Port Opener for RBPortKiller            ?");
        Console.WriteLine("??????????????????????????????????????????????????????????");
        Console.WriteLine();
        Console.WriteLine("Select test mode:");
        Console.WriteLine("  1. Simple Mode  - Open 7 ports immediately");
        Console.WriteLine("  2. Advanced Mode - Test creation time sorting");
        Console.WriteLine();
        Console.Write("Enter choice (1 or 2): ");

        var choice = Console.ReadLine();
        Console.WriteLine();

        if (choice == "2")
        {
            await AdvancedPortTester.RunAdvancedTests();
        }
        else
        {
            await RunSimpleMode();
        }
    }

    static async Task RunSimpleMode()
    {
        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine("Starting Simple Mode - Opening All Ports...");
        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine();

        // Start multiple TCP listeners
        StartTcpListener(3000, "Development Server");
        StartTcpListener(5000, "API Server");
        StartTcpListener(8080, "Web Server");
        StartTcpListener(9000, "Test Service");
        StartTcpListener(7777, "Game Server");

        // Start UDP listeners
        StartUdpListener(6000, "UDP Service 1");
        StartUdpListener(6001, "UDP Service 2");

        Console.WriteLine();
        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine("All ports are now OPEN and ready for testing!");
        Console.WriteLine("???????????????????????????????????????????????????????");
        Console.WriteLine();
        Console.WriteLine("Now you can:");
        Console.WriteLine("  1. Run RBPortKiller");
        Console.WriteLine("  2. See these ports in the list");
        Console.WriteLine("  3. Test killing this process");
        Console.WriteLine("  4. Verify creation times are displayed");
        Console.WriteLine();
        Console.WriteLine("Press Ctrl+C to stop and close all ports...");
        Console.WriteLine();

        // Keep the application running
        await Task.Delay(Timeout.Infinite);
    }

    static void StartTcpListener(int port, string description)
    {
        try
        {
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            _listeners.Add(listener);

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"? [{timestamp}] TCP Port {port,5} - {description}");

            // Accept connections in background (but don't do anything with them)
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        // Just accept and hold the connection
                        _ = Task.Run(() => HoldConnection(client, port));
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
            Console.WriteLine($"? TCP Port {port,5} - FAILED: {ex.Message}");
        }
    }

    static void StartUdpListener(int port, string description)
    {
        try
        {
            var udpClient = new UdpClient(port);
            _udpClients.Add(udpClient);

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"? [{timestamp}] UDP Port {port,5} - {description}");

            // Listen for UDP packets in background
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var result = await udpClient.ReceiveAsync();
                        // Received a packet, but we don't need to do anything
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
            Console.WriteLine($"? UDP Port {port,5} - FAILED: {ex.Message}");
        }
    }

    static async Task HoldConnection(TcpClient client, int port)
    {
        try
        {
            // Just keep the connection alive
            using (client)
            {
                await Task.Delay(Timeout.Infinite);
            }
        }
        catch
        {
            // Connection closed or error
        }
    }
}

