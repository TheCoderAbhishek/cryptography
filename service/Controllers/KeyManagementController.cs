using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Core.Dto.KeyManagement;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;
using System.Security.Claims;

namespace service.Controllers
{
    /// <summary>
    /// Controller for key management operations.
    /// </summary>
    /// <param name="logger">Logger for the controller.</param>
    /// <param name="keyManagementService">Key management service for accessing key-related operations.</param>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class KeyManagementController(ILogger<KeyManagementController> logger, IKeyManagementService keyManagementService) : ControllerBase
    {
        private readonly ILogger<KeyManagementController> _logger = logger;
        private readonly IKeyManagementService _keyManagementService = keyManagementService;

        #region Private Helper Methods for Account Controller

        /// <summary>
        /// Retrieves a list of specific user claims from the current HttpContext.
        /// </summary>
        /// <returns>A list of strings containing the user's email and role claims.</returns>
        private List<string> GetUserClaims()
        {
            var userClaims = HttpContext.User.Claims;

            // Extract specific claims and add them to a list using proper ClaimTypes
            var claimsList = new List<string>
            {
                userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!,  // Email
                userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!  // Username
            };

            return claimsList;
        }

        #endregion

        /// <summary>
        /// Gets a list of keys from the key management service.
        /// </summary>
        /// <returns>A list of keys or an error response if an error occurs.</returns>
        [ProducesResponseType(typeof(ApiResponse<List<Keys>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("GetKeysListAsync")]
        public async Task<IActionResult> GetKeysListAsync()
        {
            try
            {
                var txn = ConstantData.Txn();
                _logger.LogInformation("Fetching keys from the table with TXN: {Txn}", txn);

                var (status, keysList) = await _keyManagementService.GetKeysList();

                // Handle service-level error (exception)
                if (status == -1)
                {
                    _logger.LogError("Error occurred in KeyManagementService.GetKeysList with TXN: {Txn}", txn);

                    var errorResponse = new ApiResponse<string>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status500InternalServerError,
                        responseCode: -1,
                        errorMessage: "Error occurred while retrieving keys.",
                        errorCode: ErrorCode.GetKeysListAsyncUnhandledException,
                        txn: txn
                    );
                    return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
                }
                // Handle case when no keys are found
                else if (status == 0)
                {
                    _logger.LogWarning("No keys found with TXN: {Txn}", txn);

                    var notFoundResponse = new ApiResponse<string>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status404NotFound,
                        responseCode: 0,
                        errorMessage: "No keys found.",
                        errorCode: ErrorCode.GetKeysListAsyncNoKeysFound,
                        txn: txn
                    );
                    return NotFound(notFoundResponse);
                }
                // Keys retrieved successfully
                else
                {
                    _logger.LogInformation("{Count} keys retrieved successfully with TXN: {Txn}", keysList.Count, txn);

                    var successResponse = new ApiResponse<List<Keys>>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        responseCode: 1,
                        returnValue: keysList,
                        txn: txn
                    );
                    return Ok(successResponse);
                }
            }
            catch (Exception ex)
            {
                var txn = ConstantData.Txn();
                _logger.LogCritical(ex, "Unhandled exception occurred while fetching keys list at TXN: {Txn}", txn);

                var response = new ApiResponse<string>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.GetKeysListAsyncUnhandledException,
                    txn: txn
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Creates a new key.
        /// </summary>
        /// <param name="inCreateKeyDto">The input data for creating the key.</param>
        /// <returns>The response indicating the success or failure of the operation.</returns>
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        [Route("CreateKeyAsync")]
        public async Task<IActionResult> CreateKeyAsync(InCreateKeyDto inCreateKeyDto)
        {
            var txn = ConstantData.Txn();
            try
            {
                // Use the private method to extract claims
                var claimsList = GetUserClaims();

                var (status, message) = await _keyManagementService.CreateKey(inCreateKeyDto, claimsList[0]);

                if (status == 1)
                {
                    _logger.LogInformation("Key created with name: {KeyName} at TXN: {Txn}", inCreateKeyDto.KeyName, txn);

                    var response = new ApiResponse<string>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        responseCode: status,
                        successMessage: message,
                        txn: txn);
                    return Ok(response);
                }
                else if (status == 2)
                {
                    _logger.LogError("Error occurred while creating key with name: {KeyName} due to duplicate key name in table at TXN: {Txn}", inCreateKeyDto.KeyName, txn);

                    var response = new ApiResponse<string>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        responseCode: status,
                        errorMessage: message,
                        errorCode: ErrorCode.CreateKeyAsyncError,
                        txn: txn);
                    return Ok(response);
                }
                else if (status == -2)
                {
                    _logger.LogError("Error occurred while creating key with name: {KeyName} due to '{Message}' at TXN: {Txn}", inCreateKeyDto.KeyName, message, txn);

                    var response = new ApiResponse<string>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status302Found,
                        responseCode: status,
                        errorMessage: message,
                        errorCode: ErrorCode.CreateKeyAsyncError,
                        txn: txn);
                    return Ok(response);
                }
                else
                {
                    _logger.LogError("Error occurred while creating key with name: {KeyName} at TXN: {Txn}", inCreateKeyDto.KeyName, txn);

                    var response = new ApiResponse<string>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        responseCode: status,
                        errorMessage: message,
                        errorCode: ErrorCode.CreateKeyAsyncError,
                        txn: txn);
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception occurred while creating key with name: {KeyName} at TXN: {Txn}", inCreateKeyDto.KeyName, txn);

                var response = new ApiResponse<string>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    responseCode: -1,
                    errorMessage: "An unexpected error occurred.",
                    errorCode: ErrorCode.CreateKeyAsyncUnhandledException,
                    txn: txn
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
