using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Drawing;
using Newtonsoft.Json;

public static class Extension
{
    /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
    /// <summary>
    /// 當變數可能會是null 轉string 可以用ToSafeString 會回傳string.Empty
    /// </summary>
    /// <returns></returns>
    public static String ToSafeString(this object SourceObject)
    {
        return (SourceObject ?? string.Empty).ToString();
    }

    /// 執行Trim, 若為null時則回傳string.Empty
    /// <summary>
    /// 執行Trim, 若為null時則回傳string.Empty
    /// </summary>
    /// <param name="Source"></param>
    /// <returns></returns>
    public static String SafeTrim(this string Source)
    {
        return (Source ?? string.Empty).Trim();
    }

    /// 執行PadRight, 若為null時則先轉換成string.Empty
    /// <summary>
    /// 執行PadRight, 若為null時則先轉換成string.Empty
    /// </summary>
    /// <param name="Source"></param>
    /// <param name="totalWidth"></param>
    /// <returns></returns>
    public static String SafePadRight(this string Source, int totalWidth)
    {
        return (Source ?? string.Empty).PadRight(totalWidth);
    }

    /// 執行Upper, 若為null時則回傳string.Empty
    /// <summary>
    /// 執行Upper, 若為null時則回傳string.Empty
    /// </summary>
    /// <param name="Source"></param>
    /// <returns></returns>
    public static String SafeToUpper(this string Source)
    {
        return (Source ?? string.Empty).ToUpper();
    }

    /// 執行Upper, 若為null時則回傳string.Empty
    /// <summary>
    /// 執行Upper, 若為null時則回傳string.Empty
    /// </summary>
    /// <param name="Source"></param>
    /// <returns></returns>
    public static String SafeToLower(this string Source)
    {
        return (Source ?? string.Empty).ToLower();
    }

    /// string轉Int32 轉失敗會回傳0
    /// <summary>
    /// string轉Int32 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static Int32 ToInt32(this string sourceString)
    {
        int retVal = 0;
        int.TryParse(sourceString, out retVal);
        return retVal;
    }

    /// string轉Int64 轉失敗會回傳0
    /// <summary>
    /// string轉Int64 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static Int64 ToInt64(this string sourceString)
    {
        int retVal = 0;
        int.TryParse(sourceString, out retVal);
        return retVal;
    }

    /// double轉Int32 轉失敗會回傳0
    /// <summary>
    /// double轉Int32 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static Int32 ToInt32(this double sourceDouble)
    {
        if (double.IsNaN(sourceDouble))
        {
            return 0;
        }
        else if (double.IsInfinity(sourceDouble))
        {
            return 0;
        }
        else
        {
            return Convert.ToInt32(sourceDouble);
        }


    }

    /// string轉double 轉失敗會回傳0
    /// <summary>
    /// string轉double 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static double ToDouble(this string sourceString)
    {
        double retVal = 0;
        double.TryParse(sourceString, out retVal);
        return retVal;
    }

    /// string轉double 轉失敗會回傳0
    /// <summary>
    /// string轉double 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static decimal ToDecimal(this object sourceString)
    {
        decimal retVal = 0;
        decimal.TryParse(sourceString.ToSafeString(), out retVal);
        return retVal;
    }

    /// double?轉double 轉失敗會回傳0
    /// <summary>
    /// double?轉double 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static double ToDouble(this double? source)
    {
        double retVal = (source.HasValue) ? source.Value : 0;
        return retVal;
    }

