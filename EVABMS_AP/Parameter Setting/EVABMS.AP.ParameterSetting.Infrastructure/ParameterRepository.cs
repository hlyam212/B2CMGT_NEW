using OracleHelper.TransactSql;
using EVABMS.AP.Parameter.Domain.Entities;
using OracleAttribute.Extensions;

namespace EVABMS.AP.Parameter.Infrastructure
{
    public class ParameterRepository
    {
        public List<ParameterSetting> Query(ParameterSetting quitirua)
        {
            OracleService ora = new OracleService();

            List<ParameterSetting> result = ora.SelectByIndex(queryModel: quitirua, indexNumber: 1);

            return result;
        }

        public ParameterSetting Insert(ParameterSetting ps)
        {
            #region 取Primary Key
            OracleKeyService ora = new OracleKeyService();

            long? seq = ora.GenerateKeyWithDual("MGPS_SEQ");
            #endregion

            ps.SetID(seq.Value);
            ps.SetLastUpdatedTimestamp();

            OracleService oraService = new OracleService();
            bool result = oraService.Insert(ps);

            CreateHistory(ps, "Insert");

            return result ? ps : null;
        }

        public int Update(ParameterSetting target, ParameterSetting org)
        {
            OracleService oraService = new OracleService();
            int result = oraService.UpdateByPrimaryKey(updateMoodel: target, originalModel: org);

            CreateHistory(target, "Update");

            return result;
        }

        public int Delete(ParameterSetting ps)
        {
            OracleService oraService = new OracleService();
            int result = oraService.Delete(deleteMoodel: ps);

            CreateHistory(ps, "Delete");

            return result;
        }

        public List<History> QueryHistory(History quitirua)
        {
            OracleService ora = new OracleService();

            List<History> result = ora.SelectByIndex(queryModel: quitirua, indexNumber: 1);

            return result;
        }

        private History CreateHistory(ParameterSetting ps, string action)
        {
            #region 取Primary Key
            OracleKeyService ora = new OracleKeyService();

            long? seq = ora.GenerateKeyWithDual("MGUH_SEQ");
            #endregion

            History history = History.Create(seq.Value, ps.functionname, ps.subfunctionname, ps.settingname, ps.value, action, DateTime.Now, ps.lastupdateduserid, ps.Auth, ps.ENV);

            OracleService oraService = new OracleService();
            bool result = oraService.Insert(history);

            return result ? history : null;
        }
    }
}