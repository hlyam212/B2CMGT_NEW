using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace CommonHelper
{
    /// <summary>
    /// 設定檔的相關延伸功能
    /// </summary>
    public static class ConfigurationExtensions
    {
        private static IWebHostEnvironment? env;

        /// <summary>
        /// 傳入環境參數IWebHostEnvironment,取得EnvironmentName,組不同環境jsonsettingfile
        /// </summary>
        /// <param name="_env"></param>
        public static void SetEnvironment(IWebHostEnvironment _env) {
            env = _env;
        }

        #region Json
        /// <summary>
        /// 針對.Net Core時代，使用appSetting.json檔的設定讀取，副檔名以json為主
        /// 最後Get 可使用泛型
        /// ex : "compilerOptions:target".GetSetting("appsettings", "JSON").Get<string>();      
        ///      "compilerOptions:target".GetSetting("appsettings", "JSON", @"D:\tmpCode").Get<string>();              
        /// </summary>
        /// <param name="keyMap">設定檔中讀取Key的路徑, ex: xxx:key </param>
        /// <param name="settingFileName">檔名預設 appSetting</param>
        /// <param name="settingFileExtension">副檔名預設 json</param>
        /// <param name="FilePath">json file path</param>
        public static IConfigurationSection GetSetting(this string keyMap, string settingFileName = "appsettings", string settingFileExtension = "json" ,string FilePath ="")
        {
            try
            {
                string? EnvironmentName = env?.EnvironmentName;
                string JsonFilePath = "";
                if (String.IsNullOrEmpty(EnvironmentName))
                {
                    JsonFilePath = $@"{settingFileName}.{settingFileExtension}";
                }
                else {
                    JsonFilePath = $@"{settingFileName}.{EnvironmentName}.{settingFileExtension}";
                }
                IConfigurationBuilder? build = null;
                if (!String.IsNullOrEmpty(FilePath))
                {
                    //FilePath = System.IO.Directory.GetCurrentDirectory();
                    //FilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    FilePath = FilePath.EndsWith(@"\") ? FilePath : FilePath + @"\";
                    build = new ConfigurationBuilder().SetBasePath(FilePath).AddJsonFile(JsonFilePath);
                }
                else {
                    build = new ConfigurationBuilder().AddJsonFile(JsonFilePath);
                }
                
                IConfigurationRoot config = build.Build();
                return config.GetSection(keyMap);

            }
            catch (Exception ex)
            {
                throw new Exception($@"Get Setting Failed. {settingFileName}.{settingFileExtension}/{keyMap} - Error: {ex.ToString()} ", ex.InnerException);
            }
        }
        #endregion

        #region Config
        /// <summary>
        /// 在.Net Core時代，不推崇使用config檔，故無預設讀取config的方式(需要特別處理)，
        /// 但如果有必要還是可以使用。
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="configName"></param>
        /// <param name="configExtension"></param>
        public static string GetWebConfigSetting(this string keyName, string configName = "Web", string configExtension = "config")
        {
            string settingValue = string.Empty;

            try
            {
                string? applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                if (string.IsNullOrEmpty(applicationBase)) return settingValue;

                var fileMap = new ExeConfigurationFileMap
                { 
                    ExeConfigFilename = Path.Combine(applicationBase, $@"{configName}.{configExtension}")
                };
                var assemblyConfig = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                if (assemblyConfig.HasFile)
                {
                    AppSettingsSection? section = assemblyConfig.GetSection("appSettings") as AppSettingsSection;
                    if (section != null)
                    {
                        settingValue = section.Settings[keyName].Value;
                    }
                }
                return settingValue;
            }
            catch (Exception ex)
            {
                throw new Exception($@"Get Config Failed. {configName}.{configExtension}/{keyName} - Error: {ex.ToString()} ", ex.InnerException);
            }
        }
        #endregion
    }
}
