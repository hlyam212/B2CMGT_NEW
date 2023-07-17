using OracleAttribute.Attributes;
using System.Reflection;

namespace OracleAttribute.Extensions
{
    public static class TableNameExtensions
    {
        public static string ToTableName<T>()
        {
            Type modelType = typeof(T);
            TableNameAttribute? tableAttr = modelType.GetCustomAttribute<TableNameAttribute>(true);
            return (tableAttr == null) ? modelType.Name : tableAttr.Name;
        }
    }
}
