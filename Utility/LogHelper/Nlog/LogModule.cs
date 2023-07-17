using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Microsoft.Extensions.DependencyInjection;

namespace LogHelper.Nlog
{
    public class LogModule :ILog
    {
        //取得預設logConfig設定
        private static Logger logger = LogManager.GetLogger("ErrorInfo");

        public void Info(string logString)
        {
            try
            {
                logger.Info(logString);
            }
            catch (Exception ex) {
                string err = ex.Message;
            }                        
        }


    }
}
