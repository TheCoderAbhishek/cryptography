using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Core.Entities.KeyManagement;
using service.Core.Enums;
using service.Core.Interfaces.KeyManagement;

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
    }
}
