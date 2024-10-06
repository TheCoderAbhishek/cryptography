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

            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError("OpenSSL command error: {Error}", error);
                return $"OpenSSL command error: {error}";
            }

            return output;
        }
    }
}
