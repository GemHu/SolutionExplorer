using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Threading;

namespace DothanTech.Helpers
{
    public class DzAppDataConfig : DzConfigFile
    {
        public const String LAST_LOCATION = "Location";

        public DzAppDataConfig()
            : base(null)
        {
            this.Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static String sGetValue(String key, String defaultValue = "")
        {
            return new DzAppDataConfig().GetValue(key, defaultValue);
        }

        public static bool sSetValue(String key, String value)
        {
            return new DzAppDataConfig().SetValue(key, value);
        }
    }

    public class DzConfigFile
    {
        public DzConfigFile(Configuration config)
        {
            this.Config = config;
        }

        public Configuration Config { get; set; }

        /// <summary>
        /// 打开默认配置文件中制定的Section；
        /// </summary>
        public ConfigurationSection GetSection(string sectionName)
        {
            if (this.Config == null)
                return null;

            return this.Config.GetSection(sectionName);
        }

        public AppSettingsSection AppSettings
        {
            get { return this.Config != null ? this.Config.AppSettings : null; }
        }

        public KeyValueConfigurationCollection Settings
        {
            get { return this.Config != null ? this.Config.AppSettings.Settings : null; }
        }

        public String GetValue(String key, String defaultValue = "")
        {
            if (String.IsNullOrEmpty(key) || this.Config == null)
                return defaultValue;

            var item = this.Config.AppSettings.Settings[key];
            if (item == null)
                return defaultValue;

            return item.Value;
        }

        public bool SetValue(String key, String value)
        {
            if (String.IsNullOrEmpty(key) || this.Config == null)
                return false;

            try
            {
                this.Settings.Remove(key);
                this.Settings.Add(key, value);
                //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                //{
                //    this.Config.Save();
                //}));
                this.Config.Save();
                return true;
            }
            catch (Exception ee)
            {
                Trace.WriteLine("### [" + ee.Source + "] Exception: " + ee.Message);
                Trace.WriteLine("### " + ee.StackTrace);
                return false;
            }
        }
    }

    //public class ProjectSection : ConfigurationSection
    //{
    //    private static readonly ConfigurationProperty _propProjects = new ConfigurationProperty(null,
    //        typeof(KeyValueConfigurationCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

    //    [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
    //    public KeyValueConfigurationCollection KeyValues
    //    {
    //        get
    //        {
    //            return (KeyValueConfigurationCollection)base[_propProjects];
    //        }
    //    }
    //}
}
