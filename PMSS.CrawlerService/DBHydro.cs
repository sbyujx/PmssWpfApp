using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.SqlDataAccess;
using PMSS.Log;
using PMSS.WordProduct;

namespace PMSS.CrawlerService
{
    class DBHydro
    {
        public static void AddAllStation(List<HydrologicalRecord> list)
        {
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (HydrologicalRecord record in list)
                {
                    PMSS.SqlDataAccess.Models.hydrologicalstation station = new PMSS.SqlDataAccess.Models.hydrologicalstation();
                    station.UID = record.Station.Uid;
                    station.Address = record.Station.Address;
                    station.AdministrativeRegion = record.Station.AdministrativeRegion;
                    station.Basin = record.Station.Basin;
                    station.HydrographicNet = record.Station.HydrographicNet;
                    station.Latitude = record.Station.Latitude;
                    station.Longitude = record.Station.Longitude;
                    station.Name = record.Station.Name;
                    station.River = record.Station.River;
                    station.Type = record.Station.Type;

                    var exsitStation = db.hydrologicalstation.Find(station.UID);
                    if (exsitStation == null)
                    {
                        db.hydrologicalstation.Add(station);
                    }
                }
                db.SaveChanges();
            }
        }

        public static void AddAllRecord(List<HydrologicalRecord> list, List<OldData> listOldData, ref List<WarningData> listRiverWarningData, ref List<WarningData> listResWarningData)
        {
            int count = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (HydrologicalRecord record in list)
                {
                    if (!Double.IsNaN(record.L) && !Double.IsNaN(record.Wl1) && record.L > record.Wl1 && record.Wl1 > 1)
                    {
                        WarningData d = new WarningData();
                        d.Uid = record.Station.Uid;
                        d.Time = record.Time;
                        d.L = record.L;
                        d.WL1 = record.Wl1;
                        if(record.Station.Type.Equals("河道站"))
                        {
                            listRiverWarningData.Add(d);
                        }

                        if (record.Station.Type.Equals("水库站"))
                        {
                            listResWarningData.Add(d);
                        }
                    }
                    var exsit = from c in listOldData
                                where c.Uid == record.Station.Uid
                                where c.Time == record.Time
                                select c;

                    if (!exsit.Any())
                    {
                        PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                        data.StationId = record.Station.Uid;
                        if (Double.IsNaN(record.L))
                        {
                            data.L = null;
                        }
                        else
                        {
                            data.L = record.L;
                        }

                        if (Double.IsNaN(record.Q))
                        {
                            data.Q = null;
                        }
                        else
                        {
                            data.Q = record.Q;
                        }

                        if (Double.IsNaN(record.Wl1))
                        {
                            data.WL1 = null;
                        }
                        else
                        {
                            data.WL1 = record.Wl1;
                        }

                        if (Double.IsNaN(record.Wl2))
                        {
                            data.WL2 = null;
                        }
                        else
                        {
                            data.WL2 = record.Wl2;
                        }

                        data.Time = record.Time;
                        data.IsSign = record.isKey;
                        db.hydrologicaldata.Add(data);
                        count++;
                    }
                }
                db.SaveChanges();
            }

