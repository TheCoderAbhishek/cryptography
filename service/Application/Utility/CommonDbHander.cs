using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using service.Core.Enums;
using service.Core.Interfaces.Utility;
using System.Data;
using Dapper;
using service.Core.Entities.Utility;
using service.Infrastructure.Dependency;

namespace service.Application.Utility
{
    /// <summary>
    /// Handle Common Database Operations
    /// </summary>
    public class CommonDbHander : ICommonDbHander
    {
        private static IDbConnection MSSQLConnection => new SqlConnection(ConstantData.ConstantDbConnection);

        /// <summary>
        /// This For Add and Return id
        /// </summary>
        /// <param name="query"> database query </param>
        /// <param name="log">log object</param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="duplicateRecordError"> duplicate error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters </param>
        /// <param name="logType"></param>
        /// <returns></returns>
        public async Task<BaseResponse> AddDataReturnLatestId(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            BaseResponse baseResponse = new();
            baseResponse.Txn = $"{txn}";
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
                    cn.Close();
                    if (!string.IsNullOrEmpty(succMsg) && logType == null)
                    {
                        baseResponse.SuccessMessage = succMsg;
                        log.Information(succMsg);
                    }
                    if (logType == "debug" && !succMsg.IsNullOrEmpty())
                        log.Debug(succMsg!);
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
        /// <param name="log">log object</param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="duplicateRecordError"> duplicate error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters </param>
        /// <param name="logType"></param>
        /// <returns></returns>

        public async Task<BaseResponse> AddUpdateDeleteData(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? duplicateRecordError, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            BaseResponse baseResponse = new();
            baseResponse.Txn = $"{txn}";

            using (var cn = MSSQLConnection)
            {
                try
                {
                    cn.Open();
                    using (IDbTransaction tr = cn.BeginTransaction())
                    {
                        baseResponse.Status = await cn.ExecuteAsync(query!, param, tr).ConfigureAwait(false);
                        tr.Commit();
                    }
                    cn.Close();
                    if (!string.IsNullOrEmpty(succMsg) && logType == null)
                    {
                        baseResponse.SuccessMessage = succMsg;
                        log.Information(succMsg);
                    }
                    if (logType == "debug" && !succMsg.IsNullOrEmpty())
                        log.Debug(succMsg!);
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
        /// <param name="log">log object</param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param">database query parameters</param>
        /// <param name="logType"></param>
        /// <returns></returns>

        public async Task<List<T>> GetData<T>(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null)
        {
            List<T> res = new List<T>();
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
                    cn.Close();
                    if (res.Count > 0 && logType == null && !succMsg.IsNullOrEmpty())
                    {
                        log.Information(succMsg!);
                    }
                    if (logType == "debug" && !succMsg.IsNullOrEmpty())
                        log.Debug(succMsg!);
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
        /// <param name="log"></param>
        /// <param name="succMsg">success message</param>
        /// <param name="errMsg">error message</param>
        /// <param name="errorCode"></param>
        /// <param name="txn">transaction number</param>
        /// <param name="param"></param>
        /// <param name="logType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<T> GetSingleData<T>(string? query, Serilog.ILogger log, string? succMsg, string? errMsg, string? errorCode, string? txn, object? param = null, string? logType = null)
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
                    cn.Close();
                    if (res != null! && logType == null && !succMsg.IsNullOrEmpty())
                    {
                        log.Information(succMsg!);
                    }
                    if (logType == "debug" && !succMsg.IsNullOrEmpty())
                        log.Debug(succMsg!);
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
