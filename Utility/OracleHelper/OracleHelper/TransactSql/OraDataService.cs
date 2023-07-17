using EncryptionHelper;
using Newtonsoft.Json;
using OracleHelper.TransactSql.Utils;
using System.Data;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Xml.Serialization;

namespace OracleHelper.TransactSql
{
    public class OraDataService
    {
        public OraDataService()
        {
            oraParams = new List<OraParameters>();

            oraSqlList = new List<string>();
            oraParamsList = new List<List<OraParameters>>();
        }

        private List<OraParameters> oraParams { get; set; }
        private List<string> oraSqlList { get; set; }
        private List<List<OraParameters>> oraParamsList { get; set; }
        private IElectronTrajectory? log { get; set; }

        #region 設置參數
        /// <summary>
        /// (非透過Class Object) 設置參數
        /// </summary>
        /// <param name="name">參數欄位名稱</param>
        /// <param name="value">參數欄位非日期型態的值</param>
        /// <param name="type">參數欄位的型態</param>
        public OraParameters SetOraParameters(string name, string value, OraDataType type)
        {
            OraParameters result = new OraParameters(
                name: name,
                type: type.Convert(),
                value: value);
            oraParams.Add(result);
            return result;
        }

        /// <summary>
        /// (非透過Class Object) 設置參數
        /// </summary>
        /// <param name="name">參數欄位名稱</param>
        /// <param name="value">參數欄位日期的值</param>
        /// <param name="type">參數欄位的型態</param>
        public OraParameters SetOraParameters(string name, DateTime value, OraDataType type)
        {
            OraParameters result = new OraParameters(
                name: name,
                type: type.Convert(),
                dateTime: value);
            oraParams.Add(result);
            return result;
        }
        /// <summary>
        /// (非透過Class Object) 設置參數
        /// </summary>
        /// <param name="name">參數欄位名稱</param>
        /// <param name="value">參數欄位日期的值</param>
        /// <param name="type">參數欄位的型態</param>
        public OraParameters SetOraParameters(string name, DateTime? value, OraDataType type)
        {
            OraParameters result = new OraParameters(
                name: name,
                type: type.Convert(),
                dateTime: value);
            oraParams.Add(result);
            return result;
        }

        /// <summary>
        /// (非透過Class Object) 批次設置參數
        /// </summary>
        /// <param name="oraSql">該批次執行的Sql語法</param>
        public void AddOraParametersList(string oraSql)
        {
            oraSqlList.Add(oraSql);
            var copyParams = new List<OraParameters>(oraParams);
            oraParamsList.Add(copyParams);
            oraParams.Clear();
        }

        /// <summary>
        /// (非透過Class Object) 設置參數
        /// </summary>
        /// <param name="name">參數欄位名稱</param>
        /// <param name="value">參數欄位非日期型態的值</param>
        /// <param name="type">參數欄位的型態</param>
        public OraParameters SetOraParameters(string name, List<string> value, OraDataType type)
        {
            OraParameters result = new OraParameters(
                name: name,
                type: type.Convert());
            oraParams.Add(result);
            return result;
        }
        #endregion

        #region Log
        /// <summary>
        /// Log
        /// </summary>
        public void Log(IElectronTrajectory etlog)
        {
            log = etlog;
        }
        #endregion

        #region 一次性資料庫連線

        #region SELECT
        /// <summary>
        /// 用來查詢DB資料,CreateParameter方式傳入欄位參數
        /// </summary>
        /// <param name="oraSql">Sql語法</param>
        /// <returns>DataTable</returns>
        public DataTable Select(string oraSql)
        {
            OraDataRepository repository = new OraDataRepository();
            return repository.Get(oraSql, oraParams);
        }

