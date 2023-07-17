using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 與enum有關的轉換
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 字串轉成Enum
        /// </summary>
        /// <typeparam name="T">哪一個Enum</typeparam>
        /// <param name="value">Enum的字串</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        /// <summary>
        /// 字串轉成Enum
        /// </summary>
        /// <typeparam name="T">哪一個Enum</typeparam>
        /// <param name="value">Enum的字串</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value, T defaultEnum)
        {
            if (value == null) return defaultEnum;

            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// 判斷該字串是否存在指定的Enum
        /// </summary>
        /// <typeparam name="T">哪一個Enum</typeparam>
        /// <param name="value">Enum的字串</param>
        public static bool ExistInEnum<T>(this string value)
        {
            return Enum.IsDefined(typeof(T), value);
        }

        /// <summary>
        /// 判斷該Int是否存在指定的Enum
        /// </summary>
        /// <typeparam name="T">哪一個Enum</typeparam>
        /// <param name="value">Enum的值</param>
        public static string GetEnumDescription<T>(this int value)
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                return string.Empty;
            }

            var convertEnum = (T)Enum.ToObject(typeof(T), value);
            if (convertEnum == null)
            {
                return string.Empty;
            }

            return convertEnum.ToString().GetEnumDescription<T>();
        }

        /// <summary>
        /// 針對客製化的Enum中，取得有使用DescriptionAttribute;
        /// </summary>
        /// <typeparam name="T">哪一個Enum</typeparam>
        /// <param name="value">變數名稱</param>
        /// <returns>Description的內容</returns>
        public static string GetEnumDescription<T>(this string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type)
                            .Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                            .Select(d => d)
                            .FirstOrDefault();

            // 找無相對應的列舉
            if (name == null)
            {
                return string.Empty;
            }

            // 利用反射找出相對應的欄位
            var field = type.GetField(name);
            if (field == null) return name;

            // 取得欄位設定DescriptionAttribute的值
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // 無設定Description Attribute, 回傳Enum欄位名稱
            if (customAttribute == null || customAttribute.Length == 0)
            {
                return name;
            }

            // 回傳Description Attribute的設定
            return ((DescriptionAttribute)customAttribute[0]).Description;
        }

        /// <summary>
        /// 針對客製化的Enum中，取得有使用DescriptionAttribute;
        /// </summary>
        /// <typeparam name="T">客製化的Enum</typeparam>
        /// <param name="source">該Enum變數，不需轉型</param>
        public static string GetEnumDescription(this Enum source, bool returnEnumString = false)
        {
            Type type = source.GetType();
            FieldInfo? fi = type.GetRuntimeField(name: source.ToString());
            if (fi == null) return String.Empty;

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes.First().Description;
            }
            else
            {
                if (returnEnumString == false)
                {
                    // 找無相對應的列舉
                    return string.Empty;
                }
                return source.ToString();
            }
        }

        /// <summary>
        /// 根據指定Type的Property Name，取得它的Description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static string GetDescription<T>(this string objName)
        {
            Type type = typeof(T);
            var objDescription = type.GetProperties().SingleOrDefault(pi => pi.Name == objName);

            if (objDescription != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])objDescription.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes.First().Description;
                }
            }
            // 找無相對應的列舉
            return string.Empty;
        }

        /// <summary>
        /// 根據指定Enum設定的Description，取得對應的第一個Enum
        /// </summary>
        /// <typeparam name="T">Enum</typeparam>
        /// <param name="description">Description內容</param>
        /// <returns></returns>
        public static T? GetEnumFromDescription<T>(this string description, bool returnDefault = false)
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("The type of T is Not Enum.", nameof(description));
            }

            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T?)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T?)field.GetValue(null);
                }
            }

            if (!returnDefault)
            {
                throw new ArgumentException("Not found.", nameof(description));
            }

            return default(T);
        }
    }
}