    /// decimal?轉double 轉失敗會回傳0
    /// <summary>
    /// decimal?轉double 轉失敗會回傳0
    /// </summary>
    /// <returns></returns>
    public static double ToDouble(this decimal? source)
    {
        double retVal = (source.HasValue) ? Convert.ToDouble(source.Value) : 0;
        return retVal;
    }

    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// <summary>
    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// </summary>
    /// <returns></returns>
    public static DateTime ToDateTime(this string sourceString)
    {
        DateTime retVal = DateTime.MinValue;
        DateTime.TryParse(sourceString, out retVal);
        return retVal;
    }

    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// <summary>
    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// </summary>
    /// <returns></returns>
    public static DateTime ToDateTime<T>(this T sourceString)
    {
        string phreasingStr = sourceString == null ? "" : sourceString.ToString();
        DateTime retVal = DateTime.MinValue;
        DateTime.TryParse(phreasingStr, out retVal);
        return retVal;
    }

    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// <summary>
    /// string轉DateTime 轉失敗會回傳DateTime.MinValue
    /// </summary>
    /// <param name="sourceString"></param>
    /// <param name="format">格式</param>
    /// <returns></returns>
    public static DateTime ToDateTime(this string sourceString, string format)
    {
        DateTime retVal = DateTime.MinValue;
        DateTime.TryParseExact(sourceString, format, null, System.Globalization.DateTimeStyles.None, out retVal);
        return retVal;
    }

    /// int?轉Double ,null會回傳0
    /// <summary>
    /// int?轉Double ,null會回傳0
    /// </summary>
    /// <returns></returns>
    public static double ToDouble(this int? source)
    {
        return source ?? 0;
    }

    /// <summary>
    /// 轉JSON
    /// </summary>
    /// <typeparam name="T">IList</typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToJson<T>(this IList<T> list)
    {
        return JsonConvert.SerializeObject(list);
    }

    /// <summary>
    /// 轉JSON
    /// </summary>
    /// <typeparam name="T">IEnumerable</typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToJson<T>(this IEnumerable<T> list)
    {
        return JsonConvert.SerializeObject(list);
    }

    /// <summary>
    /// 轉JSON
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string ToJson<T>(this T list)
    {
        return JsonConvert.SerializeObject(list);
    }