        /// <summary>
        /// 用來查詢DB資料,CreateParameter方式傳入欄位參數
        /// </summary>
        /// <typeparam name="T">指定的強行別</typeparam>
        /// <param name="oraSql">Sql語法</param>
        /// <returns>回傳指定強行別的List</returns>
        public List<T> Select<T>(string oraSql, object whereCondition = null) where T : new()
        {
            OraDataRepository repository = new OraDataRepository();

            #region Set Where Conditions
            if (whereCondition != null)
            {
                Type myType = whereCondition.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo prop in props)
                {
                    Type thisPropType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    object propValue = prop.GetValue(whereCondition, null);
                    if (thisPropType == typeof(DateTime))
                    {
                        SetOraParameters(prop.Name, prop.GetValue(whereCondition).ToDateTime(), OraDataTypeUtils.GetValueOf(thisPropType));
                    }
                    else
                    {
                        string itemValue = prop.GetValue(whereCondition).ToSafeString();
                        if (Utils.AttributeExtensions.IsDbEncryption(prop))
                        {
                            // 加密
                            itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                        }

                        SetOraParameters(prop.Name, itemValue, OraDataTypeUtils.GetValueOf(thisPropType));
                    }
                }
            }
            #endregion

            #region T is struct
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            if (properties.Count == 0)
            {
                DataTable dt = repository.Get(oraSql, oraParams);
                List<T> list = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                    }
                }
                return list;
            }
            #endregion

            #region T is an object
            return repository.Get<T>(oraSql, oraParams);
            #endregion
        }
        #endregion

        #region INSERT
        /// <summary>
        /// 新增DB資料
        /// </summary>
        public int Insert(string oraSql)
        {
            OraDataRepository repository = new OraDataRepository();
            repository.Post(oraSql, oraParams, log);
            return repository.executeCount;
        }
        /// <summary>
        /// 用Class Object來新增DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public bool Insert<T>(T dbModel) where T : new()
        {
            bool processWithDbAttr = Utils.AttributeExtensions.ProcessWithDbAttr<T>();
            string tableName = typeof(T).Name;
            Utils.AttributeExtensions.GetDbTableAttrName<T>(ref tableName);

            foreach (var item in typeof(T).GetProperties().Cast<PropertyInfo>())
            {
                Type type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;

                //如果prop是Nullable且value=null,則不處理這個欄位
                if (Nullable.GetUnderlyingType(item.PropertyType) != null && item.GetValue(dbModel) == null) continue;

                string itemName = null;
                if (processWithDbAttr) Utils.AttributeExtensions.GetDbColumnAttrName(item, ref itemName);
                else itemName = item.Name;

                //在processWithDbAttr=true的情況下,沒有掛DbColumnAttr的prop,則不處理這個欄位
                if (string.IsNullOrEmpty(itemName)) continue;

                if (type == typeof(DateTime))
                {
                    SetOraParameters(itemName, item.GetValue(dbModel).ToDateTime(), OraDataTypeUtils.GetValueOf(type));
                }
                else
                {
                    string itemValue = item.GetValue(dbModel).ToSafeString();
                    if (Utils.AttributeExtensions.IsDbEncryption(item))
                    {
                        // 加密
                        itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                    }

                    SetOraParameters(itemName, itemValue, OraDataTypeUtils.GetValueOf(type));

                }
            }

            string mySql = $@"INSERT INTO {tableName} ({string.Join(", \r\n", oraParams.Select(x => x.s_name))}) 
                              VALUES ({string.Join(", \r\n", oraParams.Select(x => ":" + x.s_name))})";

            return Insert(mySql) == 1;
        }
        #endregion

        #region UPDATE
        /// <summary>
        /// 修改DB資料
        /// </summary>
        public int Update(string oraSql)
        {
            OraDataRepository repository = new OraDataRepository();
            repository.Post(oraSql, oraParams, log);
            return repository.executeCount;
        }

        /// <summary>
        /// 用Class Object來修改DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public int Update<T>(T dbModel, object whereCondition) where T : new()
        {
            bool processWithDbAttr = Utils.AttributeExtensions.ProcessWithDbAttr<T>();
            string tableName = typeof(T).Name;
            Utils.AttributeExtensions.GetDbTableAttrName<T>(ref tableName);

            List<OraParameters> setList = new List<OraParameters>();
            List<OraParameters> whereConditionList = new List<OraParameters>();

            #region Set Proprities
            foreach (var item in typeof(T).GetProperties().Cast<PropertyInfo>())
            {
                Type type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType;
                //如果prop是Nullable且value=null,則不處理這個欄位
                if (Nullable.GetUnderlyingType(item.PropertyType) != null && item.GetValue(dbModel) == null)
                {
                    continue;
                }

                string itemName = null;
                if (processWithDbAttr) Utils.AttributeExtensions.GetDbColumnAttrName(item, ref itemName);
                else itemName = item.Name;

                //在processWithDbAttr=true的情況下,沒有掛DbColumnAttr的prop,則不處理這個欄位
                if (string.IsNullOrEmpty(itemName)) continue;

                if (type == typeof(DateTime))
                {
                    setList.Add(SetOraParameters(itemName, item.GetValue(dbModel).ToDateTime(), OraDataTypeUtils.GetValueOf(type)));
                }
                else
                {
                    string itemValue = item.GetValue(dbModel).ToSafeString();
                    if (Utils.AttributeExtensions.IsDbEncryption(item))
                    {
                        // 加密
                        itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                    }

                    setList.Add(SetOraParameters(itemName, itemValue, OraDataTypeUtils.GetValueOf(type)));
                }
            }
            #endregion

            #region Set Where Conditions
            Type myType = whereCondition.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                Type thisPropType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object propValue = prop.GetValue(whereCondition, null);
                if (thisPropType == typeof(DateTime))
                {
                    whereConditionList.Add(SetOraParameters(prop.Name, prop.GetValue(whereCondition).ToDateTime(), OraDataTypeUtils.GetValueOf(thisPropType)));
                }
                else
                {
                    string itemValue = prop.GetValue(whereCondition).ToSafeString();
                    if (Utils.AttributeExtensions.IsDbEncryption(prop))
                    {
                        // 加密
                        itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                    }

                    whereConditionList.Add(SetOraParameters(prop.Name, itemValue, OraDataTypeUtils.GetValueOf(thisPropType)));
                }
            }
            #endregion

            string mySql = $@"UPDATE {tableName}
                              SET {string.Join(", \r\n", setList.Select(x => x.s_name + "= :" + x.s_name))}
                              WHERE {string.Join(", \r\n AND ", whereConditionList.Select(x => x.s_name + "= :" + x.s_name))}";

            return Update(mySql);
        }
        #endregion

        #region DELETE
        /// <summary>
        /// 刪除DB資料
        /// </summary>
        public int Delete(string oraSql)
        {
            OraDataRepository repository = new OraDataRepository();
            repository.Post(oraSql, oraParams, log);
            return repository.executeCount;
        }

        /// <summary>
        /// 用Class Object來刪除DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public int Delete<T>(object dbModel) where T : new()
        {
            string tableName = typeof(T).Name;
            Utils.AttributeExtensions.GetDbTableAttrName<T>(ref tableName);

            Type myType = dbModel.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(dbModel, null);
                if (prop.PropertyType == typeof(DateTime))
                {
                    SetOraParameters(prop.Name, prop.GetValue(dbModel).ToDateTime(), OraDataTypeUtils.GetValueOf(prop.PropertyType));
                }
                else
                {
                    string itemValue = prop.GetValue(dbModel).ToSafeString();
                    if (Utils.AttributeExtensions.IsDbEncryption(prop))
                    {
                        // 加密
                        itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                    }

                    SetOraParameters(prop.Name, itemValue, OraDataTypeUtils.GetValueOf(prop.PropertyType));
                }
            }

            string mySql = $@"DELETE {tableName}
                              WHERE {string.Join(", \r\n AND ", oraParams.Select(x => x.s_name + "= :" + x.s_name))}";

            return Delete(mySql);
        }
        #endregion

        #endregion

        #region 批次性資料庫連線
        /// <summary>
        /// 批次操作DB (Transaction)
        /// </summary>
        public int Transaction()
        {
            OraDataRepository repository = new OraDataRepository();
            repository.BatchPost(oraSqlList, oraParamsList, log);
            return repository.executeCount;
        }
        #endregion

        #region 取得流水號(PK值)
        /// <summary>
        /// 取得DB Sequence No(SequenceName.NEXTVAL)
        /// </summary>
        /// <param name="seqName">DB Sequence Name</param>
        /// <param name="provider">Table Owner</param>
        /// <returns></returns>
        public Int64 GetSeqNo(string seqName)
        {
            if (string.IsNullOrEmpty(seqName))
            {
                return 0;
            }

            string sql = "SELECT " + seqName + ".NEXTVAL AS ID FROM DUAL";
            long result = Select<long>(sql).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// (TABLE_SEQUENCE) 手動控制
        /// </summary>
        public string GetApplyNo(string applyType, string searchIndex, int addNoCount = 1)
        {
            string newApplyNo = string.Empty;

            OraDataRepository repository = new OraDataRepository();
            var st = repository.StartTransaction();

            try
            {
                string oraSql = "SELECT * FROM TABLE_SEQUENCE " +
                "WHERE SEQUENCE_TYPE = :SEQUENCE_TYPE AND " +
                "SEQUENCE_INDEX = :SEQUENCE_INDEX " +
                "ORDER BY SEQUENCE_NUMBER DESC";

                oraParams = new List<OraParameters>();
                SetOraParameters("SEQUENCE_TYPE", applyType, OraDataType.Varchar2);
                SetOraParameters("SEQUENCE_INDEX", searchIndex, OraDataType.Varchar2);

                DataTable dtApplyNo = repository.GetTransaction(st.Item1, st.Item2, oraSql, oraParams);
                if (dtApplyNo == null || dtApplyNo.Rows.Count == 0)
                {
                    newApplyNo = (0 + addNoCount).ToString();

                    oraSql = "INSERT INTO TABLE_SEQUENCE" +
                        "(SEQUENCE_NUMBER,SEQUENCE_TYPE,SEQUENCE_INDEX,LAST_UPDATE_DATE) " +
                        "VALUES(:SEQUENCE_NUMBER,:SEQUENCE_TYPE,:SEQUENCE_INDEX,:LAST_UPDATE_DATE)";
                }
                else
                {
                    string? dbApplyNo = dtApplyNo.AsEnumerable()
                        .Select(x => x["SEQUENCE_NUMBER"].ToString())
                        .FirstOrDefault();
                    if (dbApplyNo == null)
                    {
                        newApplyNo = (0 + addNoCount).ToString();
                    }
                    else
                    {
                        newApplyNo = (int.Parse(dbApplyNo) + addNoCount).ToString();
                    }

                    oraSql = "UPDATE TABLE_SEQUENCE " +
                        "SET SEQUENCE_NUMBER = :SEQUENCE_NUMBER, LAST_UPDATE_DATE = :LAST_UPDATE_DATE " +
                        "WHERE SEQUENCE_TYPE = :SEQUENCE_TYPE AND SEQUENCE_INDEX = :SEQUENCE_INDEX";
                }

                oraParams = new List<OraParameters>();
                SetOraParameters("SEQUENCE_NUMBER", newApplyNo, OraDataType.Number);
                SetOraParameters("SEQUENCE_TYPE", applyType, OraDataType.Varchar2);
                SetOraParameters("SEQUENCE_INDEX", searchIndex, OraDataType.Varchar2);
                SetOraParameters("LAST_UPDATE_DATE", DateTime.Now, OraDataType.Date);

                repository.PostTransaction(st.Item1, st.Item2, oraSql, oraParams);
                repository.EndTransaction(st.Item1, st.Item2);
            }
            catch (Exception ex)
            {
                repository.EndTransaction(st.Item1, st.Item2, false);
                throw new OracleHelperException(ex.Message, ex);
            }
            finally
            {
                oraParams = new List<OraParameters>();
            }

            return newApplyNo;
        }
        #endregion

    }
}
