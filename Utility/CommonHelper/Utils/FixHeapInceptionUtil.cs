using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    /// <summary>
    /// 修復Heap Inception的功能
    /// </summary>
    public static class FixHeapInceptionUtil
    {
        public static System.Security.SecureString ToSecureString(this string plainString)
        {
            var secureString = new System.Security.SecureString();
            secureString.Clear();
            foreach (char c in plainString.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            return secureString;
        }

        public static System.String fixHeapInspection(this String value)
        {
            return FortifySecurityUtils.CheckString(value);
        }
    }
}
