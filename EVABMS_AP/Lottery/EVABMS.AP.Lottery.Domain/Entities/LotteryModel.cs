using Newtonsoft.Json;
using OracleAttribute.Attributes;
using System.Reflection;
using UtilityHelper;

namespace EVABMS.AP.Lottery.Domain.Entities
{
    public class LotteryModel : BaseEntities
    {
        public LotteryData? query_data { get; private set; }
        public List<LotteryName>? winner_list { get; private set; }
        public List<LotteryName>? data_list { get; private set; }
        public List<LotteryPrize>? data_prize { get; private set; }

        public LotteryModel() { }

        [JsonConstructor]
        public LotteryModel(LotteryData? query_data,  List<LotteryName>? winner_list, List<LotteryName>? data_list, List<LotteryPrize>? data_prize)
        {
            this.query_data = query_data;
            this.winner_list = winner_list;
            this.data_list = data_list;
            this.data_prize = data_prize;
        }

        public static LotteryModel Create(LotteryData? query_data, List<LotteryName>? winner_list, List<LotteryName>? data_list, List<LotteryPrize>? data_prize)
        {
            LotteryModel model = new LotteryModel(query_data,  winner_list, data_list, data_prize);
            return model;
        }
    }

}
