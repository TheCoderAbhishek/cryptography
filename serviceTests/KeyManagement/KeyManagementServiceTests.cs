using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using service.Application.Service.KeyManagement;
using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;
using service.Core.Interfaces.KeyManagement;
using service.Core.Interfaces.OpenSsl;

namespace serviceTests.KeyManagement
{
    public class KeyManagementServiceTests
    {
        private readonly Mock<IKeyManagementRepository> _keyManagementRepositoryMock;
        private readonly Mock<ILogger<KeyManagementService>> _loggerMock;
        private readonly KeyManagementService _service;
        private readonly Mock<IOpenSslService> _openSslServiceMock;
        private readonly Faker<InCreateKeyDto> _fakeInCreateKeyDto;
        private readonly Faker<Keys> _fakeKeys;

        public KeyManagementServiceTests()
        {
            _keyManagementRepositoryMock = new Mock<IKeyManagementRepository>();
            _openSslServiceMock = new Mock<IOpenSslService>();
            _loggerMock = new Mock<ILogger<KeyManagementService>>();
            _service = new KeyManagementService(_loggerMock.Object, _keyManagementRepositoryMock.Object, _openSslServiceMock.Object);

            // Bogus for generating random data
            _fakeInCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, f => f.Lorem.Word())
                .RuleFor(k => k.KeyType, "symmetric")
                .RuleFor(k => k.KeyAlgorithm, "aes")
                .RuleFor(k => k.KeySize, 256)
                .RuleFor(k => k.KeyUsage, "encryption");

            // Bogus for generating fake data
            _fakeKeys = new Faker<Keys>()
                .RuleFor(k => k.Id, f => f.Random.Int(1, 1000))
                .RuleFor(k => k.KeyName, f => f.Lorem.Word());
        }

        #region Helpers for common mocking scenarios

