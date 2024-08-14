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
                    return baseResponse; 
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
    }
}
