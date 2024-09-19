using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Core.Dto.UserManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Enums;
using service.Core.Interfaces.UserManagement;

namespace service.Controllers
{
    /// <summary>
    /// API controller for user management operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController(ILogger<UserManagementController> logger, IUserManagementService userManagementService) : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger = logger;
        private readonly IUserManagementService _userManagementService = userManagementService;

        /// <summary>
        /// Getting All Data from Users Table.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<List<User>>), 200)]
        [HttpGet]
        [Route("GetUsersAsync")]
        public async Task<IActionResult> GetUsersAsync()
        {
            _logger.LogInformation("Fetching all users.");

            var (status, users) = await _userManagementService.GetAllUsers();

            ApiResponse<List<User>> response;
            switch (status)
            {
                case 1:
                    _logger.LogInformation("Users retrieved successfully.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Success, StatusCodes.Status200OK, 1, successMessage: "Users retrieved successfully.", txn: ConstantData.Txn(), returnValue: users);
                    break;
                case 0:
                    _logger.LogWarning("No users found.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status200OK, 0, errorMessage: "No users found", errorCode: ErrorCode.UserManagementNoUsersError, txn: ConstantData.Txn());
                    break;
                case -1:
                    _logger.LogError("An error occurred while retrieving users.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status500InternalServerError, -1, errorMessage: "An error occurred while retrieving users.", errorCode: ErrorCode.GetUsersError, txn: ConstantData.Txn());
                    break;
                default:
                    _logger.LogError("Unknown status.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status500InternalServerError, -1, errorMessage: "Unknown status", errorCode: ErrorCode.UserManagementUnknownError, txn: ConstantData.Txn());
                    break;
            }

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="inCreateUser">The user information to create.</param>
        /// <returns>The ID of the newly created user.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [HttpPost]
        [Route("CreateNewUserAsync")]
        public async Task<IActionResult> CreateNewUserAsync(InCreateUser inCreateUser)
        {
            try
            {
                _logger.LogInformation("Starting CreateNewUserAsync for user: {UserEmail}", inCreateUser.Email);

                var (statusCode, message) = await _userManagementService.CreateNewUser(inCreateUser);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User created successfully: {UserEmail}", inCreateUser.Email);

                    var response = new ApiResponse<string>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: "User created successfully.",
                        txn: ConstantData.Txn(),
                        returnValue: message
                    );

                    return Ok(response);
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("Failed to create user: {UserEmail}", inCreateUser.Email);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status406NotAcceptable,
                        statusCode,
                        errorMessage: "Failed to create user.",
                        errorCode: ErrorCode.CreateNewUserAsyncError,
                        txn: ConstantData.Txn()
                    );

                    return Ok(response);
                }
                else
                {
                    _logger.LogError("Exception occurred while creating user: {UserEmail}", inCreateUser.Email);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred while creating user.",
                        errorCode: ErrorCode.CreateNewUserAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while creating user: {UserEmail}", inCreateUser.Email);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.CreateNewUserAsyncUnhandledException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
