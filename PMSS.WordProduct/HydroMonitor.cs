using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.SqlDataAccess;

namespace PMSS.WordProduct
{
    public class HydroMonitor
    {
        public static List<string> Provices = new List<string>()
        {
            "北京",
            "天津",
            "重庆",
            "上海",
            "河北",
            "山西",
            "辽宁",
            "吉林",
            "黑龙江",
            "江苏",
            "浙江",
            "安徽",
            "福建",
            "江西",
            "山东",
            "河南",
            "湖北",
            "湖南",
            "广东",
            "海南",
            "四川",
            "贵州",
            "云南",
            "陕西",
            "甘肃",
            "青海",
            "台湾",
            "内蒙古",
            "广西",
            "西藏",
            "宁夏",
            "新疆",
            "香港",
            "澳门"
        };

        public static string GetProvinceByAdm(string Administrativeregion)
        {
            string province = "";
            if (Administrativeregion != null)
            {
                foreach (var item in Provices)
                {
                    if (Administrativeregion.StartsWith(item))
                    {
                        province = item;
                        return province;
                    }
                }
            }

            return province;
        }

        public static List<RiverWarningRecord> GetRiverWarningInfo()
        {
            List<RiverWarningRecord> infoList = new List<RiverWarningRecord>();
            List<RiverWarningRecord> rtnList = new List<RiverWarningRecord>();

            HydrologicReader dbReader = new HydrologicReader();
            HydrologicEntity entity = dbReader.RetrieveEntity(DateTime.Now.AddDays(-1), DateTime.Now, "河道站");    //获取最近1天河道站最新水情
            foreach (var item in entity.Items)
            {
                if ((item.L - item.Wl1) > -0.5)  //超过监测门限-0.5
                {
                    RiverWarningRecord record = new RiverWarningRecord();

                    if (item.Basin != null)
                    {
                        int index = item.Basin.IndexOf("流域");
                        record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                    }
                    else
                    {
                        record.Basin = item.Basin;
                    }

                    if (item.L > item.Wl1)
                    {
                        record.IsBeyond = true;
                    }
                    else
                    {
                        record.IsBeyond = false;
                    }

                    record.Level = item.L;
                    record.Name = item.Name;
                    record.OverWarningLevel = item.L - item.Wl1;
                    record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                    record.River = item.River;
                    record.Stationid = item.Stationid;
                    record.Time = item.Time;
                    record.WarningLevel = item.Wl1;
                    int tr = dbReader.ComparedWithRecent(item, DateTime.Now.AddDays(-2), DateTime.Now);  //最近2天内的最新趋势
                    if (tr > 0)
                    {
                        record.ComparedBefore = Trend.Rise;
                    }
                    else if (tr == 0)
                    {
                        record.ComparedBefore = Trend.Hold;
                    }
                    else
                    {
                        record.ComparedBefore = Trend.Fall;
                    }

                    if (!(double.IsNaN(record.WarningLevel) || record.WarningLevel <= 0))
                    {
                        infoList.Add(record);
                    }
                }
            }

            var groupedList = from info in infoList
                              group info by info.Basin into g
                              select g;

            /*infoList = new List<RiverWarningRecord>();
            foreach(var g in groupedList)
            {
                foreach(var x in g)
                {
                    infoList.Add(x);
                }
            }*/
            foreach (var group in groupedList)
            {
                foreach (var item in group)
                {
                    rtnList.Add(item);
                }
            }

            return rtnList;
        }

        public static List<ReservoirWarningRecord> GetReservoirWarningInfo()
        {
            List<ReservoirWarningRecord> infoList = new List<ReservoirWarningRecord>();
            List<ReservoirWarningRecord> rtnList = new List<ReservoirWarningRecord>();

            HydrologicReader dbReader = new HydrologicReader();
            HydrologicEntity entity = dbReader.RetrieveEntity(DateTime.Now.AddDays(-1), DateTime.Now, "水库站");    //获取最近1天水库站最新水情
            foreach (var item in entity.Items)
            {
                if ((item.L - item.Wl1) > -0.001 && item.Issign == false)  //超过监测门限-0.001且不为重点站
                {
                    ReservoirWarningRecord record = new ReservoirWarningRecord();

                    if (item.Basin != null)
                    {
                        int index = item.Basin.IndexOf("流域");
                        record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                    }
                    else
                    {
                        record.Basin = item.Basin;
                    }

                    if (item.L > item.Wl1)
                    {
                        record.IsBeyond = true;
                    }
                    else
                    {
                        record.IsBeyond = false;
                    }

                    record.Level = item.L;
                    record.Name = item.Name;
                    record.OverWarningLevel = item.L - item.Wl1;
                    record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                    record.River = item.River;
                    record.Stationid = item.Stationid;
                    record.Time = item.Time;
                    record.WarningLevel = item.Wl1;
                    int tr = dbReader.ComparedWithRecent(item, DateTime.Now.AddDays(-2), DateTime.Now);  //最近2天内的最新你趋势
                    if (tr > 0)
                    {
                        record.ComparedBefore = Trend.Rise;
                    }
                    else if (tr == 0)
                    {
                        record.ComparedBefore = Trend.Hold;
                    }
                    else
                    {
                        record.ComparedBefore = Trend.Fall;
                    }

                    if (!(double.IsNaN(record.WarningLevel) || record.WarningLevel <= 0))
                    {
                        infoList.Add(record);
                    }
                }
            }

            var groupedList = from info in infoList
                              group info by info.Basin into g
                              select g;

            /*infoList = new List<RiverWarningRecord>();
            foreach(var g in groupedList)
            {
                foreach(var x in g)
                {
                    infoList.Add(x);
                }
            }*/

            foreach (var group in groupedList)
            {
                foreach (var item in group)
                {
                    rtnList.Add(item);
                }
            }

            return rtnList;
        }

