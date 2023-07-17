using EncryptionHelper;
using Oracle.ManagedDataAccess.Client;
using OracleAttribute.Attributes;
using OracleAttribute.Extensions;
using System.Reflection;

namespace OracleHelper.TransactSql
{
    public class OracleTransactionService : OracleBaseService
    {
        private OraDataRepository repository;

        private Tuple<OracleConnection, OracleTransaction> transaction { get; set; }
        private List<string> oraSqlList { get; set; }
        private List<List<OraParameters>> oraParamsList { get; set; }

        public OracleTransactionService() : base()
        {
            repository = new OraDataRepository();

            oraSqlList = new List<string>();
            oraParamsList = new List<List<OraParameters>>();
        }

        #region 設置參數
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
        #endregion

        #region 批次性資料庫連線

        #region 自動
        /// <summary>
        /// (自動) 批次性資料庫連線
        /// </summary>
        /// <returns></returns>
        public int SaveAndChange()
        {
            repository = new OraDataRepository();
            repository.BatchPost(oraSqlList, oraParamsList, log);
            return repository.executeCount;
        }
        #endregion

        #region 手動
        private void Start()
        {
            if (transaction == null)
            {
                repository = new OraDataRepository();
                transaction = repository.StartTransaction();
            }
        }

        #region 需組好SQL相關資料
        public List<T> GetTransaction<T>(string oraSql) where T : new()
        {
            Start();
            return repository.GetTransaction<T>(transaction.Item1, transaction.Item2, oraSql, oraParams);
        }

        public int PostTransaction(string oraSql)
        {
            Start();
            repository.PostTransaction(transaction.Item1, transaction.Item2, oraSql, oraParams);

            return repository.executeCount;
        }
        #endregion

        #region 根據指定泛型組成SQL相關資料

        #region Insert
        public List<T> Select<T>(string oraSql) where T : new()
        {
            return GetTransaction<T>(oraSql);
        }
        public List<T> SelectByPrimaryKey<T>(T queryModel) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            SelectEachPropertyByPrimaryKey(modelType, queryModel);

            return repository.GetTransaction<T>(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetSelectSql(tableName),
                oraParams: oraParams);
        }
        public List<T> SelectByIndex<T>(T queryModel, int indexNumber) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            SelectEachPropertyByIndex(modelType, queryModel, indexNumber);

            return repository.GetTransaction<T>(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetSelectSql(tableName),
                oraParams: oraParams);
        }
        #endregion

        #region Insert
        public int Insert(string oraSql)
        {
            return PostTransaction(oraSql);
        }

        public int Insert<T>(T insertMoodel) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            InsertEachProperty(modelType, insertMoodel);

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetInsertSql(tableName),
                oraParams: oraParams);

            return repository.executeCount;
        }
        public int Insert(string tableName, object? insertModel)
        {
            Start();

            oraParams.Clear();

            foreach (PropertyInfo propertyInfo in insertModel.GetType().GetProperties().Cast<PropertyInfo>())
            {
                if (!string.IsNullOrEmpty(propertyInfo.GetValue(insertModel).ToSafeString()))
                {
                    SetOraParames(propertyInfo, propertyInfo.GetValue(insertModel));
                }
            }

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetInsertSql(tableName),
                oraParams: oraParams);

            return repository.executeCount;
        }
        #endregion

        #region Update
        public int Update(string oraSql)
        {
            return PostTransaction(oraSql);
        }

        public int Update<T>(T updateMoodel, T originalModel, T whereCondition) where T : new()
        {
            Start();

            oraParams.Clear();

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

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetUpdateSql(tableName, oraSetSqls, oraWhereSqls),
                oraParams: oraParams);

            return repository.executeCount;
        }

        public int UpdateByPrimaryKey<T>(T updateMoodel, T originalModel) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            List<string> oraSetSqls = new List<string>();
            List<string> oraWhereSqls = new List<string>();

            UpdateEachPropertyByPrimaryKey(modelType, updateMoodel, originalModel, ref oraSetSqls, ref oraWhereSqls);

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetUpdateSql(tableName, oraSetSqls, oraWhereSqls),
                oraParams: oraParams);

            return repository.executeCount;
        }

        public int UpdateByIndex<T>(T updateMoodel, T originalModel, int indexNumber) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;
           
            List<string> oraSetSqls = new List<string>();
            List<string> oraWhereSqls = new List<string>();

            UpdateEachPropertyByIndex(modelType, updateMoodel, originalModel, indexNumber, ref oraSetSqls, ref oraWhereSqls);

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetUpdateSql(tableName, oraSetSqls, oraWhereSqls),
                oraParams: oraParams);

            return repository.executeCount;
        }
        #endregion

        #region Delete
        public int Delete(string oraSql)
        {
            return PostTransaction(oraSql);
        }

        public int Delete<T>(T deleteMoodel, bool byPrimaryKey = true) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            DeleteEachProperty(modelType, deleteMoodel, byPrimaryKey);

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetDeleteSql(tableName),
                oraParams: oraParams);

            return repository.executeCount;
        }

        public int DeleteByIndex<T>(T deleteMoodel, int indexNumber) where T : new()
        {
            Start();

            oraParams.Clear();

            Type modelType = typeof(T);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            DeleteEachPropertyByIndex(modelType, deleteMoodel, indexNumber);

            repository.PostTransaction(
                oraConn: transaction.Item1,
                transaction: transaction.Item2,
                oraSql: SetDeleteSql(tableName),
                oraParams: oraParams);

            return repository.executeCount;
        }
        #endregion

        #endregion

        #region 實際異動資料庫
        public int Commit()
        {
            repository.EndTransaction(transaction.Item1, transaction.Item2);

            transaction = null;

            return repository.executeCount;
        }

        public void Rollback()
        {
            if (transaction != null)
            {
                repository.EndTransaction(transaction.Item1, transaction.Item2, false);
            }

            transaction = null;
        }
        #endregion

        #endregion
        
        #endregion
    }
}
