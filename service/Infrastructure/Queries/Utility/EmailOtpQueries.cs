namespace service.Infrastructure.Queries.Utility
{
    /// <summary>
    /// Contains SQL queries related to OTP storage.
    /// </summary>
    public static class EmailOtpQueries
    {
        /// <summary>
        /// SQL query to insert an OTP into the OtpStorage table.
        /// </summary>
        public const string InsertOtpStorage = "INSERT INTO OtpStorage (UserId, Email, GeneratedOn, ValidUntil, Otp, Salt, AttemptCount, OtpUseCase) " +
                       "VALUES (@UserId, @Email, @GeneratedOn, @ValidUntil, @Otp, @Salt, @AttemptCount, @OtpUseCase)";
    }
}
