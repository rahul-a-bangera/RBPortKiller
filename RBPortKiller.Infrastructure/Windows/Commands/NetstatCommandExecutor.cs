using System.Diagnostics;

namespace RBPortKiller.Infrastructure.Windows.Commands;

/// <summary>
/// Executes the netstat command to retrieve network connection information.
/// Extracted for testability and to separate I/O concerns from parsing logic.
/// </summary>
internal sealed class NetstatCommandExecutor
{
    private const string NetstatExecutable = "netstat";
    private const string NetstatArguments = "-ano";

    /// <summary>
    /// Executes netstat -ano and returns the output.
    /// </summary>
    /// <returns>The command output, or empty string on failure</returns>
    public string Execute()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = NetstatExecutable,
                Arguments = NetstatArguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return string.Empty;
            }

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }
        catch
        {
            return string.Empty;
        }
    }
}
