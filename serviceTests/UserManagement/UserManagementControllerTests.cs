using Bogus;
using Microsoft.Extensions.Logging;
using Moq;
using service.Application.Service.UserManagement;
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
        private readonly Faker<InUpdateUserDetails> _inUpdateUserDetailsFaker;
        private readonly Faker<User> _userFaker;

        public UserManagementControllerTests()
        {
            _userManagementRepositoryMock = new Mock<IUserManagementRepository>();
            _loggerMock = new Mock<ILogger<UserManagementService>>();
            _userManagementService = new UserManagementService(_loggerMock.Object, _userManagementRepositoryMock.Object);
            _inUpdateUserDetailsFaker = new Faker<InUpdateUserDetails>()
                .RuleFor(u => u.Id, f => f.Random.Int(1, 100))
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, f => f.Internet.UserName());

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Int(1, 100))
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.IsDeleted, true);
        }

        #region GetAllUsers Tests
        [Fact]
        public async Task GetAllUsers_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new() { UserId = "1", Name = "John" } };
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
            var users = new List<User> { new() { UserId = "1", Name = "John", IsDeleted = true } };
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

        #region UpdateUserDetails Tests

        [Fact]
        public async Task UpdateUserDetails_ShouldUpdateSuccessfully()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsMailUsernameExceptCurrentIdAsync(inUpdateUserDetails.Id, inUpdateUserDetails.Email!, inUpdateUserDetails.UserName!))
                .ReturnsAsync(0); // No duplicate
            _userManagementRepositoryMock
                .Setup(repo => repo.UpdateUserDetailsAsync(inUpdateUserDetails))
                .ReturnsAsync(1); // Successful update

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("User details updated successfully.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnEmailAlreadyExists()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsMailUsernameExceptCurrentIdAsync(inUpdateUserDetails.Id, inUpdateUserDetails.Email!, inUpdateUserDetails.UserName!))
                .ReturnsAsync(1); // Duplicate email

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(2, result.Item1);
            Assert.Equal("Email address already exists.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnUsernameAlreadyExists()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsMailUsernameExceptCurrentIdAsync(inUpdateUserDetails.Id, inUpdateUserDetails.Email!, inUpdateUserDetails.UserName!))
                .ReturnsAsync(2); // Duplicate username

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(3, result.Item1);
            Assert.Equal("Username already exists.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnUsernameAndEmailAlreadyExists()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsMailUsernameExceptCurrentIdAsync(inUpdateUserDetails.Id, inUpdateUserDetails.Email!, inUpdateUserDetails.UserName!))
                .ReturnsAsync(3); // Duplicate email and username

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(4, result.Item1);
            Assert.Equal("Username and email already exists.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnErrorWhenUserNotFound()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(4, result.Item1);
            Assert.Equal("User not found.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldReturnErrorOnUpdateFailure()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.UpdateUserDetailsAsync(inUpdateUserDetails))
                .ReturnsAsync(0); // Update failed

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Equal("Error occurred while updating user details.", result.Item2);
        }

        [Fact]
        public async Task UpdateUserDetails_ShouldHandleException()
        {
            // Arrange
            var inUpdateUserDetails = _inUpdateUserDetailsFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(inUpdateUserDetails.Id))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Contains("Error", result.Item2);
        }

        #endregion

        #region HardDeleteUser Tests

        [Fact]
        public async Task HardDeleteUser_ShouldDeleteSuccessfully()
        {
            // Arrange
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.HardDeleteUserAsync(user.Id))
                .ReturnsAsync(1); // Successful deletion

            // Act
            var result = await _userManagementService.HardDeleteUser(user.Id);

            // Assert
            Assert.Equal(1, result.Item1);
            Assert.Equal("User deleted successfully.", result.Item2);
        }

        [Fact]
        public async Task HardDeleteUser_ShouldReturnUserNotFound()
        {
            // Arrange
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _userManagementService.HardDeleteUser(1);

            // Assert
            Assert.Equal(2, result.Item1);
            Assert.Equal("User not found.", result.Item2);
        }

        [Fact]
        public async Task HardDeleteUser_ShouldReturnErrorOnDeleteFailure()
        {
            // Arrange
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ReturnsAsync(user);
            _userManagementRepositoryMock
                .Setup(repo => repo.HardDeleteUserAsync(user.Id))
                .ReturnsAsync(0); // Deletion failed

            // Act
            var result = await _userManagementService.HardDeleteUser(user.Id);

            // Assert
            Assert.Equal(0, result.Item1);
            Assert.Equal("Error occurred while deleting user.", result.Item2);
        }

        [Fact]
        public async Task HardDeleteUser_ShouldHandleException()
        {
            // Arrange
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _userManagementService.HardDeleteUser(1);

            // Assert
            Assert.Equal(-1, result.Item1);
            Assert.Contains("Error", result.Item2);
        }

        #endregion

        #region RestoreUserDetails Tests
        [Fact]
        public async Task RestoreUserDetails_UserRestoredSuccessfully_ReturnsSuccess()
        {
            // Arrange
            var user = _userFaker.Generate();
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userManagementRepositoryMock
                .Setup(repo => repo.RestoreSoftDeletedUserAsync(user.Id))
                .ReturnsAsync(1); // Success indicator

            // Act
            var result = await _userManagementService.RestoreUserDetails(user.Id);

            // Assert
            Assert.Equal(1, result.Item1); // Result ID
            Assert.Equal("User successfully restored.", result.Item2);
        }

        [Fact]
        public async Task RestoreUserDetails_UserAlreadyActive_ReturnsFailure()
        {
            // Arrange
            var user = _userFaker.Generate();
            user.IsDeleted = false; // Active user

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Act
            var result = await _userManagementService.RestoreUserDetails(user.Id);

            // Assert
            Assert.Equal(2, result.Item1); // Already active indicator
            Assert.Equal("Failed to restore soft deleted user because user already in in active state.", result.Item2);
        }

        [Fact]
        public async Task RestoreUserDetails_UserNotFound_ReturnsFailure()
        {
            // Arrange
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null!); // User not found

            // Act
            var result = await _userManagementService.RestoreUserDetails(99);

            // Assert
            Assert.Equal(3, result.Item1); // User not found indicator
            Assert.Equal("Failed to retrieve user details from table.", result.Item2);
        }

        [Fact]
        public async Task RestoreUserDetails_RestoreFails_ReturnsFailure()
        {
            // Arrange
            var user = _userFaker.Generate();
            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userManagementRepositoryMock
                .Setup(repo => repo.RestoreSoftDeletedUserAsync(user.Id))
                .ReturnsAsync(0); // Restore fails

            // Act
            var result = await _userManagementService.RestoreUserDetails(user.Id);

            // Assert
            Assert.Equal(0, result.Item1); // Failure indicator
            Assert.Equal("Failed to restore soft deleted user.", result.Item2);
        }

        [Fact]
        public async Task RestoreUserDetails_ThrowsException_ReturnsFailure()
        {
            // Arrange
            var user = _userFaker.Generate();

            _userManagementRepositoryMock
                .Setup(repo => repo.GetUserDetailsByIdAsync(user.Id))
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _userManagementService.RestoreUserDetails(user.Id);

            // Assert
            Assert.Equal(-1, result.Item1); // Exception indicator
            Assert.Contains("Database failure", result.Item2);
        } 
        #endregion
    }
}
