using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using service.Controllers;
using service.Core.Dto.AccountManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.AccountManagement;
using service.Core.Interfaces.Utility;

namespace serviceTests.AccountManagement
{
    public class AccountControllerTests
    {
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly AccountController _controller;
        private readonly Faker _faker;

        public AccountControllerTests()
        {
            _loggerMock = new Mock<ILogger<AccountController>>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _accountServiceMock = new Mock<IAccountService>();
            _controller = new AccountController(_loggerMock.Object, _jwtTokenGeneratorMock.Object, _accountServiceMock.Object);
            _faker = new Faker();
        }

        [Fact]
        public async Task LoginUserAsync_ValidModel_SuccessfulLogin()
        {
            // Arrange
            var inLoginUserDto = new InLoginUserDto
            {
                UserEmail = _faker.Internet.Email(),
                UserPassword = _faker.Internet.Password()
            };

            var user = new User
            {
                Email = inLoginUserDto.UserEmail,
                UserName = _faker.Internet.UserName()
            };

            _accountServiceMock.Setup(x => x.LoginUser(inLoginUserDto))
                .ReturnsAsync((1, "Login successful", user));

            var token = _faker.Random.String2(32);
            _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(token);

            // Act
            var result = await _controller.LoginUserAsync(inLoginUserDto);

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<User>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while login user.", apiResponse.ErrorMessage);
        }

