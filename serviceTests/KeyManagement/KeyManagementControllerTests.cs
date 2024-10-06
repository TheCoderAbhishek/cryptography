using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using service.Controllers;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;

namespace serviceTests.KeyManagement
{
    public class KeyManagementControllerTests
    {
        private readonly Mock<IKeyManagementService> _keyManagementServiceMock;
        private readonly Mock<ILogger<KeyManagementController>> _loggerMock;
        private readonly KeyManagementController _controller;

        public KeyManagementControllerTests()
        {
            _keyManagementServiceMock = new Mock<IKeyManagementService>();
            _loggerMock = new Mock<ILogger<KeyManagementController>>();
            _controller = new KeyManagementController(_loggerMock.Object, _keyManagementServiceMock.Object);
        }

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
    }
}
