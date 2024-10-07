using service.Core.Commands;
using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;
using service.Core.Interfaces.KeyManagement;
using service.Core.Interfaces.OpenSsl;

namespace service.Application.Service.KeyManagement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyManagementService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging errors and information.</param>
    /// <param name="keyManagementRepository">The repository interface to handle key management operations.</param>
    /// <param name="openSslService">The service interface to handle openssl commands.</param>
    public class KeyManagementService(ILogger<KeyManagementService> logger, IKeyManagementRepository keyManagementRepository, IOpenSslService openSslService) : IKeyManagementService
    {
        private readonly ILogger<KeyManagementService> _logger = logger;
        private readonly IKeyManagementRepository _keyManagementRepository = keyManagementRepository;
        private readonly IOpenSslService _openSslService = openSslService;

        /// <summary>
        /// Retrieves the list of keys from the key management repository.
        /// </summary>
        /// <returns>A list of keys if successful; otherwise, an empty list.</returns>
        /// <remarks>
        /// Logs an error message in case of any exception and returns an empty list.
        /// </remarks>
        public async Task<(int, List<Keys>)> GetKeysList()
        {
            try
            {
                var keysList = await _keyManagementRepository.GetKeysListAsync();
                if (keysList == null || keysList.Count == 0)
                {
                    _logger.LogError("No keys found.");
                    return (0, keysList)!;
                }
                else
                {
                    _logger.LogInformation("{Keys} keys retrieved successfully.", keysList.Count);
                    return (1, keysList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
                return (-1, []);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inCreateKeyDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<(int, string)> CreateKey(InCreateKeyDto inCreateKeyDto)
        {
            try
            {
                string? createAesKeyCommand = null;

                if (inCreateKeyDto.KeyAlgorithm == "AES" && inCreateKeyDto.KeyType == "Symmetric")
                {
                    createAesKeyCommand = inCreateKeyDto.KeySize switch
                    {
                        128 => OpenSslCommands.GenerateAes128KeyData,// Generate AES-128 key
                        192 => OpenSslCommands.GenerateAes192KeyData,// Generate AES-192 key
                        256 => OpenSslCommands.GenerateAes256KeyData,// Generate AES-256 key
                        _ => throw new ArgumentException("Invalid key size for AES."),
                    };
                    string aesKeyData = await _openSslService.RunOpenSslCommandAsync(createAesKeyCommand);
                    return (1, aesKeyData);
                }
                else
                {
                    _logger.LogError("Error occurred while creating a key. Invalid parameters passed.");
                    return (0, "Error occurred while creating a key. Invalid parameters passed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while creating a key: {Message}", ex.Message);
                return (-1, $"An error occurred while creating key: {ex.Message}");
            }
        }
    }
}
