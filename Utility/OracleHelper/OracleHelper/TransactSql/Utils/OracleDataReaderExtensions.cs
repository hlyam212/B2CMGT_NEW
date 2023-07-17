using EncryptionHelper;
using Oracle.ManagedDataAccess.Client;
using OracleAttribute.Attributes;
using System.Data;
using System.Reflection;

namespace OracleHelper.TransactSql.Utils
{
    public static class OracleDataReaderExtensions
    {
        public static dynamic? SetValue(this OracleDataReader reader, Type modelType, List<string> columns)
        {
            var item = Activator.CreateInstance(modelType);

            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            string tableName = (tableAttr == null) ? modelType.Name : tableAttr.Name;

            foreach (PropertyInfo property in modelType.GetProperties().Cast<PropertyInfo>())
            {
                if (property.PropertyType.IsClass
                    && property.PropertyType.Assembly.FullName == modelType.Assembly.FullName)
                {
                    Type propertyType = property.PropertyType;

                    TableNameAttribute? propertyTableAttr = propertyType.GetCustomAttribute<TableNameAttribute>(true);
                    string propertyTableName = (propertyTableAttr == null) ? propertyType.Name : propertyTableAttr.Name;

                    if (tableName == propertyTableName)
                    {
                        Type nodeType = property.PropertyType;

                        var nodeItem = reader.SetValue(nodeType, columns);

                        property.SetValue(
                               item,
                               nodeItem,
                               null);
                    }
                }
                else
                {
                    ColumnNameAttribute? columnAttr = property.GetCustomAttribute<ColumnNameAttribute>(true);
                    string propertyName = (columnAttr != null) ? columnAttr.Name : property.Name;

                    bool matchCloumn = columns.Contains(propertyName);

                    if (matchCloumn)
                    {
                        property.SetValue(
                            item,
                            reader.ConvertValue(property, columnAttr, propertyName),
                            null);
                    }
                }
            }

            return item;
        }

        public static dynamic? ConvertValue(this OracleDataReader reader, PropertyInfo propertyInfo, ColumnNameAttribute? columnAttr, string columnName)
        {
            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
            string? itemValue = reader.GetValue(columnName)?.ToSafeString();

            if (!string.IsNullOrEmpty(itemValue) && columnAttr != null && columnAttr.IsEncrypted)
            {
                try
                {
                    // 解密
                    itemValue = itemValue.AesDecrypt(Config.OraConfigOptions.EncryptionKey);
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(Config.OraConfigOptions.SecondaryEncryptionKey))
                    {
                        itemValue = itemValue.AesDecrypt(Config.OraConfigOptions.SecondaryEncryptionKey);
                    }
                }
            }

            if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("zh-TW", true);
                if (DateTime.TryParseExact(itemValue, "yyyy/M/d tt hh:mm:ss", cultureInfo, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
            {
                if (decimal.TryParse(itemValue, out decimal val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else if (propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(double?))
            {
                if (double.TryParse(itemValue, out double val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else if (propertyInfo.PropertyType == typeof(long) || propertyInfo.PropertyType == typeof(long?))
            {
                if (long.TryParse(itemValue, out long val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
            {
                if (int.TryParse(itemValue, out int val))
                {
                    return val;
                }
                else
                {
                    return null;
                }
            }
            else if (propertyInfo.PropertyType == typeof(bool) || propertyInfo.PropertyType == typeof(bool?))
            {
                if (bool.TryParse(itemValue, out bool val))
                {
                    return val;
                }
                else if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null && reader.GetValue(columnName) == DBNull.Value)
                {
                    return null;
                }
                else
                {

                    return (itemValue != null && (itemValue.ToUpper() == "Y" || itemValue.ToUpper() == "V")) ? true : false;
                }
            }
            else if (propertyInfo.PropertyType.IsEnum || propertyType.IsEnum)
            {
                if (propertyInfo.PropertyType.IsEnum == false && string.IsNullOrEmpty(itemValue))
                {
                    return null;
                }

                var allEnums = Enum.GetValues(propertyType).Cast<Enum>().ToList()
                    .Select(enm => new
                    {
                        enm,
                        dbValue = propertyType.GetMember(enm.ToString())[0].GetCustomAttributesData().Where(x => x.AttributeType.Name == nameof(DataValueAttribute)).SelectMany(x => x.ConstructorArguments).Select(x => x.Value).FirstOrDefault()
                    })
                    .ToList();
                if (allEnums != null && allEnums.Count > 0)
                {
                    var enumValue = allEnums.Where(x => x.dbValue?.ToString() == itemValue)
                        .FirstOrDefault();
                    if (enumValue != null)
                    {
                        return enumValue.enm;
                    }
                }

                if (Enum.IsDefined(propertyType, itemValue))
                {
                    var val = Enum.Parse(propertyType, itemValue, true);

                    return val;
                }

                return null;
            }
            else
            {
                if (reader.GetValue(columnName) != DBNull.Value)
                {
                    return itemValue;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
