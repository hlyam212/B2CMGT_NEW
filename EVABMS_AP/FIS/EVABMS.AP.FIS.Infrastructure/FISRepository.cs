using OracleHelper.TransactSql;
using OracleHelper.TransactSql.Config;
using System.Data;

namespace EVABMS.AP.FIS.Infrastructure
{
    public class FISRepository
    {
        public DataTable ScheduleAllQuery()
        {
            OraConfigOptions.ChangeConnectionString("BRFIS");
            //List<string> result = new List<string>();

            OracleService ora = new OracleService();

            #region Query VFS_FLIGHT_SCHEDULE_ALL
            string sql = @"  SELECT * 
                             FROM AAF0.VFS_FLIGHT_SCHEDULE_ALL
                             WHERE CARRIER_CODE='BR'
                             AND FLIGHT_NO='0061'
                             AND STD_LOCAL BETWEEN TO_DATE('2023/01/02 00:00:00','YYYY/MM/DD HH24:MI:SS') AND TO_DATE('2023/01/02 23:59:59','YYYY/MM/DD HH24:MI:SS')";
            DataTable result = ora.Select(sql);
            #endregion
            return result;
        }
    }
}