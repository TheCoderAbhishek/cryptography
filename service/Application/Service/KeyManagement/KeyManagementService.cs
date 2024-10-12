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

        #region Private Methods for Support

        /// <summary>
        /// Generates a new unique key identifier using a Guid.
        /// </summary>
        /// <returns>A string representing the newly generated key ID.</returns>
        private static string GenerateKeyId()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

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
        /// Creates a new key based on the provided parameters.
        /// </summary>
        /// <param name="inCreateKeyDto">A DTO containing information about the key to be created.</param>
        /// <param name="keyOwner">The owner of the key.</param>
        /// <returns>A tuple containing the status of the operation (0 for success, -1 for error) and a corresponding message.</returns>
        /// <exception cref="ArgumentException">Thrown if an invalid key size is specified for the AES algorithm.</exception>
        public async Task<(int, string)> CreateKey(InCreateKeyDto inCreateKeyDto, string keyOwner)
        {
            try
            {
                int isKeyNameUnique = await _keyManagementRepository.CheckUniqueKeyName(inCreateKeyDto.KeyName!);

                if (isKeyNameUnique == 1)
                {
                    string? createAesKeyCommand = null;
                    string? createRsaKeyPairCommand = null;
                    string? keyData = null;
                    string? rsaPrivateKeyData = null;
                    string KeyId = GenerateKeyId();

                    if (inCreateKeyDto.KeyAlgorithm == "AES" && inCreateKeyDto.KeyType == "Symmetric")
                    {
                        createAesKeyCommand = inCreateKeyDto.KeySize switch
                        {
                            128 => OpenSslCommands.GenerateAes128KeyData,// Generate AES-128 key
                            192 => OpenSslCommands.GenerateAes192KeyData,// Generate AES-192 key
                            256 => OpenSslCommands.GenerateAes256KeyData,// Generate AES-256 key
                            _ => throw new ArgumentException("Invalid key size for AES."),
                        };
                        keyData = await _openSslService.RunOpenSslCommandAsync(createAesKeyCommand);
                    }
                    else if (inCreateKeyDto.KeyAlgorithm == "RSA" && inCreateKeyDto.KeyType == "Asymmetric")
                    {
                        // Step 1: Generate RSA Private Key
                        createRsaKeyPairCommand = inCreateKeyDto.KeySize switch
                        {
                            2048 => OpenSslCommands.GenerateRsa2048PrivateKey,// Generate RSA-2048 key
                            3072 => OpenSslCommands.GenerateRsa3072PrivateKey,// Generate RSA-3072 key
                            4096 => OpenSslCommands.GenerateRsa4096PrivateKey,// Generate RSA-4096 key
                            _ => throw new ArgumentException("Invalid key size for RSA."),
                        };

                        // Generate RSA Private Key
                        rsaPrivateKeyData = await _openSslService.RunOpenSslCommandAsync(createRsaKeyPairCommand);

                        // Step 2: Extract RSA Public Key from the generated Private Key
                        keyData = await _openSslService.RunOpenSslCommandAsyncWithInput(OpenSslCommands.ExtractPublicKeyFromPrivateKey, rsaPrivateKeyData);

                        var secureKey = new SecureKeys
                        {
                            KeyId = KeyId,
                            KeyName = inCreateKeyDto.KeyName,
                            KeyType = inCreateKeyDto.KeyType,
                            KeyAlgorithm = inCreateKeyDto.KeyAlgorithm,
                            KeySize = inCreateKeyDto.KeySize,
                            KeyOwner = keyOwner,
                            KeyStatus = true,
                            KeyAccess = "Private",
                            KeyMaterial = rsaPrivateKeyData
                        };

                        int storeSecureKey = await _keyManagementRepository.InsertPrivateDataAsync(secureKey);

                        if (storeSecureKey == 1)
                        {
                            _logger.LogInformation("RSA private key data successfully inserted into table.");
                        }
                        else
                        {
                            _logger.LogError("Error occurred while inserting rsa private key data.");
                            return (-2, $"Error occurred while creating RSA private key {inCreateKeyDto.KeyName}");
                        }
                    }
                    else
                    {
                        _logger.LogError("Error occurred while creating a key. Invalid parameters passed.");
                        return (0, "Error occurred while creating a key. Invalid parameters passed.");
                    }

                    var newKey = new Keys
                    {
                        KeyId = KeyId,
                        KeyName = inCreateKeyDto.KeyName,
                        KeyType = inCreateKeyDto.KeyType,
                        KeyAlgorithm = inCreateKeyDto.KeyAlgorithm,
                        KeySize = inCreateKeyDto.KeySize,
                        KeyOwner = keyOwner,
                        KeyStatus = true,
                        KeyState = 1,
                        KeyAccess = "Public",
                        KeyUsage = inCreateKeyDto.KeyUsage,
                        KeyCreatedOn = DateTime.Now,
                        KeyUpdatedOn = DateTime.Now,
                        KeyMaterial = keyData
                    };

                    int status = await _keyManagementRepository.CreateKeyAsync(newKey);

                    if (status == 1)
                    {
                        _logger.LogInformation("Key created successfully.");
                        return (status, "Key created successfully.");
                    }
                    else
                    {
                        _logger.LogError("Failed to create key.");
                        return (status, "Failed to create key.");
                    } 
                }
                else
                {
                    _logger.LogError("{KeyName} already present in table please use different key name.", inCreateKeyDto.KeyName);
                    return (2, $"{inCreateKeyDto.KeyName} already present in table please use different key name.");
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
