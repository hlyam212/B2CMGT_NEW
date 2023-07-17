using Newtonsoft.Json;
using OracleAttribute.Attributes;
using UtilityHelper;

namespace EVABMS.AP.Lottery.Domain.Entities
{
    [TableName("MGT_LOTTERY_LIST")]
    public class LotteryName : BaseEntities
    {
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        [ColumnName("FK_MLOD_ID", indexes: new int[] { 1 })]
        public long fk_mlod_id { get; private set; }
        [ColumnName("LIST_CONTENT_ENC")]
        public string? list_content_enc { get; private set; }
        [ColumnName("FK_MLOP_ID")]
        public long? fk_mlop_id { get; private set; }
        [ColumnName("WINNING_TIMESTAMP")]
        public DateTime? winning_timestamp { get; private set; }
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }
        [ColumnName("LAST_UPDATED_USERID")]
        public string? lastupdateduserid { get; private set; }

        public LotteryName() { }

        [JsonConstructor]
        public LotteryName(long id, long fk_mlod_id, string list_content_enc, long? fk_mlop_id, DateTime? winning_timestamp, DateTime lastupdatedtimestamp, string? lastupdateduserid)
        {
            this.id = id;
            this.fk_mlod_id = fk_mlod_id;
            this.list_content_enc = list_content_enc;
            this.fk_mlop_id = fk_mlop_id;
            this.winning_timestamp = winning_timestamp;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
        }

        public static LotteryName Create(long id, long fk_mlod_id, string list_content_enc, long? fk_mlop_id, DateTime? winning_timestamp, DateTime lastupdatedtimestamp, string? lastupdateduserid)
        {
            LotteryName model=new LotteryName(id ,fk_mlod_id,list_content_enc,fk_mlop_id,winning_timestamp,lastupdatedtimestamp,lastupdateduserid);
            return model;
        }

        public LotteryName SetLastUpdatedTimestamp()
        {
            this.lastupdatedtimestamp = DateTime.Now;
            return this;
        }

        public LotteryName SetWinning(long id)
        {
            this.fk_mlop_id = id;
            this.winning_timestamp = DateTime.Now;
            return this;
        }
        public LotteryName SetListContentEnc(string data)
        {
            this.list_content_enc = data;
            return this;
        }
        public LotteryName SetMLODID(long id)
        {
            this.fk_mlod_id = id;
            return this;
        }
        public LotteryName SetID(long id)
        {
            this.id = id;
            return this;
        }
    }
}
