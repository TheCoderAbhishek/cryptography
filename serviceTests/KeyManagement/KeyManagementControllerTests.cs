using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using service.Controllers;
using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;
using System.Security.Claims;

namespace serviceTests.KeyManagement
{
    public class KeyManagementControllerTests
    {
        private readonly Mock<IKeyManagementService> _keyManagementServiceMock;
        private readonly Mock<ILogger<KeyManagementController>> _loggerMock;
        private readonly KeyManagementController _controller;
        private readonly Faker _faker;

        public KeyManagementControllerTests()
        {
            _keyManagementServiceMock = new Mock<IKeyManagementService>();
            _loggerMock = new Mock<ILogger<KeyManagementController>>();
            _controller = new KeyManagementController(_loggerMock.Object, _keyManagementServiceMock.Object);
            _faker = new Faker();
            SetUserClaims();
        }

        #region Private Methods for Support

        private void SetUserClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _faker.Internet.Email()),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(claims);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        #endregion

        #region GetKeysListAsync Tests
        [Fact]
        public async Task GetKeysListAsync_ReturnsOk_WithKeys()
        {
            // Arrange
            var fakeKeys = new Faker<Keys>()
                .RuleFor(k => k.Id, f => f.Random.Int())
                .RuleFor(k => k.KeyId, f => f.Random.Guid().ToString())
                .RuleFor(k => k.KeyName, f => f.Lorem.Word())
                .Generate(3);

            _keyManagementServiceMock.Setup(s => s.GetKeysList())
                .ReturnsAsync((1, fakeKeys));

            // Act
            var result = await _controller.GetKeysListAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<List<Keys>>>(okResult.Value);
            Assert.Equal(1, response.ResponseCode);
            Assert.Equal(3, response.ReturnValue!.Count);
        }

        [Fact]
        public async Task GetKeysListAsync_ReturnsNotFound_WhenNoKeysFound()
        {
            // Arrange
            _keyManagementServiceMock.Setup(s => s.GetKeysList())
                .ReturnsAsync((0, []));

            // Act
            var result = await _controller.GetKeysListAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(notFoundResult.Value);
            Assert.Equal(0, response.ResponseCode);
            Assert.Equal("No keys found.", response.ErrorMessage);
        }

        [Fact]
        public async Task GetKeysListAsync_ReturnsServerError_OnException()
        {
            // Arrange
            _keyManagementServiceMock.Setup(s => s.GetKeysList())
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _controller.GetKeysListAsync();

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            var response = Assert.IsType<ApiResponse<string>>(serverErrorResult.Value);
            Assert.Equal(-1, response.ResponseCode);
            Assert.Equal("An unexpected error occurred.", response.ErrorMessage);
        }
        #endregion

        #region CreateKeyAsync Tests

