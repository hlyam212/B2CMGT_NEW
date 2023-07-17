using EncryptionHelper;
using OracleAttribute.Attributes;
using OracleAttribute.Extensions;
using System.Data;
using System.Reflection;

namespace OracleHelper.TransactSql
{
    public class OracleService : OracleBaseService
    {
        public OracleService() : base() { }

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
        public List<T> Select<T>()
        {
            Type modelType = typeof(T);
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            return Select<T>(oraSql: SetSelectSql(tableName));
        }

        /// <summary>
        /// 用來查詢DB資料,CreateParameter方式傳入欄位參數
        /// </summary>
        /// <typeparam name="T">指定的強行別</typeparam>
        /// <param name="oraSql">Sql語法</param>
        /// <returns>回傳指定強行別的List</returns>
        public List<T> Select<T>(string oraSql) 
        {
            OraDataRepository repository = new OraDataRepository();
            return repository.Get<T>(oraSql, oraParams);
        }

        public List<T> SelectByPrimaryKey<T>(T queryModel) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            SelectEachPropertyByPrimaryKey(modelType, queryModel);

            return Select<T>(
                oraSql: SetSelectSql(tableName));
        }

        public List<T> SelectByIndex<T>(T queryModel, int indexNumber) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            SelectEachPropertyByIndex(modelType, queryModel, indexNumber);

            return Select<T>(
                oraSql: SetSelectSql(tableName));
        }
        #endregion

        #region Insert
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
        public bool Insert<T>(T insertMoodel) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            InsertEachProperty(modelType, insertMoodel);

            return Insert(SetInsertSql(tableName)) == 1;
        }
        #endregion

        #region Update
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
        public int Update<T>(T updateMoodel, T originalModel, T whereCondition) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            #region Set Proprities
            List<string> oraSetSqls = new List<string>();
            UpdateEachProperty(modelType, updateMoodel, originalModel, ref oraSetSqls);
            #endregion

            #region Set Where Conditions
            List<string> oraWhereSqls = new List<string>();
            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? conditionValue = propertyInfo.GetValue(whereCondition, null);

                // 如果Property是Nullable型態，且value = null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null &&
                    conditionValue == null)
                {
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                // 在有設定TableNameAttribute的情況下，沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                if (tableAttr != null && columnAttr == null) continue;

                string columnName = (tableAttr != null && columnAttr != null)
                    ? columnAttr.Name
                    : propertyInfo.Name;

                oraWhereSqls.Add(columnName);

                // 判斷Enum是否為DataValueAttribute
                if (propertyInfo.PropertyType.IsEnum && Enum.IsDefined(propertyInfo.PropertyType, conditionValue.ToSafeString()))
                {
                    conditionValue = propertyInfo.PropertyType.ToDbValue(conditionValue);
                }

                if (propertyInfo.PropertyType == typeof(DateTime))
                {
                    SetOraParameters(columnName, conditionValue.ToDateTime(), propertyInfo.PropertyType.GetValueOf());
                }
                else
                {
                    string itemValue = conditionValue.ToSafeString();
                    if (columnAttr.IsEncrypted)
                    {
                        itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                    }

                    SetOraParameters(columnName, itemValue, propertyInfo.PropertyType.GetValueOf());
                }
            }
            #endregion

            return Update(SetUpdateSql(tableName, oraSetSqls, oraWhereSqls));
        }

        /// <summary>
        /// 用Class Object來修改DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public int UpdateByPrimaryKey<T>(T updateMoodel, T originalModel) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            List<string> oraSetSqls = new List<string>();
            List<string> oraWhereSqls = new List<string>();

            UpdateEachPropertyByPrimaryKey(modelType, updateMoodel, originalModel, ref oraSetSqls, ref oraWhereSqls);

            return Update(SetUpdateSql(tableName, oraSetSqls, oraWhereSqls));
        }

        /// <summary>
        /// 用Class Object來修改DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public int UpdateByIndex<T>(T updateMoodel, T originalModel, int indexNumber) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            List<string> oraSetSqls = new List<string>();
            List<string> oraWhereSqls = new List<string>();

            UpdateEachPropertyByIndex(modelType, updateMoodel, originalModel, indexNumber, ref oraSetSqls, ref oraWhereSqls);

            return Update(SetUpdateSql(tableName, oraSetSqls, oraWhereSqls));
        }
        #endregion

        #region DELETE
        /// <summary>
        /// 修改DB資料
        /// </summary>
        public int Delete(string oraSql)
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
        public int Delete<T>(T deleteMoodel, bool byPrimaryKey = true) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            DeleteEachProperty(modelType, deleteMoodel, byPrimaryKey);

            return Delete(SetDeleteSql(tableName));
        }

        /// <summary>
        /// 用Class Object來修改DB資料
        /// </summary>
        /// <param name = "dbModel" >dbModel</param>
        /// <returns></returns>
        public int DeleteByIndex<T>(T deleteMoodel, int indexNumber) where T : new()
        {
            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            DeleteEachPropertyByIndex(modelType, deleteMoodel, indexNumber);

            return Delete(SetDeleteSql(tableName));
        }
        #endregion

        #endregion
    }
}
