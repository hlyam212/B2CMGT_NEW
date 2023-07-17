using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHelper.Nlog
{
    public interface ILog
    {


        /// <summary>
        /// Info
        /// </summary>
        /// <param name="logString">寫入文字</param>
        /// <returns></returns>
        public void Info(string logString);


    }
}
