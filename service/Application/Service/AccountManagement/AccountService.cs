using Konscious.Security.Cryptography;
using service.Core.Dto.AccountManagement;
using service.Core.Entities.AccountManagement;
using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.AccountManagement;
using service.Core.Interfaces.Utility;
using System.Security.Cryptography;
using System.Text;

namespace service.Application.Service.AccountManagement
{
    /// <summary>
    /// Represents the account service for managing user accounts.
    /// </summary>
    public class AccountService(ILogger<AccountService> logger, IAccountRepository accountRepository, IEmailOtpService emailOtpService, IEmailOtpRepository emailOtpRepository) : IAccountService
    {
        private readonly ILogger<AccountService> _logger = logger;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IEmailOtpService _emailOtpService = emailOtpService;
        private readonly IEmailOtpRepository _emailOtpRepository = emailOtpRepository;

        #region Private Methods for Support
        /// <summary>
        /// Generates a random base64 encoded salt string of length 16 bytes.
        /// </summary>
        /// <returns>A base64 encoded string representing the random salt.</returns>
        private static string GetGenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var buffer = new byte[16];
            rng.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// Hashes a password using SHA256 with a provided salt.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <param name="salt">The salt to be used for hashing.</param>
        /// <returns>A base64 encoded string representing the hashed password.</returns>
        private static string HashPassword(string password, string salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = Convert.FromBase64String(salt),
                DegreeOfParallelism = 8,
                MemorySize = 65536,
                Iterations = 4
            };

            var hash = argon2.GetBytes(16);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Generates a new unique client identifier using a Guid.
        /// </summary>
        /// <returns>A string representing the newly generated client ID.</returns>
        private static string GenerateClientId()
        {
            return Guid.NewGuid().ToString();
        }
        #endregion