        [Fact]
        public async Task CreateKeyAsync_ReturnsSuccess_WhenKeyCreated()
        {
            // Arrange
            var createKeyDto = new InCreateKeyDto { KeyName = _faker.Random.Word() };
            var expectedStatus = 1;
            var expectedMessage = "Key created successfully.";

            _keyManagementServiceMock
                .Setup(service => service.CreateKey(createKeyDto, It.IsAny<string>()))
                .ReturnsAsync((expectedStatus, expectedMessage));

            // Act
            var result = await _controller.CreateKeyAsync(createKeyDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ApiResponseStatus.Success, apiResponse!.Status);
            Assert.Equal(expectedMessage, apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task CreateKeyAsync_ReturnsDuplicateKeyError_WhenKeyAlreadyExists()
        {
            // Arrange
            var createKeyDto = new InCreateKeyDto { KeyName = _faker.Random.Word() };
            var expectedStatus = 2;
            var expectedMessage = "Duplicate key name.";

            _keyManagementServiceMock
                .Setup(service => service.CreateKey(createKeyDto, It.IsAny<string>()))
                .ReturnsAsync((expectedStatus, expectedMessage));

            // Act
            var result = await _controller.CreateKeyAsync(createKeyDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse!.Status);
            Assert.Equal(expectedMessage, apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task CreateKeyAsync_ReturnsError_WhenServiceReturnsOtherError()
        {
            // Arrange
            var createKeyDto = new InCreateKeyDto { KeyName = _faker.Random.Word() };
            var expectedStatus = -2;
            var expectedMessage = "Service error occurred.";

            _keyManagementServiceMock
                .Setup(service => service.CreateKey(createKeyDto, It.IsAny<string>()))
                .ReturnsAsync((expectedStatus, expectedMessage));

            // Act
            var result = await _controller.CreateKeyAsync(createKeyDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse!.Status);
            Assert.Equal(expectedMessage, apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task CreateKeyAsync_ReturnsBadRequest_WhenKeyCreationFails()
        {
            // Arrange
            var createKeyDto = new InCreateKeyDto { KeyName = _faker.Random.Word() };
            var expectedStatus = 0;
            var expectedMessage = "Key creation failed.";

            _keyManagementServiceMock
                .Setup(service => service.CreateKey(createKeyDto, It.IsAny<string>()))
                .ReturnsAsync((expectedStatus, expectedMessage));

            // Act
            var result = await _controller.CreateKeyAsync(createKeyDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse!.Status);
            Assert.Equal(expectedMessage, apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task CreateKeyAsync_ReturnsServerError_WhenExceptionOccurs()
        {
            // Arrange
            var createKeyDto = new InCreateKeyDto { KeyName = _faker.Random.Word() };
            _keyManagementServiceMock
                .Setup(service => service.CreateKey(createKeyDto, It.IsAny<string>()))
                .ThrowsAsync(new System.Exception("Unhandled exception."));

            // Act
            var result = await _controller.CreateKeyAsync(createKeyDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            var apiResponse = result.Value as ApiResponse<string>;
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse!.Status);
            Assert.Equal("An unexpected error occurred.", apiResponse.ErrorMessage);
        }

        #endregion

        #region ExportKeyAsync Tests

        [Fact]
        public async Task ExportKeyAsync_ShouldReturn200_WhenKeyExportIsSuccessful()
        {
            // Arrange
            int keyId = _faker.Random.Int(1, 1000);
            var successMessage = _faker.Lorem.Sentence();
            var keyMaterial = _faker.Lorem.Word();
            _keyManagementServiceMock
                .Setup(s => s.ExportKey(It.IsAny<int>()))
                .ReturnsAsync((1, successMessage, keyMaterial));

            // Act
            var result = await _controller.ExportKeyAsync(keyId) as OkObjectResult;
            var response = result?.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
            Assert.Equal(ApiResponseStatus.Success, response?.Status);
            Assert.Equal(successMessage, response?.SuccessMessage);
            Assert.Equal(keyMaterial, response?.ReturnValue);
        }

        [Fact]
        public async Task ExportKeyAsync_ShouldReturn302_WhenKeyExportFailsWithStatusZero()
        {
            // Arrange
            int keyId = _faker.Random.Int(1, 1000);
            var errorMessage = _faker.Lorem.Sentence();
            _keyManagementServiceMock
                .Setup(s => s.ExportKey(It.IsAny<int>()))
                .ReturnsAsync((0, errorMessage, string.Empty));

            // Act
            var result = await _controller.ExportKeyAsync(keyId) as OkObjectResult;
            var response = result?.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, response?.Status);
            Assert.Equal(errorMessage, response?.ErrorMessage);
        }

        [Fact]
        public async Task ExportKeyAsync_ShouldReturn400_WhenKeyExportFailsWithStatusNegativeTwo()
        {
            // Arrange
            int keyId = _faker.Random.Int(1, 1000);
            var errorMessage = _faker.Lorem.Sentence();
            _keyManagementServiceMock
                .Setup(s => s.ExportKey(It.IsAny<int>()))
                .ReturnsAsync((-2, errorMessage, string.Empty));

            // Act
            var result = await _controller.ExportKeyAsync(keyId) as OkObjectResult;
            var response = result?.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, response?.Status);
            Assert.Equal(errorMessage, response?.ErrorMessage);
        }

        [Fact]
        public async Task ExportKeyAsync_ShouldReturn400_WhenKeyExportFailsWithUnhandledStatus()
        {
            // Arrange
            int keyId = _faker.Random.Int(1, 1000);
            var errorMessage = _faker.Lorem.Sentence();
            _keyManagementServiceMock
                .Setup(s => s.ExportKey(It.IsAny<int>()))
                .ReturnsAsync((5, errorMessage, string.Empty));

            // Act
            var result = await _controller.ExportKeyAsync(keyId) as OkObjectResult;
            var response = result?.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result!.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, response?.Status);
            Assert.Equal(errorMessage, response?.ErrorMessage);
        }

        [Fact]
        public async Task ExportKeyAsync_ShouldReturn500_WhenExceptionOccurs()
        {
            // Arrange
            int keyId = _faker.Random.Int(1, 1000);
            _keyManagementServiceMock
                .Setup(s => s.ExportKey(It.IsAny<int>()))
                .ThrowsAsync(new System.Exception("Test exception"));

            // Act
            var result = await _controller.ExportKeyAsync(keyId) as ObjectResult;
            var response = result?.Value as ApiResponse<string>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result!.StatusCode);
            Assert.Equal(ApiResponseStatus.Failure, response?.Status);
            Assert.Equal("An unexpected error occurred.", response?.ErrorMessage);
        }

        #endregion
    }
}
