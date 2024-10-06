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
    [Authorize]
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
        /// Getting All Data from of soft deleted Users Table.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResponse<List<User>>), 200)]
        [HttpGet]
        [Route("GetDeletedUsersAsync")]
        public async Task<IActionResult> GetDeletedUsersAsync()
        {
            _logger.LogInformation("Fetching all soft deleted users.");

            var (status, users) = await _userManagementService.GetSoftDeletedUsers();

            ApiResponse<List<User>> response;
            switch (status)
            {
                case 1:
                    _logger.LogInformation("Soft deleted users retrieved successfully.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Success, StatusCodes.Status200OK, 1, successMessage: "Soft deleted users retrieved successfully.", txn: ConstantData.Txn(), returnValue: users);
                    break;
                case 0:
                    _logger.LogWarning("No soft deleted users found.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status200OK, 0, errorMessage: "No soft deleted users found", errorCode: ErrorCode.UserManagementNoUsersError, txn: ConstantData.Txn());
                    break;
                case -1:
                    _logger.LogError("An error occurred while retrieving soft deleted users.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status500InternalServerError, -1, errorMessage: "An error occurred while retrieving soft deleted users.", errorCode: ErrorCode.GetDeletedUsersError, txn: ConstantData.Txn());
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
                else if (statusCode > 1)
                {
                    _logger.LogError("Failed to create user: {UserEmail} because {Message}", inCreateUser.Email, message);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.GetUserDetailsMailUsernameAsyncDuplicateError,
                        txn: ConstantData.Txn()
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

        /// <summary>
        /// Locks or unlocks a user based on their ID.
        /// </summary>
        /// <param name="id">The ID of the user to lock or unlock.</param>
        /// <returns>
        /// An API response indicating the success or failure of the operation.
        /// If successful, the response contains the status code 200 and the updated status of the user.
        /// If the user is not found, the response contains the status code 404 and an error message.
        /// If an exception occurs, the response contains the status code 400 or 500 and an error message.
        /// </returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPut]
        [Route("LockUnlockUserAsync/{id}")]
        public async Task<IActionResult> LockUnlockUserAsync(int id)
        {
            try
            {
                var (statusCode, message) = await _userManagementService.LockUnlockUser(id);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User {Id} successfully locked/unlocked.", id);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: message,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.LockUnlockUserAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else
                {
                    _logger.LogError("Exception occurred while locking/unlocking user: {Id}", id);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred while locking/unlocking user.",
                        errorCode: ErrorCode.LockUnlockUserAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while locking/unlocking user: {Id}", id);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.LockUnlockUserAsyncUnhandledException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Soft deletes a user with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to soft delete.</param>
        /// <returns>An API response indicating the result of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPatch]
        [Route("SoftDeleteUserAsync/{id}")]
        public async Task<IActionResult> SoftDeleteUserAsync(int id)
        {
            try
            {
                var (statusCode, message) = await _userManagementService.SoftDeleteUser(id);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User {Id} successfully soft deleted.", id);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: message,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.SoftDeleteUserAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode > 1)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.SoftDeleteUserAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else
                {
                    _logger.LogError("Exception occurred while soft deleting user: {Id}", id);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred while soft deleting user.",
                        errorCode: ErrorCode.SoftDeleteUserAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while soft deletion of user: {Id}", id);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.SoftDeleteUserAsyncUnhandledException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Updates user details asynchronously.
        /// </summary>
        /// <param name="inUpdateUserDetails">The input object containing the updated user details.</param>
        /// <returns>An HTTP response indicating the outcome of the update operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPut]
        [Route("UpdateUserDetailsAsync")]
        public async Task<IActionResult> UpdateUserDetailsAsync(InUpdateUserDetails inUpdateUserDetails)
        {
            try
            {
                var (statusCode, message) = await _userManagementService.UpdateUserDetails(inUpdateUserDetails);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User details update successfully.");
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: message,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.UpdateUserDetailsAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode > 1)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.UpdateUserDetailsAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else
                {
                    _logger.LogError("Exception occurred while updating user details.");

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred while updating user details.",
                        errorCode: ErrorCode.UpdateUserDetailsAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while updating user details.");

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.UpdateUserDetailsAsyncUnhandledException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Restores a soft-deleted user based on the provided ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user to restore.</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPatch]
        [Route("RestoreSoftDeleteUserAsync/{id}")]
        public async Task<IActionResult> RestoreSoftDeleteUserAsync(int id)
        {
            try
            {
                var (statusCode, message) = await _userManagementService.RestoreUserDetails(id);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User {Id} successfully restore soft deleted user.", id);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: message,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.RestoreSoftDeletedUsersAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode > 1)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.RestoreSoftDeletedUsersAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else
                {
                    _logger.LogError("Exception occurred while restore soft deleted user: {Id}", id);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred whilerestore soft deleted user.",
                        errorCode: ErrorCode.RestoreSoftDeletedUserAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while restore soft deleted user: {Id}", id);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.RestoreSoftDeletedUserAsyncUnhandledException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Hard deletes a user from the system.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>An API response indicating the success or failure of the operation.</returns>
        /// <remarks>
        /// This endpoint is used to permanently remove a user from the system.
        /// The operation is irreversible and should be used with caution.
        /// </remarks>
        [ProducesResponseType(typeof(ApiResponse<int> ), 200)]
        [HttpDelete]
        [Route("HardDeleteUserAsync/{id}")]
        public async Task<IActionResult> HardDeleteUserAsync(int id)
        {
            try
            {
                var (statusCode, message) = await _userManagementService.HardDeleteUser(id);

                if (statusCode == 1)
                {
                    _logger.LogInformation("User {Id} successfully hard deleted.", id);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: message,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode == 0)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.HardDeleteUserAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else if (statusCode > 1)
                {
                    _logger.LogError("{Message}", message);
                    return Ok(new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        statusCode,
                        errorMessage: message,
                        errorCode: ErrorCode.HardDeleteUserAsyncError,
                        txn: ConstantData.Txn()
                    ));
                }
                else
                {
                    _logger.LogError("Exception occurred while hard deleting user: {Id}", id);

                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Exception occurred while hard deleting user.",
                        errorCode: ErrorCode.HardDeleteUserAsyncException,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while hard deletion of user: {Id}", id);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.HardDeleteUserAsyncException,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
