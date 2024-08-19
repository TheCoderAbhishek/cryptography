namespace service.Core.Enums
{
    /// <summary>
    /// This class provides a collection of custom error codes used throughout the application.
    /// </summary>
    public static class ErrorCode
    {
        #region Error Codes for Account Management from `ERR-1000-001` to `ERR-1000-999`

        /// <summary>
        /// Error code indicating a error occurred while getting list of users.
        /// </summary>
        public const string GetAllUsersAsyncError = "ERR-1000-001";

        /// <summary>
        /// Error code indicating no users present.
        /// </summary>
        public const string NoUsersError = "ERR-1000-002";

        /// <summary>
        /// Error code indicating error occurred while getting users.
        /// </summary>
        public const string GetAllUsersError = "ERR-1000-003";

        /// <summary>
        /// Error code indicating error occurred while adding new user.
        /// </summary>
        public const string AddNewUserAsyncError = "ERR-1000-004";

        /// <summary>
        /// Error code indicating error occurred while adding user.
        /// </summary>
        public const string AddUserFailedError = "ERR-1000-005";

        /// <summary>
        /// Error code indicating error occurred while adding OTP details in table.
        /// </summary>
        public const string AddOtpAsyncError = "ERR-1000-006";

        /// <summary>
        /// Error code indicating error occurred while getting ID associated with email.
        /// </summary>
        public const string GetIdEmailAsyncError = "ERR-1000-007";

        /// <summary>
        /// Error code indicating exception occurred while OTP generation
        /// </summary>
        public const string OtpGenerationExceptionError = "ERR-1000-008";

        /// <summary>
        /// Error code indicating error occurred while getting Id based upon username or email.
        /// </summary>
        public const string GetUserUsernameEmailAsyncError = "ERR-1000-009";

        /// <summary>
        /// Error occurred indicating error occurred while getting OTP details based upon email.
        /// </summary>
        public const string GetIdValidUntilEmailAsyncError = "ERR-1000-010";

        /// <summary>
        /// Error code indicating error occurred while updating OTP details.
        /// </summary>
        public const string UpdateOtpDetailsAsyncError = "ERR-1000-011";

        /// <summary>
        /// Error code indicating error occurred while getting user details by email.
        /// </summary>
        public const string GetUserEmailAsyncError = "ERR-1000-012";

        /// <summary>
        /// Error code indicating error occurred while login user.
        /// </summary>
        public const string LoginUserError = "ERR-1000-013";

        /// <summary>
        /// Error code indicating invalid email provide while verifying OTP.
        /// </summary>
        public const string InvalidEmailError = "ERR-1000-014";

        /// <summary>
        /// Error code indicating unable to fetch OTP details associated with email.
        /// </summary>
        public const string GetOtpDetailsEmailAsyncError = "ERR-1000-015";

        /// <summary>
        /// Error code indicating unable to find OTP associated with Email.
        /// </summary>
        public const string InvalidOtpNEmailError = "ERR-1000-016";

        /// <summary>
        /// Error code indicating unable to unlock user.
        /// </summary>
        public const string UpdateUserDetailsUnlockUserAsyncError = "ERR-1000-017";

        /// <summary>
        /// Error code indicating user not found associated with email.
        /// </summary>
        public const string UserNotFoundError = "ERR-1000-018";

        /// <summary>
        /// Error code indicating unable to update attempt count
        /// </summary>
        public const string UpdateFailedLoginAttemptsAsyncError = "ERR-1000-019";

        /// <summary>
        /// Error code indicating unable to locked in a user due to multiple unauthorised requests.
        /// </summary>
        public const string UpdateFailedLoginAttemptsLockedUserAsyncError = "ERR-1000-020";


        /// <summary>
        /// Error code indicating a validation error occurred.
        /// </summary>
        public const string ModelValidationError = "ERR-1000-900";

        /// <summary>
        /// Error code indicating a internal server error.
        /// </summary>
        public const string InternalServerError = "ERR-1000-901";

        /// <summary>
        /// Error code indicating a model is in invalid state
        /// </summary>
        public const string InvalidModelRequestError = "ERR-1000-902";

        /// <summary>
        /// Error code indicating a bad request
        /// </summary>
        public const string BadRequestError = "ERR-1000-903";


        /// <summary>
        /// Error indicating unknown error
        /// </summary>
        public const string UnknownError = "ERR-1000-999";

        #endregion
    }
}
