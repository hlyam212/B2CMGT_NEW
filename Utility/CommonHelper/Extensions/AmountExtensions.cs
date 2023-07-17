using CommonHelper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper.Extensions
{
    /// <summary>
    /// 與金額有關的轉換
    /// </summary>
    public static class AmountExtensions
    {
        public static decimal ToRound(this decimal amount, string currency)
        {
            int exponent = 2;
            if (currency.ExistInEnum<CurrencyDecimal>())
            {
                exponent = (int)currency.ToEnum<CurrencyDecimal>();
            }

            return decimal.Round(amount, exponent);
        }

        public static string ToRoundString(this decimal amount, string currency)
        {
            int exponent = 2;
            if (currency.ExistInEnum<CurrencyDecimal>())
            {
                exponent = (int)currency.ToEnum<CurrencyDecimal>();
            }

            string outputFormat = $@"f{exponent}";

            return decimal.Round(amount, exponent).ToString(outputFormat);
        }

        public static string MoneyFormat<T>(this T amount, string currency)
        {
            return Convert.ToDecimal(amount).MoneyFormat(currency);
        }

        public static string MoneyFormat(this decimal amount, string currency)
        {
            int exponent = 2;
            if (currency.ExistInEnum<CurrencyDecimal>())
            {
                exponent = (int)currency.ToEnum<CurrencyDecimal>();
            }

            string outputFormat = $@"C{exponent}";

            return decimal.Round(amount, exponent).ToString(outputFormat).RegexReplace(@"NT\$", "");
        }
    }
}
