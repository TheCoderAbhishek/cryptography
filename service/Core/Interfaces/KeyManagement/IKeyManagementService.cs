using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;

namespace service.Core.Interfaces.KeyManagement
{
    /// <summary>
    /// Defines the interface for key management services.
    /// </summary>
    public interface IKeyManagementService
    {
        /// <summary>
        /// Gets a list of keys.
        /// </summary>
        /// <returns>A list of keys.</returns>
        Task<(int, List<Keys>)> GetKeysList();

        /// <summary>
        /// Creates a new key asynchronously.
        /// </summary>
        /// <param name="inCreateKeyDto">The input data for creating the key.</param>
        /// <param name="keyOwner">The owner of the creating the key.</param>
        /// <returns>A task representing the operation. The result is a tuple containing the status code (int) and a message (string).</returns>
        Task<(int, string)> CreateKey(InCreateKeyDto inCreateKeyDto, string keyOwner);

        /// <summary>
        /// Exports a key associated with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the key to be exported.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with an integer status code and a string message.</returns>
        Task<(int, string, string)> ExportKey(int id);
    }
}
