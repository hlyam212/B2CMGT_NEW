using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 針對時間進行處理，不論是Datetime、String型態
    /// </summary>
    public static class DatetimeExtensions
    {
        /// <summary>
        ///  時間轉換函數
        /// </summary>
        /// <param name="value">轉換對象，可能為Null的Datetime格式</param>
        /// <param name="format">欲轉換的日期時間格式</param>
        /// <param name="cultureInfo">欲轉換的日期時間區域</param>
        /// <returns></returns>
        public static string ConvertString(this DateTime? value, string format, string cultureInfo = "en-US")
        {
            if (value == (DateTime?)null) return "";

            return ((DateTime)value).ToString(format, CultureInfo.CreateSpecificCulture(cultureInfo));
        }

        /// <summary>
        ///  時間轉換函數
        /// </summary>
        /// <param name="value">轉換對象，不為Null的Datetime格式</param>
        /// <param name="format">欲轉換的日期時間格式</param>
        /// <param name="cultureInfo">欲轉換的日期時間區域</param>
        /// <returns></returns>
        public static string ConvertString(this DateTime value, string format, string cultureInfo = "en-US")
        {
            return value.ToString(format, CultureInfo.CreateSpecificCulture(cultureInfo));
        }

        /// <summary>
        ///  時間轉換函數
        /// </summary>
        /// <param name="value">轉換對象，為string格式</param>
        /// <param name="format">欲轉換的日期時間格式</param>
        /// <param name="isExactly">如果inputFormat的時間已經有明確的年份，則為true</param>
        /// <param name="cultureInfo">欲轉換的日期時間區域</param>
        /// <returns></returns>
        public static DateTime ConvertDatetime(this string value, string format, bool isExactly = true, string cultureInfo = "en-US")
        {
            try
            {
                DateTime convertedDatetime = DateTime.ParseExact(value, format, CultureInfo.CreateSpecificCulture(cultureInfo));
                if (!isExactly)
                {
                    if (convertedDatetime < DateTime.Today)
                    {
                        convertedDatetime = convertedDatetime.AddYears(1);
                    }
                }

                return convertedDatetime;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 時間轉換函數
        /// </summary>
        /// <param name="date">日期字串</param>
        /// <param name="convertType">轉換型態</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static System.DateTime ConvertToDatetime(this String date, string convertType, CultureInfo? cultureInfo = null)
        {


            if (cultureInfo == null)
            {
                DateTime rvdate = new DateTime();
                if (DateTime.TryParseExact(date, convertType, null, System.Globalization.DateTimeStyles.None, out rvdate))
                {
                    return rvdate;
                }
                else
                {
                    throw new Exception("ConvertToDatetime convertType Error");
                }
            }
            else {
                DateTime rvdate = new DateTime();
                if (DateTime.TryParseExact(date, convertType, cultureInfo, System.Globalization.DateTimeStyles.None, out rvdate))
                {
                    return rvdate;
                }
                else
                {
                    throw new Exception("ConvertToDatetime convertType Error");
                }
            }

        }
        /// <summary>
        ///  時間轉換函數
        /// </summary>
        /// <param name="value">轉換對象，為string格式</param>
        /// <param name="format">欲轉換的日期時間格式</param>
        /// <param name="isExactly">如果inputFormat的時間已經有明確的年份，則為true</param>
        /// <param name="cultureInfo">欲轉換的日期時間區域</param>
        /// <returns></returns>
        public static string ConvertDatetimeFormat(this string value, string oriFormat, string format, bool isExactly = true, string cultureInfo = "en-US")
        {
            try
            {
                DateTime convertedDatetime = DateTime.ParseExact(value, oriFormat, CultureInfo.CreateSpecificCulture(cultureInfo));
                if (!isExactly)
                {
                    if (convertedDatetime < DateTime.Today)
                    {
                        convertedDatetime = convertedDatetime.AddYears(1);
                    }
                }

                return convertedDatetime.ToString(format, CultureInfo.CreateSpecificCulture(cultureInfo));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 要比較的時間與當天時間差距天數
        /// </summary>
        /// <param name="value">要比較的時間</param>
        /// <param name="format">要比較的時間格式</param>
        /// <param name="cultureInfo">欲轉換的日期時間區域</param>
        /// <returns></returns>
        public static int DayDifferUtilNow(this string value, string format, string cultureInfo = "en-US")
        {
            DateTime targetDate = DateTime.ParseExact(value, format, CultureInfo.CreateSpecificCulture(cultureInfo));
            TimeSpan differDateTime = targetDate.Subtract(DateTime.Today);
            return differDateTime.Days;
        }

        /// <summary>
        /// 時間轉換函數
        /// </summary>
        /// <param name="TimeInput">要修改的時間</param>
        /// <param name="InputFormat">進來的時間格式</param>
        /// <param name="OutputFormat">出去的時間格式</param>
        /// <param name="OutputFormat">自動轉換成大於今天的年份</param>
        /// <remarks></remarks>
        public static string DateTimeType(this string TimeInput, string InputFormat, string OutputFormat, bool AfterToday = true)
        {
            try
            {
                DateTime A = DateTime.ParseExact(TimeInput, InputFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                if (AfterToday)
                {
                    while (A < DateTime.Today)
                    {
                        A = A.AddYears(1);
                    }
                }
                string StrDate = A.ToString(OutputFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                return StrDate;
            }
            catch (Exception ex)
            {
                throw new Exception("DateTimeType Fail");
            }
        }

        public static bool CheckDateRange(string startDate, string endDate, string format)
        {
            DateTime start = startDate.ConvertToDatetime(format);
            DateTime end = endDate.ConvertToDatetime(format);

            return (start < end) || (start == end);
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
    }
}
