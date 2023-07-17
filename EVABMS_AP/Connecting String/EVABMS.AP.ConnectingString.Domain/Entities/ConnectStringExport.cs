using Newtonsoft.Json;
using System.Collections.Generic;
using UtilityHelper;

namespace EVABMS.AP.ConnectingString.Domain.Entities
{
    public  class ConnectStringExport: BaseEntities
    {
        public string userID { get; private set; }
        public List<ConnectingStringQuery> newmodel { get; private set; }

        public ConnectStringExport() { }

        [JsonConstructor]
        public ConnectStringExport(string userID, List<ConnectingStringQuery> newmodel)
        {
            this.userID = userID;
            this.newmodel = newmodel;
        }

        public static ConnectStringExport Create(string userID, List<ConnectingStringQuery> newmodel)
        {
            ConnectStringExport model = new ConnectStringExport(userID, newmodel);
            return model;
        }
    }
}
