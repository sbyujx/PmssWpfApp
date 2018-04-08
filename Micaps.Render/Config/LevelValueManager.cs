using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.Config
{
    public static class LevelValueManager
    {
        public static readonly string temperatureName = "温度";
        public static readonly string RainName1 = "1小时降水";
        public static readonly string RainName3 = "3小时降水";
        public static readonly string RainName6 = "6小时降水";
        public static readonly string RainName12 = "12小时降水";
        public static readonly string RainName24 = "24小时降水";
        public static readonly string LevelNameFlood = "山洪和中小河流等级";
        public static readonly string LevelNameDisaster = "地质灾害和渍涝等级";
        public static readonly string RainName05Days = "5天累计降水等级";
        public static readonly string RainName14Days = "14天累计降水等级";

        public static Dictionary<string, LevelSettingList> Settings { get; set; } = new Dictionary<string, LevelSettingList>();

        public static void AddToSettings(string KeyName, string levels, string colors)
        {
            var setting = new LevelSettingList(KeyName, levels, colors);
            Settings.Add(KeyName, setting);
        }

        public static LevelSettingList GetSettingsFromMicapsDescription(string description)
        {
            if(description.Contains("温度"))
            {
                return Settings[temperatureName];
            }

            if (description.Contains("1小时") && description.Contains("降水"))
            {
                return Settings[RainName1];
            }

            if (description.Contains("3小时") && description.Contains("降水"))
            {
                return Settings[RainName3];
            }

            if (description.Contains("6小时") && description.Contains("降水"))
            {
                return Settings[RainName6];
            }

            if (description.Contains("12小时") && description.Contains("降水"))
            {
                return Settings[RainName12];
            }

            if (description.Contains("24小时") && description.Contains("降水"))
            {
                return Settings[RainName24];
            }

            if (description.Contains("level") && description.Contains("flood"))
            {
                return Settings[LevelNameFlood];
            }

            if (description.Contains("level"))
            {
                return Settings[LevelNameDisaster];
            }

            return null;
        }
    }
}