            LogHelper.WriteLog(typeof(DBHydro), "数据库新增水情数据" + count + "条！");
        }

        public static List<OldData> UpdateBigRiver(List<RiverHydrology> list, List<OldData> listOldData, ref List<WarningData> listWarningData)
        {
            List<OldData> newData = new List<OldData>();
            int riverCount = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (RiverHydrology river in list)
                {
                    var exsitStation = from c in db.hydrologicalstation
                                       where c.Name == river.StationName
                                       where c.River == river.RiverName
                                       where c.Type == "河道站"
                                       select new
                                       {
                                           StationId = c.UID
                                       };  //查找该站点的ID号

                    if (exsitStation.Any())  //存在该站点
                    {
                        string sid = exsitStation.FirstOrDefault().StationId;
                        newData.Add(new OldData(sid, river.Time));
                        if (river.WaterLevel > 0 && river.WarningWaterLevel > 1 && river.WaterLevel > (river.WarningWaterLevel - 0.5))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = sid;
                            d.Time = river.Time;
                            d.L = river.WaterLevel;
                            d.WL1 = river.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                        var exsitRecord = from c in listOldData
                                          where c.Uid == sid
                                          where c.Time == river.Time
                                          select c;
                        if (!exsitRecord.Any())  //不存在相同记录
                        {
                            PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                            data.StationId = sid;
                            data.L = river.WaterLevel;  //水位
                            data.Q = river.Flow;  //流量
                            data.WL1 = river.WarningWaterLevel;
                            data.Time = river.Time;
                            data.IsSign = false;
                            db.hydrologicaldata.Add(data);
                            riverCount++;
                        }
                    }
                    else  //不存在该站点
                    {
                        //增加站点数据
                        PMSS.SqlDataAccess.Models.hydrologicalstation station = new PMSS.SqlDataAccess.Models.hydrologicalstation();
                        station.Name = river.StationName;
                        station.UID = "hd" + (river.StationName + river.RiverName + river.StationAddress).GetHashCode().ToString();
                        station.River = river.RiverName;
                        station.Address = river.StationAddress;
                        station.Basin = river.Basin;
                        station.AdministrativeRegion = river.AdministrativeRegion;
                        station.Type = "河道站";
                        db.hydrologicalstation.Add(station);

                        //增加水情数据
                        PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                        data.StationId = river.StationName;
                        data.L = river.WaterLevel;  //水位
                        data.Q = river.Flow;  //流量
                        data.WL1 = river.WarningWaterLevel;
                        data.Time = river.Time;
                        data.IsSign = false;
                        db.hydrologicaldata.Add(data);
                        riverCount++;
                        newData.Add(new OldData(station.UID, river.Time));
                        if (river.WaterLevel > 0 && river.WarningWaterLevel > 1 && river.WaterLevel > (river.WarningWaterLevel - 0.5))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = station.UID;
                            d.Time = river.Time;
                            d.L = river.WaterLevel;
                            d.WL1 = river.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                    }
                }
                db.SaveChanges();
                LogHelper.WriteLog(typeof(DBHydro), "数据库新增大江大河水情数据" + riverCount + "条！");
            }
            return newData;
        }

        public static List<OldData> UpdateBigReservoir(List<ReservoirHydrology> list, List<OldData> listOldData, ref List<WarningData> listWarningData)
        {
            List<OldData> newData = new List<OldData>();
            int reservoirCount = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (ReservoirHydrology reservoir in list)
                {
                    var exsitStation = from c in db.hydrologicalstation
                                       where c.Name == reservoir.StationName
                                       where c.River == reservoir.RiverName
                                       where c.Type == "水库站"
                                       select new
                                       {
                                           StationId = c.UID
                                       };  //查找该站点的ID号

                    if (exsitStation.Any())  //存在该站点
                    {
                        string sid = exsitStation.FirstOrDefault().StationId;
                        newData.Add(new OldData(sid, reservoir.Time));
                        if (reservoir.WaterLevel > 0 && reservoir.WarningWaterLevel > 1 && reservoir.WaterLevel > (reservoir.WarningWaterLevel - 0.5))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = sid;
                            d.Time = reservoir.Time;
                            d.L = reservoir.WaterLevel;
                            d.WL1 = reservoir.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                        var exsitRecord = from c in listOldData
                                          where c.Uid == sid
                                          where c.Time == reservoir.Time
                                          select c;
                        if (!exsitRecord.Any())  //不存在相同记录
                        {
                            PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                            data.StationId = sid;
                            data.L = reservoir.WaterLevel;  //水位
                            data.Q = reservoir.Pondage;  //流量
                            data.WL1 = reservoir.WarningWaterLevel;
                            data.Time = reservoir.Time;
                            data.IsSign = false;
                            db.hydrologicaldata.Add(data);
                            reservoirCount++;
                        }
                    }
                    else  //不存在该站点
                    {
                        if (reservoir.StationName.Equals("石门"))
                        {
                            continue;
                        }
                        //增加站点数据
                        PMSS.SqlDataAccess.Models.hydrologicalstation station = new PMSS.SqlDataAccess.Models.hydrologicalstation();
                        station.Name = reservoir.StationName;
                        
                        station.River = reservoir.RiverName;
                        station.UID = "sk" + (reservoir.StationName + reservoir.RiverName + reservoir.Basin + reservoir.AdministrativeRegion).GetHashCode().ToString();
                        station.Basin = reservoir.Basin;
                        station.AdministrativeRegion = reservoir.AdministrativeRegion;
                        station.Address = reservoir.StationAddress;
                        station.Type = "水库站";
                        db.hydrologicalstation.Add(station);

                        //增加水情数据
                        PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                        data.StationId = reservoir.StationName;
                        data.L = reservoir.WaterLevel;  //水位
                        data.Q = reservoir.Pondage;  //流量
                        data.WL1 = reservoir.WarningWaterLevel;
                        data.Time = reservoir.Time;
                        data.IsSign = false;
                        db.hydrologicaldata.Add(data);
                        reservoirCount++;
                        newData.Add(new OldData(station.UID, reservoir.Time));
                        if (reservoir.WaterLevel > 0 && reservoir.WarningWaterLevel > 1 && reservoir.WaterLevel > (reservoir.WarningWaterLevel - 0.5))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = station.UID;
                            d.Time = reservoir.Time;
                            d.L = reservoir.WaterLevel;
                            d.WL1 = reservoir.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                    }
                }
                db.SaveChanges();
                LogHelper.WriteLog(typeof(DBHydro), "数据库新增大型水库水情数据" + reservoirCount + "条！");
            }

            return newData;
        }

        public static List<OldData> UpdateKeyRiver(List<RiverHydrology> list, List<OldData> listOldData)
        {
            List<OldData> newData = new List<OldData>();
            int keyRiverCount = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (RiverHydrology river in list)
                {
                    var exsitStation = from c in db.hydrologicalstation
                                       where c.Name == river.StationName
                                       where c.Type == "河道站"
                                       select new
                                       {
                                           StationId = c.UID
                                       };  //查找该站点的ID号

                    if (exsitStation.Any())  //存在该站点
                    {

                        string sid = exsitStation.FirstOrDefault().StationId;
                        newData.Add(new OldData(sid, river.Time));
                        var exsitRecord = from c in listOldData
                                          where c.Uid == sid
                                          where c.Time == river.Time
                                          select c;
                        if (!exsitRecord.Any())  //不存在相同记录
                        {
                            PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                            data.StationId = sid;
                            data.L = river.WaterLevel;  //水位
                            data.Q = river.Flow;  //流量
                            data.WL1 = river.WarningWaterLevel;
                            data.Time = river.Time;
                            data.IsSign = true;
                            db.hydrologicaldata.Add(data);
                            keyRiverCount++;
                        }
                        else
                        {
                            //if (exsitRecord.FirstOrDefault().IsSign == false)  //若存在数据为非重点数据，更新为重点数据
                            //{
                            //    exsitRecord.FirstOrDefault().IsSign = true;
                            //    //int id = exsitRecord.FirstOrDefault().RecordId;
                            //    //LogHelper.WriteLog(typeof(DBHydro), "数据库将ID为" + id + "的河道水情数据更新为重点数据！");
                            //    updateKeyCount++;
                            //}

                        }
                    }
                    else  //不存在该站点
                    {
                        //增加站点数据
                        PMSS.SqlDataAccess.Models.hydrologicalstation station = new PMSS.SqlDataAccess.Models.hydrologicalstation();
                        station.Name = river.StationName;
                        station.UID = "zdhd" + (river.StationName + river.RiverName + river.StationAddress).GetHashCode().ToString();
                        station.River = river.RiverName;
                        station.Address = river.StationAddress;
                        station.Type = "河道站";
                        db.hydrologicalstation.Add(station);

                        //增加水情数据
                        PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                        data.StationId = river.StationName;
                        data.L = river.WaterLevel;  //水位
                        data.Q = river.Flow;  //流量
                        data.WL1 = river.WarningWaterLevel;
                        data.Time = river.Time;
                        data.IsSign = true;
                        db.hydrologicaldata.Add(data);
                        keyRiverCount++;

                        newData.Add(new OldData(station.UID, river.Time));
                    }
                }
                db.SaveChanges();
                LogHelper.WriteLog(typeof(DBHydro), "数据库新增重点河道水情数据" + keyRiverCount + "条！");
            }

            return newData;
        }

        public static List<OldData> UpdateKeyReservoir(List<ReservoirHydrology> list, List<OldData> listOldData, ref List<WarningData> listWarningData)
        {
            List<OldData> newData = new List<OldData>();
            int keyReservoirCount = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                foreach (ReservoirHydrology reservoir in list)
                {
                    var exsitStation = from c in db.hydrologicalstation
                                       where c.Name == reservoir.StationName
                                       where c.Type == "水库站"
                                       select new
                                       {
                                           StationId = c.UID
                                       };  //查找该站点的ID号

                    if (exsitStation.Any())  //存在该站点
                    {
                        string sid = exsitStation.FirstOrDefault().StationId;
                        newData.Add(new OldData(sid, reservoir.Time));
                        if (reservoir.WaterLevel > 0 && reservoir.WarningWaterLevel > 1 && reservoir.WaterLevel > (reservoir.WarningWaterLevel - 20))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = sid;
                            d.Time = reservoir.Time;
                            d.L = reservoir.WaterLevel;
                            d.WL1 = reservoir.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                        var exsitRecord = from c in listOldData
                                          where c.Uid == sid
                                          where c.Time == reservoir.Time
                                          select c;
                        if (!exsitRecord.Any())  //不存在相同记录
                        {
                            PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                            data.StationId = sid;
                            data.L = reservoir.WaterLevel;  //水位
                            data.Q = reservoir.Pondage;  //蓄水量
                            data.WL1 = reservoir.WarningWaterLevel;
                            data.Time = reservoir.Time;
                            data.IsSign = true;
                            db.hydrologicaldata.Add(data);
                            keyReservoirCount++;
                        }
                        else
                        {
                            //if (exsitRecord.FirstOrDefault().IsSign == false)  //若存在数据为非重点数据，更新为重点数据
                            //{
                            //    exsitRecord.FirstOrDefault().IsSign = true;
                            //    //int id = exsitRecord.FirstOrDefault().RecordId;
                            //    //LogHelper.WriteLog(typeof(DBHydro), "数据库将ID为" + id + "的水库水情数据更新为重点数据！");
                            //    updateKeyCount++;
                            //}

                        }
                    }
                    else  //不存在该站点
                    {
                        //增加站点数据
                        PMSS.SqlDataAccess.Models.hydrologicalstation station = new PMSS.SqlDataAccess.Models.hydrologicalstation();
                        station.Name = reservoir.StationName;
                        station.UID = "zdsk" + (reservoir.StationName + reservoir.RiverName + reservoir.StationAddress).GetHashCode().ToString();
                        station.River = reservoir.RiverName;
                        station.Address = reservoir.StationAddress;
                        station.Type = "水库站";
                        db.hydrologicalstation.Add(station);

                        //增加水情数据
                        PMSS.SqlDataAccess.Models.hydrologicaldata data = new PMSS.SqlDataAccess.Models.hydrologicaldata();
                        data.StationId = reservoir.StationName;
                        data.L = reservoir.WaterLevel;  //水位
                        data.Q = reservoir.Pondage;  //蓄水量
                        data.WL1 = reservoir.WarningWaterLevel;
                        data.Time = reservoir.Time;
                        data.IsSign = true;
                        db.hydrologicaldata.Add(data);
                        keyReservoirCount++;
                        newData.Add(new OldData(station.UID, reservoir.Time));

                        if (reservoir.WaterLevel > 0 && reservoir.WarningWaterLevel > 1 && reservoir.WaterLevel > (reservoir.WarningWaterLevel - 20))  //超警数据
                        {
                            WarningData d = new WarningData();
                            d.Uid = station.UID;
                            d.Time = reservoir.Time;
                            d.L = reservoir.WaterLevel;
                            d.WL1 = reservoir.WarningWaterLevel;
                            listWarningData.Add(d);
                        }
                    }
                }
                db.SaveChanges();
                LogHelper.WriteLog(typeof(DBHydro), "数据库新增重点水库水情数据" + keyReservoirCount + "条！");
            }

            return newData;
        }

        public static List<PMSS.SqlDataAccess.Models.hydrologicalstation> GetAllNumsStation()
        {
            List<PMSS.SqlDataAccess.Models.hydrologicalstation> listId = new List<PMSS.SqlDataAccess.Models.hydrologicalstation>();
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                List<PMSS.SqlDataAccess.Models.hydrologicalstation> listStation = db.hydrologicalstation.ToList();
                foreach (PMSS.SqlDataAccess.Models.hydrologicalstation s in listStation)
                {
                    double number;
                    bool isNum = double.TryParse(s.UID, out number);
                    if (isNum)
                    {
                        listId.Add(s);
                    }
                }
            }

            return listId;
        }

        public static int GetMaxDataId()
        {
            int maxId = 0;
            using (var db = new PMSS.SqlDataAccess.Models.HydroModel())
            {
                var query = (from d in db.hydrologicaldata
                             select d.RecordId).Max();
                maxId = query;
            }
            return maxId;
        }
    }
}