        public static List<ReservoirWarningRecord> GetKeyReservoirWarningInfo()
        {
            List<ReservoirWarningRecord> infoList = new List<ReservoirWarningRecord>();

            HydrologicReader dbReader = new HydrologicReader();
            HydrologicEntity entity = dbReader.RetrieveEntity(DateTime.Now.AddDays(-1), DateTime.Now, "水库站");    //获取最近1天水库站最新水情
            foreach (var item in entity.Items)
            {
                if ((item.L - item.Wl1) > -20 && item.Issign == true)  //超过监测门限-20且为重点站
                {
                    ReservoirWarningRecord record = new ReservoirWarningRecord();

                    if (item.Basin != null)
                    {
                        int index = item.Basin.IndexOf("流域");
                        record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                    }
                    else
                    {
                        record.Basin = item.Basin;
                    }

                    if (item.L > item.Wl1)
                    {
                        record.IsBeyond = true;
                    }
                    else
                    {
                        record.IsBeyond = false;
                    }

                    record.Level = item.L;
                    record.Name = item.Name;
                    record.OverWarningLevel = item.L - item.Wl1;
                    record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                    record.River = item.River;
                    record.Stationid = item.Stationid;
                    record.Time = item.Time;
                    record.WarningLevel = item.Wl1;
                    int tr = dbReader.ComparedWithRecent(item, DateTime.Now.AddDays(-2), DateTime.Now);  //最近2天内的最新趋势
                    if (tr > 0)
                    {
                        record.ComparedBefore = Trend.Rise;
                    }
                    else if (tr == 0)
                    {
                        record.ComparedBefore = Trend.Hold;
                    }
                    else
                    {
                        record.ComparedBefore = Trend.Fall;
                    }

                    if (!(double.IsNaN(record.WarningLevel) || record.WarningLevel <= 0))
                    {
                        infoList.Add(record);
                    }
                }
            }

            return infoList;
        }

        public static List<RiverWarningRecord> RiverGetByWarningList(List<WarningData> listWarning)
        {
            List<RiverWarningRecord> listRecord = new List<RiverWarningRecord>();
            List<RiverWarningRecord> listRtnRecord = new List<RiverWarningRecord>();
            HydrologicReader dbReader = new HydrologicReader();

            foreach (WarningData warning in listWarning)
            {
                RiverWarningRecord record = new RiverWarningRecord();
                bool isBeyond = false;
                double OverWarningLevel = 0;
                HydrologicEntityItem item = new HydrologicEntityItem();
                int t = dbReader.GetLastWarningData(DateTime.Now.AddDays(-2), DateTime.Now, warning.Uid, out item, out isBeyond, out OverWarningLevel);

                if (item.Basin != null || !item.Basin.Contains("其他"))
                {
                    int index = item.Basin.IndexOf("流域");
                    record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                }
                else
                {
                    record.Basin = item.Basin;
                }

                record.IsBeyond = isBeyond;
                record.Level = item.L;
                record.Name = item.Name;
                record.OverWarningLevel = OverWarningLevel;
                record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                record.River = item.River;
                record.Time = item.Time;
                record.WarningLevel = item.Wl1;

                if (t > 0)
                {
                    record.ComparedBefore = Trend.Rise;
                }
                else if (t == 0)
                {
                    record.ComparedBefore = Trend.Hold;
                }
                else
                {
                    record.ComparedBefore = Trend.Fall;
                }

                if (!(double.IsNaN(record.WarningLevel) || record.WarningLevel <= 0))
                {
                    listRecord.Add(record);
                }
            }

            var groupedList = from info in listRecord
                              group info by info.Basin into g
                              select g;

            foreach (var group in groupedList)
            {
                List<RiverWarningRecord> toSortList = new List<RiverWarningRecord>();
                foreach (var item in group)
                {
                    toSortList.Add(item);
                }
                toSortList.Sort(delegate(RiverWarningRecord a, RiverWarningRecord b) { return b.OverWarningLevel.CompareTo(a.OverWarningLevel); });
                listRtnRecord.AddRange(toSortList);
            }

            return listRtnRecord;
        }

