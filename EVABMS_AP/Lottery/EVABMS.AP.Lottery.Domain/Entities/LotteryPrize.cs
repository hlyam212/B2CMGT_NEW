using Newtonsoft.Json;
using OracleAttribute.Attributes;
using UtilityHelper;

namespace EVABMS.AP.Lottery.Domain.Entities
{
    [TableName("MGT_LOTTERY_PRIZE")]
    public class LotteryPrize : BaseEntities
    {
        [ColumnName("ID", isPrimaryKey: true)]
        public long id { get; private set; }
        [ColumnName("FK_MLOD_ID", indexes: new int[] { 1 })]
        public long fk_mlod_id { get; private set; }
        [ColumnName("NAME")]
        public string? name { get; private set; }
        [ColumnName("NUMBERS")]
        public long numbers { get; private set; }
        [ColumnName("LAST_UPDATED_TIMESTAMP")]
        public DateTime lastupdatedtimestamp { get; private set; }
        [ColumnName("LAST_UPDATED_USERID")]
        public string? lastupdateduserid { get; private set; }

        public LotteryPrize() { }

        [JsonConstructor]
        public LotteryPrize(long id, long fk_mlod_id, string name, long numbers, DateTime lastupdatedtimestamp, string? lastupdateduserid)
        {
            this.id = id;
            this.fk_mlod_id = fk_mlod_id;
            this.name = name;
            this.numbers = numbers;
            this.lastupdatedtimestamp = lastupdatedtimestamp;
            this.lastupdateduserid = lastupdateduserid;
        }

        public static LotteryPrize Create(long id, long fk_mlod_id, string name, long numbers, DateTime lastupdatedtimestamp, string? lastupdateduserid)
        {
            LotteryPrize model = new LotteryPrize(id, fk_mlod_id, name, numbers, lastupdatedtimestamp, lastupdateduserid);
            return model;
        }

        public LotteryPrize SetLastUpdatedTimestamp()
        {
            this.lastupdatedtimestamp = DateTime.Now;
            return this;
        }

        public LotteryPrize SetMLODID(long id)
        {
            this.fk_mlod_id = id;
            return this;
        }

    }
}
