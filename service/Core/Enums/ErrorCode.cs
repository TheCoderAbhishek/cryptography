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
        /// Error code indicating a validation error occurred.
        /// </summary>
        public const string ModelValidationError = "ERR-1000-900";


        /// <summary>
        /// Error indicating unknown error
        /// </summary>
        public const string UnknownError = "ERR-1000-999";

        #endregion
    }
}
