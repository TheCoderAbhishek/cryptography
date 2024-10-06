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
        /// Error code indicating exception catched while soft deletion of user.
        /// </summary>
        public const string SoftDeleteUserExceptionError = "ERR-1000-021";

        /// <summary>
        /// Error code indicating user is already in soft deletion state.
        /// </summary>
        public const string UserDeletedStateError = "ERR-1000-022";

        /// <summary>
        /// Error code indicating soft delete user error.
        /// </summary>
        public const string UpdateSoftDeleteUserAsyncError = "ERR-1000-023";

        /// <summary>
        /// Error code indicating failed soft delete user request.
        /// </summary>
        public const string SoftDeleteUserRequestAsyncError = "ERR-1000-024";

        /// <summary>
        /// Error code indicating exception catched while restore soft deleted user.
        /// </summary>
        public const string RestoreSoftDeletedUserExceptionError = "ERR-1000-025";

        /// <summary>
        /// Error code indicating query execution failed.
        /// </summary>
        public const string EnableDisableUserAsyncError = "ERR-1000-026";

        /// <summary>
        /// Error code indicating restore soft deleted user failed.
        /// </summary>
        public const string RestoreSoftDeletedUserAsyncError = "ERR-1000-027";

        /// <summary>
        /// Error code indicating enable user request error.
        /// </summary>
        public const string EnableUserRequestAsyncError = "ERR-1000-028";

        /// <summary>
        /// Error code indicating exception occurred while enabling user.
        /// </summary>
        public const string EnableActiveUserExceptionError = "ERR-1000-029";

        /// <summary>
        /// Error code indicating user state is already same as before.
        /// </summary>
        public const string UserActiveStateError = "ERR-1000-030";

        /// <summary>
        /// Error code indicating exception occurred while disabling user.
        /// </summary>
        public const string DisableInactiveUserExceptionError = "ERR-1000-031";

        /// <summary>
        /// Error code indicating user is already in deactivate state.
        /// </summary>
        public const string UserDeactivateStateError = "ERR-1000-032";

        /// <summary>
        /// Error code indicating disable user.
        /// </summary>
        public const string DisableUserAsyncError = "ERR-1000-033";

        /// <summary>
        /// Error code indicating query failed while hard deleting user.
        /// </summary>
        public const string HardDeleteUserAsyncError = "ERR-1000-034";

        /// <summary>
        /// Error code indicating exception catched during hard deletion of user.
        /// </summary>
        public const string HardDeleteUserExceptionError = "ERR-1000-035";

        /// <summary>
        /// Error code indicating hard delete user
        /// </summary>
        public const string HardDeleteUserError = "ERR-1000-036";


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
        /// Error code indicating key pair generation
        /// </summary>
        public const string GenerateRsaKeyPairError = "ERR-1000-904";


        /// <summary>
        /// Error indicating unknown error
        /// </summary>
        public const string UnknownError = "ERR-1000-999";

        #endregion

        #region Error Codes for User Management from `ERR-2000-001` to `ERR-1000-999`

        /// <summary>
        /// Error code indicating a error occurred while getting list of users.
        /// </summary>
        public const string GetUsersAsyncError = "ERR-2000-001";

        /// <summary>
        /// Exception code for errors encountered during the GetUsersAsync operation.
        /// </summary>
        public const string GetUsersAsyncException = "ERR-2000-002";

        /// <summary>
        /// Error code for general errors during user retrieval.
        /// </summary>
        public const string GetUsersError = "ERR-2000-003";

        /// <summary>
        /// Error code indicating that no users were found during a user management operation.
        /// </summary>
        public const string UserManagementNoUsersError = "ERR-2000-004";

        /// <summary>
        /// Error code indicating exception occurred while inserting new user in table.
        /// </summary>
        public const string CreateNewUserAsyncException = "ERR-2000-005";

        /// <summary>
        /// Error code indicating error occurred while inserting new user details.
        /// </summary>
        public const string CreateNewUserAsyncError = "ERR-2000-006";

        /// <summary>
        /// Error code indicating SQL exception occurred while inserting new user in table.
        /// </summary>
        public const string CreateNewUserAsyncSqlException = "ERR-2000-007";

        /// <summary>
        /// Error code indicating unhandled exception occurred while creating new user.
        /// </summary>
        public const string CreateNewUserAsyncUnhandledException = "ERR-2000-008";

        /// <summary>
        /// Error code indicating error occurred while getting user details based upon email or username.
        /// </summary>
        public const string GetUserDetailsMailUsernameAsyncError = "ERR-2000-009";

        /// <summary>
        /// Error code indicating SQL exception occurred while getting user details based upon email or username.
        /// </summary>
        public const string GetUserDetailsMailUsernameAsyncSqlException = "ERR-2000-010";

        /// <summary>
        /// Error code indicating unhandled exception occurred while getting user details based upon email or username.
        /// </summary>
        public const string GetUserDetailsMailUsernameAsyncException = "ERR-2000-011";

        /// <summary>
        /// Error code indicating duplicate record found in table.
        /// </summary>
        public const string GetUserDetailsMailUsernameAsyncDuplicateError = "ERR-2000-012";

        /// <summary>
        /// Error code indicating error occurred while locking or unlocking user.
        /// </summary>
        public const string LockUnlockUserAsyncError = "ERR-2000-013";

        /// <summary>
        /// Error code indicating SQL exception occurred while locking or unlocking user.
        /// </summary>
        public const string LockUnlockUserAsyncSqlException = "ERR-2000-014";

        /// <summary>
        /// Error code indicating exception occurred while locking or unlocking user.
        /// </summary>
        public const string LockUnlockUserAsyncException = "ERR-2000-015";

        /// <summary>
        /// Error code indicating unhandled exception occurred while locking/unlocking user.
        /// </summary>
        public const string LockUnlockUserAsyncUnhandledException = "ERR-2000-016";

        /// <summary>
        /// Error code indicating error occurred while getting deleted users
        /// </summary>
        public const string GetDeletedUsersAsyncError = "ERR-2000-017";

        /// <summary>
        /// Error code indicating SQL exception occurred while fetching soft deleted users.
        /// </summary>
        public const string GetDeletedUsersAsyncSqlException = "ERR-2000-018";

        /// <summary>
        /// Error code indicating unhandled exception occurred while fetching soft deleted users.
        /// </summary>
        public const string GetDeletedUsersAsyncException = "ERR-2000-019";

        /// <summary>
        /// Error code indicating error occurred while getting deleted users list.
        /// </summary>
        public const string GetDeletedUsersError = "ERR-2000-020";

        /// <summary>
        /// Error code indicating error occurred while soft deleting user.
        /// </summary>
        public const string SoftDeleteUserAsyncError = "ERR-2000-021";

        /// <summary>
        /// Error code indicating SQL exception occurred while soft deleting user.
        /// </summary>
        public const string SoftDeleteUserAsyncSqlException = "ERR-2000-022";

        /// <summary>
        /// Error code indicating exception occurred while soft deleting user.
        /// </summary>
        public const string SoftDeleteUserAsyncException = "ERR-2000-023";

        /// <summary>
        /// Error code indicating error occurred while getting user details.
        /// </summary>
        public const string GetUserDetailsByIdAsyncError = "ERR-2000-024";

        /// <summary>
        /// Error code indicating SQL exception occurred while getting user details.
        /// </summary>
        public const string GetUserDetailsByIdAsyncSqlException = "ERR-2000-025";

        /// <summary>
        /// Error code indicating unhandled eexception occurred while getting user details.
        /// </summary>
        public const string GetUserDetailsByIdAsyncException = "ERR-2000-026";

        /// <summary>
        /// Error code indicating unhandled exception occurred while soft deleting user during API call.
        /// </summary>
        public const string SoftDeleteUserAsyncUnhandledException = "ERR-2000-027";

        /// <summary>
        /// Error code indicating unhandled exception occurred while updating user details.
        /// </summary>
        public const string UpdateUserDetailsAsyncUnhandledException = "ERR-2000-028";

        /// <summary>
        /// Error code indicating SQL exception occurred while updating user details.
        /// </summary>
        public const string UpdateUserDetailsAsyncSqlException = "ERR-2000-029";

        /// <summary>
        /// Error code indicating exception occurred while updating user details.
        /// </summary>
        public const string UpdateUserDetailsAsyncException = "ERR-2000-030";

        /// <summary>
        /// Error code indicating updating user details.
        /// </summary>
        public const string UpdateUserDetailsAsyncError = "ERR-2000-031";

        /// <summary>
        /// Error code indicating checking duplicate email or username
        /// </summary>
        public const string GetUserDetailsMailUsernameExceptCurrentIdAsyncError = "ERR-2000-032";

        /// <summary>
        /// Error code indicating SQL exception occurred while checking duplicate email or username
        /// </summary>
        public const string GetUserDetailsMailUsernameExceptCurrentIdAsyncSqlException = "ERR-2000-033";

        /// <summary>
        /// Error code indicating exception occurred while checking duplicate email or username
        /// </summary>
        public const string GetUserDetailsMailUsernameExceptCurrentIdAsyncException = "ERR-2000-034";

        /// <summary>
        /// This error code indicates that there was an error while attempting to hard delete users asynchronously.
        /// </summary>
        public const string HardDeleteUsersAsyncError = "ERR-2000-035";

        /// <summary>
        /// This error code indicates that a SQL exception occurred while attempting to hard delete users asynchronously.
        /// </summary>
        public const string HardDeleteUserAsyncSqlException = "ERR-2000-036";

        /// <summary>
        /// This error code indicates that an unexpected exception occurred while attempting to hard delete users asynchronously.
        /// </summary>
        public const string HardDeleteUserAsyncException = "ERR-2000-037";

        /// <summary>
        /// Error code for the RestoreSoftDeletedUserAsync method when the user does not exist.
        /// </summary>
        public const string RestoreSoftDeletedUsersAsyncError = "ERR-2000-038";

        /// <summary>
        /// Error code for the RestoreSoftDeletedUserAsync method when a SQL exception occurs.
        /// </summary>
        public const string RestoreSoftDeletedUserAsyncSqlException = "ERR-2000-039";

        /// <summary>
        /// Error code for the RestoreSoftDeletedUserAsync method when a generic exception occurs.
        /// </summary>
        public const string RestoreSoftDeletedUserAsyncException = "ERR-2000-040";

        /// <summary>
        /// Error code for the unhandled exception
        /// </summary>
        public const string RestoreSoftDeletedUserAsyncUnhandledException = "ERR-2000-041";

        /// <summary>
        /// Error code indicating a validation error occurred in User Management.
        /// </summary>
        public const string UserManagementModelValidationError = "ERR-2000-900";

        /// <summary>
        /// Error code indicating a internal server error in User Management.
        /// </summary>
        public const string UserManagementInternalServerError = "ERR-2000-901";

        /// <summary>
        /// Error code indicating a model is in invalid state in User Management.
        /// </summary>
        public const string UserManagementInvalidModelRequestError = "ERR-2000-902";

        /// <summary>
        /// Error code indicating a bad request in User Management.
        /// </summary>
        public const string UserManagementBadRequestError = "ERR-2000-903";

        /// <summary>
        /// Error code indicating key pair generation in User Management.
        /// </summary>
        public const string UserManagementGenerateRsaKeyPairError = "ERR-2000-904";

        /// <summary>
        /// Error indicating unknown error in User Management.
        /// </summary>
        public const string UserManagementUnknownError = "ERR-2000-999";

        #endregion
    }
}
