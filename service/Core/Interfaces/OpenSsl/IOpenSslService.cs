namespace service.Core.Interfaces.OpenSsl
{
    /// <summary>
    /// Provides an interface for interacting with OpenSSL commands.
    /// </summary>
    public interface IOpenSslService
    {
        /// <summary>
        /// Executes the specified OpenSSL command asynchronously.
        /// </summary>
        /// <param name="command">The OpenSSL command to execute.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the output of the executed command.</returns>
        Task<string> RunOpenSslCommandAsync(string command);
    }
}
