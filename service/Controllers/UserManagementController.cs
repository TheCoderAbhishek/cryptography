using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}
