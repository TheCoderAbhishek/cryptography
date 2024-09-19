using Newtonsoft.Json;
using Serilog;
using service.Core.Entities.Utility;
using service.Core.Interfaces.Utility;
using System.Text;

namespace service.Application.Service.Utility
{
    /// <summary>
    /// Class that implements IWebRequestHandler interface and provides methods to send HTTP requests.
    /// </summary>
    /// <param name="httpClient"></param>
    public class WebRequestHandler(HttpClient httpClient) : IWebRequestHandler
    {
        private readonly HttpClient _httpClient = httpClient;

        /// <summary>
        /// Sends an HTTP request asynchronously and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="request">The HttpRequestMessage object representing the request.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        private async Task<HttpBaseResponse<T>> SendRequestAsync<T>(HttpRequestMessage request, string successMessage, string errorMessage, string errorCode)
        {
            var response = new HttpBaseResponse<T>();

            try
            {
                var httpResponse = await _httpClient.SendAsync(request);
                response.StatusCode = (int)httpResponse.StatusCode;
                response.IsSuccess = httpResponse.IsSuccessStatusCode;

                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.Content.Headers.ContentType?.MediaType == "application/json")
                {
                    response.ReturnValue = JsonConvert.DeserializeObject<T>(responseContent);
                }
                else
                {
                    response.ReturnValue = (T)(object)responseContent;
                }

                response.SuccessMessage = successMessage;
                Log.Information("{SuccessMessage}: {StatusCode}, {Url}", successMessage, response.StatusCode, request.RequestUri);
            }
            catch (HttpRequestException ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = errorMessage;
                response.ErrorCode = errorCode;
                Log.Error(ex, "{ErrorMessage}: {Url}", errorMessage, request.RequestUri);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = "An unexpected error occurred";
                response.ErrorCode = "GENERAL_ERROR";
                Log.Error(ex, "Unexpected error: {Url}", request.RequestUri);
            }

            return response;
        }

        /// <summary>
        /// Creates a StringContent object with the specified data serialized as JSON.
        /// </summary>
        /// <param name="data">The object to be serialized as JSON.</param>
        /// <returns>A StringContent object containing the JSON representation of the data.</returns>
        private static StringContent CreateJsonContent(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        /// <summary>
        /// Sends an asynchronous GET request to the specified URL and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="url">The URL to send the GET request to.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        public async Task<HttpBaseResponse<T>> GetAsync<T>(string url, string successMessage, string errorMessage, string errorCode)
        {
            return await SendRequestAsync<T>(new HttpRequestMessage(HttpMethod.Get, url), successMessage, errorMessage, errorCode);
        }

        /// <summary>
        /// Sends an asynchronous POST request to the specified URL with the specified data and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="url">The URL to send the POST request to.</param>
        /// <param name="data">The data to send in the POST request body.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        public async Task<HttpBaseResponse<T>> PostAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode)
        {
            var content = CreateJsonContent(data);
            return await SendRequestAsync<T>(new HttpRequestMessage(HttpMethod.Post, url) { Content = content }, successMessage, errorMessage, errorCode);
        }

        /// <summary>
        /// Sends an asynchronous PUT request to the specified URL with the specified data and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="url">The URL to send the PUT request to.</param>
        /// <param name="data">The data to send in the PUT request body.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        public async Task<HttpBaseResponse<T>> PutAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode)
        {
            var content = CreateJsonContent(data);
            return await SendRequestAsync<T>(new HttpRequestMessage(HttpMethod.Put, url) { Content = content }, successMessage, errorMessage, errorCode);
        }

        /// <summary>
        /// Sends an asynchronous PATCH request to the specified URL with the specified data and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="url">The URL to send the PATCH request to.</param>
        /// <param name="data">The data to send in the PATCH request body.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        public async Task<HttpBaseResponse<T>> PatchAsync<T>(string url, object data, string successMessage, string errorMessage, string errorCode)
        {
            var content = CreateJsonContent(data);
            return await SendRequestAsync<T>(new HttpRequestMessage(HttpMethod.Patch, url) { Content = content }, successMessage, errorMessage, errorCode);
        }

        /// <summary>
        /// Sends an asynchronous DELETE request to the specified URL and returns the response as an HttpBaseResponse object.
        /// </summary>
        /// <typeparam name="T">The type of the expected data in the response.</typeparam>
        /// <param name="url">The URL to send the DELETE request to.</param>
        /// <param name="successMessage">The message to log on successful response.</param>
        /// <param name="errorMessage">The message to log on error response.</param>
        /// <param name="errorCode">The error code to return in case of an error.</param>
        /// <returns>An HttpBaseResponse object containing the status code, success flag, return value, success message, error message, and error code.</returns>
        public async Task<HttpBaseResponse<T>> DeleteAsync<T>(string url, string successMessage, string errorMessage, string errorCode)
        {
            return await SendRequestAsync<T>(new HttpRequestMessage(HttpMethod.Delete, url), successMessage, errorMessage, errorCode);
        }
    }
}
