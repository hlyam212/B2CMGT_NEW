using EncryptionHelper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace OracleHelper.TransactSql.Config
{
    public static class OraConfigOptions
    {
        public static bool OraConfigIsDone { get; private set; } = false;

        #region Connection
        public static string OraConnectionString { get; private set; } = "";

        public static List<OraConnection> OraConnections { get; private set; } = new List<OraConnection>();

        /// <summary>
        /// Default is 0 seconds, which enforces no time limit.
        /// </summary>
        private static int CommandTimeout { get; set; } = 15;

        /// <summary>
        /// (sqlnet.ora)
        ///  This property specifies the time, in seconds, for a client to establish a TCP connection (PROTOCOL=tcp in the TNS connect address) to the database server before it can time out. 
        /// Default value is 60 seconds.
        /// </summary>
        private static int TcpConnectionTimeout { get; set; } = 5;
        /// <summary>
        /// This property specifies the tnsnames.ora and/or sqlnet.ora directory location.
        /// OracleConfiguration.TnsAdmin = @"D:\oracle\client\admin";
        /// </summary>
        private static string? TnsAdminFilePath { get; set; }
        #endregion

        #region Command
        /// <summary>
        /// This property specifies whether the binding method used for the parameter collection is by name or by position. 
        /// Default value (false) is bind by position.
        /// </summary>
        private static bool BindByName { get; set; } = true;

        /// <summary>
        /// 針對DB Table個資欄位加解密的密碼
        /// </summary>
        public static string EncryptionKey { get; private set; }
        /// <summary>
        /// (duringChgKey為true) 針對DB Table個資欄位加解密的 備用Key
        /// </summary>
        public static string SecondaryEncryptionKey { get; private set; }
        #endregion

        #region Debug Tracing
        /// <summary>
        /// This property specifies the destination directory to output provider traces.
        /// On Windows, the default TraceFileLocation is <Windows user temporary folder>\ODP.NET\core\trace
        /// </summary>
        private static string? TraceFileLocaiton { get; set; }
        /// <summary>
        /// This property specifies the generated trace level to trace ODP.NET calls and diagnose provider issues. Errors will always be traced. 
        /// Default value is 0 indicating tracing is disabled.
        /// Valid Values:
        /// 1 = public APIs
        /// 2 = private APIs
        /// 4 = network APIs/data
        /// These values can be bitwise ORed.To enable all traces, set TraceLevel to 7.
        /// </summary>
        private static int TraceLevel { get; set; } = 4;
        /// <summary>
        /// This property specifies whether to generate a single trace file or multiple trace files for multithreaded applications. 
        /// Default value is 0 indicating single trace file for all application threads.
        /// </summary>
        private static int TraceOption { get; set; } = 1;
        #endregion

        public static void finishOraConfig(bool isDone = false)
        {
            OraConfigIsDone = isDone;
        }

        public static void SetConnectionString(Dictionary<string, string> connStringDic)
        {
            if (connStringDic == null)
            {
                throw new OracleHelperException("Database connectionString is missing.");
            }

            try
            {
                Func<string, string, string> _removeUnnecessarySetting = new Func<string, string, string>((string connString, string removeSettingStr) =>
                {
                    if (connString.IndexOf(removeSettingStr) == -1)
                    {
                        return connString;
                    }
                    connString = connString.Remove(connString.IndexOf(removeSettingStr), (connString.IndexOf(";", connString.IndexOf(removeSettingStr)) - connString.IndexOf(removeSettingStr)));
                    return connString;
                });

                int index = 0;
                foreach (var x in connStringDic)
                {
                    string temp = x.Value;
                    temp = _removeUnnecessarySetting(temp, "Integrated Security");
                    temp = _removeUnnecessarySetting(temp, "Provider");
                    temp = _removeUnnecessarySetting(temp, "OLEDB.NET");
                    temp = _removeUnnecessarySetting(temp, "USER ROLE");

                    if (index == 0)
                    {
                        OraConnectionString = temp;
                    }

                    OraConnections.Add(OraConnection.Create(name: x.Key, connectionString: temp));
                    index++;
                }
            }
            catch
            {
                throw new OracleHelperException("Database connectionString is missing.");
            }
        }

        public static void SetOraConnections(IConfigurationSection connecitonStringSection, string[] connectionStringKeys, string encryptKey)
        {
            for (int i = 0; i < connectionStringKeys.Length; i++)
            {
                string connectionStringKey = connectionStringKeys[i].ToString();

                ConnectionStringOptions connectionStringOption = connecitonStringSection.GetSection(connectionStringKey).Get<ConnectionStringOptions>();
                if (connectionStringOption.encryptMode && string.IsNullOrEmpty(encryptKey))
                {
                    throw new OracleHelperException("The encryption key of Database is missing.");
                }

                if (string.IsNullOrEmpty(connectionStringOption.dataSource) &&
                    connectionStringOption.hosts.Count == 0 &&
                    string.IsNullOrEmpty(connectionStringOption.port) &&
                    string.IsNullOrEmpty(connectionStringOption.serviceName))
                {
                    throw new OracleHelperException($@"The connectionString settting of {connectionStringKey} is missing.");
                }

                if (string.IsNullOrEmpty(connectionStringOption.userID) ||
                    string.IsNullOrEmpty(connectionStringOption.password))
                {
                    throw new OracleHelperException($@"The connectionString settting of {connectionStringKey} is missing.");
                }

                if (connectionStringOption.encryptMode)
                {
                    connectionStringOption.password = connectionStringOption.password.AesDecrypt(encryptKey);
                }

                //string connString = string.Empty;

                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.UserID = connectionStringOption.userID;
                ocsb.Password = connectionStringOption.password;
                ocsb.Pooling = connectionStringOption.pooling;

                if (!string.IsNullOrEmpty(connectionStringOption.dataSource))
                {
                    ocsb.DataSource = connectionStringOption.dataSource;
                    //connString = $@"DATA SOURCE={connectionStringOption.dataSource};USER ID={connectionStringOption.userID};PASSWORD={connectionStringOption.password};";
                }
                else
                {
                    if (connectionStringOption.hosts.Count == 0 ||
                        string.IsNullOrEmpty(connectionStringOption.port) ||
                        string.IsNullOrEmpty(connectionStringOption.serviceName))
                    {
                        throw new OracleHelperException($@"The connectionString settting of {connectionStringKey} is missing.");
                    }

                    string addressList = string.Empty;
                    foreach (string host in connectionStringOption.hosts)
                    {
                        addressList += $@"(ADDRESS=(PROTOCOL=tcp)(HOST={host})(PORT={connectionStringOption.port}))";
                    }

                    ocsb.DataSource = $@"(DESCRIPTION=(ADDRESS_LIST={addressList})(CONNECT_DATA=(SERVICE_NAME={connectionStringOption.serviceName})))";
                    //connString = $@"DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST={addressList})(CONNECT_DATA=(SERVICE_NAME={connectionStringOption.serviceName})));USER ID={connectionStringOption.userID};PASSWORD={connectionStringOption.password};";
                }

                // 如果Key要比照其他密碼的作法，就要把這段解開來，並且Password必須符合AES256
                //if (!string.IsNullOrEmpty(connectionStringOption.encryptDataKey))
                //{
                //    connectionStringOption.encryptDataKey = connectionStringOption.encryptDataKey.AesDecrypt(encryptKey);
                //}
                //string secondaryEncryptDataPwd = string.Empty;
                //if (connectionStringOption.duringChgKey == true && !string.IsNullOrEmpty(connectionStringOption.secondaryEncryptDataKey))
                //{
                //    secondaryEncryptDataPwd = connectionStringOption.secondaryEncryptDataKey.AesDecrypt(encryptKey);
                //}

                OraConnections.Add(OraConnection.Create(
                        name: connectionStringKey,
                        connectionString: $@"{ocsb.ConnectionString}",
                        encryptDataKey: connectionStringOption.encryptDataKey,
                        secondaryEncryptDataKey: (connectionStringOption.duringChgKey == true) ? connectionStringOption.secondaryEncryptDataKey : ""));

                if (i == 0)
                {
                    OraConnectionString = $@"{ocsb.ConnectionString}";
                    EncryptionKey = connectionStringOption.encryptDataKey;
                    SecondaryEncryptionKey = connectionStringOption.secondaryEncryptDataKey; ;
                }
            }
        }

        public static void ChangeConnectionString(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new OracleHelperException("Database targetConnection name is missing.");
            }

            OraConnection? oraConnection = OraConnections.Where(x => x.name == connectionName).FirstOrDefault();

            if (oraConnection == null)
            {
                throw new OracleHelperException($@"The connectionString of {connectionName} is missing.");
            }

            OraConnectionString = oraConnection.connectionString;
            EncryptionKey = oraConnection.encryptDataKey;
            SecondaryEncryptionKey = oraConnection.secondaryEncryptDataKey;
        }

        public static void SetOtherOraSetting(IConfigurationSection oraSection)
        {
            // 設定sqlnet.ora / tnsnames.ora / ldap.ora 的路徑
            string? tnsAdmin = oraSection.GetValue<string?>("tnsAdminFilePath");
            if (!string.IsNullOrEmpty(tnsAdmin))
            {
                TnsAdminFilePath = tnsAdmin;
            }
            else
            {
                string localTnsnamesPath = Path.Combine(Directory.GetCurrentDirectory(), "tnsnames.ora");
                if (File.Exists(localTnsnamesPath))
                {
                    TnsAdminFilePath = localTnsnamesPath;
                }
            }

            // Parameters是否要按照欄位名稱對應
            bool? bindByName = oraSection.GetValue<bool?>("bindByName");
            if (bindByName != null) BindByName = (bool)bindByName;

            // 系統有異常時，可以開啟追蹤模式
            string? filePath = oraSection.GetValue<string?>("traceFilePath");
            if (!string.IsNullOrEmpty(filePath))
            {
                TraceFileLocaiton = filePath;

                int? traceLevel = oraSection.GetValue<int?>("traceLevel");
                if (traceLevel != null) TraceLevel = (int)traceLevel;

                int? traceOption = oraSection.GetValue<int?>("traceOption");
                if (traceOption != null) TraceOption = (int)traceOption;
            }
        }

        public static void setOtherOraSetting(IConfigurationSection oraSection)
        {
            // 設定sqlnet.ora / tnsnames.ora / ldap.ora 的路徑
            string? tnsAdmin = oraSection.GetValue<string?>("tnsAdminFilePath");
            if (tnsAdmin != null) TnsAdminFilePath = tnsAdmin;

            // Parameters是否要按照欄位名稱對應
            bool? bindByName = oraSection.GetValue<bool?>("bindByName");
            if (bindByName != null) BindByName = (bool)bindByName;

            // 系統有異常時，可以開啟追蹤模式
            string? filePath = oraSection.GetValue<string?>("traceFilePath");
            if (filePath != null)
            {
                TraceFileLocaiton = filePath;

                int? traceLevel = oraSection.GetValue<int?>("traceLevel");
                if (traceLevel != null) TraceLevel = (int)traceLevel;

                int? traceOption = oraSection.GetValue<int?>("traceOption");
                if (traceOption != null) TraceOption = (int)traceOption;
            }
        }

        public static void AddOracleConfiguration()
        {
            if (OraConfigIsDone == true)
            {
                return;
            }

            OracleConfiguration.CommandTimeout = CommandTimeout;
            OracleConfiguration.TcpConnectTimeout = TcpConnectionTimeout;
            if (!string.IsNullOrEmpty(TnsAdminFilePath))
            {
                OracleConfiguration.TnsAdmin = TnsAdminFilePath;
            }
            OracleConfiguration.BindByName = BindByName;

            if (!string.IsNullOrEmpty(TraceFileLocaiton))
            {
                OracleConfiguration.TraceFileLocation = TraceFileLocaiton;
                OracleConfiguration.TraceLevel = TraceLevel;
                OracleConfiguration.TraceOption = TraceOption;
            }
            OraConfigIsDone = true;
        }
    }
}
