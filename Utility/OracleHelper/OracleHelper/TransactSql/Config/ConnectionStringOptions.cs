namespace OracleHelper.TransactSql.Config
{
    public class ConnectionStringOptions
    {
        public bool? duringChgKey { get; set; }
        public bool encryptMode { get; set; }
        public string? dataSource { get; set; }
        public List<string> hosts { get; set; } = new List<string>();
        public string? port { get; set; }
        public string? serviceName { get; set; }
        public string userID { get; set; }
        public string password { get; set; }
        public string? encryptDataKey { get; set; }
        public string? secondaryEncryptDataKey { get; set; }
        public bool pooling { get; set; } = false;
    }

    public class OraConnection
    {
        public string name { get; private set; }
        public string connectionString { get; private set; }
        public string? encryptDataKey { get; private set; }
        public string? secondaryEncryptDataKey { get; private set; }

        public static OraConnection Create(string name, string connectionString, string? encryptDataKey = "", string? secondaryEncryptDataKey = "")
        {
            OraConnection connection = new OraConnection();
            connection.name = name;
            connection.connectionString = connectionString;
            connection.encryptDataKey = encryptDataKey;
            connection.secondaryEncryptDataKey = secondaryEncryptDataKey;

            return connection;
        }
    }
}
