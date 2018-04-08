using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PMSS.SqlDataOutput
{
    public class HydrologicEntity
    {
        public List<HydrologicEntityItem> Items { get; set; }
    }

    public class HydrologicEntityItem
    {
        public int Recordid { get; set; }  //记录号
        public string Stationid { get; set; }  //站点号
        public double L { get; set; }  //水位（河道站） 库水量（水库站）潮位（潮位站）
        public double Q { get; set; }  //流量（河道站） 蓄水量（水库站）
        public double Wl1 { get; set; }  //警戒水位（河道站）汛限水位（水库站） 警戒潮位（潮位站）
        public double Wl2 { get; set; }  //警戒水位2（河道站）汛限水位2（水库站）
        public DateTime Time { get; set; }  //时间
        public bool Issign { get; set; }  //是否为重要站点信息
        public string Name { get; set; }   //站名
        public double Longitude { get; set; }  //经度
        public double Latitude { get; set; }  //纬度
        public string River { get; set; }   //河流名
        public string Hydrographicnet { get; set; }  //水系
        public string Basin { get; set; }   //流域
        public string Administrativeregion { get; set; }  //行政区域
        public string Address { get; set; }  //站点地址
        public string Type { get; set; }   //站点类型（河道站/水库站/潮位站）
    }

    public class HisHydrologicEntity
    {
        public List<HisHydrologicEntityItem> Items { get; set; }
    }

    public class HisHydrologicEntityItem
    {
        public int Recordid { get; set; }  //记录号
        public string Stationid { get; set; }  //站点号
        public double L { get; set; }  //水位（河道站） 库水量（水库站）潮位（潮位站）
        public double Q { get; set; }  //流量（河道站） 蓄水量（水库站）
        public double Wl1 { get; set; }  //警戒水位（河道站）汛限水位（水库站） 警戒潮位（潮位站）
        public double Wl2 { get; set; }  //警戒水位2（河道站）汛限水位2（水库站）
        public DateTime Time { get; set; }  //时间
        public bool Issign { get; set; }  //是否为重要站点信息
        public string Name { get; set; }   //站名
        public double Longitude { get; set; }  //经度
        public double Latitude { get; set; }  //纬度
        public string River { get; set; }   //河流名
        public string Hydrographicnet { get; set; }  //水系
        public string Basin { get; set; }   //流域
        public string Administrativeregion { get; set; }  //行政区域
        public string Address { get; set; }  //站点地址
        public string Type { get; set; }   //站点类型（河道站/水库站/潮位站）
        public double MaxL { get; set; }  //历史时间段内最大水位（河道站） 库水量（水库站）潮位（潮位站）
    }
}
