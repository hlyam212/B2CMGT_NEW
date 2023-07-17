using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OracleHelper.TransactSql.Config;

namespace OracleHelper.TransactSql
{
    /// <summary>
    /// 載入Oracle DB的相關基本設定
    /// </summary>
    public static class OraConfigProviderExtensions
    {
        public const string CONNECTION_STRING_KEY = "Default";

        public const string CRYPTOGRAPHY_KEY_PATH = "Security:CryptographyKey";

        public const string CONTENT_ROOT_PLACE_HOLDER = "%CONTENTROOTPATH%";

        #region ConnectionString 透過檔案加解密
        /// <summary>
        /// Web API使用載入基本設定
        /// 
        /// builder.Services.AddOraConfiguration(
        ///    configRoot: builder.Configuration,
        ///    connectionStringKey: "testInet");
        /// </summary>
        /// <param name="services">Service Collections</param>
        /// <param name="configRoot">appSetting.json</param>
        /// <param name="contentRootPath">DB連線字串設定的Section</param>
        /// <param name="connectionStringKey">指定的DB</param>
        public static void AddOraConfiguration(
            this IServiceCollection services,
            IConfigurationRoot configRoot,
            string contentRootPath = "ConnectionSettings",
            string connectionStringKey = CONNECTION_STRING_KEY)
        {
            SetOracleConfiguration(configRoot, contentRootPath, connectionStringKey);
        }

        /// <summary>
        /// Console使用載入基本設定
        /// 
        /// ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        /// configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        /// IConfigurationRoot config = configBuilder.Build();
        /// ServiceCollection services = new ServiceCollection();
        /// services.AddOraConfiguration(
        ///    configRoot: config,
        ///    connectionStringKey: "testInet");
        /// </summary>
        /// <param name="configRoot">appSetting.json</param>
        /// <param name="contentRootPath">DB連線字串設定的Section</param>
        /// <param name="connectionStringKey">指定的DB</param>
        public static void AddOraConfiguration(
            IConfigurationRoot configRoot,
            string contentRootPath = "ConnectionSettings",
            string connectionStringKey = CONNECTION_STRING_KEY)
        {
            SetOracleConfiguration(configRoot, contentRootPath, connectionStringKey);
        }

        /// <summary>
        /// 載入基本設定
        /// </summary>
        private static void SetOracleConfiguration(
            IConfigurationRoot configRoot,
            string contentRootPath = "ConnectionSettings",
            string connectionStringKey = CONNECTION_STRING_KEY)
        {
            // ConnStr
            #region Connection String read from .ini
            Dictionary<string,string> connectionString = new InIService().ConnectStringGet(configRoot, configRoot.GetSection($"ConnectionSettings:{connectionStringKey}").Value);
            #endregion

            OraConfigOptions.SetConnectionString(connectionString);

            // Other Oracle Setting
            var oraDbSection = configRoot.GetSection("OraDbSetting");
            if (oraDbSection != null)
            {
                OraConfigOptions.setOtherOraSetting(oraDbSection);
            }
            OraConfigOptions.AddOracleConfiguration();
        }
        #endregion

        #region ConnectionString 在AppSetting 且可控加解密
        /// <summary>
        /// Web API使用載入基本設定
        /// 
        /// builder.Services.AddOraConfiguration(
        ///    configRoot: builder.Configuration,
        ///    connectionStringKey: "testInet");
        /// </summary>
        /// <param name="services">Service Collections</param>
        /// <param name="configRoot">appSetting.json</param>
        /// <param name="contentRootPath">DB連線字串設定的Section</param>
        /// <param name="connectionStringKeys">指定的所有DB</param>
        public static void AddOraConfiguration(
            this IServiceCollection services,
            IConfigurationRoot configRoot,
            string[] connectionStringKeys,
            string contentRootPath = "ConnectionSettings",
            string encryptKeyPath = CRYPTOGRAPHY_KEY_PATH)
        {
            SetOracleConfiguration(
                configRoot: configRoot,
                connectionStringKeys: connectionStringKeys,
                contentRootPath: contentRootPath,
                encryptKeyPath: encryptKeyPath);
        }

        /// <summary>
        /// Console使用載入基本設定
        /// 
        /// ConfigurationBuilder configBuilder = new ConfigurationBuilder();
        /// configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        /// IConfigurationRoot config = configBuilder.Build();
        /// ServiceCollection services = new ServiceCollection();
        /// services.AddOraConfiguration(
        ///    configRoot: config,
        ///    connectionStringKey: "testInet");
        /// </summary>
        /// <param name="configRoot">appSetting.json</param>
        /// <param name="contentRootPath">DB連線字串設定的Section</param>
        /// <param name="connectionStringKey">指定的DB</param>
        public static void AddOraConfiguration(
            IConfigurationRoot configRoot,
            string[] connectionStringKeys,
            string contentRootPath = "ConnectionSettings",
            string encryptKeyPath = CRYPTOGRAPHY_KEY_PATH)
        {
            SetOracleConfiguration(
                configRoot: configRoot,
                connectionStringKeys: connectionStringKeys,
                contentRootPath: contentRootPath,
                encryptKeyPath: encryptKeyPath);
        }

        /// <summary>
        /// 載入基本設定
        /// </summary>
        private static void SetOracleConfiguration(
            IConfigurationRoot configRoot,
            string[] connectionStringKeys,
            string contentRootPath = "ConnectionSettings",
            string encryptKeyPath = CRYPTOGRAPHY_KEY_PATH)
        {
            string encryptKey = configRoot.GetValue<string>(encryptKeyPath);

            // ConnStr
            var connecitonStringSection = configRoot.GetSection(contentRootPath);
            if (connecitonStringSection == null || connectionStringKeys.Length == 0)
            {
                throw new OracleHelperException($@"Database Setting Error: {contentRootPath} Section is empty.");
            }

            OraConfigOptions.SetOraConnections(connecitonStringSection, connectionStringKeys, encryptKey);

            // Other Oracle Setting
            var oraDbSection = configRoot.GetSection("OraDbSetting");
            if (oraDbSection != null)
            {
                OraConfigOptions.SetOtherOraSetting(oraDbSection);
            }
            OraConfigOptions.AddOracleConfiguration();
        }
        #endregion
    }
}