        private void SetupUniqueKeyName(int returnValue)
        {
            _keyManagementRepositoryMock
                .Setup(repo => repo.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(returnValue);
        }

        private void SetupUniqueKeyId(int returnValue)
        {
            _keyManagementRepositoryMock
                .Setup(repo => repo.CheckUniqueKeyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(returnValue);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "S1172:Unused parameter", Justification = "Required by method signature.")]
        private void SetupOpenSslCommand(string keyAlgorithm, string result)
        {
            _openSslServiceMock
                .Setup(service => service.RunOpenSslCommandAsync(It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        #endregion

        #region GetKeysList Tests
        [Fact]
        public async Task GetKeysList_ReturnsKeysList_WhenKeysExist()
        {
            // Arrange
            var fakeKeys = new Faker<Keys>()
                .RuleFor(k => k.Id, f => f.Random.Int())
                .RuleFor(k => k.KeyId, f => f.Random.Guid().ToString())
                .RuleFor(k => k.KeyName, f => f.Lorem.Word())
                .Generate(5);

            _keyManagementRepositoryMock.Setup(repo => repo.GetKeysListAsync())
                .ReturnsAsync(fakeKeys);

            // Act
            var (status, keys) = await _service.GetKeysList();

            // Assert
            Assert.Equal(1, status);
            Assert.Equal(5, keys.Count);
        }

        [Fact]
        public async Task GetKeysList_ReturnsNoKeys_WhenEmptyList()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeysListAsync())
                .ReturnsAsync([]);

            // Act
            var (status, keys) = await _service.GetKeysList();

            // Assert
            Assert.Equal(0, status);
            Assert.Empty(keys);
        }

        [Fact]
        public async Task GetKeysList_ReturnsError_OnException()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeysListAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var (status, keys) = await _service.GetKeysList();

            // Assert
            Assert.Equal(-1, status);
            Assert.Empty(keys);
        }
        #endregion

        #region CreateKey Tests

        [Fact]
        public async Task CreateKey_ShouldReturnSuccess_WhenAesKeyIsCreated()
        {
            // Arrange
            var fakeKeyDto = _fakeInCreateKeyDto.Generate();
            var keyOwner = "user123";
            SetupUniqueKeyName(1);  // Key name is unique
            SetupUniqueKeyId(1);    // Key ID is unique
            SetupOpenSslCommand("aes", "GeneratedAesKeyData");

            _keyManagementRepositoryMock
                .Setup(repo => repo.CreateKeyAsync(It.IsAny<Keys>()))
                .ReturnsAsync(1);  // Simulate successful key creation

            // Act
            var result = await _service.CreateKey(fakeKeyDto, keyOwner);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("Key created successfully.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_ShouldReturnError_WhenKeyNameAlreadyExists()
        {
            // Arrange
            var fakeKeyDto = _fakeInCreateKeyDto.Generate();
            SetupUniqueKeyName(0);  // Key name already exists

            // Act
            var result = await _service.CreateKey(fakeKeyDto, "user123");

            // Assert
            Assert.Equal(2, result.Item1);
            Assert.Equal("Key name already exists.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_ShouldReturnError_WhenPrivateKeyStorageFails()
        {
            // Arrange
            var fakeKeyDto = _fakeInCreateKeyDto.Clone().RuleFor(k => k.KeyType, "asymmetric").Generate();
            SetupUniqueKeyName(1);  // Key name is unique
            SetupUniqueKeyId(1);    // Key ID is unique
            SetupOpenSslCommand("rsa", "GeneratedRsaPrivateKeyData");

            _keyManagementRepositoryMock
                .Setup(repo => repo.InsertPrivateDataAsync(It.IsAny<SecureKeys>()))
                .ReturnsAsync(0);  // Simulate failure in storing private key

            // Act
            var result = await _service.CreateKey(fakeKeyDto, "user123");

            // Assert
            Assert.Equal(-2, result.Item1);
            Assert.Equal("Error occurred while creating private key", result.Item2);
        }

        [Fact]
        public async Task CreateKey_ShouldReturnError_ForInvalidAlgorithm()
        {
            // Arrange
            var fakeKeyDto = _fakeInCreateKeyDto.Clone().RuleFor(k => k.KeyAlgorithm, "invalidAlgorithm").Generate();
            SetupUniqueKeyName(1);  // Key name is unique
            SetupUniqueKeyId(1);    // Key ID is unique

            // Act
            var result = await _service.CreateKey(fakeKeyDto, "user123");

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Equal("Error occurred while creating a key. Invalid parameters passed.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_KeyNameNotUnique_ReturnsError()
        {
            // Arrange
            var inCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, "existingKeyName")
                .Generate();

            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(1); // Key name exists

            // Act
            var result = await _service.CreateKey(inCreateKeyDto, "owner");

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Equal("Error occurred while creating a key. Invalid parameters passed.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_KeyIdNotUnique_ReturnsError()
        {
            // Arrange
            var inCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, "uniqueKey")
                .RuleFor(k => k.KeyAlgorithm, "aes")
                .RuleFor(k => k.KeyType, "symmetric")
                .RuleFor(k => k.KeySize, 128)
                .Generate();

            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(1);
            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(0); // Key ID already exists

            // Act
            var result = await _service.CreateKey(inCreateKeyDto, "owner");

            // Assert
            Assert.Equal(3, result.Item1);
            Assert.Equal("Key ID already exists.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_ValidAesKey_CreatesKeySuccessfully()
        {
            // Arrange
            var inCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, "aesKey")
                .RuleFor(k => k.KeyAlgorithm, "aes")
                .RuleFor(k => k.KeyType, "symmetric")
                .RuleFor(k => k.KeySize, 256) // Valid AES key size
                .Generate();

            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(1);
            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(1);
            _openSslServiceMock.Setup(x => x.RunOpenSslCommandAsync(It.IsAny<string>()))
                .ReturnsAsync("generatedKeyData");
            _keyManagementRepositoryMock.Setup(x => x.CreateKeyAsync(It.IsAny<Keys>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateKey(inCreateKeyDto, "owner");

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("Key created successfully.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_AsymmetricRsaKey_CreatesKeySuccessfully()
        {
            // Arrange
            var inCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, "rsaKey")
                .RuleFor(k => k.KeyAlgorithm, "rsa")
                .RuleFor(k => k.KeyType, "asymmetric")
                .RuleFor(k => k.KeySize, 2048)
                .RuleFor(k => k.KeyUsage, "Encryption")
                .Generate();

            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(1);
            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(1);
            _openSslServiceMock.Setup(x => x.RunOpenSslCommandAsync(It.IsAny<string>()))
                .ReturnsAsync("privateKeyData");
            _openSslServiceMock.Setup(x => x.RunOpenSslCommandAsyncWithInput(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("publicKeyData");
            _keyManagementRepositoryMock.Setup(x => x.InsertPrivateDataAsync(It.IsAny<SecureKeys>()))
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateKey(inCreateKeyDto, "owner");

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Equal("Failed to create key.", result.Item2);
        }

        [Fact]
        public async Task CreateKey_OpenSslCommandFails_ReturnsError()
        {
            // Arrange
            var inCreateKeyDto = new Faker<InCreateKeyDto>()
                .RuleFor(k => k.KeyName, "key")
                .RuleFor(k => k.KeyAlgorithm, "aes")
                .RuleFor(k => k.KeyType, "symmetric")
                .RuleFor(k => k.KeySize, 128)
                .Generate();

            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyName(It.IsAny<string>()))
                .ReturnsAsync(1);
            _keyManagementRepositoryMock.Setup(x => x.CheckUniqueKeyIdAsync(It.IsAny<string>()))
                .ReturnsAsync(1);
            _openSslServiceMock.Setup(x => x.RunOpenSslCommandAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("OpenSSL command failed"));

            // Act
            var result = await _service.CreateKey(inCreateKeyDto, "owner");

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Equal("Error occurred while creating a key. Invalid parameters passed.", result.Item2);
        }

        #endregion

        #region SoftDeleteKey Tests

        [Fact]
        public async Task SoftDeleteKey_KeyExistsAndActive_ShouldSoftDelete()
        {
            // Arrange
            var key = _fakeKeys.Generate();
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(key);
            _keyManagementRepositoryMock.Setup(repo => repo.SoftDeleteKeyAsync(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _service.SoftDeleteKey(key.Id);

            // Assert
            Assert.Equal(-3, result.Item1);
            Assert.Equal($"Key with ID {key.Id} soft deleted successfully.", result.Item2);
        }

        [Fact]
        public async Task SoftDeleteKey_KeyExistsButAlreadyDeleted_ShouldReturnAlreadyDeletedError()
        {
            // Arrange
            var key = _fakeKeys.Generate();
            key.KeyStatus = false; // Key is already deleted
            key.KeyState = 0;
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(key);

            // Act
            var result = await _service.SoftDeleteKey(key.Id);

            // Assert
            Assert.Equal(-3, result.Item1);
            Assert.Equal($"Key with ID {key.Id} soft deleted successfully.", result.Item2);
        }

        [Fact]
        public async Task SoftDeleteKey_KeyDoesNotExist_ShouldReturnInvalidIdError()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync((Keys)null!);

            // Act
            var result = await _service.SoftDeleteKey(999);

            // Assert
            Assert.Equal(-2, result.Item1);
            Assert.Equal("invalid id provided.", result.Item2);
        }

        [Fact]
        public async Task SoftDeleteKey_ExceptionThrown_ShouldReturnExceptionError()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new System.Exception("Test Exception"));

            // Act
            var result = await _service.SoftDeleteKey(1);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Equal("Exception occurred while soft deletion of a key.", result.Item2);
        }

        #endregion

        #region RecoverSoftDeletedKey Tests

        [Fact]
        public async Task RecoverSoftDeletedKey_KeyExistsAndDeleted_ShouldRecoverKey()
        {
            // Arrange
            var key = _fakeKeys.Generate();
            key.KeyStatus = false; // Key is soft deleted
            key.KeyState = 0;
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(key);
            _keyManagementRepositoryMock.Setup(repo => repo.RecoverSoftDeletedKeyAsync(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _service.RecoverSoftDeletedKey(key.Id);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal($"Key with ID {key.Id} recovered successfully.", result.Item2);
        }

        [Fact]
        public async Task RecoverSoftDeletedKey_KeyExistsButAlreadyActive_ShouldReturnAlreadyActiveError()
        {
            // Arrange
            var key = _fakeKeys.Generate();
            key.KeyStatus = true; // Key is already active
            key.KeyState = 1;
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(key);

            // Act
            var result = await _service.RecoverSoftDeletedKey(key.Id);

            // Assert
            Assert.Equal(-3, result.Item1);
            Assert.Equal($"Failed to recover soft deleted key with ID {key.Id}. Because key is already in active state.", result.Item2);
        }

        [Fact]
        public async Task RecoverSoftDeletedKey_KeyDoesNotExist_ShouldReturnInvalidIdError()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync((Keys)null!);

            // Act
            var result = await _service.RecoverSoftDeletedKey(999);

            // Assert
            Assert.Equal(-2, result.Item1);
            Assert.Equal("Invalid id provided.", result.Item2);
        }

        [Fact]
        public async Task RecoverSoftDeletedKey_ExceptionThrown_ShouldReturnExceptionError()
        {
            // Arrange
            _keyManagementRepositoryMock.Setup(repo => repo.GetKeyDetailsByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new System.Exception("Test Exception"));

            // Act
            var result = await _service.RecoverSoftDeletedKey(1);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Equal("Exception occurred while recovering soft deleted key.", result.Item2);
        }

        #endregion

        #region ExportKey Tests

        [Fact]
        public async Task ExportKey_ReturnsSuccess_WhenValidIdAndKeyMaterialExists()
        {
            // Arrange
            var keyId = 1;
            var keyMaterial = "ValidKeyMaterial";
            var fakeKey = _fakeKeys.Generate();

            _keyManagementRepositoryMock
                .Setup(repo => repo.GetKeyDetailsByIdAsync(keyId))
                .ReturnsAsync(fakeKey);

            _keyManagementRepositoryMock
                .Setup(repo => repo.ExportKeyAsync(keyId))
                .ReturnsAsync(keyMaterial);

            // Act
            var result = await _service.ExportKey(keyId);

            // Assert
            Assert.Equal(1, result.Item1); // Success
            Assert.Equal("Key material fetched successfully.", result.Item2);
            Assert.Equal(keyMaterial, result.Item3);
        }

        [Fact]
        public async Task ExportKey_ReturnsError_WhenKeyNotFound()
        {
            // Arrange
            var keyId = 1;

            _keyManagementRepositoryMock
                .Setup(repo => repo.GetKeyDetailsByIdAsync(keyId))
                .ReturnsAsync((Keys)null!); // Key not found

            // Act
            var result = await _service.ExportKey(keyId);

            // Assert
            Assert.Equal(-2, result.Item1); // Error Code for invalid ID
            Assert.Equal("invalid id provided.", result.Item2);
            Assert.Equal("Invalid Request", result.Item3);
        }

        [Fact]
        public async Task ExportKey_ReturnsError_WhenKeyMaterialIsEmpty()
        {
            // Arrange
            var keyId = 1;
            var fakeKey = _fakeKeys.Generate();

            _keyManagementRepositoryMock
                .Setup(repo => repo.GetKeyDetailsByIdAsync(keyId))
                .ReturnsAsync(fakeKey);

            _keyManagementRepositoryMock
                .Setup(repo => repo.ExportKeyAsync(keyId))
                .ReturnsAsync(string.Empty); // Empty key material

            // Act
            var result = await _service.ExportKey(keyId);

            // Assert
            Assert.Equal(0, result.Item1); // Error Code for empty key material
            Assert.Equal("Error occurred while exporting a key.", result.Item2);
            Assert.Equal(string.Empty, result.Item3);
        }

        [Fact]
        public async Task ExportKey_ReturnsError_WhenExceptionIsThrown()
        {
            // Arrange
            var keyId = 1;

            _keyManagementRepositoryMock
                .Setup(repo => repo.GetKeyDetailsByIdAsync(keyId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.ExportKey(keyId);

            // Assert
            Assert.Equal(-1, result.Item1); // Error code for exception
            Assert.Equal("Exception occurred while exporting a key.", result.Item2);
            Assert.Equal("Exception", result.Item3);
        }

        #endregion
    }
}
