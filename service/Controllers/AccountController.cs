using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service.Core.Dto.AccountManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.AccountManagement;
using service.Core.Interfaces.Utility;
using System.IdentityModel.Tokens.Jwt;

namespace service.Controllers
{
    /// <summary>
    /// Controller class for handling account management related operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(ILogger<AccountController> logger, IJwtTokenGenerator jwtTokenGenerator, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IAccountService _accountService = accountService;

        #region Private Helper Methods for Account Controller
        /// <summary>
        /// Private method to extract claims from a JWT token.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <returns>A dictionary containing claim types and their values.</returns>
        private static Dictionary<string, string> ExtractClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var claims = new Dictionary<string, string>();
            foreach (var claim in jsonToken.Claims)
            {
                claims[claim.Type] = claim.Value;
            }

            return claims;
        }
        #endregion

        /// <summary>
        /// Handles the login process for a user and returns an API response.
        /// </summary>
        /// <param name="inLoginUserDto">The login data containing the user's credentials.</param>
        /// <returns>
        /// An IActionResult containing an ApiResponse with the login result. 
        /// - If successful, the response includes a JWT token, user details, and claims.
        /// - If unsuccessful, the response contains an error message and error code.
        /// </returns>
        /// <response code="200">Returns an ApiResponse indicating the login result, whether successful or not.</response>
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
        [HttpPost]
        [Route("LoginUserAsync")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUserAsync(InLoginUserDto inLoginUserDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var (success, message, user) = await _accountService.LoginUser(inLoginUserDto);

                    if (success <= 0)
                    {
                        _logger.LogError("An error occurred while login user: {Message}", message);

                        var response = new ApiResponse<User>(
                            ApiResponseStatus.Failure,
                            StatusCodes.Status200OK,
                            success,
                            errorMessage: message,
                            errorCode: ErrorCode.LoginUserError,
                            txn: ConstantData.Txn(),
                            returnValue: user
                        );

                        return Ok(response);
                    }
                    else
                    {
                        _logger.LogInformation("{Message}", message);

                        var token = _jwtTokenGenerator.GenerateToken(user!.UserId!.ToString(), user!.Email!, user!.UserName!);

                        var claims = ExtractClaimsFromToken(token);

                        var responseDto = new LoginResponseDto
                        {
                            Token = token,
                            User = user,
                            Claims = claims
                        };

                        var response = new ApiResponse<LoginResponseDto>(
                            ApiResponseStatus.Success,
                            StatusCodes.Status200OK,
                            success,
                            successMessage: message,
                            txn: ConstantData.Txn(),
                            returnValue: responseDto
                        );

                        return Ok(response);
                    }
                }
                else
                {
                    _logger.LogError("An invalid model provided while logging user.");

                    var response = new ApiResponse<User>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        0,
                        errorMessage: "An invalid model provided while logging user.",
                        errorCode: ErrorCode.BadRequestError,
                        txn: ConstantData.Txn(),
                        returnValue: null
                    );

                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while login user: {Message}", ex.Message);

                var response = new ApiResponse<User>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    0,
                    errorMessage: "An unexpected error occurred while login user.",
                    errorCode: ErrorCode.InternalServerError,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Adds a new user to the system asynchronously.
        /// </summary>
        /// <param name="inAddUserDto">The DTO containing the details of the user to be added.</param>
        /// <returns>An <see cref="IActionResult"/> containing an <see cref="ApiResponse{T}"/> with the ID of the newly created user.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPost]
        [Route("AddUserAsync")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUserAsync(InAddUserDto inAddUserDto)
        {
            try
            {
                var (statusCode, userId) = await _accountService.AddNewUser(inAddUserDto);

                if (userId > 0 && statusCode == 1)
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
                else if (statusCode == 0)
                {
                    var response = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status406NotAcceptable,
                        statusCode,
                        errorMessage: $"Failed to add user because user is already registered with Username: {inAddUserDto.UserName} or Email: {inAddUserDto.Email}.",
                        errorCode: ErrorCode.AddUserFailedError,
                        txn: ConstantData.Txn()
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

        /// <summary>
        /// Generates an OTP based on the request data provided.
        /// </summary>
        /// <param name="inOtpRequestDto">The request data needed to generate the OTP.</param>
        /// <returns>A response containing the OTP or an error message.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPut]
        [Route("OtpGenerationRequestAsync")]
        [AllowAnonymous]
        public async Task<IActionResult> OtpGenerationRequestAsync(InOtpRequestDto inOtpRequestDto)
        {
            if (inOtpRequestDto == null)
            {
                return BadRequest(new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status400BadRequest,
                    0,
                    errorMessage: "Invalid OTP request data.",
                    errorCode: ErrorCode.InvalidModelRequestError,
                    txn: ConstantData.Txn()
                ));
            }

            try
            {
                BaseResponse response = await _accountService.OtpGeneration(inOtpRequestDto);

                if (response.Status == 1)
                {
                    var apiResponse = new ApiResponse<int>(
                        ApiResponseStatus.Success,
                        StatusCodes.Status200OK,
                        1,
                        successMessage: response.SuccessMessage,
                        txn: ConstantData.Txn(),
                        returnValue: response.Status
                    );

                    return Ok(apiResponse);
                }
                else
                {
                    var apiResponse = new ApiResponse<int>(
                        ApiResponseStatus.Failure,
                        StatusCodes.Status400BadRequest,
                        0,
                        errorMessage: response.ErrorMessage,
                        errorCode: ErrorCode.BadRequestError,
                        txn: ConstantData.Txn()
                    );

                    return BadRequest(apiResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generation of otp: {Message}", ex.Message);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    0,
                    errorMessage: "An unexpected error occurred while generation of otp.",
                    errorCode: ErrorCode.InternalServerError,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Handles the verification of an OTP (One-Time Password).
        /// </summary>
        /// <param name="inVerifyOtpDto">The data transfer object containing the email and OTP to be verified.</param>
        /// <returns>
        /// An IActionResult representing the API response. 
        /// Returns Ok with ApiResponse containing success or failure details based on OTP verification.
        /// </returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPatch]
        [Route("VerifyOtpRequestAsync")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtpRequestAsync(InVerifyOtpDto inVerifyOtpDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BaseResponse baseResponse = await _accountService.VerifyOtp(inVerifyOtpDto);
                    if (baseResponse.Status > 0)
                    {
                        return Ok(new ApiResponse<int>(
                            ApiResponseStatus.Success,
                            StatusCodes.Status200OK,
                            1,
                            successMessage: baseResponse.SuccessMessage,
                            txn: ConstantData.Txn(),
                            returnValue: baseResponse.Status));
                    }
                    else
                    {
                        return Ok(new ApiResponse<int>(
                            ApiResponseStatus.Failure,
                            StatusCodes.Status200OK,
                            0,
                            errorMessage: baseResponse.ErrorMessage,
                            errorCode: baseResponse.ErrorCode,
                            txn: ConstantData.Txn(),
                            returnValue: baseResponse.Status));
                    }
                }
                else
                {
                    _logger.LogError("Invalid Verification of OTP request data.");

                    return BadRequest(new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status400BadRequest,
                    0,
                    errorMessage: "Invalid Verification of OTP request data.",
                    errorCode: ErrorCode.InvalidModelRequestError,
                    txn: ConstantData.Txn()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while verification of otp: {Message}", ex.Message);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    0,
                    errorMessage: "An unexpected error occurred while verification of otp.",
                    errorCode: ErrorCode.InternalServerError,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        /// <summary>
        /// Soft deletes a user by email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        [ProducesResponseType(typeof(ApiResponse<int>), 200)]
        [HttpPut]
        [Route("SoftDeleteUserRequestAsync")]
        [AllowAnonymous]
        public async Task<IActionResult> SoftDeleteUserRequestAsync(string email)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BaseResponse baseResponse = await _accountService.SoftDeleteUser(email);
                    if (baseResponse.Status > 0)
                    {
                        _logger.LogInformation("{Message}", baseResponse.SuccessMessage);

                        return Ok(new ApiResponse<int>(
                                ApiResponseStatus.Success,
                                StatusCodes.Status200OK,
                                1,
                                successMessage: baseResponse.SuccessMessage,
                                txn: ConstantData.Txn()));
                    }
                    else
                    {
                        _logger.LogError("{Message}", baseResponse.ErrorMessage);

                        return Ok(new ApiResponse<int>(
                                ApiResponseStatus.Failure,
                                StatusCodes.Status200OK,
                                0,
                                errorMessage: baseResponse.ErrorMessage,
                                errorCode: ErrorCode.SoftDeleteUserRequestAsyncError,
                                txn: ConstantData.Txn()));
                    }
                }
                else
                {
                    _logger.LogError("Invalid Soft Deletion of User request data.");

                    return BadRequest(new ApiResponse<int>(
                            ApiResponseStatus.Failure,
                            StatusCodes.Status400BadRequest,
                            0,
                            errorMessage: "Invalid soft deletion of user request data.",
                            errorCode: ErrorCode.InvalidModelRequestError,
                            txn: ConstantData.Txn()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deletion of user: {Message}", ex.Message);

                var response = new ApiResponse<int>(
                    ApiResponseStatus.Failure,
                    StatusCodes.Status500InternalServerError,
                    0,
                    errorMessage: "An unexpected error occurred while soft deletion of user.",
                    errorCode: ErrorCode.InternalServerError,
                    txn: ConstantData.Txn()
                );

                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
