using service.Core.Entities.Utility;

namespace service.Core.Interfaces.Utility
{
    /// <summary>
    /// Defines the interface for handling HTTP requests with custom success, error messages, and codes.
    /// </summary>
    public interface IWebRequestHandler
    {
        /// <summary>
        /// Sends an asynchronous GET request to the specified URL with custom success, error messages, and codes.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="successMessage">The success message to be returned in the response.</param>
        /// <param name="errorMessage">The error message to be returned in the response.</param>
        /// <param name="errorCode">The error code to be returned in the response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an HTTP response containing the response data.</returns>
        Task<HttpBaseResponse<T>> GetAsync<T>(string url, string successMessage, string errorMessage, string errorCode);

        /// <summary>
        /// Sends an asynchronous POST request to the specified URL with the given data, custom success, error messages, and codes.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="successMessage">The success message to be returned in the response.</param>
        /// <param name="errorMessage">The error message to be returned in the response.</param>
        /// <param name="errorCode">The error code to be returned in the response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an HTTP response containing the response data.</returns>
        Task<HttpBaseResponse<T>> PostAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode);

        /// <summary>
        /// Sends an asynchronous PUT request to the specified URL with the given data, custom success, error messages, and codes.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="successMessage">The success message to be returned in the response.</param>
        /// <param name="errorMessage">The error message to be returned in the response.</param>
        /// <param name="errorCode">The error code to be returned in the response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an HTTP response containing the response data.</returns>
        Task<HttpBaseResponse<T>> PutAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode);

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified URL with the given data, custom success, error messages, and codes.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to send in the request body.</param>
        /// <param name="successMessage">The success message to be returned in the response.</param>
        /// <param name="errorMessage">The error message to be returned in the response.</param>
        /// <param name="errorCode">The error code to be returned in the response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an HTTP response containing the response data.</returns>
        Task<HttpBaseResponse<T>> PatchAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode);

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified URL with custom success, error messages, and codes.
        /// </summary>
        /// <typeparam name="T">The type of the expected response.</typeparam>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="successMessage">The success message to be returned in the response.</param>
        /// <param name="errorMessage">The error message to be returned in the response.</param>
        /// <param name="errorCode">The error code to be returned in the response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is an HTTP response containing the response data.</returns>
        Task<HttpBaseResponse<T>> DeleteAsync<T>(string url, string successMessage, string errorMessage, string errorCode);
    }
}