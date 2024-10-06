namespace service.Core.Entities.Utility
{
    /// <summary>
    /// Represents a generic HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public class HttpBaseResponse<T>
    {
        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets or sets the success message for the response.
        /// </summary>
        public string? SuccessMessage { get; set; }

        /// <summary>
        /// Gets or sets the error message for the response.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error code for the response.
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the return value of the request.
        /// </summary>
        public T? ReturnValue { get; set; }
    }
}
