using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityHelper
{
    public class BaseEntities
    {
        public string environment = "DVP";
        //public string environment
        //{
        //    get
        //    {
        //        return _environment;
        //    }
        //    set
        //    {
        //        _environment = value;
        //    }
        //}
        public BaseEntities() 
        {
            environment = "DVP";
        }
    }
}
