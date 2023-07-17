using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using OracleHelper.TransactSql.Config;
using OracleHelper.TransactSql.Utils;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace OracleHelper.TransactSql
{
    /// <summary>
    /// 實作處理DB相關請求
    /// </summary>
    public class OraDataRepository
    {
        public OraDataRepository()
        {
            if (OraConfigOptions.OraConnectionString == null)
            {
                throw new OracleHelperException("OraConnectionString is empty.");
            }
        }

        private IElectronTrajectory? electronTrajectory { get; set; }
        /// <summary>
        /// Post成功的筆數
        /// </summary>
        public int executeCount { get; private set; } = 0;

        private Exception oracleException { get; set; }

        #region 系統自動控制DB連線
        /// <summary>
        /// 取得DB資料
        /// </summary>
        public DataTable Get(string oraSql, List<OraParameters>? oraParams)
        {
            DataTable dtResult = new DataTable();
            OracleConnection? oraConn = null;

            try
            {
                // Open a connection
                oraConn = new OracleConnection(OraConfigOptions.OraConnectionString);
                oraConn.Open();

                // Execute simple select statement that returns data from table
                OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);

                if (oraParams != null)
                {
                    foreach (OraParameters oraParam in oraParams)
                    {
                        if (oraParam.s_type == OracleDbType.Date)
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_dateTime;
                        }
                        else
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_value;
                        }
                    }
                }

                using (OracleDataAdapter adapter = new OracleDataAdapter(oraCommand))
                {
                    adapter.SelectCommand = oraCommand;
                    adapter.Fill(dtResult);

                    adapter.Dispose();
                }

                oraCommand.Dispose();
            }
            catch (Exception ex)
            {
                oracleException = ex;
            }
            finally
            {
                EndOraConn(oraConn);

                if (oracleException != null)
                {
                    throw new OracleHelperException(oracleException.Message, oracleException);
                }
            }

            return dtResult;
        }

        /// <summary>
        /// 取得DB資料
        /// </summary>
        public List<T> Get<T>(string oraSql, List<OraParameters>? oraParams)
        {
            List<T> result = new List<T>();
            OracleConnection? oraConn = null;

            try
            {
                IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();

                // Open a connection
                oraConn = new OracleConnection(OraConfigOptions.OraConnectionString);

                // Open a connection
                using (oraConn)
                {
                    // Execute simple select statement that returns data from table
                    OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);

                    if (oraParams != null)
                    {
                        for (int i = 0; i < oraParams.Count; i++)
                        {
                            if (oraParams[i].s_type == OracleDbType.Date)
                            {
                                oraCommand.Parameters.Add(new OracleParameter(oraParams[i].s_name, oraParams[i].s_type)).Value = oraParams[i].s_dateTime;
                            }
                            else
                            {
                                oraCommand.Parameters.Add(new OracleParameter(oraParams[i].s_name, oraParams[i].s_type)).Value = oraParams[i].s_value;
                            }
                        }
                    }

                    oraConn.Open();
                    using (OracleDataReader reader = oraCommand.ExecuteReader())
                    {
                        List<string?> columns = reader.GetSchemaTable().AsEnumerable()
                            .Select(x => x["ColumnName"]?.ToString())
                            .ToList();

                        while (reader.Read())
                        {
                            T item = default(T);

                            #region T is struct
                            if (properties.Count == 0)
                            {
                                object value = reader.GetValue(0);
                                if (value != DBNull.Value)
                                {
                                    item = (T)Convert.ChangeType(value, typeof(T));
                                }
                                result.Add(item);
                                continue;
                            }
                            #endregion

                            #region T is an object
                            var convertItem = reader.SetValue(
                                modelType: typeof(T),
                                columns: columns);
                            if (convertItem is T)
                            {
                                item = (T)Convert.ChangeType(convertItem, typeof(T));
                            }
                            result.Add(item);
                            #endregion
                        }
                    }

                    oraCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                oracleException = new OracleHelperException(ex.Message, ex);
            }
            finally
            {
                EndOraConn(oraConn);

                if (oracleException != null)
                {
                    throw new OracleHelperException(oracleException.Message, oracleException);
                }
            }

            return result;
        }

        /// <summary>
        /// 操作DB資料
        /// </summary>
        public void Post(string oraSql, List<OraParameters> oraParams, IElectronTrajectory? log = null)
        {
            OracleConnection? oraConn = null;
            OracleTransaction? oraTransaction = null;

            try
            {
                // Open a connection
                oraConn = new OracleConnection(OraConfigOptions.OraConnectionString);
                oraConn.Open();

                if (System.Transactions.Transaction.Current == null) oraTransaction = oraConn.BeginTransaction();

                // Execute simple select statement that returns data from table
                OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);
                if (oraTransaction != null) oraCommand.Transaction = oraTransaction;

                foreach (OraParameters x in oraParams)
                {
                    if (x.s_type == OracleDbType.Date)
                    {
                        oraCommand.Parameters.Add(new OracleParameter(x.s_name, x.s_type)).Value = x.s_dateTime;
                    }
                    else
                    {
                        oraCommand.Parameters.Add(new OracleParameter(x.s_name, x.s_type)).Value = x.s_value;
                    }
                }

                executeCount = oraCommand.ExecuteNonQuery();
                oraCommand.Parameters.Clear();

                if (oraTransaction != null) oraTransaction.Commit();

                if (electronTrajectory != null)
                {
                    electronTrajectory.oraSql = oraSql;
                    electronTrajectory.oraParams = oraParams;

                    electronTrajectory.Excute();
                }
            }
            catch (Exception ex)
            {
                if (oraTransaction != null) oraTransaction.Rollback();
                oracleException = ex;
            }
            finally
            {
                if (oraTransaction != null) oraTransaction.Dispose();

                EndOraConn(oraConn);

                if (oracleException != null)
                {
                    throw new OracleHelperException(oracleException.Message, oracleException);
                }
            }
        }

        /// <summary>
        /// 批次操作DB資料
        /// </summary>
        public void BatchPost(List<string> oraSqlList, List<List<OraParameters>> oraParamsList, IElectronTrajectory? log = null)
        {
            OracleConnection? oraConn = null;
            OracleTransaction? oraTransaction = null;
            executeCount = 0;

            try
            {
                // Open a connection
                oraConn = new OracleConnection(OraConfigOptions.OraConnectionString);
                oraConn.Open();

                if (System.Transactions.Transaction.Current == null) oraTransaction = oraConn.BeginTransaction();
                using (oraConn)
                {
                    for (int i = 0; i < oraSqlList.Count; i++)
                    {
                        // Execute simple select statement that returns data from table
                        OracleCommand oraCommand = new OracleCommand(oraSqlList[i], oraConn);
                        if (oraTransaction != null) oraCommand.Transaction = oraTransaction;

                        for (int j = 0; j < oraParamsList[i].Count; j++)
                        {
                            if (oraParamsList[i][j].s_type == OracleDbType.Date)
                            {
                                oraCommand.Parameters.Add(new OracleParameter(oraParamsList[i][j].s_name, oraParamsList[i][j].s_type)).Value = oraParamsList[i][j].s_dateTime;
                            }
                            else
                            {
                                oraCommand.Parameters.Add(new OracleParameter(oraParamsList[i][j].s_name, oraParamsList[i][j].s_type)).Value = oraParamsList[i][j].s_value;
                            }
                        }

                        executeCount += oraCommand.ExecuteNonQuery();
                        oraCommand.Parameters.Clear();
                    }

                    if (oraTransaction != null) oraTransaction.Commit();
                }

                if (electronTrajectory != null)
                {
                    electronTrajectory.oraSqlList = oraSqlList;
                    electronTrajectory.oraParamsList = oraParamsList;

                    electronTrajectory.Excute();
                }
            }
            catch (Exception ex)
            {
                if (oraTransaction != null) oraTransaction.Rollback();
                oracleException = ex;
            }
            finally
            {
                if (oraTransaction != null) oraTransaction.Dispose();

                EndOraConn(oraConn);

                if (oracleException != null)
                {
                    throw new OracleHelperException(oracleException.Message, oracleException);
                }
            }
        }
        #endregion

        #region 自行控制DB連線
        /// <summary>
        /// 自行控制DB連線，Transaction Start
        /// </summary>
        public Tuple<OracleConnection, OracleTransaction> StartTransaction()
        {
            OracleConnection? oraConn = null;

            try
            {
                oraConn = new OracleConnection(OraConfigOptions.OraConnectionString);

                oraConn.Open();

                OracleTransaction oraTransaction = oraConn.BeginTransaction();

                return Tuple.Create(oraConn, oraTransaction);
            }
            catch (Exception ex)
            {
                oracleException = new OracleHelperException(ex.Message, ex);

                EndOraConn(oraConn);

                throw oracleException;
            }
        }

        /// <summary>
        /// 自行控制DB連線，查詢資料庫
        /// </summary>
        public DataTable GetTransaction(OracleConnection oraConn, OracleTransaction transaction, string oraSql, List<OraParameters>? oraParams)
        {
            DataTable dtResult = new DataTable();

            // Execute simple select statement that returns data from table
            OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);
            oraCommand.Transaction = transaction;

            if (oraParams != null)
            {
                foreach (OraParameters oraParam in oraParams)
                {
                    if (oraParam.s_type == OracleDbType.Date)
                    {
                        oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_dateTime;
                    }
                    else
                    {
                        oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_value;
                    }
                }
            }

            using (OracleDataAdapter adapter = new OracleDataAdapter(oraCommand))
            {
                adapter.SelectCommand = oraCommand;
                adapter.Fill(dtResult);
            }

            oraCommand.Dispose();

            return dtResult;
        }

        public List<T> GetTransaction<T>(OracleConnection oraConn, OracleTransaction transaction, string oraSql, List<OraParameters>? oraParams) where T : new()
        {
            List<T> result = new List<T>();

            try
            {
                IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();

                // Execute simple select statement that returns data from table
                OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);
                oraCommand.Transaction = transaction;

                if (oraParams != null)
                {
                    for (int i = 0; i < oraParams.Count; i++)
                    {
                        if (oraParams[i].s_type == OracleDbType.Date)
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParams[i].s_name, oraParams[i].s_type)).Value = oraParams[i].s_dateTime;
                        }
                        else
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParams[i].s_name, oraParams[i].s_type)).Value = oraParams[i].s_value;
                        }
                    }


                    using (OracleDataReader reader = oraCommand.ExecuteReader())
                    {
                        List<string?> columns = reader.GetSchemaTable().AsEnumerable()
                            .Select(x => x["ColumnName"]?.ToString())
                            .ToList();

                        while (reader.Read())
                        {
                            T item = new T();

                            #region T is struct
                            if (properties.Count == 0)
                            {
                                item = (T)Convert.ChangeType(reader, typeof(T));
                                result.Add(item);
                                continue;
                            }
                            #endregion

                            #region T is an object
                            var convertItem = reader.SetValue(
                                modelType: typeof(T),
                                columns: columns);
                            if (convertItem is T)
                            {
                                item = (T)Convert.ChangeType(convertItem, typeof(T));
                            }
                            result.Add(item);
                            #endregion
                        }
                    }

                    oraCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                oracleException = new OracleHelperException(ex.Message, ex);

                EndTransaction(oraConn, transaction, false);
            }
            finally
            {
                if (oracleException != null)
                {
                    throw new OracleHelperException(oracleException.Message, oracleException);
                }
            }

            return result;
        }

        /// <summary>
        /// 自行控制DB連線，新增/修改/刪除資料庫
        /// </summary>
        public void PostTransaction(OracleConnection oraConn, OracleTransaction transaction, string oraSql, List<OraParameters>? oraParams)
        {
            try
            {
                // Execute simple select statement that returns data from table
                OracleCommand oraCommand = new OracleCommand(oraSql, oraConn);
                oraCommand.Transaction = transaction;

                if (oraParams != null)
                {
                    foreach (OraParameters oraParam in oraParams)
                    {
                        if (oraParam.s_type == OracleDbType.Date)
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_dateTime;
                        }
                        else
                        {
                            oraCommand.Parameters.Add(new OracleParameter(oraParam.s_name, oraParam.s_type)).Value = oraParam.s_value;
                        }
                    }
                }

                executeCount = oraCommand.ExecuteNonQuery();
                oraCommand.Parameters.Clear();
            }
            catch (Exception ex)
            {
                oracleException = new OracleHelperException(ex.Message, ex);

                EndTransaction(oraConn, transaction, false);
            }
            finally
            {
                if (oracleException != null)
                {
                    throw oracleException;
                }
            }
        }

        /// <summary>
        /// 自行控制DB連線，Transaction Commit/Rollback
        /// </summary>
        public void EndTransaction(OracleConnection oraConn, OracleTransaction oraTransaction, bool isCommit = true)
        {
            try
            {
                if (isCommit)
                {
                    oraTransaction.Commit();
                }
                else
                {
                    oraTransaction.Rollback();
                }
                oraTransaction.Dispose();
            }
            catch (Exception ex)
            {
                oracleException = new OracleHelperException(ex.Message, ex);

                if (oraTransaction != null)
                {
                    oraTransaction.Rollback();
                }
            }
            finally
            {
                EndOraConn(oraConn);

                if (oracleException != null)
                {
                    throw oracleException;
                }
            }
        }
        #endregion

        private void EndOraConn(OracleConnection oraConn)
        {
            try
            {
                // Close and Dispose OracleConnection object
                oraConn.Close();
                oraConn.Dispose();

                // Clears the connection pool associated with connection 'conn'
                OracleConnection.ClearPool(oraConn);
            }
            catch
            {

            }
        }
    }
}
