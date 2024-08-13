using Microsoft.AspNetCore.Mvc;
using service.Core.Dto.AccountManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Enums;
using service.Core.Interfaces.AccountManagement;

namespace service.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IAccountService _accountService = accountService;

        /// <summary>
        /// Adds a new user to the system asynchronously.
        /// </summary>
        /// <param name="inAddUserDto">The DTO containing the details of the user to be added.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with the ID of the newly created user.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPost]
        [Route("AddUserAsync")]
        public async Task<IActionResult> AddUserAsync(InAddUserDto inAddUserDto)
        {
            try
            {
                var (statusCode, userId) = await _accountService.AddNewUser(inAddUserDto);

                if (userId > 0)
                {
                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        statusCode,
                        successMessage: "User added successfully.",
                        txn: ConstantData.Txn(),
                        returnValue: userId
                    );

                    return Ok(response);
                }
                else
                {
                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        statusCode,
                        errorMessage: "Failed to add user.",
                        errorCode: ErrorCode.AddUserFailedError,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new user: {Message}", ex.Message);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    0,
                    errorMessage: "An unexpected error occurred while adding the user.",
                    errorCode: ErrorCode.InternalServerError,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

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

            var (status, users) = await _accountService.GetAllUsers();

            ApiResponse<List<User>> response;
            switch (status)
            {
                case 1:
                    _logger.LogInformation("Users retrieved successfully.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Success, StatusCodes.Status200OK, 1, successMessage: "Users retrieved successfully.", txn: ConstantData.Txn(), returnValue: users);
                    break;
                case 0:
                    _logger.LogWarning("No users found.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status404NotFound, 0, errorMessage: "No users found", errorCode: ErrorCode.NoUsersError, txn: ConstantData.Txn());
                    break;
                case -1:
                    _logger.LogError("An error occurred while retrieving users.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status500InternalServerError, -1, errorMessage: "An error occurred while retrieving users.", errorCode: ErrorCode.GetAllUsersError, txn: ConstantData.Txn());
                    break;
                default:
                    _logger.LogError("Unknown status.");
                    response = new ApiResponse<List<User>>(ApiResponseStatus.Failure, StatusCodes.Status500InternalServerError, -1, errorMessage: "Unknown status", errorCode: ErrorCode.UnknownError, txn: ConstantData.Txn());
                    break;
            }

            return StatusCode(response.StatusCode, response);
        }
    }
}