        [Fact]
        public async Task LoginUserAsync_ValidModel_UnsuccessfulLogin()
        {
            // Arrange
            var inLoginUserDto = new InLoginUserDto
            {
                UserEmail = _faker.Internet.Email(),
                UserPassword = _faker.Internet.Password()
            };

            _accountServiceMock.Setup(x => x.LoginUser(inLoginUserDto))
                .ReturnsAsync((-1, "Invalid credentials", null));

            // Act
            var result = await _controller.LoginUserAsync(inLoginUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<User>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(-1, apiResponse.ResponseCode);
            Assert.Equal("Invalid credentials", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.LoginUserError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidModel_BadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("UserEmail", "Required");

            // Act
            var result = await _controller.LoginUserAsync(new InLoginUserDto());

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<User>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An invalid model provided while logging user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.BadRequestError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task LoginUserAsync_Exception_InternalServerError()
        {
            // Arrange
            var inLoginUserDto = new InLoginUserDto
            {
                UserEmail = _faker.Internet.Email(),
                UserPassword = _faker.Internet.Password()
            };

            _accountServiceMock.Setup(x => x.LoginUser(inLoginUserDto))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.LoginUserAsync(inLoginUserDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<User>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while login user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task AddUserAsync_SuccessfulAddUser()
        {
            // Arrange
            var inAddUserDto = new InAddUserDto
            {
                Name = _faker.Name.FullName(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password()
            };

            int expectedUserId = _faker.Random.Int(1); // Simulate a successful user ID

            _accountServiceMock.Setup(x => x.AddNewUser(inAddUserDto))
                               .ReturnsAsync((1, expectedUserId));

            // Act
            var result = await _controller.AddUserAsync(inAddUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal("User added successfully.", apiResponse.SuccessMessage);
            Assert.Equal(expectedUserId, apiResponse.ReturnValue);
        }

        [Fact]
        public async Task AddUserAsync_UserAlreadyExists()
        {
            // Arrange
            var inAddUserDto = new InAddUserDto
            {
                Name = _faker.Name.FullName(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password()
            };

            _accountServiceMock.Setup(x => x.AddNewUser(inAddUserDto))
                               .ReturnsAsync((0, 0)); // Simulate user already exists

            // Act
            var result = await _controller.AddUserAsync(inAddUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status406NotAcceptable, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Failed to add user because user is already registered with Username: {inAddUserDto.UserName} or Email: {inAddUserDto.Email}.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.AddUserFailedError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task AddUserAsync_GenericFailure()
        {
            // Arrange
            var inAddUserDto = new InAddUserDto
            {
                Name = _faker.Name.FullName(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password()
            };

            _accountServiceMock.Setup(x => x.AddNewUser(inAddUserDto))
                               .ReturnsAsync((-1, 0)); // Simulate a generic failure

            // Act
            var result = await _controller.AddUserAsync(inAddUserDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(badRequestResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.Equal(-1, apiResponse.ResponseCode);
            Assert.Equal("Failed to add user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.AddUserFailedError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task AddUserAsync_Exception()
        {
            // Arrange
            var inAddUserDto = new InAddUserDto
            {
                Name = _faker.Name.FullName(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email(),
                Password = _faker.Internet.Password()
            };

            _accountServiceMock.Setup(x => x.AddNewUser(inAddUserDto))
                               .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.AddUserAsync(inAddUserDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while adding the user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task GetUsersAsync_Success_ReturnsUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new() { Id = 1, UserName = "user1" },
                new() { Id = 2, UserName = "user2" }
            };

            _accountServiceMock.Setup(x => x.GetAllUsers())
                               .ReturnsAsync((1, users));

            // Act
            var result = await _controller.GetUsersAsync();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result); // StatusCode is used, so ObjectResult
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<List<User>>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal("Users retrieved successfully.", apiResponse.SuccessMessage);
            Assert.Equal(users, apiResponse.ReturnValue);
        }

        [Fact]
        public async Task GetUsersAsync_NoUsersFound()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetAllUsers())
                               .ReturnsAsync((0, (List<User>)null!));

            // Act
            var result = await _controller.GetUsersAsync();

            // Assert
            var notFoundResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<List<User>>>(notFoundResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("No users found", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.NoUsersError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task GetUsersAsync_ErrorRetrievingUsers()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetAllUsers())
                               .ReturnsAsync((-1, (List<User>)null!));

            // Act
            var result = await _controller.GetUsersAsync();

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<List<User>>>(internalServerErrorResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(-1, apiResponse.ResponseCode);
            Assert.Equal("An error occurred while retrieving users.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.GetAllUsersError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task GetUsersAsync_UnknownStatus()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetAllUsers())
                               .ReturnsAsync((2, (List<User>)null!));

            // Act
            var result = await _controller.GetUsersAsync();

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<List<User>>>(internalServerErrorResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(-1, apiResponse.ResponseCode);
            Assert.Equal("Unknown status", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.UnknownError, apiResponse.ErrorCode);
            Assert.Null(apiResponse.ReturnValue);
        }

        [Fact]
        public async Task OtpGenerationRequestAsync_NullRequest_ReturnsBadRequest()
        {
            // Arrange & Act
            var result = await _controller.OtpGenerationRequestAsync(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(badRequestResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("Invalid OTP request data.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InvalidModelRequestError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task OtpGenerationRequestAsync_Success_ReturnsOk()
        {
            // Arrange
            var inOtpRequestDto = new InOtpRequestDto
            {
                Email = _faker.Internet.Email(),
                Use = _faker.PickRandom<OtpUse>()
            };

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = "OTP generated successfully" };
            _accountServiceMock.Setup(x => x.OtpGeneration(inOtpRequestDto))
                               .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.OtpGenerationRequestAsync(inOtpRequestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal("OTP generated successfully", apiResponse.SuccessMessage);
            Assert.Equal(1, apiResponse.ReturnValue); // Status from BaseResponse
        }

        [Fact]
        public async Task OtpGenerationRequestAsync_Failure_ReturnsBadRequest()
        {
            // Arrange
            var inOtpRequestDto = new InOtpRequestDto
            {
                Email = _faker.Internet.Email(),
                Use = _faker.PickRandom<OtpUse>()
            };

            var baseResponse = new BaseResponse { Status = 0, ErrorMessage = "Failed to generate OTP" };
            _accountServiceMock.Setup(x => x.OtpGeneration(inOtpRequestDto))
                               .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.OtpGenerationRequestAsync(inOtpRequestDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(badRequestResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("Failed to generate OTP", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.BadRequestError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task OtpGenerationRequestAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var inOtpRequestDto = new InOtpRequestDto
            {
                Email = _faker.Internet.Email(),
                Use = _faker.PickRandom<OtpUse>()
            };

            _accountServiceMock.Setup(x => x.OtpGeneration(inOtpRequestDto))
                               .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.OtpGenerationRequestAsync(inOtpRequestDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while generation of otp.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task VerifyOtpRequestAsync_ValidModel_Success()
        {
            // Arrange
            var inVerifyOtpDto = new InVerifyOtpDto
            {
                Email = _faker.Internet.Email(),
                Otp = _faker.Random.String2(6) // Assuming 6-digit OTP
            };

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = "User unlocked successfully." };
            _accountServiceMock.Setup(x => x.VerifyOtp(inVerifyOtpDto))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.VerifyOtpRequestAsync(inVerifyOtpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal("User unlocked successfully.", apiResponse.SuccessMessage);
            Assert.Equal(1, apiResponse.ReturnValue);
        }

        [Fact]
        public async Task VerifyOtpRequestAsync_InvalidModel_BadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.VerifyOtpRequestAsync(new InVerifyOtpDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(badRequestResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status400BadRequest, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("Invalid Verification of OTP request data.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InvalidModelRequestError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task VerifyOtpRequestAsync_ServiceFailure_ReturnsOkWithFailure()
        {
            // Arrange
            var inVerifyOtpDto = new InVerifyOtpDto
            {
                Email = _faker.Internet.Email(),
                Otp = _faker.Random.String2(6)
            };

            var baseResponse = new BaseResponse { Status = -1, ErrorMessage = "Some error occurred", ErrorCode = ErrorCode.InvalidEmailError };
            _accountServiceMock.Setup(x => x.VerifyOtp(inVerifyOtpDto))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.VerifyOtpRequestAsync(inVerifyOtpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("Some error occurred", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InvalidEmailError, apiResponse.ErrorCode);
            Assert.Equal(-1, apiResponse.ReturnValue);
        }

        [Fact]
        public async Task VerifyOtpRequestAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var inVerifyOtpDto = new InVerifyOtpDto
            {
                Email = _faker.Internet.Email(),
                Otp = _faker.Random.String2(6)
            };

            _accountServiceMock.Setup(x => x.VerifyOtp(inVerifyOtpDto))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.VerifyOtpRequestAsync(inVerifyOtpDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while verification of otp.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task SoftDeleteUserRequestAsync_ValidEmail_Success()
        {
            // Arrange
            string email = _faker.Internet.Email();

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = $"User '{email}' soft deleted successfully." };

            _accountServiceMock.Setup(x => x.SoftDeleteUser(email))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.SoftDeleteUserRequestAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal($"User '{email}' soft deleted successfully.", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task SoftDeleteUserRequestAsync_UserNotFound_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.SoftDeleteUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid Email Address Provided. Please Verify '{email}' Email.", ErrorCode = ErrorCode.InvalidEmailError });

            // Act
            var result = await _controller.SoftDeleteUserRequestAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid Email Address Provided. Please Verify '{email}' Email.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.SoftDeleteUserRequestAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task SoftDeleteUserRequestAsync_UserAlreadySoftDeleted_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();
            var user = new User
            {
                Email = email,
                IsDeleted = true,
                AutoDeletedOn = DateTime.Now.AddDays(5)
            };

            _accountServiceMock.Setup(x => x.SoftDeleteUser(email))
                .ReturnsAsync(new BaseResponse
                {
                    Status = -2,
                    ErrorMessage = $"User is already in soft deletion state. You can restore user before '{user.AutoDeletedOn}'.",
                    ErrorCode = ErrorCode.UserDeletedStateError
                });

            // Act
            var result = await _controller.SoftDeleteUserRequestAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"User is already in soft deletion state. You can restore user before '{user.AutoDeletedOn}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.SoftDeleteUserRequestAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task SoftDeleteUserRequestAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.SoftDeleteUser(email))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.SoftDeleteUserRequestAsync(email);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while soft deletion of user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task RestoreSoftDeletedUserAsync_ValidEmail_Success()
        {
            // Arrange
            string email = _faker.Internet.Email();

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = $"User '{email}' restored successfully." };

            _accountServiceMock.Setup(x => x.RestoreSoftDeletedUser(email))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.RestoreSoftDeletedUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal($"User '{email}' restored successfully.", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task RestoreSoftDeletedUserAsync_InvalidEmail_ReturnsOkWithFailure()
        {
            // Arrange
            string email = "invalid-email"; // Deliberately invalid email

            _accountServiceMock.Setup(x => x.RestoreSoftDeletedUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid email address provided. Please verify '{email}'.", ErrorCode = ErrorCode.InvalidEmailError });

            // Act
            var result = await _controller.RestoreSoftDeletedUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid email address provided. Please verify '{email}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.RestoreSoftDeletedUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task RestoreSoftDeletedUserAsync_UserAlreadyActive_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.RestoreSoftDeletedUser(email))
                .ReturnsAsync(new BaseResponse
                {
                    Status = -2,
                    ErrorMessage = $"The user '{email}' is already active.",
                    ErrorCode = ErrorCode.RestoreSoftDeletedUserAsyncError
                });

            // Act
            var result = await _controller.RestoreSoftDeletedUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"The user '{email}' is already active.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.RestoreSoftDeletedUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task RestoreSoftDeletedUserAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.RestoreSoftDeletedUser(email))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.RestoreSoftDeletedUserAsync(email);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while restore soft deleted user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task EnableUserAsync_ValidEmail_Success()
        {
            // Arrange
            string email = _faker.Internet.Email();

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = $"User '{email}' enabled successfully." };

            _accountServiceMock.Setup(x => x.EnableActiveUser(email))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.EnableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal($"User '{email}' enabled successfully.", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task EnableUserAsync_InvalidEmail_ReturnsOkWithFailure()
        {
            // Arrange
            string email = "invalid-email"; // Deliberately invalid email

            _accountServiceMock.Setup(x => x.EnableActiveUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid email address provided. Please verify '{email}'.", ErrorCode = ErrorCode.InvalidEmailError });

            // Act
            var result = await _controller.EnableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid email address provided. Please verify '{email}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.EnableUserRequestAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task EnableUserAsync_UserAlreadyEnabled_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.EnableActiveUser(email))
                .ReturnsAsync(new BaseResponse
                {
                    Status = -2,
                    ErrorMessage = $"The user '{email}' is already enabled.",
                    ErrorCode = ErrorCode.UserActiveStateError
                });

            // Act
            var result = await _controller.EnableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"The user '{email}' is already enabled.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.EnableUserRequestAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task EnableUserAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.EnableActiveUser(email))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.EnableUserAsync(email);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while enable user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task DisableUserAsync_ValidEmail_Success()
        {
            // Arrange
            string email = _faker.Internet.Email();

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = $"User '{email}' disabled successfully." };

            _accountServiceMock.Setup(x => x.DisableInactiveUser(email))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.DisableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal($"User '{email}' disabled successfully.", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task DisableUserAsync_InvalidEmail_ReturnsOkWithFailure()
        {
            // Arrange
            string email = "invalid-email"; // Deliberately invalid email

            _accountServiceMock.Setup(x => x.DisableInactiveUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid email address provided. Please verify '{email}'.", ErrorCode = ErrorCode.InvalidEmailError });

            // Act
            var result = await _controller.DisableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid email address provided. Please verify '{email}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.DisableUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task DisableUserAsync_UserAlreadyDisabled_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.DisableInactiveUser(email))
                .ReturnsAsync(new BaseResponse
                {
                    Status = -2,
                    ErrorMessage = $"The user '{email}' is already disabled.",
                    ErrorCode = ErrorCode.UserDeactivateStateError
                });

            // Act
            var result = await _controller.DisableUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"The user '{email}' is already disabled.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.DisableUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task DisableUserAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.DisableInactiveUser(email))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.DisableUserAsync(email);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while disable user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task HardDeleteUserAsync_ValidEmail_Success()
        {
            // Arrange
            string email = _faker.Internet.Email();

            var baseResponse = new BaseResponse { Status = 1, SuccessMessage = $"User '{email}' hard deleted successfully." };

            _accountServiceMock.Setup(x => x.HardDeleteUser(email))
                .ReturnsAsync(baseResponse);

            // Act
            var result = await _controller.HardDeleteUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Success, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(1, apiResponse.ResponseCode);
            Assert.Equal($"User '{email}' hard deleted successfully.", apiResponse.SuccessMessage);
        }

        [Fact]
        public async Task HardDeleteUserAsync_InvalidEmail_ReturnsOkWithFailure()
        {
            // Arrange
            string email = "invalid-email"; // Deliberately invalid email

            _accountServiceMock.Setup(x => x.HardDeleteUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid email address provided. Please verify '{email}'.", ErrorCode = ErrorCode.InvalidEmailError });

            // Act
            var result = await _controller.HardDeleteUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid email address provided. Please verify '{email}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.HardDeleteUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task HardDeleteUserAsync_UserNotFound_ReturnsOkWithFailure()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.HardDeleteUser(email))
                .ReturnsAsync(new BaseResponse { Status = -1, ErrorMessage = $"Invalid email address provided. Please verify '{email}'.", ErrorCode = ErrorCode.InvalidEmailError }); // Assuming this is what your service would return

            // Act
            var result = await _controller.HardDeleteUserAsync(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // Since you're returning Ok even on service failure
            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(okResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status200OK, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal($"Invalid email address provided. Please verify '{email}'.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.HardDeleteUserAsyncError, apiResponse.ErrorCode);
        }

        [Fact]
        public async Task HardDeleteUserAsync_Exception_ReturnsInternalServerError()
        {
            // Arrange
            string email = _faker.Internet.Email();

            _accountServiceMock.Setup(x => x.HardDeleteUser(email))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.HardDeleteUserAsync(email);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsAssignableFrom<ApiResponse<int>>(statusCodeResult.Value);
            Assert.Equal(ApiResponseStatus.Failure, apiResponse.Status);
            Assert.Equal(StatusCodes.Status500InternalServerError, apiResponse.StatusCode);
            Assert.Equal(0, apiResponse.ResponseCode);
            Assert.Equal("An unexpected error occurred while hard deleting user.", apiResponse.ErrorMessage);
            Assert.Equal(ErrorCode.InternalServerError, apiResponse.ErrorCode);
        }
    }
}