    /// <summary>
    /// 表示指定的型別 是 null 或 沒有包含任何項目
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable == null || !enumerable.Any();
    }

    /// <summary>
    /// IsNullOrEmpty==False
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static bool HasValue<T>(this IEnumerable<T> enumerable)
    {
        return IsNullOrEmpty<T>(enumerable) == false;
    }

    /// <summary>
    /// 表示指定的型別 不是null 和 有包含項目
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> enumerable)
    {
        return enumerable != null && enumerable.Any();
    }

    /// <summary>
    /// 表示指定的型別 不是null 和 有包含項目
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <returns></returns>
    public static bool IsAny<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        return enumerable != null && enumerable.Any(predicate);
    }

    /// 判斷兩個生效期間是否重疊
    /// <summary>
    /// 判斷兩個生效期間是否重疊
    /// </summary>
    /// <param name="AEffective">第一組日期的生效日</param>
    /// <param name="AExpired">第一組日期的失效日</param>
    /// <param name="BEffective">第二組日期的生效日</param>
    /// <param name="BExpired">第二組日期的失效日</param>
    /// <returns></returns>
    public static bool DateIntervalDuplicateCheck(DateTime AEffective, DateTime? AExpired, DateTime BEffective, DateTime? BExpired)
    {
        AExpired = (AExpired == null || AExpired.Value == DateTime.MinValue) ? DateTime.MaxValue : AExpired;
        BExpired = (BExpired == null || BExpired.Value == DateTime.MinValue) ? DateTime.MaxValue : BExpired;

        //生效日不能大於失效日
        if (AEffective > AExpired || BEffective > BExpired)
        {
            return false;
        }

        //True:有重疊;False:沒有重疊;
        bool result = ((AExpired < BEffective) || (BExpired < AEffective)) == false;

        return result;
    }

    #region string 轉 Enum
    /// <summary>
    /// 取得指定Text的Enum
    /// </summary>
    /// <typeparam name=""></typeparam>
    /// <returns></returns>
    public static T GetAssignEnum<T>(string type)
        where T : struct
    {
        var result = Activator.CreateInstance<T>();
        if (!typeof(T).IsEnum || string.IsNullOrEmpty(type))
        {
            return result;
        }

        foreach (T Production in Enum.GetValues(typeof(T)))
        {
            //FieldInfo fi = Production.GetType().GetField(Production.ToString());
            if (Production.ToString() == type)
            {
                result = Production;
                break;
            }
        }
        return result;
    }


    /// <summary>
    /// 取得指定Attribute的Enum，如果沒指定參數則判斷Text
    /// </summary>
    /// <typeparam name=""></typeparam>
    /// <returns></returns>
    public static T GetAssignEnum<T, TAttr>(string type, Func<TAttr, string> expr)
        where T : struct
        where TAttr : Attribute
    {
        var result = Activator.CreateInstance<T>();
        if (!typeof(T).IsEnum || string.IsNullOrEmpty(type))
        {
            return result;
        }

        foreach (T Production in Enum.GetValues(typeof(T)))
        {
            FieldInfo fi = Production.GetType().GetField(Production.ToString());
            var attributes = fi.GetCustomAttributes<TAttr>(false).ToArray();
            string val = (attributes != null && attributes.Length > 0) ? expr(attributes.First()) : Production.ToString();
            if (val == type)
            {
                result = Production;
                break;
            }
        }
        return result;
    }
    #endregion

    /// <summary>
    /// base 64字串格式的圖片轉成Image物件
    /// </summary>
    /// <param name="base64String"></param>
    /// <returns></returns>
    public static Image Base64StringToImage(string base64String)
    {
        // Convert base 64 string to byte[]
        byte[] Buffer = Convert.FromBase64String(base64String);

        byte[] data = null;
        Image oImage = null;
        MemoryStream oMemoryStream = null;
        Bitmap oBitmap = null;
        //建立副本
        data = (byte[])Buffer.Clone();
        try
        {
            oMemoryStream = new MemoryStream(data);
            //設定資料流位置
            oMemoryStream.Position = 0;
            oImage = System.Drawing.Image.FromStream(oMemoryStream);
            //建立副本
            oBitmap = new Bitmap(oImage);
        }
        catch
        {
            throw;
        }
        finally
        {
            oMemoryStream.Close();
            oMemoryStream.Dispose();
            oMemoryStream = null;
        }
        //return oImage;
        return oBitmap;
    }

    public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            // Convert Image to byte[]
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();

            // Convert byte[] to base 64 string
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }

    /// DB Model 和 ViewModel 轉換
    /// <summary>
    /// DB Model 和 ViewModel 轉換
    /// </summary>
    /// <typeparam name="T1">來源型別</typeparam>
    /// <typeparam name="T2">目的型別</typeparam>
    /// <param name="model">來源資料</param>
    /// <returns></returns>
    public static T2 ModelConvert<T1, T2>(T1 model) where T2 : new()
    {
        T2 newModel = new T2();
        if (model != null)
        {
            foreach (PropertyInfo itemInfo in model.GetType().GetProperties())
            {
                PropertyInfo propInfoT2 = typeof(T2).GetProperty(itemInfo.Name);
                if (propInfoT2 != null)
                {
                    // 型別相同才可轉換
                    if (propInfoT2.PropertyType == itemInfo.PropertyType)
                    {
                        propInfoT2.SetValue(newModel, itemInfo.GetValue(model, null), null);
                    }
                }
            }
        }
        return newModel;
    }

    /// <summary>
    /// Convert List T1 type to List T2 type
    /// </summary>
    /// <typeparam name="T1">T1 type</typeparam>
    /// <typeparam name="T2">T2 type</typeparam>
    /// <param name="model">T1 type model</param>
    /// <returns>T2 type</returns>
    public static List<T2> ModelConvert<T1, T2>(List<T1> model) where T2 : new()
    {
        List<T2> lstT2 = new List<T2>();
        if (model.IsAny())
        {
            foreach (T1 item in model)
            {
                T2 newModel = ModelConvert<T1, T2>(item);
                lstT2.Add(newModel);
            }
        }
        return lstT2;
    }
}