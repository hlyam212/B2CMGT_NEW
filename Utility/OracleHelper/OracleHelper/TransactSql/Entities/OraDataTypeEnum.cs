using Oracle.ManagedDataAccess.Client;
using System.Reflection.Metadata;

namespace OracleHelper.TransactSql
{
    public enum OraDataType
    {
        Varchar2,
        Date,
        Int64,
        Int32,
        Int16,
        Byte,
        Decimal,
        Single,
        Double,
        Blob,
        Clob,
        Char,
        NVarchar2,
        NChar,
        Long,
        NClob,
        Number,
    }

    public static class OraDataTypeUtils
    {
        #region 自行指定Oracle Data Type
        public static OracleDbType Convert(this OraDataType oraDataType)
        {
            if (oraDataType == OraDataType.Varchar2) return OracleDbType.Varchar2;
            if (oraDataType == OraDataType.Date) return OracleDbType.Date;
            if (oraDataType == OraDataType.Int64) return OracleDbType.Int64;
            if (oraDataType == OraDataType.Int32) return OracleDbType.Int32;
            if (oraDataType == OraDataType.Int16) return OracleDbType.Int16;
            if (oraDataType == OraDataType.Byte) return OracleDbType.Byte;
            if (oraDataType == OraDataType.Decimal) return OracleDbType.Decimal;
            if (oraDataType == OraDataType.Single) return OracleDbType.Single;
            if (oraDataType == OraDataType.Double) return OracleDbType.Double;
            if (oraDataType == OraDataType.Blob) return OracleDbType.Blob;
            if (oraDataType == OraDataType.Clob) return OracleDbType.Clob;
            if (oraDataType == OraDataType.Char) return OracleDbType.Char;
            if (oraDataType == OraDataType.NVarchar2) return OracleDbType.NVarchar2;
            if (oraDataType == OraDataType.NChar) return OracleDbType.NChar;
            if (oraDataType == OraDataType.Long) return OracleDbType.Long;
            if (oraDataType == OraDataType.NClob) return OracleDbType.NClob;
            if (oraDataType == OraDataType.Number) return OracleDbType.Decimal;
            throw new OracleHelperException("CreateParameter Set OracleDbType Fail");
        }
        #endregion

        #region 透過傳入的參數判斷
        public static OraDataType OraType(this string data)
        {
            return OraDataType.Varchar2;
        }
        public static OraDataType OraTypeN(this string data)
        {
            return OraDataType.NVarchar2;
        }
        public static OraDataType OraType(this DateTime data)
        {
            return OraDataType.Date;
        }
        public static OraDataType OraType(this Int64 data)
        {
            return OraDataType.Int64;
        }
        public static OraDataType OraType(this Int32 data)
        {
            return OraDataType.Int32;
        }
        public static OraDataType OraType(this Int16 data)
        {
            return OraDataType.Int16;
        }
        public static OraDataType OraType(this Byte data)
        {
            return OraDataType.Byte;
        }
        public static OraDataType OraType(this Decimal data)
        {
            return OraDataType.Decimal;
        }
        public static OraDataType OraType(this Single data)
        {
            return OraDataType.Single;
        }
        public static OraDataType OraType(this Double data)
        {
            return OraDataType.Double;
        }
        public static OraDataType OraType(this Blob data)
        {
            return OraDataType.Blob;
        }
        public static OraDataType OraType(this Char data)
        {
            return OraDataType.Char;
        }
        public static OraDataType OraTypeN(this Char data)
        {
            return OraDataType.NChar;
        }
        public static OraDataType OraTypeL(this Int64 data)
        {
            return OraDataType.Long;
        }
        public static OraDataType GetValueOf<T>(this T oraParameter)
        {
            return oraParameter.GetType().GetValueOf();
        }
        public static OraDataType GetValueOf(this Type type)
        {
            if (type == typeof(string)) return OraDataType.Varchar2;
            if (type == typeof(DateTime)) return OraDataType.Date;
            if (type == typeof(Int64)) return OraDataType.Int64;
            if (type == typeof(Int32)) return OraDataType.Int32;
            if (type == typeof(Int16)) return OraDataType.Int16;
            if (type == typeof(Byte)) return OraDataType.Byte;
            if (type == typeof(Decimal)) return OraDataType.Decimal;
            if (type == typeof(Single)) return OraDataType.Single;
            if (type == typeof(Double)) return OraDataType.Double;
            if (type == typeof(Blob)) return OraDataType.Blob;
            if (type == typeof(Char)) return OraDataType.Char;

            return OraDataType.Varchar2;
        }
        #endregion

        #region 轉換
        /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
        /// <summary>
        /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
        /// </summary>
        /// <returns></returns>
        public static String ToSafeString(this object SourceObject)
        {
            return (SourceObject ?? string.Empty).ToString();
        }

        /// string轉DateTime 轉失敗會回傳DateTime.MinValue
        /// <summary>
        /// string轉DateTime 轉失敗會回傳DateTime.MinValue
        /// </summary>
        /// <returns></returns>
        public static DateTime ToDateTime<T>(this T sourceString)
        {
            string phreasingStr = sourceString == null ? "" : sourceString.ToSafeString();
            DateTime retVal = DateTime.MinValue;
            DateTime.TryParse(phreasingStr, out retVal);
            return retVal;
        }
        #endregion
    }
}
