using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using service.Core.Entities.Utility;
using service.Core.Interfaces.Utility;
using service.Infrastructure.Dependency;
using System.Data;

namespace service.Application.Utility
{
    /// <summary>
    /// Handle Common Database Operations
    /// </summary>
    public class CommonDbHander(IConfiguration configuration, ILogger<CommonDbHander> log) : ICommonDbHander
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        private readonly ILogger<CommonDbHander> _log = log;

        /// <summary>
        /// Connection String from appsettings.json
        /// </summary>
        public IDbConnection MSSQLConnection => new SqlConnection(_connectionString);

        /// <summary>
        /// This For Add and Return id
        /// </summary>
        /// <param name="query"> database query </param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="duplicateRecordError"> duplicate error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters </param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AddDataReturnLatestId(string? query, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            BaseResponse baseResponse = new()
            {
                Txn = $"{txn}"
            };
            using (var cn = MSSQLConnection)
            {
                try
                {
                    cn.Open();
                    using (IDbTransaction tr = cn.BeginTransaction())
                    {
                        baseResponse.Status = await cn.ExecuteScalarAsync<int>(query!, param, tr).ConfigureAwait(false);
                        tr.Commit();
                    }
                    if (!string.IsNullOrEmpty(succMsg))
                    {
                        if (logType == null)
                        {
                            _log.LogInformation("Operation succeeded: {SuccessMessage}", succMsg);
                        }
                        else if (logType == "debug")
                        {
                            _log.LogDebug("Debugging: {SuccessMessage}", succMsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    baseResponse.ErrorCode = errorCode;
                    if (ex.GetBaseException() is SqlException sqlException)
                    {
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            throw new CustomException($"{duplicateRecordError}", ex, errorCode, txn);
                        }
                        else
                        {
                            throw new CustomException($"{errMsg}", ex, errorCode, txn);
                        }
                    }
                    else
                    {
                        throw new CustomException($"{errMsg}", ex, errorCode, txn);
                    }
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
            return baseResponse;
        }

        /// <summary>
        /// This For Add , Update and Delete Without Return id
        /// </summary>
        /// <param name="query"> database query </param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="duplicateRecordError"> duplicate error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters </param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AddUpdateDeleteData(string? query, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            BaseResponse baseResponse = new()
            {
                Txn = $"{txn}"
            };

            using (var cn = MSSQLConnection)
            {
                try
                {
                    cn.Open();
                    using (IDbTransaction tr = cn.BeginTransaction())
                    {
                        baseResponse.Status = await cn.ExecuteAsync(query!, param, tr).ConfigureAwait(false);
                        if (baseResponse.Status > 0)
                        {
                            baseResponse.SuccessMessage = succMsg;
                        }
                        else
                        {
                            baseResponse.ErrorMessage = errMsg;
                        }
                        tr.Commit();
                    }
                    if (!string.IsNullOrEmpty(succMsg))
                    {
                        if (logType == null)
                        {
                            _log.LogInformation("Operation succeeded: {SuccessMessage}", succMsg);
                        }
                        else if (logType == "debug")
                        {
                            _log.LogDebug("Debugging: {SuccessMessage}", succMsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    baseResponse.ErrorCode = errorCode;
                    if (ex.GetBaseException() is SqlException sqlException)
                    {
                        if (sqlException.Number == 2627 || sqlException.Number == 2601)
                        {
                            throw new CustomException($"{duplicateRecordError}", ex, errorCode, txn);
                        }
                        else
                        {
                            throw new CustomException($"{errMsg}", ex, errorCode, txn);
                        }
                    }
                    else
                    {
                        throw new CustomException($"{errMsg}", ex, errorCode, txn);
                    }
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
            return baseResponse;
        }

        /// <summary>
        /// get single  data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">database query</param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters</param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public async Task<List<T>> GetData<T>(string? query, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            List<T> res = [];
            using (IDbConnection cn = MSSQLConnection)
            {
                try
                {
                    cn.Open();
                    using (IDbTransaction tr = cn.BeginTransaction())
                    {
                        res = (await cn.QueryAsync<T>(query!, param, tr).ConfigureAwait(false)).ToList();
                        tr.Commit();
                    }
                    if (res.Count > 0 && !string.IsNullOrEmpty(succMsg))
                    {
                        if (logType == null)
                        {
                            _log.LogInformation("Operation succeeded: {SuccessMessage}", succMsg);
                        }
                        else if (logType == "debug")
                        {
                            _log.LogDebug("Debugging: {SuccessMessage}", succMsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new CustomException(errMsg!, ex, errorCode, txn);
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
            return res;
        }

        /// <summary>
        /// get list of data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<T> GetSingleData<T>(string? query, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            T? res;
            using (IDbConnection cn = MSSQLConnection)
            {
                try
                {
                    cn.Open();
                    using (IDbTransaction tr = cn.BeginTransaction())
                    {
                        res = (await cn.QueryAsync<T>(query!, param, tr).ConfigureAwait(false)).SingleOrDefault();
                        tr.Commit();
                    }
                    if (!string.IsNullOrEmpty(succMsg))
                    {
                        if (logType == null)
                        {
                            _log.LogInformation("Operation succeeded: {SuccessMessage}", succMsg);
                        }
                        else if (logType == "debug")
                        {
                            _log.LogDebug("Debugging: {SuccessMessage}", succMsg);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new CustomException(errMsg!, ex, errorCode, txn);
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }
            return res!;
        }
    }
}
