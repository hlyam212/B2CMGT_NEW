using EncryptionHelper;
using OracleAttribute.Attributes;
using OracleAttribute.Extensions;
using System.Reflection;

namespace OracleHelper.TransactSql
{
    public class OracleBaseService
    {
        public OracleBaseService()
        {
            oraParams = new List<OraParameters>();
        }

        protected List<OraParameters> oraParams { get; set; }
        protected IElectronTrajectory? log { get; set; }

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

        #region 根據指定泛型組成SQL相關資料

        #region Select
        protected void SelectEachPropertyByPrimaryKey(Type modelType, object queryMoodel)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? queryValue = propertyInfo.GetValue(queryMoodel);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                // 在有設定TableNameAttribute的情況下，沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                if (columnAttr == null) continue;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        SelectEachPropertyByPrimaryKey(propertyType, queryMoodel);
                    }
                }

                // 如果Property是Nullable型態，且value = null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && queryValue == null)
                {
                    continue;
                }

                // 在有設定TableNameAttribute的情況下， 掛ColumnNameAttribute的Property
                // 判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True，當作WHERE的條件 (oraWhereSqls)
                if (columnAttr != null && !columnAttr.IsPrimaryKey)
                {
                    continue;
                }

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: queryValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }
        protected void SelectEachPropertyByIndex(Type modelType, object queryMoodel, int indexNumber)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? propertyValue = propertyInfo.GetValue(queryMoodel);

                // 如果prop是Nullable，且value=null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && propertyValue == null)
                {
                    continue;
                }

                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToSafeString()))
                {
                    // 如果值等於Null，就不繼續
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        DeleteEachPropertyByIndex(propertyType, propertyValue, indexNumber);
                    }
                }

                /* 在有設定TableNameAttribute的情況下:
                 * 1. 沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                 * 2. 掛ColumnNameAttribute的Property，且要使用Primary Key為Where Conditions的話，
                 *    判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True
                 */
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && !columnAttr.IsIndex) ||
                    (columnAttr != null && columnAttr.IsIndex && !columnAttr.IndexCombination.Contains(indexNumber)))
                {
                    continue;
                }

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: propertyValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected string SetSelectSql(string tableName)
        {
            // Build the WHERE clause for the update query
            var whereClauses = oraParams.Select(x => $"{x.s_name} = :{x.s_name}");
            var whereClauseString = string.Join(" AND ", whereClauses);
            whereClauseString = string.IsNullOrWhiteSpace(whereClauseString) ? "" : $"WHERE {whereClauseString}";

            // Build the complete update query
            return $@"
                   SELECT * FROM {tableName} 
                   {whereClauseString}".Trim();
        }
        #endregion

        #region Insert
        protected void InsertEachProperty(Type modelType, object insertMoodel)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? propertyValue = propertyInfo.GetValue(insertMoodel);
                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToSafeString()))
                {
                    // 如果值等於Null，就不繼續
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        InsertEachProperty(propertyType, propertyValue);
                    }
                }

                // 在有設定TableNameAttribute的情況下，沒有掛DbColumnAttr的prop，則不處理這個欄位
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && columnAttr.onlyQuery)) continue;

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: propertyValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected string SetInsertSql(string tableName)
        {
            var parameterNames = string.Join(", ", oraParams.Select(x => x.s_name));
            var parameterPlaceholders = string.Join(", ", oraParams.Select(x => $":{x.s_name}"));

            return $@"
                   INSERT INTO {tableName} ({parameterNames}) 
                   VALUES ({parameterPlaceholders})".Trim();
        }
        #endregion

        #region Update
        protected void UpdateEachProperty(Type modelType, object updateMoodel, object originalModel, ref List<string> oraSetSqls)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? modifiedValue = propertyInfo.GetValue(updateMoodel);
                object? originalValue = propertyInfo.GetValue(originalModel);

                // 如果Property是Nullable型態，且value = null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && modifiedValue == null)
                {
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        UpdateEachProperty(propertyType, modifiedValue, originalValue, ref oraSetSqls);
                    }
                }

                // 在有設定TableNameAttribute的情況下，沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && columnAttr.onlyQuery)) continue;

                string columnName = (tableAttr != null && columnAttr != null) ? columnAttr.Name : propertyInfo.Name;

                // 判斷值是否有改變，針對要改變的值，處理oraSetSqls
                if (Equals(modifiedValue, originalValue)) { continue; }

                oraSetSqls.Add(columnName);

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: modifiedValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected void UpdateEachPropertyByPrimaryKey(Type modelType, object updateMoodel, object originalModel, ref List<string> oraSetSqls, ref List<string> oraWhereSqls)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? modifiedValue = propertyInfo.GetValue(updateMoodel);
                object? originalValue = propertyInfo.GetValue(originalModel);

                // 如果Property是Nullable型態，且value = null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && modifiedValue == null)
                {
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        UpdateEachPropertyByPrimaryKey(propertyType, modifiedValue, originalValue, ref oraSetSqls, ref oraWhereSqls);
                    }
                }

                // 在有設定TableNameAttribute的情況下，沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && columnAttr.onlyQuery)) continue;

                string columnName = (tableAttr != null && columnAttr != null)
                    ? columnAttr.Name
                    : propertyInfo.Name;

                // 在有設定TableNameAttribute的情況下， 掛ColumnNameAttribute的Property
                // 判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True，當作WHERE的條件 (oraWhereSqls)
                if (columnAttr != null && columnAttr.IsPrimaryKey)
                {
                    oraWhereSqls.Add(columnName);
                }

                // 判斷值是否有改變，針對要改變的值，處理oraSetSqls
                if (!Equals(modifiedValue, originalValue))
                {
                    oraSetSqls.Add(columnName);
                }
                // 如果值沒有改變，且該property不需要當作where條件，則跳過不處理
                else if (!oraWhereSqls.Contains(columnName))
                {
                    continue;
                }

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: modifiedValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected void UpdateEachPropertyByIndex(Type modelType, object updateMoodel, object originalModel, int indexNumber, ref List<string> oraSetSqls, ref List<string> oraWhereSqls)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? modifiedValue = propertyInfo.GetValue(updateMoodel);
                object? originalValue = propertyInfo.GetValue(originalModel);

                // 如果prop是Nullable，且value=null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null &&
                    modifiedValue == null)
                {
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        UpdateEachPropertyByIndex(propertyType, modifiedValue, originalValue, indexNumber, ref oraSetSqls, ref oraWhereSqls);
                    }
                }

                // 在有設定TableNameAttribute的情況下，沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && columnAttr.onlyQuery)) continue;

                string columnName = (tableAttr != null && columnAttr != null) ? columnAttr.Name : propertyInfo.Name;

                // 在有設定TableNameAttribute的情況下， 掛ColumnNameAttribute的Property
                // 判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True，當作WHERE的條件 (oraWhereSqls)
                if (columnAttr != null && columnAttr.IsIndex && columnAttr.IndexCombination.Contains(indexNumber))
                {
                    oraWhereSqls.Add(columnName);
                }

                // 判斷值是否有改變，針對要改變的值，處理oraSetSqls
                if (!Equals(modifiedValue, originalValue))
                {
                    oraSetSqls.Add(columnName);
                }
                // 如果值沒有改變，且該property不需要當作where條件，則跳過不處理
                else if (!oraWhereSqls.Contains(columnName))
                {
                    continue;
                }

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: modifiedValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected string SetUpdateSql(string tableName, List<string> oraSetSqls, List<string> oraWhereSqls)
        {
            // Build the SET clause for the update query
            var setClauses = oraSetSqls.Select(oraSetSql => $"{oraSetSql} = :{oraSetSql}");
            var setClauseString = string.Join(",\n", setClauses);

            // Build the WHERE clause for the update query
            var whereClauses = oraWhereSqls.Select(oraWhereSql => $"{oraWhereSql} = :{oraWhereSql}");
            var whereClauseString = string.Join(" AND ", whereClauses);

            // Build the complete update query
            return $@"
                   UPDATE {tableName} 
                   SET {setClauseString}
                   WHERE {whereClauseString}".Trim();
        }
        #endregion

        #region Delete
        protected void DeleteEachProperty(Type modelType, object deleteMoodel, bool byPrimaryKey = true)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? propertyValue = propertyInfo.GetValue(deleteMoodel);

                // 如果prop是Nullable，且value=null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && propertyValue == null)
                {
                    continue;
                }

                // 如果值等於Null，就不繼續
                if (byPrimaryKey &&
                    (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToSafeString())))
                {
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        DeleteEachProperty(propertyType, propertyValue, byPrimaryKey);
                    }
                }

                /* 在有設定TableNameAttribute的情況下:
                 * 1. 沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                 * 2. 掛ColumnNameAttribute的Property，且要使用Primary Key為Where Conditions的話，
                 *    判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True
                 */
                if ((tableAttr != null && columnAttr == null) ||
                    (byPrimaryKey && columnAttr != null && !columnAttr.IsPrimaryKey) ||
                    (columnAttr != null && columnAttr.onlyQuery)) continue;

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: propertyValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected void DeleteEachPropertyByIndex(Type modelType, object deleteMoodel, int indexNumber)
        {
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo propertyInfo in modelType.GetProperties().Cast<PropertyInfo>())
            {
                object? propertyValue = propertyInfo.GetValue(deleteMoodel);

                // 如果prop是Nullable，且value=null，則不處理這個欄位
                if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && propertyValue == null)
                {
                    continue;
                }

                if (propertyValue == null || string.IsNullOrEmpty(propertyValue.ToSafeString()))
                {
                    // 如果值等於Null，就不繼續
                    continue;
                }

                ColumnNameAttribute? columnAttr = propertyInfo.GetCustomAttribute<ColumnNameAttribute>(true);

                Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                // 針對Property中有Class進行檢查
                if (tableAttr != null && columnAttr == null &&
                    propertyType.IsClass &&
                    propertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;
                    if (tableName == propertyTableName)
                    {
                        DeleteEachPropertyByIndex(propertyType, propertyValue, indexNumber);
                    }
                }

                /* 在有設定TableNameAttribute的情況下:
                 * 1. 沒有掛ColumnNameAttribute的Property，則不處理這個欄位
                 * 2. 掛ColumnNameAttribute的Property，且要使用Primary Key為Where Conditions的話，
                 *    判斷Property是否有掛ColumnNameAttribute，且IsPrimaryKey必須為True
                 */
                if ((tableAttr != null && columnAttr == null) ||
                    (columnAttr != null && !columnAttr.IsIndex) ||
                    (columnAttr != null && columnAttr.IsIndex && !columnAttr.IndexCombination.Contains(indexNumber)) ||
                    (columnAttr != null && columnAttr.onlyQuery))
                {
                    continue;
                }

                SetOraParames(
                    tableName: tableName,
                    propertyInfo: propertyInfo,
                    propertyValue: propertyValue,
                    tableAttr: tableAttr,
                    columnAttr: columnAttr);
            }
        }

        protected string SetDeleteSql(string tableName)
        {
            // Build the WHERE clause for the delete query
            var whereClauseString = string.Join(" AND ", oraParams.Select(x => $"{x.s_name} = :{x.s_name}"));

            // Build the complete delete query
            return $@"
                   DELETE {tableName} 
                   WHERE {whereClauseString}".Trim();
        }
        #endregion

        #endregion

        #region SetOraParameters
        private void SetOraParames(string tableName, PropertyInfo propertyInfo, object? propertyValue, TableNameAttribute? tableAttr, ColumnNameAttribute? columnAttr)
        {
            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            // 判斷Enum是否為DataValueAttribute
            if (propertyType.IsEnum && Enum.IsDefined(propertyType, propertyValue.ToSafeString()))
            {
                propertyValue = propertyType.ToDbValue(propertyValue);
            }

            string columnName = (columnAttr != null) ? columnAttr.Name : propertyInfo.Name;

            if (propertyType == typeof(DateTime))
            {
                SetOraParameters(columnName, propertyValue.ToDateTime(), propertyType.GetValueOf());
            }
            else
            {
                string itemValue = propertyValue.ToSafeString();
                if (columnAttr.IsEncrypted)
                {
                    if (string.IsNullOrEmpty(Config.OraConfigOptions.EncryptionKey))
                    {
                        throw new OracleHelperException($@"Missing Encryption Password for {columnName} cloumn in the {tableName} Table.");
                    }
                    itemValue = itemValue.AesEncrypt(Config.OraConfigOptions.EncryptionKey);
                }

                SetOraParameters(columnName, itemValue, propertyType.GetValueOf());
            }
        }
        protected void SetOraParames(PropertyInfo propertyInfo, object? propertyValue)
        {
            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            string columnName = propertyInfo.Name;

            if (propertyType == typeof(DateTime))
            {
                SetOraParameters(columnName, propertyValue.ToDateTime(), propertyType.GetValueOf());
            }
            else
            {
                SetOraParameters(columnName, propertyValue.ToSafeString(), propertyType.GetValueOf());
            }
        }
        #endregion
    }
}
