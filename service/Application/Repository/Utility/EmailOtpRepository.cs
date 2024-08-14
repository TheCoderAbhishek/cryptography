using service.Core.Entities.Utility;
using service.Core.Enums;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using service.Infrastructure.Queries.Utility;

namespace service.Application.Repository.Utility
{
    /// <summary>
    /// Repository class for Email OTP related operations.
    /// </summary>
    public class EmailOtpRepository(ILogger<EmailOtpRepository> logger, ICommonDbHander commonDbHander) : IEmailOtpRepository
    {
        private readonly ILogger<EmailOtpRepository> _logger = logger;
        private readonly ICommonDbHander _commonDbHander = commonDbHander;

        /// <summary>
        /// Adds a new OTP record to the database asynchronously.
        /// </summary>
        /// <param name="otpStorage">OtpStorage object containing OTP details.</param>
        /// <returns>A BaseResponse object indicating the outcome of the operation.</returns>
        public async Task<BaseResponse> AddOtpAsync(OtpStorage otpStorage)
        {
            string query = EmailOtpQueries.InsertOtpStorage;

            try
            {
                var parameters = new
                {
                    otpStorage.UserId,
                    otpStorage.Email,
                    otpStorage.GeneratedOn,
                    otpStorage.ValidUntil,
                    otpStorage.Otp,
                    otpStorage.Salt,
                    otpStorage.AttemptCount,
                    otpStorage.OtpUseCase
                };

                BaseResponse baseResponse = await _commonDbHander.AddUpdateDeleteData(query, "OTP details added successfully.", "An error occurred while adding OTP details. Please try again.", "An OTP for this user might already exist. Please resend OTP if needed.", ErrorCode.AddOtpAsyncError, ConstantData.Txn(), parameters);

                return baseResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding OTP details into table. {Message}", ex.Message);
                throw new CustomException("Error adding OTP details into table from the email and otp repository.", ex,
                                   ErrorCode.AddNewUserAsyncError, ConstantData.Txn());
            }
        }
    }
}
