using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PMSS.CrawlerService
{
    public static class CrawlerConfig
    {
        public static string KeyHydrUrl
        {
            get
            {
                string str = ConfigurationManager.AppSettings["KeyHydrUrl"];
                return str;
            }
        }

        public static string AllHydrUrl
        {
            get
            {
                string str = ConfigurationManager.AppSettings["AllHydrUrl"];
                return str;
            }
        }

        public static string CacheFilePath
        {
            get
            {
                string str = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CacheFilePath"];
                return str;
            }
        }

        public static int CrawlIntervalMS
        {
            get
            {
                int minute = Convert.ToInt32(ConfigurationManager.AppSettings["CrawlInterval"]);
                return minute * 60 * 1000;
            }
        }
    }
}
