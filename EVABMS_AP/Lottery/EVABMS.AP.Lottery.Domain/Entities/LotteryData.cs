using Newtonsoft.Json;
using OracleAttribute.Attributes;
using UtilityHelper;

namespace EVABMS.AP.Lottery.Domain.Entities
{
    [TableName("MGT_LOTTERY_DATA")]
    public class LotteryData : BaseEntities
    {
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }

        [ColumnName("SUBJECT")]
        public string? subject { get; private set; }

        [ColumnName("LIST_COLUMN", indexes: new int[] { 1 })]
        public string? list_column { get; private set; }

        [ColumnName("PROCCESS_STATUS")]
        public string? process_status { get; private set; }

        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime? last_updated_timestamp { get; private set; }

        [ColumnName("LAST_UPDATED_USERID")]
        public string? last_updated_userid { get; private set; }

        [ColumnName("ENV")]
        public string? ENV { get; private set; }

        public LotteryData() { }

        [JsonConstructor]
        public LotteryData(long id, string? subject, string? list_column, string? process_status, DateTime? last_updated_timestamp, string? last_updated_userid, string? ENV)
        {
            this.id = id;
            this.subject = subject;
            this.list_column = list_column;
            this.process_status = process_status;
            this.last_updated_timestamp = last_updated_timestamp;
            this.last_updated_userid = last_updated_userid;
            this.ENV = ENV;
        }

        public static LotteryData Create(long id, string? subject, string? list_column, string? process_status, DateTime last_updated_timestamp, string? last_updated_userid, string? ENV)
        {
            LotteryData model = new(id, subject, list_column, process_status, last_updated_timestamp, last_updated_userid, ENV);
            return model;
        }

        public LotteryData SetLastUpdatedTimestamp()
        {
            this.last_updated_timestamp = DateTime.Now;
            return this;
        }

        public LotteryData SetListColumn(string data)
        {
            this.list_column= data;
            return this;
        }

        public LotteryData SetLastUpdatedUserID(string data)
        {
            this.last_updated_userid = data;
            return this;
        }

        public LotteryData SetID(long id)
        {
            this.id = id;
            return this;
        }
    }
}
