using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace LogHelper.Nlog
{
    public static class LogExtension
    {
        private static Logger logger = LogManager.GetLogger("ErrorInfo");
        public static void SetLog(this string _value)
        {
            try
            {
                logger.Info(_value);
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }
    }
}
