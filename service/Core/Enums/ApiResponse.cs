namespace service.Core.Enums
{
    /// <summary>
    /// Represents the status of an API response.
    /// </summary>
    public enum ApiResponseStatus
    {
        /// <summary>
        /// Indicates that the API call was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates that the API call failed.
        /// </summary>
        Failure,

        /// <summary>
        /// Indicates that the API call is pending.
        /// </summary>
        Pending
    }

    /// <summary>
    /// Represents a standard API response structure.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets the status of the API response.
        /// </summary>
        public ApiResponseStatus Status { get; set; }
        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// Gets or sets the response code specific to the API.
        /// </summary>
        public int ResponseCode { get; set; }
        /// <summary>
        /// Gets or sets a success message if the API call was successful.
        /// </summary>
        public string? SuccessMessage { get; set; }
        /// <summary>
        /// Gets or sets an error message if the API call failed.
        /// </summary>
        public string? ErrorMessage { get; set; }
        /// <summary>
        /// Gets or sets an error code if the API call failed.
        /// </summary>
        public string? ErrorCode { get; set; }
        /// <summary>
        /// Gets or sets the transaction identifier.
        /// </summary>
        public string? Txn { get; set; }
        /// <summary>
        /// Gets or sets the return value of the API response.
        /// </summary>
        public T? ReturnValue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class with specified parameters.
        /// </summary>
        /// <param name="status">The status of the API response.</param>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <param name="responseCode">The response code specific to the API.</param>
        /// <param name="successMessage">A success message if applicable.</param>
        /// <param name="errorMessage">An error message if applicable.</param>
        /// <param name="errorCode">An error code if applicable.</param>
        /// <param name="txn">The transaction identifier.</param>
        /// <param name="returnValue">The return value of the API response.</param>
        public ApiResponse(ApiResponseStatus status, int statusCode, int responseCode, string? successMessage = null, string? errorMessage = null, string? errorCode = null, string? txn = null, T? returnValue = default)
        {
            Status = status;
            StatusCode = statusCode;
            ResponseCode = responseCode;
            SuccessMessage = successMessage;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            Txn = txn;
            ReturnValue = returnValue;
        }
    }
}
