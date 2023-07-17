using OracleHelper.TransactSql;
using EVABMS.AP.Lottery.Domain.Entities;
using EncryptionHelper;

namespace EVABMS.AP.Lottery.Infrastructure
{
    public class LotteryRepository
    {
        public List<LotteryData> Query()
        {
            OracleService ora = new OracleService();
            List<LotteryData> result = ora.Select<LotteryData>();
            return result;
        }

        public LotteryData QueryDatabyID(long id)
        {
            OracleService ora = new();
            LotteryData lotteryData = new();
            lotteryData.SetID(id);
            List<LotteryData> lotteryDatas = ora.SelectByPrimaryKey<LotteryData>(lotteryData);
            return lotteryDatas.IsNullOrEmpty() ? null : lotteryDatas.FirstOrDefault();
        }

        public List<LotteryName> QueryName(long id, bool decrypt = true)
        {
            OracleService ora = new();
            LotteryName name = new LotteryName();
            name.SetMLODID(id);
            List<LotteryName> lotteryNames = ora.SelectByIndex<LotteryName>(name, 1);
            if (decrypt == false)
            {
                return lotteryNames;
            }

            foreach (LotteryName x in lotteryNames)
            {
                x.SetListContentEnc(x.list_content_enc.IsNullOrEmpty() ? "" : EncryptionOrgService.GDPRDecrypt(x.list_content_enc));
            }
            return lotteryNames;
        }

        public List<LotteryPrize> QueryPrize(long id)
        {
            OracleService ora = new();
            LotteryPrize pri = new();
            pri.SetMLODID(id);
            List<LotteryPrize> lotteryPrizes = ora.SelectByIndex<LotteryPrize>(pri, 1);
            return lotteryPrizes;
        }

        public long DataInsert(LotteryData input)
        {
            OracleKeyService orakey = new();
            long? id = orakey.GenerateKeyWithDual("MLOD_SEQ");

            input.SetID(id == null ? 0 : id.Value);
            input.SetLastUpdatedTimestamp();

            OracleService ora = new();
            bool result = ora.Insert(input);
            return result ? id.Value : 0;
        }

        public bool NameInsert(List<string> file_list, long mlodid, string userid)
        {
            bool result = false;
            foreach (string x in file_list)
            {
                OracleKeyService orakey = new();
                OracleService ora = new();
                long? id = orakey.GenerateKeyWithDual("MLOL_SEQ");
                LotteryName lotteryName = LotteryName.Create(id == null ? 0 : id.Value,
                                                             mlodid,
                                                             EncryptionOrgService.GDPREncrypt(x),
                                                             null,
                                                             null,
                                                             DateTime.Now,
                                                             userid);
                if (id == 0) return false;
                result = ora.Insert(lotteryName);
                if (!result) return false;
            }
            return result;
        }

        public bool PrizeInsert(List<string> file_prize, long mlodid, string userid)
        {
            bool result = false;
            foreach (string x in file_prize)
            {
                OracleKeyService orakey = new();
                OracleService ora = new();
                string[] sArray = x.Split(';');
                long? id = orakey.GenerateKeyWithDual("MLOP_SEQ");
                LotteryPrize lotteryPrize = LotteryPrize.Create(id == null ? 0 : id.Value,
                                                                mlodid,
                                                                sArray[0],
                                                                sArray[1].ToInt64(),
                                                                DateTime.Now,
                                                                userid);
                if (id == 0) return false;
                result = ora.Insert(lotteryPrize);
                if (!result) return false;
            }
            return result;
        }

        public int DataUpdate(LotteryData target, LotteryData org)
        {
            OracleService ora = new();
            int result;
            target.SetLastUpdatedTimestamp();
            result = ora.UpdateByPrimaryKey(target, org);
            return result;
        }

        public int NameUpdate(LotteryName target, long id)
        {
            OracleService ora = new();
            LotteryName lotteryName = new LotteryName();
            lotteryName.SetID(id);
            LotteryName org = ora.SelectByPrimaryKey(lotteryName).FirstOrDefault();
            ora = new();
            int result;
            target.SetLastUpdatedTimestamp();
            result = ora.UpdateByPrimaryKey(target, org);
            return result;
        }

        public int DeleteData(LotteryData input, long id)
        {
            OracleService ora = new();
            input.SetID(id);
            int result = ora.Delete(input, true);
            return result;
        }

        public int DeleteName(LotteryName input, long id)
        {
            OracleService ora = new();
            input.SetMLODID(id);
            int result = ora.DeleteByIndex(input, 1);
            return result;
        }

        public int DeletePrize(LotteryPrize input, long id)
        {
            OracleService ora = new();
            input.SetMLODID(id);
            int result = ora.DeleteByIndex(input, 1);
            return result;
        }

    }
}
