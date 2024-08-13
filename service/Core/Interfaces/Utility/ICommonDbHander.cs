using service.Core.Entities.Utility;

namespace service.Core.Interfaces.Utility
{
    /// <summary>
    /// Defines a common interface for database operations.
    /// </summary>
    public interface ICommonDbHander
    {
        /// <summary>
        /// Adds data to the database and returns the latest ID.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="log">The Serilog logger instance.</param>
        /// <param name="succMsg">The success message.</param>
        /// <param name="errMsg">The error message.</param>
        /// <param name="duplicateRecordError">The duplicate record error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="txn">The transaction ID.</param>
        /// <param name="param">Optional parameters for the query.</param>
        /// <param name="logType">The log type.</param>
        /// <returns>A task representing the asynchronous operation, containing a BaseResponse object.</returns>
        Task<BaseResponse> AddDataReturnLatestId(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null);

        /// <summary>
        /// Adds, updates, or deletes data based on the provided query.
        /// </summary>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="log">The Serilog logger instance.</param>
        /// <param name="succMsg">The success message.</param>
        /// <param name="errMsg">The error message.</param>
        /// <param name="duplicateRecordError">The duplicate record error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="txn">The transaction ID.</param>
        /// <param name="param">Optional parameters for the query.</param>
        /// <param name="logType">The log type.</param>
        /// <returns>A task representing the asynchronous operation, containing a BaseResponse object.</returns>
        Task<BaseResponse> AddUpdateDeleteData(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null);

        /// <summary>
        /// Retrieves a list of data based on the provided query.
        /// </summary>
        /// <typeparam name="T">The type of the data to retrieve.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="log">The Serilog logger instance.</param>
        /// <param name="succMsg">The success message.</param>
        /// <param name="errMsg">The error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="txn">The transaction ID.</param>
        /// <param name="param">Optional parameters for the query.</param>
        /// <param name="logType">The log type.</param>
        /// <returns>A task representing the asynchronous operation, containing a list of objects of type T.</returns>
        Task<List<T>> GetData<T>(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null);

        /// <summary>
        /// Retrieves a single data item based on the provided query.
        /// </summary>
        /// <typeparam name="T">The type of the data to retrieve.</typeparam>
        /// <param name="query">The SQL query to execute.</param>
        /// <param name="log">The Serilog logger instance.</param>
        /// <param name="succMsg">The success message.</param>
        /// <param name="errMsg">The error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="txn">The transaction ID.</param>
        /// <param name="param">Optional parameters for the query.</param>
        /// <param name="logType">The log type.</param>
        /// <returns>A task representing the asynchronous operation, containing an object of type T.</returns>
        Task<T> GetSingleData<T>(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null);
    }
}
