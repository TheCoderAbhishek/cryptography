using service.Core.Entities.Utility;

namespace service.Core.Interfaces.Utility
{
    /// <summary>
    /// Defines the contract for an OTP storage repository.
    /// </summary>
    public interface IEmailOtpRepository
    {
        /// <summary>
        /// Adds an OTP to the storage.
        /// </summary>
        /// <param name="otpStorage">The OTP storage object to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<BaseResponse> AddOtpAsync(OtpStorage otpStorage);
    }
}
