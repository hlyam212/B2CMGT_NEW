using OracleAttribute.Attributes;

namespace OracleAttribute.Extensions
{
    public static class DataValueExtensions
    {
        public static string ToDbValue(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attr = field.GetCustomAttributes(typeof(DataValueAttribute), true).SingleOrDefault() as DataValueAttribute;
                if (attr != null)
                {
                    return attr.Value;
                }
            }
            return value.ToString();
        }

        public static object ToDbValue(this Type propertyType, object value)
        {
            var field = propertyType.GetField(value.ToString());
            if (field != null)
            {
                var attr = field.GetCustomAttributes(typeof(DataValueAttribute), true).SingleOrDefault() as DataValueAttribute;
                if (attr != null)
                {
                    return attr.Value;
                }
            }
            return value.ToString();
        }

        /// <summary>
        /// 取得 從 Database 之值轉換成指定列舉
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">字串值</param>
        /// <returns>T.</returns>
        public static T FromDbValue<T>(this string value) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            else
            {
                var type = typeof(T);
                var type2 = Nullable.GetUnderlyingType(type);
                if (type2 != null && type2.IsEnum)//Nullable類別不能直接用
                {
                    type = type2;
                }
                var val = from d in
                              (from T enm in Enum.GetValues(type)//取得所有項目
                               select new//取得項目和他的DbValue
                               {
                                   enm,
                                   Attr = type.GetMember(enm.ToString())[0].GetCustomAttributes(typeof(DataValueAttribute), true).SingleOrDefault() as DataValueAttribute
                               })
                          where (d.Attr != null ? d.Attr.Value : d.ToString()) == value
                          select d.enm;
                return val.FirstOrDefault();
            }
        }
    }
}