        public static List<ReservoirWarningRecord> ResGetByWarningList(List<WarningData> listWarning)
        {
            List<ReservoirWarningRecord> listRecord = new List<ReservoirWarningRecord>();
            List<ReservoirWarningRecord> listRtnRecord = new List<ReservoirWarningRecord>();
            HydrologicReader dbReader = new HydrologicReader();

            foreach (WarningData warning in listWarning)
            {
                ReservoirWarningRecord record = new ReservoirWarningRecord();
                bool isBeyond = false;
                double OverWarningLevel = 0;
                HydrologicEntityItem item = new HydrologicEntityItem();
                int t = dbReader.GetLastWarningData(DateTime.Now.AddDays(-2), DateTime.Now, warning.Uid, out item, out isBeyond, out OverWarningLevel);

                if (item.Basin != null || !item.Basin.Contains("其他"))
                {
                    int index = item.Basin.IndexOf("流域");
                    record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                }
                else
                {
                    record.Basin = item.Basin;
                }

                record.IsBeyond = isBeyond;
                record.Level = item.L;
                record.Name = item.Name;
                record.OverWarningLevel = OverWarningLevel;
                record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                record.River = item.River;
                record.Time = item.Time;
                record.WarningLevel = item.Wl1;

                if (t > 0)
                {
                    record.ComparedBefore = Trend.Rise;
                }
                else if (t == 0)
                {
                    record.ComparedBefore = Trend.Hold;
                }
                else
                {
                    record.ComparedBefore = Trend.Fall;
                }

                if (!(double.IsNaN(record.WarningLevel) || record.WarningLevel <= 0))
                {
                    listRecord.Add(record);
                }
            }

            var groupedList = from info in listRecord
                              group info by info.Basin into g
                              select g;

            foreach (var group in groupedList)
            {
                List<ReservoirWarningRecord> toSortList = new List<ReservoirWarningRecord>();

                foreach (var item in group)
                {
                    toSortList.Add(item);
                }
                toSortList.Sort(delegate (ReservoirWarningRecord a, ReservoirWarningRecord b) { return b.OverWarningLevel.CompareTo(a.OverWarningLevel); });
                listRtnRecord.AddRange(toSortList);
            }

            return listRtnRecord;
        }

        public static List<RiverWarningRecord> LakeGetByWarningList()
        {
            List<WarningData> listWarning = new List<WarningData>();
            listWarning.Add(new WarningData { Uid = "61512000", Time = DateTime.Now, L = 0, WL1 = 0 });  //61512000    城陵矶(七里山)    洞庭湖湖口
            listWarning.Add(new WarningData { Uid = "62601600", Time = DateTime.Now, L = 0, WL1 = 0 });    //62601600        湖口              湖口水道
            listWarning.Add(new WarningData { Uid = "63201999", Time = DateTime.Now, L = 0, WL1 = 0 });   //63201999    太湖水位          太湖
            List<RiverWarningRecord> listRecord = new List<RiverWarningRecord>();
            HydrologicReader dbReader = new HydrologicReader();

            foreach (WarningData warning in listWarning)
            {
                RiverWarningRecord record = new RiverWarningRecord();
                bool isBeyond = false;
                double OverWarningLevel = 0;
                HydrologicEntityItem item = new HydrologicEntityItem();
                int t = dbReader.GetLastWarningData(DateTime.Now.AddDays(-2), DateTime.Now, warning.Uid, out item, out isBeyond, out OverWarningLevel);

                if (item.Basin != null || !item.Basin.Contains("其他"))
                {
                    int index = item.Basin.IndexOf("流域");
                    record.Basin = (index < 0) ? item.Basin : item.Basin.Remove(index);
                }
                else
                {
                    record.Basin = item.Basin;
                }

                record.IsBeyond = isBeyond;
                record.Level = item.L;
                record.Name = item.Name;
                record.OverWarningLevel = OverWarningLevel;
                record.Province = GetProvinceByAdm(item.Administrativeregion);   //从行政地址获得省名
                record.River = item.River;
                record.Time = item.Time;
                record.WarningLevel = item.Wl1;

                if (t > 0)
                {
                    record.ComparedBefore = Trend.Rise;
                }
                else if (t == 0)
                {
                    record.ComparedBefore = Trend.Hold;
                }
                else
                {
                    record.ComparedBefore = Trend.Fall;
                }
                                
                listRecord.Add(record);
            }

           return listRecord;
        }

    }
}
