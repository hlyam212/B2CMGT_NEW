using Oracle.ManagedDataAccess.Client;

namespace OracleHelper.TransactSql
{
    /// <summary>
    /// Operate Db by Parameters
    /// </summary>
    public class OraParameters
    {
        public string s_name { get; private set; }
        public OracleDbType s_type { get; private set; }
        public string? s_value { get; private set; }
        public DateTime? s_dateTime { get; private set; }

        public OraParameters(string name, OracleDbType type, string? value = null, DateTime? dateTime = null)
        {
            s_name = name;
            s_type = type;
            s_value = value;
            s_dateTime = dateTime;
        }
    }
}
