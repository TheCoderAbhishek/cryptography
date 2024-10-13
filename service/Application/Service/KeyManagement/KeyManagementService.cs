using service.Core.Commands;
using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
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
                    inCreateKeyDto.KeyType = inCreateKeyDto.KeyType!.ToLower();
                    inCreateKeyDto.KeyAlgorithm = inCreateKeyDto.KeyAlgorithm!.ToLower();

                    string? createAesKeyCommand = null;
                    string? createDesKeyCommand = null;
                    string? create3DesKeyCommand = null;
                    string? createSeedKeyCommand = null;
                    string? createRsaKeyPairCommand = null;
                    string? createDsaKeyPairCommand = null;
                    string? createEcKeyPairCommand = null;
                    string? keyData = null;
                    string? privateKeyData = null;
                    string KeyId = GenerateKeyId();

                    int isKeyIdUnique = await _keyManagementRepository.CheckUniqueKeyIdAsync(KeyId);

                    if (isKeyIdUnique == 1)
                    {
                        if (inCreateKeyDto.KeyAlgorithm == "aes" && inCreateKeyDto.KeyType == "symmetric")
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
                        else if (inCreateKeyDto.KeyAlgorithm == "des" && inCreateKeyDto.KeyType == "symmetric")
                        {
                            createDesKeyCommand = inCreateKeyDto.KeySize switch
                            {
                                112 => OpenSslCommands.GenerateDes112KeyData,// Generate DES-112 key
                                168 => OpenSslCommands.GenerateDes168KeyData,// Generate DES-168 key
                                _ => throw new ArgumentException("Invalid key size for AES."),
                            };
                            keyData = await _openSslService.RunOpenSslCommandAsync(createDesKeyCommand);
                        }
                        else if (inCreateKeyDto.KeyAlgorithm == "3des" && inCreateKeyDto.KeyType == "symmetric")
                        {
                            create3DesKeyCommand = inCreateKeyDto.KeySize switch
                            {
                                64 => OpenSslCommands.Generate3Des64KeyData,// Generate 3DES-64 key
                                128 => OpenSslCommands.Generate3Des128KeyData,// Generate 3DES-128 key
                                192 => OpenSslCommands.Generate3Des192KeyData,// Generate 3DES-192 key
                                _ => throw new ArgumentException("Invalid key size for AES."),
                            };
                            keyData = await _openSslService.RunOpenSslCommandAsync(create3DesKeyCommand);
                        }
                        else if (inCreateKeyDto.KeyAlgorithm == "seed" && inCreateKeyDto.KeyType == "symmetric")
                        {
                            createSeedKeyCommand = inCreateKeyDto.KeySize switch
                            {
                                128 => OpenSslCommands.GenerateSeed128KeyData,// Generate SEED-128 key
                                _ => throw new ArgumentException("Invalid key size for AES."),
                            };
                            keyData = await _openSslService.RunOpenSslCommandAsync(createSeedKeyCommand);
                        }
                        else if (inCreateKeyDto.KeyType == "asymmetric")
                        {
                            if (inCreateKeyDto.KeyAlgorithm == "rsa")
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
                                privateKeyData = await _openSslService.RunOpenSslCommandAsync(createRsaKeyPairCommand);

                                // Step 2: Extract RSA Public Key from the generated Private Key
                                keyData = await _openSslService.RunOpenSslCommandAsyncWithInput(OpenSslCommands.ExtractPublicKeyFromPrivateKey, privateKeyData);
                            }
                            else if (inCreateKeyDto.KeyAlgorithm == "dsa")
                            {
                                // Define the output directory for the keys
                                string outputDirectory = ConstantData.KeyStorePath!;

                                string dsaParameterFileName = $"{outputDirectory}\\{ConstantData.DsaParameterFileName}";
                                string dsaKeyFileName = $"{outputDirectory}\\{ConstantData.DsaKeyFileName}";

                                // Generate the DSA private key based on the provided key size
                                createDsaKeyPairCommand = inCreateKeyDto.KeySize switch
                                {
                                    1024 => OpenSslCommands.GenerateDsa1024KeyData(outputDirectory), // Generate DSA-1024 key
                                    2048 => OpenSslCommands.GenerateDsa2048KeyData(outputDirectory), // Generate DSA-2048 key
                                    3072 => OpenSslCommands.GenerateDsa3072KeyData(outputDirectory), // Generate DSA-3072 key
                                    _ => throw new ArgumentException("Invalid key size for DSA."),
                                };

                                // Step 1: Generate DSA Private Key and store it in the specified directory
                                privateKeyData = await _openSslService.RunOpenSslCommandAsync(createDsaKeyPairCommand);

                                // Step 2: Extract DSA Public Key from the generated Private Key
                                keyData = await _openSslService.RunOpenSslCommandAsyncWithInput(OpenSslCommands.ExtractPublicKeyFromPrivateKeyDsa, privateKeyData);

                                File.Delete(dsaParameterFileName);
                                File.Delete(dsaKeyFileName);
                            }
                            else if (inCreateKeyDto.KeyAlgorithm == "ec")
                            {
                                // Step 1: Generate EC Private Key
                                createEcKeyPairCommand = inCreateKeyDto.KeySize switch
                                {
                                    160 => OpenSslCommands.GenerateEcc160PrivateKey,// Generate EC-160 key. This size is considered to provide security equivalent to a 1024-bit RSA key
                                    224 => OpenSslCommands.GenerateEcc224PrivateKey,// Generate EC-224 key. This size is considered to provide security equivalent to a 2048-bit RSA key
                                    256 => OpenSslCommands.GenerateEcc256PrivateKey,// Generate EC-256 key. This size is considered to provide security equivalent to a 3072-bit RSA key
                                    384 => OpenSslCommands.GenerateEcc384PrivateKey,// Generate EC-384 key. This size is considered to provide security equivalent to a 7680-bit RSA key
                                    521 => OpenSslCommands.GenerateEcc521PrivateKey,// Generate EC-521 key. This size is considered to provide security equivalent to a 15360-bit RSA key
                                    _ => throw new ArgumentException("Invalid key size for RSA."),
                                };

                                // Generate EC Private Key
                                privateKeyData = await _openSslService.RunOpenSslCommandAsync(createEcKeyPairCommand);

                                // Step 2: Extract EC Public Key from the generated Private Key
                                keyData = await _openSslService.RunOpenSslCommandAsyncWithInput(OpenSslCommands.ExtractPublicKeyFromPrivateKeyEcc, privateKeyData);
                            }
                            else
                            {
                                _logger.LogError("Invalid key algorithm provided: {KeyAlgorithm}", inCreateKeyDto.KeyAlgorithm);
                                return (-2, $"Error occurred while creating private key");
                            }

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
                                KeyMaterial = privateKeyData
                            };

                            int storeSecureKey = await _keyManagementRepository.InsertPrivateDataAsync(secureKey);

                            if (storeSecureKey == 1)
                            {
                                _logger.LogInformation("Asymmetric private key data successfully inserted into table.");
                            }
                            else
                            {
                                _logger.LogError("Error occurred while inserting asymmetric private key data.");
                                return (-2, $"Error occurred while creating {inCreateKeyDto.KeyAlgorithm} private key {inCreateKeyDto.KeyName}");
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
                        _logger.LogError("{KeyId} already present in table please use different key name.", KeyId);
                        return (3, $"Key ID already exists.");
                    }
                }
                else
                {
                    _logger.LogError("{KeyName} already present in table please use different key name.", inCreateKeyDto.KeyName);
                    return (2, $"Key name already exists.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while creating a key: {Message}", ex.Message);
                return (-1, $"Error occurred while creating a key. Invalid parameters passed.");
            }
        }

        /// <summary>
        /// Exports the key material for a given key ID.
        /// </summary>
        /// <param name="id">The ID of the key to export.</param>
        /// <returns>A tuple containing the status code and a message indicating the result of the operation.</returns>
        /// <exception cref="Exception">Throws an exception if an unexpected error occurs during the export process.</exception>
        public async Task<(int, string, string)> ExportKey(int id)
        {
            try
            {
                Keys? key = await _keyManagementRepository.GetKeyDetailsByIdAsync(id);

                if (key != null)
                {
                    string keyMaterial = await _keyManagementRepository.ExportKeyAsync(id);

                    if (!string.IsNullOrEmpty(keyMaterial))
                    {
                        _logger.LogInformation("Key Material fetched successfully.");
                        return (1, "Key material fetched successfully.", keyMaterial);
                    }
                    else
                    {
                        _logger.LogError("An unhandled exception occurred while exporting a key.");
                        return (0, "Error occurred while exporting a key.", keyMaterial);
                    } 
                }
                else
                {
                    _logger.LogError("Invalid id provided while exporting key material. {Id}", id);
                    return (-2, "invalid id provided.", "Invalid Request");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while exporting a key: {Message}", ex.Message);
                return (-1, $"Exception occurred while exporting a key.", "Exception");
            }
        }
    }
}
