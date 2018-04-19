//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESS.FW.Common.Configurations
//{
//    public class ConfigManager
//    {
//        private static readonly AppSettingsSection _appSettingSection = null;
//        private static readonly ConnectionStringsSection _connectionStringSection = null;
//        private const string _configPath = "cfg";

//        static ConfigManager()
//        {
//            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap()
//            {
//                ExeConfigFilename = CONFIG_PATH
//            };

//            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            
//            _appSettingSection = config.AppSettings;
//            _connectionStringSection = config.ConnectionStrings;
//        }

//        public string GetAppSetting(string key)
//        {
//            return _appSettingSection.Settings[key].Value;
//        }

//        public string GetConnectionString(string name)
//        {
//            return _connectionStringSection.ConnectionStrings[name].ConnectionString;
//        }

//        private ExeConfigurationFileMap GetFileMap(string filename)
//        {
//        }
//    }
//}
