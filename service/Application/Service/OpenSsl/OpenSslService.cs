using service.Core.Interfaces.OpenSsl;
using System.Diagnostics;

namespace service.Application.Service.OpenSsl
{
    /// <summary>
    /// Provides services for interacting with OpenSSL commands.
    /// </summary>
    public class OpenSslService(ILogger<OpenSslService> logger) : IOpenSslService
    {
        private readonly ILogger<OpenSslService> _logger = logger;

        /// <summary>
        /// Executes the specified OpenSSL command asynchronously.
        /// </summary>
        /// <param name="command">The OpenSSL command to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the output of the command.</returns>
        /// <exception cref="System.Exception">Throws an exception if the command execution fails.</exception>
        public async Task<string> RunOpenSslCommandAsync(string command)
        {
            _logger.LogInformation("OpenSSL command executing: {Command}", command);

            var processInfo = new ProcessStartInfo("cmd.exe", $"/C {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(processInfo);
            using var outputReader = process!.StandardOutput;
            using var errorReader = process.StandardError;

            string output = await outputReader.ReadToEndAsync();
            string error = await errorReader.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(error))
            {
                _logger.LogError("OpenSSL command error: {Error}", error);
            }

            return output.Trim();
        }

        /// <summary>
        /// Executes an OpenSSL command asynchronously, providing the specified input.
        /// </summary>
        /// <param name="command">The OpenSSL command to execute.</param>
        /// <param name="input">The input to be provided to the command.</param>
        /// <returns>A task representing the operation, which will return the output of the command upon completion.</returns>
        public async Task<string> RunOpenSslCommandAsyncWithInput(string command, string input)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", $"/C {command}")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(processInfo);

            // Write the private key to the process' input stream
            using (var writer = process!.StandardInput)
            {
                await writer.WriteLineAsync(input);
            }

            // Read the public key from the output
            using var reader = process.StandardOutput;
            string result = await reader.ReadToEndAsync();
            return result.Trim();
        }
    }
}
