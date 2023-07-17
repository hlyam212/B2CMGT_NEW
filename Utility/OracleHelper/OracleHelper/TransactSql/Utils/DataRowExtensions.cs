using EncryptionHelper;
using System.Data;
using System.Reflection;

namespace OracleHelper.TransactSql.Utils
{
    public static class DataRowExtensions
    {
        public static object? SetValue(this DataRow dr, Type type, DataColumnCollection columns)
        {
            object? item = Activator.CreateInstance(type);

            IList<PropertyInfo> properties = type.GetProperties().ToList();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass
                    && property.PropertyType.Assembly.FullName == type.Assembly.FullName)
                {
                    Type nodeType = property.PropertyType;
                    object? nodeItem = dr.SetValue(nodeType, columns);

                    property.SetValue(
                           item,
                           nodeItem,
                           null);
                }
                else
                {
                    string propertyName = property.Name;
                    Utils.AttributeExtensions.GetDbColumnAttrName(property, ref propertyName);

                    bool matchCloumn = columns.Contains(propertyName);

                    if (matchCloumn)
                    {
                        property.SetValue(
                            item,
                            dr.ConvertValue(property, propertyName),
                            null);
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// (DataRow) 針對欄位的型態去轉換
        /// </summary>
        public static dynamic? ConvertValue(this DataRow dr, PropertyInfo property, string propertyName)
        {
            string? itemValue = dr[propertyName]?.ToString();
            if (Utils.AttributeExtensions.IsDbEncryption(property))
            {
                // 解密
                itemValue = itemValue.AesDecrypt(Config.OraConfigOptions.EncryptionKey);
            }

            if (property.PropertyType == typeof(DateTime) ||
                property.PropertyType == typeof(DateTime?))
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
            else if (property.PropertyType == typeof(decimal) ||
                property.PropertyType == typeof(decimal?))
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
            else if (property.PropertyType == typeof(double) ||
                property.PropertyType == typeof(double?))
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
            else if (property.PropertyType == typeof(int) ||
                property.PropertyType == typeof(int?))
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
            else if (property.PropertyType == typeof(bool) ||
                property.PropertyType == typeof(bool?))
            {
                if (bool.TryParse(itemValue, out bool val) ||
                    dr[propertyName] == DBNull.Value)
                {
                    return val;
                }
                else
                {
                    string? tmpVal = (string?)dr[propertyName];
                    val = (tmpVal != null && (tmpVal.ToUpper() == "Y" || tmpVal.ToUpper() == "V")) ? true : false;

                    return val;
                }
            }
            else if (property.PropertyType.IsEnum)
            {
                var allEnums = Enum.GetValues(property.PropertyType).Cast<Enum>().ToList()
                    .Select(enm => new
                    {
                        enm,
                        dbValue = property.PropertyType.GetMember(enm.ToString())[0].GetCustomAttributesData().Where(x => x.AttributeType.Name == "DbValueAttribute").SelectMany(x => x.ConstructorArguments).Select(x => x.Value).FirstOrDefault()
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

                if (Enum.IsDefined(property.PropertyType, itemValue))
                {
                    var val = Enum.Parse(property.PropertyType, itemValue, true);

                    return val;
                }

                return null;
            }
            else
            {
                if (dr[propertyName] != DBNull.Value)
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
