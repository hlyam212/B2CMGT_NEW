using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OracleHelper.TransactSql.Utils
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 判斷Model是否使用DbTableAttribute
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <returns>true:Model has DbTableAttribute; false:Model without DbTableAttribute</returns>
        public static bool ProcessWithDbAttr<T>()
        {
            var attributeList = typeof(T).GetCustomAttributes(true);
            if (attributeList.Length == 0)
            {
                return false;
            }
            var dbTableAttr = attributeList.Where(attr => attr.GetType().Name == "DbTableAttribute")
                                           .FirstOrDefault();
            return dbTableAttr != null;
        }
        public static bool ProcessWithDbAttr(this PropertyInfo propertyInfo)
        {
            var attributeList = propertyInfo.PropertyType.GetCustomAttributes(true);
            if (attributeList.Length == 0)
            {
                return false;
            }
            var dbTableAttr = attributeList.Where(attr => attr.GetType().Name == "DbTableAttribute")
                                           .FirstOrDefault();
            return dbTableAttr != null;
        }

        /// <summary>
        /// DbTableAttribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        public static void GetDbTableAttrName<T>(ref string tableName)
        {
            if (ProcessWithDbAttr<T>())
            {
                var tableAttrName = typeof(T).GetCustomAttributesData()
                    .Where(x => x.AttributeType.Name == "DbTableAttribute")
                    .Select(x => x.ConstructorArguments)
                    .SelectMany(x => x).Select(x => x.Value)
                    .FirstOrDefault();
                if (tableAttrName != null)
                {
                    tableName = (string)tableAttrName;
                }
            }
        }
        /// <summary>
        /// DbTableAttribute - DB Table Name
        /// </summary>
        public static void GetDbTableAttrName(this Type type, ref string tableName)
        {
            if (type.GetCustomAttributes(true).Where(attr => attr.GetType().Name == "DbTableAttribute").Count() > 0)
            {
                var tableAttrName = type.GetCustomAttributesData()
                    .Where(x => x.AttributeType.Name == "DbTableAttribute")
                    .Select(x => x.ConstructorArguments)
                    .SelectMany(x => x).Select(x => x.Value)
                    .FirstOrDefault();
                if (tableAttrName != null)
                {
                    tableName = (string)tableAttrName;
                }
            }
        }
        /// <summary>
        /// DbTableAttribute - DB Table Name
        /// </summary>
        public static void GetDbTableAttrName(this PropertyInfo propertyInfo, ref string tableName)
        {
            if (propertyInfo.ProcessWithDbAttr())
            {
                var tableAttrName = propertyInfo.PropertyType.GetCustomAttributesData()
                    .Where(x => x.AttributeType.Name == "DbTableAttribute")
                    .Select(x => x.ConstructorArguments)
                    .SelectMany(x => x).Select(x => x.Value)
                    .FirstOrDefault();
                if (tableAttrName != null)
                {
                    tableName = (string)tableAttrName;
                }
            }
        }

        /// <summary>
        /// DbColumn Attribute
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="propertyName"></param>
        public static void GetDbColumnAttrName(this PropertyInfo propertyInfo, ref string propertyName)
        {
            var dbColumnAttr = propertyInfo.GetCustomAttributesData()
                .Where(x => x.AttributeType.Name == "DbColumnAttribute")
                .ToList();
            if (dbColumnAttr.Count == 0)
            {
                return;
            }

            var columnName = dbColumnAttr.SelectMany(x => x.ConstructorArguments)
                                         .Select(x => x.Value)
                                         .FirstOrDefault();
            if (columnName != null)
            {
                propertyName = (string)columnName;
            }
        }

        /// <summary>
        /// (DbEncryption Attribute) 該資料是否需要進行加解密
        /// </summary>
        public static bool IsDbEncryption(this PropertyInfo propertyInfo)
        {
            var dbEncryptionAttr = propertyInfo.GetCustomAttributesData()
                .Where(x => x.AttributeType.Name == "DbEncryptionAttribute")
                .ToList();

            return (dbEncryptionAttr.Count > 0);
        }
    }
}
