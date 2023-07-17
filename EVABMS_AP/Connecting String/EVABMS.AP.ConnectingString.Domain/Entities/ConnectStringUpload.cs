using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityHelper;

namespace EVABMS.AP.ConnectingString.Domain.Entities
{
    public class ConnectStringUpload : BaseEntities
    {
        public string filecontent { get; private set; }

        public ConnectStringUpload() { }

        [JsonConstructor]
        public ConnectStringUpload(string filecontent)
        {
            this.filecontent = filecontent;
        }

        public static ConnectStringUpload Create(string filecontent)
        {
            ConnectStringUpload model = new ConnectStringUpload(filecontent);
            return model;
        }
    }
}
