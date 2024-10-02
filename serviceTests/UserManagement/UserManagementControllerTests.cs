using Microsoft.Extensions.Logging;
using Moq;
using service.Application.Repository.UserManagement;
using service.Core.Dto.UserManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Interfaces.UserManagement;

namespace serviceTests.UserManagement
{
    public class UserManagementControllerTests
    {
        private readonly Mock<IUserManagementRepository> _userManagementRepositoryMock;
        private readonly Mock<ILogger<UserManagementService>> _loggerMock;
        private readonly UserManagementService _userManagementService;

        public UserManagementControllerTests()
        {
            _userManagementRepositoryMock = new Mock<IUserManagementRepository>();
            _loggerMock = new Mock<ILogger<UserManagementService>>();
            _userManagementService = new UserManagementService(_loggerMock.Object, _userManagementRepositoryMock.Object);
        }

        #region GetAllUsers Tests
        [Fact]
        public async Task GetAllUsers_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new User { UserId = "1", Name = "John" } };
            _userManagementRepositoryMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userManagementService.GetAllUsers();

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.NotNull(result.Item2);
            Assert.Single(result.Item2);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnEmpty_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();
            _userManagementRepositoryMock.Setup(repo => repo.GetUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userManagementService.GetAllUsers();

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Empty(result.Item2);
        }

        [Fact]
        public async Task GetAllUsers_ShouldHandleException()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.GetUsersAsync()).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.GetAllUsers();

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Null(result.Item2);
        }
        #endregion

        #region GetSoftDeletedUsers Tests
        [Fact]
        public async Task GetSoftDeletedUsers_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new User { UserId = "1", Name = "John", IsDeleted = true } };
            _userManagementRepositoryMock.Setup(repo => repo.GetDeletedUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userManagementService.GetSoftDeletedUsers();

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.NotNull(result.Item2);
            Assert.Single(result.Item2);
        }

        [Fact]
        public async Task GetSoftDeletedUsers_ShouldReturnEmpty_WhenNoDeletedUsersExist()
        {
            // Arrange
            var users = new List<User>();
            _userManagementRepositoryMock.Setup(repo => repo.GetDeletedUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _userManagementService.GetSoftDeletedUsers();

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Empty(result.Item2);
        }

        [Fact]
        public async Task GetSoftDeletedUsers_ShouldHandleException()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.GetDeletedUsersAsync()).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.GetSoftDeletedUsers();

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Null(result.Item2);
        }
        #endregion

        #region CreateNewUser Tests
        [Fact]
        public async Task CreateNewUser_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var inCreateUser = new InCreateUser { Email = "test@example.com", UserName = "testuser", Password = "password" };
            _userManagementRepositoryMock.Setup(repo => repo.GetUserDetailsMailUsernameAsync(inCreateUser.Email, inCreateUser.UserName)).ReturnsAsync(0);
            _userManagementRepositoryMock.Setup(repo => repo.CreateNewUserAsync(It.IsAny<User>())).ReturnsAsync(1);

            // Act
            var result = await _userManagementService.CreateNewUser(inCreateUser);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("New user created successfully.", result.Item2);
        }

        [Fact]
        public async Task CreateNewUser_ShouldReturnDuplicateEmailError()
        {
            // Arrange
            var inCreateUser = new InCreateUser { Email = "test@example.com", UserName = "testuser" };
            _userManagementRepositoryMock.Setup(repo => repo.GetUserDetailsMailUsernameAsync(inCreateUser.Email, inCreateUser.UserName)).ReturnsAsync(1);

            // Act
            var result = await _userManagementService.CreateNewUser(inCreateUser);

            // Assert
            Assert.Equal(2, result.Item1);
            Assert.Equal("Email address already exists.", result.Item2);
        }

        [Fact]
        public async Task CreateNewUser_ShouldHandleException()
        {
            // Arrange
            var inCreateUser = new InCreateUser { Email = "test@example.com", UserName = "testuser" };
            _userManagementRepositoryMock.Setup(repo => repo.CreateNewUserAsync(It.IsAny<User>())).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.CreateNewUser(inCreateUser);

            // Assert
            Assert.Equal(-1, result.Item1);
        }
        #endregion

        #region LockUnlockUser Tests
        [Fact]
        public async Task LockUnlockUser_ShouldReturnSuccess_WhenOperationIsSuccessful()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.LockUnlockUserAsync(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _userManagementService.LockUnlockUser(1);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("User successfully locked/unlocked.", result.Item2);
        }

        [Fact]
        public async Task LockUnlockUser_ShouldReturnFailure_WhenOperationFails()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.LockUnlockUserAsync(It.IsAny<int>())).ReturnsAsync(0);

            // Act
            var result = await _userManagementService.LockUnlockUser(1);

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Equal("Failed to lock/unlock user.", result.Item2);
        }

        [Fact]
        public async Task LockUnlockUser_ShouldHandleException()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.LockUnlockUserAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.LockUnlockUser(1);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Contains("Error", result.Item2);
        }
        #endregion

        #region SoftDeleteUser Tests
        [Fact]
        public async Task SoftDeleteUser_ShouldReturnSuccess_WhenUserIsSoftDeleted()
        {
            // Arrange
            var user = new User { UserId = "1", IsDeleted = false };
            _userManagementRepositoryMock.Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _userManagementRepositoryMock.Setup(repo => repo.SoftDeleteUserAsync(It.IsAny<int>())).ReturnsAsync(1);

            // Act
            var result = await _userManagementService.SoftDeleteUser(1);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("User successfully soft deleted.", result.Item2);
        }

        [Fact]
        public async Task SoftDeleteUser_ShouldReturnAlreadyDeleted_WhenUserIsAlreadySoftDeleted()
        {
            // Arrange
            var user = new User { UserId = "1", IsDeleted = true };
            _userManagementRepositoryMock.Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>())).ReturnsAsync(user);

            // Act
            var result = await _userManagementService.SoftDeleteUser(1);

            // Assert
            Assert.Equal(2, result.Item1);
            Assert.Equal("Failed to soft delete user because user already in soft deleted state.", result.Item2);
        }

        [Fact]
        public async Task SoftDeleteUser_ShouldHandleException()
        {
            // Arrange
            _userManagementRepositoryMock.Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.SoftDeleteUser(1);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Contains("Error", result.Item2);
        }
        #endregion
    }
}
