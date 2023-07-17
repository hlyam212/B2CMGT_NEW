using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using UtilityHelper;

namespace EVABMS.AP.ConnectingString.Domain.Entities
{
    [TableName("MGT_INI_HISTORY")]
    public class ConnectStringDBModel:BaseEntities
    {
        [ColumnName("ID",isPrimaryKey:true)]
        public long id { get; private set; }
        [ColumnName("FILE_NAME")]
        public string file_name { get; private set; }
        [ColumnName("DESCRIPTION")]
        public string description { get; private set; }
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime last_updated_timestamp { get; private set; }
        [ColumnName("LAST_UPDATED_USERID")]
        public string last_updated_userid { get; private set; }
        [ColumnName("ENV")]
        public string? ENV { get; private set; }

        public ConnectStringDBModel() { }

        [JsonConstructor]
        public ConnectStringDBModel(long id, string file_name, string description, DateTime last_updated_timestamp, string last_updated_userid, string? ENV)
        {
            this.id = id;
            this.file_name = file_name;
            this.description = description;
            this.last_updated_userid = last_updated_userid;
            this.last_updated_timestamp = last_updated_timestamp;
            this.ENV = ENV;
        }

        public static ConnectStringDBModel Create(long id, string file_name, string description, DateTime last_updated_timestamp, string last_updated_userid, string? ENV)
        {
            ConnectStringDBModel model = new ConnectStringDBModel(id,file_name,description,last_updated_timestamp,last_updated_userid,ENV);
            return model;
        }

    }
}