        /// <summary>
        /// Handles the login process for a client.
        /// </summary>
        /// <param name="inLoginUserDto">The login data containing the user's email and password.</param>
        /// <returns>A tuple containing:
        ///     - An integer status code indicating the login result (1 for success, 0 for invalid email, -2 for invalid password, -1 for unexpected error).
        ///     - A message describing the login result.
        ///     - The User object if the login is successful, otherwise null.
        /// </returns>
        public async Task<(int, string, User?)> LoginUser(InLoginUserDto inLoginUserDto)
        {
            try
            {
                User? user = await _accountRepository.GetUserEmailAsync(inLoginUserDto.UserEmail!);

                if (user != null)
                {
                    if (user.IsLocked == true && user.LockedUntil < DateTime.Now)
                    {
                        user.IsLocked = false;
                        user.LockedUntil = null;
                        await _accountRepository.UpdateFailedLoginAttemptsLockedUserAsync(user);
                    }

                    if (user.IsActive == true && user.IsLocked == false && (user.LockedUntil < DateTime.Now || user.LockedUntil == null))
                    {
                        string providedHashedPassword = HashPassword(inLoginUserDto.UserPassword!, user.Salt!);

                        if (providedHashedPassword == user.Password)
                        {
                            _logger.LogInformation("User logged in successfully with email {Email}", inLoginUserDto.UserEmail);
                            user.LoginAttempts = 0;
                            user.LastLoginDateTime = DateTime.Now;
                            await _accountRepository.UpdateFailedLoginAttemptsAsync(user);
                            return (1, $"User logged in successfully with email {inLoginUserDto.UserEmail}", user);
                        }
                        else
                        {
                            _logger.LogError("Invalid password provided.");
                            user.LoginAttempts++;
                            if (user.LoginAttempts < 3)
                            {
                                await _accountRepository.UpdateFailedLoginAttemptsAsync(user);
                                return (-3, $"Invalid password provided. '{3 - user.LoginAttempts}' attempts left.", null);
                            }
                            else
                            {
                                user.LockedUntil = DateTime.Now.AddMinutes(15);
                                user.IsLocked = true;
                                await _accountRepository.UpdateFailedLoginAttemptsLockedUserAsync(user);
                                return (-3, $"Invalid password provided. Account locked for 15 minutes.", null);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogError("User associated with email '{Email}' is inactive or locked.", inLoginUserDto.UserEmail);
                        return (-2, $"User associated with email '{inLoginUserDto.UserEmail}' is inactive or locked. Please activate this user or try again after 15 minutes.", null);
                    }
                }
                else
                {
                    _logger.LogError("Invalid email {Email} provided.", inLoginUserDto.UserEmail);
                    return (0, "Invalid email provided.", user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while login user: {Message}", ex.Message);
                return (-1, "", null);
            }
        }

        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="inAddUserDto">The DTO containing the details of the user to be added.</param>
        /// <returns>
        /// A tuple containing the status code is 1 for success and 0 for failure. If an error occurs, the user ID will be -1.
        /// The user ID of the newly added user and a status code.
        /// </returns>
        public async Task<(int, int)> AddNewUser(InAddUserDto inAddUserDto)
        {
            try
            {
                int id = await _accountRepository.GetUserUsernameEmailAsync(inAddUserDto.UserName!, inAddUserDto.Email!);

                if (id <= 0)
                {
                    var salt = GetGenerateSalt();
                    var hashedPassword = HashPassword(inAddUserDto.Password!, salt);

                    var newUser = new User
                    {
                        UserId = GenerateClientId(),
                        Name = inAddUserDto.Name,
                        UserName = inAddUserDto.UserName,
                        Email = inAddUserDto.Email,
                        Password = hashedPassword,
                        CreatedOn = DateTime.Now,
                        IsAdmin = true,
                        IsActive = true,
                        IsLocked = true,
                        IsDeleted = false,
                        DeletedStatus = DeletedState.NotDeleted,
                        LoginAttempts = 0,
                        RoleId = RoleId.SuperAdmin,
                        Salt = salt
                    };

                    int userId = await _accountRepository.AddNewUserAsync(newUser);

                    return (1, userId);
                }
                else
                {
                    return (0, id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
                return (-1, 0);
            }
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns>A tuple containing the status code and a list of users.</returns>
        public async Task<(int, List<User>)> GetAllUsers()
        {
            try
            {
                List<User> user = await _accountRepository.GetAllUsersAsync();
                if (user == null || user.Count == 0)
                {
                    _logger.LogError("No users found.");
                    return (0, user)!;
                }
                else
                {
                    _logger.LogInformation("{User} users retrieved successfully.", user);
                    return (1, user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users: {Message}", ex.Message);
                return (-1, null)!;
            }
        }

        /// <summary>
        /// Generates an OTP.
        /// </summary>
        /// <param name="inOtpRequestDto">The OTP request data containing necessary information for OTP generation.</param>
        /// <returns>An integer representing the generated OTP.</returns>
        public async Task<BaseResponse> OtpGeneration(InOtpRequestDto inOtpRequestDto)
        {
            try
            {
                int id = await _accountRepository.GetIdEmailAsync(inOtpRequestDto.Email!);

                if (id > 0)
                {
                    OtpStorage isOtpFound = await _accountRepository.GetIdValidUntilEmailAsync(inOtpRequestDto.Email!);

                    if (isOtpFound == null)
                    {
                        var salt = GetGenerateSalt();
                        string otp = _emailOtpService.GenerateOtpAsync();
                        var hashedOtp = HashPassword(otp, salt);

                        // OTP Sending on Email
                        await _emailOtpService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Account Activation Code", "Your One Time Password (OTP) is: ", true);

                        OtpStorage otpStorage = new()
                        {
                            UserId = id,
                            Email = inOtpRequestDto.Email,
                            GeneratedOn = DateTime.Now,
                            ValidUntil = DateTime.Now.AddMinutes(15),
                            Otp = hashedOtp,
                            Salt = salt,
                            AttemptCount = 1,
                            OtpUseCase = 1,
                        };

                        BaseResponse baseResponse = await _emailOtpRepository.AddOtpAsync(otpStorage);
                        baseResponse.SuccessMessage = $"OTP sent successfully on email '{inOtpRequestDto.Email}'.";
                        baseResponse.Status = 1;
                        return baseResponse;
                    }
                    else
                    {
                        var salt = isOtpFound.Salt;
                        string otp = _emailOtpService.GenerateOtpAsync();
                        var hashedOtp = HashPassword(otp, salt!);

                        // OTP Sending on Email
                        await _emailOtpService.SendOtpEmailAsync(inOtpRequestDto.Email!, otp, "Ayerhs - Account Activation Code", "Your One Time Password (OTP) is: ", true);

                        OtpStorage otpStorage = new()
                        {
                            Email = inOtpRequestDto.Email,
                            GeneratedOn = DateTime.Now,
                            ValidUntil = DateTime.Now.AddMinutes(15),
                            Otp = hashedOtp,
                        };

                        BaseResponse baseResponse = await _accountRepository.UpdateOtpDetailsAsync(otpStorage);
                        baseResponse.SuccessMessage = $"OTP sent successfully on email '{inOtpRequestDto.Email}'.";
                        baseResponse.Status = 1;
                        return baseResponse;
                    }
                }
                else
                {
                    _logger.LogError("Email \"{Email}\" is not registered with the application.", inOtpRequestDto.Email);
                    BaseResponse baseResponse = new()
                    {
                        Status = -1,
                        ErrorMessage = $"Email '{inOtpRequestDto.Email}' is not registered with the application. Please verify it.",
                        ErrorCode = ErrorCode.OtpGenerationExceptionError
                    };
                    return baseResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while OTP generation: {Message}", ex.Message);
                BaseResponse baseResponse = new()
                {
                    Status = -1,
                    ErrorMessage = ex.Message,
                    ErrorCode = ErrorCode.OtpGenerationExceptionError
                };
                return baseResponse;
            }
        }

        /// <summary>
        /// Verifies the provided OTP (One-Time Password) for the given email address and unlocks the associated user account if successful.
        /// </summary>
        /// <param name="inVerifyOtpDto">The data transfer object containing the email and OTP to be verified.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains a BaseResponse indicating the success or failure of the verification and any relevant messages.
        /// </returns>
        public async Task<BaseResponse> VerifyOtp(InVerifyOtpDto inVerifyOtpDto)
        {
            try
            {
                int id = await _accountRepository.GetIdEmailAsync(inVerifyOtpDto.Email!);

                if (id > 0)
                {
                    OtpStorage otpStorage = await _accountRepository.GetOtpDetailsEmailAsync(inVerifyOtpDto.Email!);

                    if (otpStorage != null)
                    {
                        if (otpStorage.Otp == HashPassword(inVerifyOtpDto.Otp!, otpStorage.Salt!))
                        {
                            User? user = await _accountRepository.GetUserEmailAsync(inVerifyOtpDto.Email!);
                            if (user != null)
                            {
                                user.IsLocked = false;

                                int res = await _accountRepository.UpdateUserDetailsUnlockUserAsync(user);
                                if (res == 1)
                                {
                                    _logger.LogInformation("User '{Email}' unlocked successfully.", user.Email);

                                    BaseResponse baseResponse = new()
                                    {
                                        Status = 1,
                                        SuccessMessage = $"User '{user.Email}' unlocked successfully."
                                    };
                                    return baseResponse;
                                }
                                else
                                {
                                    _logger.LogError("Internal error occurred while unlocking user '{Email}'", inVerifyOtpDto.Email!);

                                    BaseResponse baseResponse = new()
                                    {
                                        Status = -4,
                                        ErrorMessage = $"Internal error occurred while unlocking '{inVerifyOtpDto.Email!}' User. Please try again.",
                                        ErrorCode = ErrorCode.UserNotFoundError
                                    };
                                    return baseResponse;
                                }
                            }
                            else
                            {
                                _logger.LogError("Unable to find user '{Email}'", inVerifyOtpDto.Email!);

                                BaseResponse baseResponse = new()
                                {
                                    Status = -4,
                                    ErrorMessage = $"Unable to find '{inVerifyOtpDto.Email!}' User.",
                                    ErrorCode = ErrorCode.UserNotFoundError
                                };
                                return baseResponse;
                            }
                        }
                        else
                        {
                            _logger.LogError("Invalid OTP Provided for user '{Email}'", inVerifyOtpDto.Email!);

                            BaseResponse baseResponse = new()
                            {
                                Status = -3,
                                ErrorMessage = $"Invalid OTP Provided for '{inVerifyOtpDto.Email!}' User. Please Check OTP Again.",
                                ErrorCode = ErrorCode.InvalidOtpNEmailError
                            };
                            return baseResponse;
                        }
                    }
                    else
                    {
                        _logger.LogError("Invalid Email Address Provided. {Email}", inVerifyOtpDto.Email!);

                        BaseResponse baseResponse = new()
                        {
                            Status = -2,
                            ErrorMessage = $"Invalid OTP Provided for '{inVerifyOtpDto.Email!}' Email. Please Generate a new OTP.",
                            ErrorCode = ErrorCode.InvalidOtpNEmailError
                        };
                        return baseResponse;
                    }
                }
                else
                {
                    _logger.LogError("Invalid Email Address Provided. {Email}", inVerifyOtpDto.Email!);

                    BaseResponse baseResponse = new()
                    {
                        Status = -1,
                        ErrorMessage = $"Invalid Email Address Provided. Please Verify '{inVerifyOtpDto.Email!}' Email.",
                        ErrorCode = ErrorCode.InvalidEmailError
                    };
                    return baseResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while OTP verification: {Message}", ex.Message);
                BaseResponse baseResponse = new()
                {
                    Status = 0,
                    ErrorMessage = ex.Message,
                    ErrorCode = ErrorCode.OtpGenerationExceptionError
                };
                return baseResponse;
            }
        }

        /// <summary>
        /// Soft deletes a user by their email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <returns>A BaseResponse indicating success or failure, with relevant status codes and messages.</returns>
        public async Task<BaseResponse> SoftDeleteUser(string email)
        {
            try
            {
                User? user = await _accountRepository.GetUserEmailAsync(email);

                if (user != null)
                {
                    if (user.IsDeleted == false)
                    {
                        user.IsActive = false;
                        user.IsDeleted = true;
                        user.DeletedStatus = DeletedState.SoftDeleted;
                        user.UpdatedOn = DateTime.Now;
                        user.DeletedOn = DateTime.Now;
                        user.AutoDeletedOn = DateTime.Now.AddDays(30);
                        BaseResponse baseResponse = await _accountRepository.UpdateSoftDeleteRestoreUserAsync(user);

                        if (baseResponse.Status > 0)
                        {
                            _logger.LogInformation("User '{Email}' soft deleted successfully.", email);

                            baseResponse = new()
                            {
                                Status = 1,
                                SuccessMessage = $"User '{email}' soft deleted successfully."
                            };
                            return baseResponse;
                        }
                        else
                        {
                            _logger.LogError("Error occurred while soft deletion of '{Email}' user.", email);

                            baseResponse = new()
                            {
                                Status = -3,
                                ErrorMessage = $"Error occurred while soft deletion of '{user.AutoDeletedOn}' user.",
                                ErrorCode = ErrorCode.UserDeletedStateError
                            };
                            return baseResponse;
                        }
                    }
                    else
                    {
                        _logger.LogError("User is already in soft deletion state. {Email}", email);

                        BaseResponse baseResponse = new()
                        {
                            Status = -2,
                            ErrorMessage = $"User is already in soft deletion state. You can restore user before '{user.AutoDeletedOn}'.",
                            ErrorCode = ErrorCode.UserDeletedStateError
                        };
                        return baseResponse;
                    }
                }
                else
                {
                    _logger.LogError("Invalid Email Address Provided. {Email}", email);

                    BaseResponse baseResponse = new()
                    {
                        Status = -1,
                        ErrorMessage = $"Invalid Email Address Provided. Please Verify '{email}' Email.",
                        ErrorCode = ErrorCode.InvalidEmailError
                    };
                    return baseResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while soft deletion of user: {Message}", ex.Message);

                BaseResponse baseResponse = new()
                {
                    Status = 0,
                    ErrorMessage = ex.Message,
                    ErrorCode = ErrorCode.SoftDeleteUserExceptionError
                };
                return baseResponse;
            }
        }

        /// <summary>
        /// Restores a soft-deleted user account.
        /// </summary>
        /// <param name="email">The email address of the user to restore.</param>
        /// <returns>A BaseResponse indicating the success or failure of the restoration.</returns>
        public async Task<BaseResponse> RestoreSoftDeletedUser(string email)
        {
            try
            {
                User? user = await _accountRepository.GetUserEmailAsync(email);

                if (user != null)
                {
                    if (user.IsDeleted == true)
                    {
                        user.IsActive = true;
                        user.IsDeleted = false;
                        user.DeletedStatus = DeletedState.NotDeleted;
                        user.UpdatedOn = DateTime.Now;
                        user.DeletedOn = null;
                        user.AutoDeletedOn = null;
                        BaseResponse baseResponse = await _accountRepository.UpdateSoftDeleteRestoreUserAsync(user);

                        if (baseResponse.Status > 0)
                        {
                            _logger.LogInformation("User '{Email}' restored successfully.", email);

                            baseResponse = new()
                            {
                                Status = 1,
                                SuccessMessage = $"User '{email}' restored successfully."
                            };
                            return baseResponse;
                        }
                        else
                        {
                            _logger.LogError("An error occurred while restoring the soft-deleted user '{Email}'.", email);

                            baseResponse = new()
                            {
                                Status = -3,
                                ErrorMessage = $"An error occurred while restoring the soft-deleted user '{email}'.",
                                ErrorCode = ErrorCode.UserDeletedStateError
                            };
                            return baseResponse;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("The user '{Email}' is already active.", email);

                        BaseResponse baseResponse = new()
                        {
                            Status = -2,
                            ErrorMessage = $"The user '{email}' is already active.",
                            ErrorCode = ErrorCode.UserDeletedStateError
                        };
                        return baseResponse;
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid email address provided: '{Email}'", email);

                    BaseResponse baseResponse = new()
                    {
                        Status = -1,
                        ErrorMessage = $"Invalid email address provided. Please verify '{email}'.",
                        ErrorCode = ErrorCode.InvalidEmailError
                    };
                    return baseResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring the soft-deleted user: {Message}", ex.Message);

                BaseResponse baseResponse = new()
                {
                    Status = 0,
                    ErrorMessage = "An unexpected error occurred while restoring the user. Please try again later or contact support.",
                    ErrorCode = ErrorCode.RestoreSoftDeletedUserExceptionError
                };
                return baseResponse;
            }
        }

        /// <summary>
        /// Enables an active user.
        /// </summary>
        /// <param name="email">The email address of the user to enable.</param>
        /// <returns>A <see cref="BaseResponse"/> indicating the success or failure of the operation.</returns>
        public async Task<BaseResponse> EnableActiveUser(string email)
        {
            try
            {
                User? user = await _accountRepository.GetUserEmailAsync(email);

                if (user != null)
                {
                    if (user.IsActive == false)
                    {
                        user.IsActive = true;
                        user.UpdatedOn = DateTime.Now;

                        BaseResponse baseResponse = await _accountRepository.EnableDisableUserAsync(user);

                        if (baseResponse.Status > 0)
                        {
                            _logger.LogInformation("User '{Email}' enabled successfully.", email);

                            baseResponse = new()
                            {
                                Status = 1,
                                SuccessMessage = $"User '{email}' enabled successfully."
                            };
                            return baseResponse;
                        }
                        else
                        {
                            _logger.LogError("An error occurred while enabling the user '{Email}'.", email);

                            baseResponse = new()
                            {
                                Status = -3,
                                ErrorMessage = $"An error occurred while enabling the user '{email}'.",
                                ErrorCode = ErrorCode.UserActiveStateError
                            };
                            return baseResponse;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("The user '{Email}' is already enabled.", email);

                        BaseResponse baseResponse = new()
                        {
                            Status = -2,
                            ErrorMessage = $"The user '{email}' is already enabled.",
                            ErrorCode = ErrorCode.UserActiveStateError
                        };
                        return baseResponse;
                    }
                }
                else
                {
                    _logger.LogWarning("Invalid email address provided: '{Email}'", email);

                    BaseResponse baseResponse = new()
                    {
                        Status = -1,
                        ErrorMessage = $"Invalid email address provided. Please verify '{email}'.",
                        ErrorCode = ErrorCode.InvalidEmailError
                    };
                    return baseResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while restoring the soft-deleted user: {Message}", ex.Message);

                BaseResponse baseResponse = new()
                {
                    Status = 0,
                    ErrorMessage = "An unexpected error occurred while restoring the user. Please try again later or contact support.",
                    ErrorCode = ErrorCode.EnableActiveUserExceptionError
                };
                return baseResponse;
            }
        }
    }
}
