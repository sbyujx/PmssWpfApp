using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.SqlDataAccess
{
    public class HydrologicReader
    {
        public HydrologicEntity RetrieveEntity(DateTime fromDate, DateTime toDate)
        {
            var entity = new HydrologicEntity();
            entity.Items = new List<HydrologicEntityItem>();
            var listRepeat = new List<HydrologicEntityItem>();

            Models.HydroModel db = new Models.HydroModel();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = from d in datas
                        join s in stations
                        on d.StationId equals s.UID
                        where (d.Time < toDate) && (d.Time > fromDate)
                        select new
                        {
                            RecordId = d.RecordId,
                            StationId = d.StationId,
                            L = d.L,
                            Q = d.Q,
                            WL1 = d.WL1,
                            WL2 = d.WL2,
                            Time = d.Time,
                            IsSign = d.IsSign,
                            Name = s.Name,
                            Longitude = s.Longitude,
                            Latitude = s.Latitude,
                            River = s.River,
                            Hydrographicnet = s.HydrographicNet,
                            Basin = s.Basin,
                            Administrativeregion = s.AdministrativeRegion,
                            Address = s.Address,
                            Type = s.Type
                        };

            /*var query = from e in
                           (from d in datas
                            join s in stations
                            on d.StationId equals s.UID
                            where (d.Time < toDate) && (d.Time > fromDate)
                            select new
                            {
                                StationId = d.StationId,
                                RecordId = d.RecordId,
                                Time = d.Time,
                                River = s.River
                            })
                        group e by e.StationId into g
                        select new
                        {
                            StationId = g.FirstOrDefault().StationId,
                            RecordId = g.FirstOrDefault().RecordId,
                            Time = g.Max(tg => tg.Time),
                            River = g.FirstOrDefault().River
                        };*/


            foreach (var data in query)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                try
                {
                    item.L = (double)data.L;
                }
                catch (InvalidOperationException)
                {
                    item.L = double.NaN;
                }

                try
                {
                    item.Q = (double)data.Q;
                }
                catch (InvalidOperationException)
                {
                    item.Q = double.NaN;
                }

                try
                {
                    item.Wl1 = (double)data.WL1;
                }
                catch (InvalidOperationException)
                {
                    item.Wl1 = double.NaN;
                }
                try
                {
                    item.Wl2 = (double)data.WL2;
                }
                catch (InvalidOperationException)
                {
                    item.Wl2 = double.NaN;
                }

                item.Time = (DateTime)data.Time;
                try
                {
                    item.Issign = (bool)data.IsSign;
                }
                catch (InvalidOperationException)
                {
                    item.Issign = false;
                }
                item.Name = data.Name;
                try
                {
                    item.Longitude = (double)data.Longitude;
                }
                catch (InvalidOperationException)
                {
                    item.Longitude = double.NaN;
                }
                try
                {
                    item.Latitude = (double)data.Latitude;
                }
                catch (InvalidOperationException)
                {
                    item.Latitude = double.NaN;
                }
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                listRepeat.Add(item);
            }

            var queryNotRepeat = from t in listRepeat
                                 group t by t.Stationid into g
                                 select new
                                 {
                                     RecordId = g.FirstOrDefault().Recordid,
                                     StationId = g.FirstOrDefault().Stationid,
                                     L = g.FirstOrDefault().L,
                                     Q = g.FirstOrDefault().Q,
                                     WL1 = g.FirstOrDefault().Wl1,
                                     WL2 = g.FirstOrDefault().Wl2,
                                     Time = g.Max(tg => tg.Time),
                                     IsSign = g.FirstOrDefault().Issign,
                                     Name = g.FirstOrDefault().Name,
                                     Longitude = g.FirstOrDefault().Longitude,
                                     Latitude = g.FirstOrDefault().Latitude,
                                     River = g.FirstOrDefault().River,
                                     Hydrographicnet = g.FirstOrDefault().Hydrographicnet,
                                     Basin = g.FirstOrDefault().Basin,
                                     Administrativeregion = g.FirstOrDefault().Administrativeregion,
                                     Address = g.FirstOrDefault().Address,
                                     Type = g.FirstOrDefault().Type
                                 };

            foreach (var data in queryNotRepeat)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                item.L = data.L;
                item.Q = data.Q;
                item.Wl1 = data.WL1;
                item.Wl2 = data.WL2;
                item.Time = (DateTime)data.Time;
                item.Issign = data.IsSign;
                item.Name = data.Name;
                item.Longitude = data.Longitude;
                item.Latitude = data.Latitude;
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                entity.Items.Add(item);
            }



            return entity;
        }

        public HisHydrologicEntity RetrieveHisEntity(DateTime fromDate, DateTime toDate)
        {
            var entity = new HisHydrologicEntity();
            entity.Items = new List<HisHydrologicEntityItem>();
            var listRepeat = new List<HydrologicEntityItem>();

            Models.HydroModel db = new Models.HydroModel();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = from d in datas
                        join s in stations
                        on d.StationId equals s.UID
                        where (d.Time < toDate) && (d.Time > fromDate)
                        select new
                        {
                            RecordId = d.RecordId,
                            StationId = d.StationId,
                            L = d.L,
                            Q = d.Q,
                            WL1 = d.WL1,
                            WL2 = d.WL2,
                            Time = d.Time,
                            IsSign = d.IsSign,
                            Name = s.Name,
                            Longitude = s.Longitude,
                            Latitude = s.Latitude,
                            River = s.River,
                            Hydrographicnet = s.HydrographicNet,
                            Basin = s.Basin,
                            Administrativeregion = s.AdministrativeRegion,
                            Address = s.Address,
                            Type = s.Type
                        };

            /*var query = from e in
                           (from d in datas
                            join s in stations
                            on d.StationId equals s.UID
                            where (d.Time < toDate) && (d.Time > fromDate)
                            select new
                            {
                                StationId = d.StationId,
                                RecordId = d.RecordId,
                                Time = d.Time,
                                River = s.River
                            })
                        group e by e.StationId into g
                        select new
                        {
                            StationId = g.FirstOrDefault().StationId,
                            RecordId = g.FirstOrDefault().RecordId,
                            Time = g.Max(tg => tg.Time),
                            River = g.FirstOrDefault().River
                        };*/


            foreach (var data in query)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                try
                {
                    item.L = (double)data.L;
                }
                catch (InvalidOperationException)
                {
                    item.L = double.NaN;
                }

                try
                {
                    item.Q = (double)data.Q;
                }
                catch (InvalidOperationException)
                {
                    item.Q = double.NaN;
                }

                try
                {
                    item.Wl1 = (double)data.WL1;
                }
                catch (InvalidOperationException)
                {
                    item.Wl1 = double.NaN;
                }
                try
                {
                    item.Wl2 = (double)data.WL2;
                }
                catch (InvalidOperationException)
                {
                    item.Wl2 = double.NaN;
                }

                item.Time = (DateTime)data.Time;
                try
                {
                    item.Issign = (bool)data.IsSign;
                }
                catch (InvalidOperationException)
                {
                    item.Issign = false;
                }
                item.Name = data.Name;
                try
                {
                    item.Longitude = (double)data.Longitude;
                }
                catch (InvalidOperationException)
                {
                    item.Longitude = double.NaN;
                }
                try
                {
                    item.Latitude = (double)data.Latitude;
                }
                catch (InvalidOperationException)
                {
                    item.Latitude = double.NaN;
                }
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                listRepeat.Add(item);
            }

            var queryNotRepeat = from t in listRepeat
                                 group t by t.Stationid into g
                                 select new
                                 {
                                     RecordId = g.FirstOrDefault().Recordid,
                                     StationId = g.FirstOrDefault().Stationid,
                                     L = g.FirstOrDefault().L,
                                     Q = g.FirstOrDefault().Q,
                                     WL1 = g.FirstOrDefault().Wl1,
                                     WL2 = g.FirstOrDefault().Wl2,
                                     Time = g.Max(tg => tg.Time),
                                     IsSign = g.FirstOrDefault().Issign,
                                     Name = g.FirstOrDefault().Name,
                                     Longitude = g.FirstOrDefault().Longitude,
                                     Latitude = g.FirstOrDefault().Latitude,
                                     River = g.FirstOrDefault().River,
                                     Hydrographicnet = g.FirstOrDefault().Hydrographicnet,
                                     Basin = g.FirstOrDefault().Basin,
                                     Administrativeregion = g.FirstOrDefault().Administrativeregion,
                                     Address = g.FirstOrDefault().Address,
                                     Type = g.FirstOrDefault().Type,
                                     MaxL = g.Max(tt => tt.L)
                                 };

            foreach (var data in queryNotRepeat)
            {
                HisHydrologicEntityItem item = new HisHydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                item.L = data.L;
                item.Q = data.Q;
                item.Wl1 = data.WL1;
                item.Wl2 = data.WL2;
                item.Time = (DateTime)data.Time;
                item.Issign = data.IsSign;
                item.Name = data.Name;
                item.Longitude = data.Longitude;
                item.Latitude = data.Latitude;
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                item.MaxL = data.MaxL;
                entity.Items.Add(item);
            }

            return entity;
        }

        public HydrologicEntity RetrieveEntity(DateTime fromDate, DateTime toDate, string type)
        {
            var entity = new HydrologicEntity();
            entity.Items = new List<HydrologicEntityItem>();
            var listRepeat = new List<HydrologicEntityItem>();

            Models.HydroModel db = new Models.HydroModel();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = from d in datas
                        join s in stations
                        on d.StationId equals s.UID
                        where (d.Time < toDate) && (d.Time > fromDate)
                        where s.Type.Equals(type)
                        select new
                        {
                            RecordId = d.RecordId,
                            StationId = d.StationId,
                            L = d.L,
                            Q = d.Q,
                            WL1 = d.WL1,
                            WL2 = d.WL2,
                            Time = d.Time,
                            IsSign = d.IsSign,
                            Name = s.Name,
                            Longitude = s.Longitude,
                            Latitude = s.Latitude,
                            River = s.River,
                            Hydrographicnet = s.HydrographicNet,
                            Basin = s.Basin,
                            Administrativeregion = s.AdministrativeRegion,
                            Address = s.Address,
                            Type = s.Type
                        };

            foreach (var data in query)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                try
                {
                    item.L = (double)data.L;
                }
                catch (InvalidOperationException)
                {
                    item.L = double.NaN;
                }

                try
                {
                    item.Q = (double)data.Q;
                }
                catch (InvalidOperationException)
                {
                    item.Q = double.NaN;
                }

                try
                {
                    item.Wl1 = (double)data.WL1;
                }
                catch (InvalidOperationException)
                {
                    item.Wl1 = double.NaN;
                }
                try
                {
                    item.Wl2 = (double)data.WL2;
                }
                catch (InvalidOperationException)
                {
                    item.Wl2 = double.NaN;
                }

                item.Time = (DateTime)data.Time;
                try
                {
                    item.Issign = (bool)data.IsSign;
                }
                catch (InvalidOperationException)
                {
                    item.Issign = false;
                }
                item.Name = data.Name;
                try
                {
                    item.Longitude = (double)data.Longitude;
                }
                catch (InvalidOperationException)
                {
                    item.Longitude = double.NaN;
                }
                try
                {
                    item.Latitude = (double)data.Latitude;
                }
                catch (InvalidOperationException)
                {
                    item.Latitude = double.NaN;
                }
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                listRepeat.Add(item);
            }

            var queryNotRepeat = from t in listRepeat
                                 group t by t.Stationid into g
                                 select new
                                 {
                                     RecordId = g.FirstOrDefault().Recordid,
                                     StationId = g.FirstOrDefault().Stationid,
                                     L = g.FirstOrDefault().L,
                                     Q = g.FirstOrDefault().Q,
                                     WL1 = g.FirstOrDefault().Wl1,
                                     WL2 = g.FirstOrDefault().Wl2,
                                     Time = g.Max(tg => tg.Time),
                                     IsSign = g.FirstOrDefault().Issign,
                                     Name = g.FirstOrDefault().Name,
                                     Longitude = g.FirstOrDefault().Longitude,
                                     Latitude = g.FirstOrDefault().Latitude,
                                     River = g.FirstOrDefault().River,
                                     Hydrographicnet = g.FirstOrDefault().Hydrographicnet,
                                     Basin = g.FirstOrDefault().Basin,
                                     Administrativeregion = g.FirstOrDefault().Administrativeregion,
                                     Address = g.FirstOrDefault().Address,
                                     Type = g.FirstOrDefault().Type
                                 };

            foreach (var data in queryNotRepeat)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                item.L = data.L;
                item.Q = data.Q;
                item.Wl1 = data.WL1;
                item.Wl2 = data.WL2;
                item.Time = (DateTime)data.Time;
                item.Issign = data.IsSign;
                item.Name = data.Name;
                item.Longitude = data.Longitude;
                item.Latitude = data.Latitude;
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                entity.Items.Add(item);
            }

            return entity;
        }

        public HydrologicEntity RetrieveHighestEntity(DateTime fromDate, DateTime toDate, string type)
        {
            var entity = new HydrologicEntity();
            entity.Items = new List<HydrologicEntityItem>();
            var listRepeat = new List<HydrologicEntityItem>();

            Models.HydroModel db = new Models.HydroModel();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = from d in datas
                        join s in stations
                        on d.StationId equals s.UID
                        where (d.Time < toDate) && (d.Time > fromDate)
                        where s.Type.Equals(type)
                        select new
                        {
                            RecordId = d.RecordId,
                            StationId = d.StationId,
                            L = d.L,
                            Q = d.Q,
                            WL1 = d.WL1,
                            WL2 = d.WL2,
                            Time = d.Time,
                            IsSign = d.IsSign,
                            Name = s.Name,
                            Longitude = s.Longitude,
                            Latitude = s.Latitude,
                            River = s.River,
                            Hydrographicnet = s.HydrographicNet,
                            Basin = s.Basin,
                            Administrativeregion = s.AdministrativeRegion,
                            Address = s.Address,
                            Type = s.Type
                        };

            foreach (var data in query)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                try
                {
                    item.L = (double)data.L;
                }
                catch (InvalidOperationException)
                {
                    item.L = double.NaN;
                }

                try
                {
                    item.Q = (double)data.Q;
                }
                catch (InvalidOperationException)
                {
                    item.Q = double.NaN;
                }

                try
                {
                    item.Wl1 = (double)data.WL1;
                }
                catch (InvalidOperationException)
                {
                    item.Wl1 = double.NaN;
                }
                try
                {
                    item.Wl2 = (double)data.WL2;
                }
                catch (InvalidOperationException)
                {
                    item.Wl2 = double.NaN;
                }

                item.Time = (DateTime)data.Time;
                try
                {
                    item.Issign = (bool)data.IsSign;
                }
                catch (InvalidOperationException)
                {
                    item.Issign = false;
                }
                item.Name = data.Name;
                try
                {
                    item.Longitude = (double)data.Longitude;
                }
                catch (InvalidOperationException)
                {
                    item.Longitude = double.NaN;
                }
                try
                {
                    item.Latitude = (double)data.Latitude;
                }
                catch (InvalidOperationException)
                {
                    item.Latitude = double.NaN;
                }
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                listRepeat.Add(item);
            }

            var queryNotRepeat = from t in listRepeat
                                 group t by t.Stationid into g
                                 select new
                                 {
                                     RecordId = g.FirstOrDefault().Recordid,
                                     StationId = g.FirstOrDefault().Stationid,
                                     L = g.Max( tg => tg.L),
                                     Q = g.Max(tf => tf.Q),
                                     WL1 = g.FirstOrDefault().Wl1,
                                     WL2 = g.FirstOrDefault().Wl2,
                                     Time = g.FirstOrDefault().Time,
                                     IsSign = g.FirstOrDefault().Issign,
                                     Name = g.FirstOrDefault().Name,
                                     Longitude = g.FirstOrDefault().Longitude,
                                     Latitude = g.FirstOrDefault().Latitude,
                                     River = g.FirstOrDefault().River,
                                     Hydrographicnet = g.FirstOrDefault().Hydrographicnet,
                                     Basin = g.FirstOrDefault().Basin,
                                     Administrativeregion = g.FirstOrDefault().Administrativeregion,
                                     Address = g.FirstOrDefault().Address,
                                     Type = g.FirstOrDefault().Type
                                 };

            foreach (var data in queryNotRepeat)
            {
                HydrologicEntityItem item = new HydrologicEntityItem();
                item.Recordid = data.RecordId;
                item.Stationid = data.StationId;
                item.L = data.L;
                item.Q = data.Q;
                item.Wl1 = data.WL1;
                item.Wl2 = data.WL2;
                item.Time = (DateTime)data.Time;
                item.Issign = data.IsSign;
                item.Name = data.Name;
                item.Longitude = data.Longitude;
                item.Latitude = data.Latitude;
                item.River = data.River;
                item.Hydrographicnet = data.Hydrographicnet;
                item.Basin = data.Basin;
                item.Administrativeregion = data.Administrativeregion;
                item.Address = data.Address;
                item.Type = data.Type;
                entity.Items.Add(item);
            }

            return entity;
        }


        public int ComparedWithRecent(HydrologicEntityItem item, DateTime fromDate, DateTime toDate)
        {
            Models.HydroModel db = new Models.HydroModel();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = from d in datas
                        where (d.Time < toDate) && (d.Time > fromDate)
                        where d.StationId.Equals(item.Stationid)
                        orderby d.Time descending
                        select new
                        {
                            RecordId = d.RecordId,
                            StationId = d.StationId,
                            L = d.L,
                            Q = d.Q,
                            WL1 = d.WL1,
                            WL2 = d.WL2,
                            Time = d.Time,
                            IsSign = d.IsSign,
                        };

            try
            {
                var first = query.First();
                var second = query.Skip(1).Take(1).Single();
                if (first == null || second == null)
                {
                    return 0;
                }
                else
                {
                    if ((first.L - second.L) > 0.001)
                    {
                        return 1;
                    }
                    else if ((first.L - second.L) < -0.001)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetLastWarningData(DateTime fromDate, DateTime toDate, string Uid, out HydrologicEntityItem item, out bool IsBeyond, out double OverWarningLevel)
        {

            //public string Stationid { get; set; }  //站点号
            //public string Name { get; set; }   //站名
            //public string Basin { get; set; }   //流域
            //public string Province { get; set; }  //省区
            //public string River { get; set; }   //河流名
            //public DateTime Time { get; set; }  //时间
            //public double Level { get; set; }  //水位（河道站）
            //public double WarningLevel { get; set; }  //警戒水位（河道站）
            //public double OverWarningLevel { get; set; }  //超警戒水位
            //public bool IsBeyond { get; set; }  //是否已经超过警戒水位
            //public Trend ComparedBefore { get; set; }  //水位与之前比较的趋势

            Models.HydroModel db = new Models.HydroModel();
            item = new HydrologicEntityItem();
            var datas = db.hydrologicaldata;
            var stations = db.hydrologicalstation;

            var query = (from d in datas
                        where (d.Time < toDate) && (d.Time > fromDate)
                        where d.StationId.Equals(Uid)
                        orderby d.Time descending
                        select new
                        {
                            L = d.L,
                            WL1 = d.WL1,
                            Time = d.Time,
                        }).Take(2);

            var queryStation = from s in stations
                               where s.UID.Equals(Uid)
                               select new
                               {
                                   Name = s.Name,
                                   Basin = s.Basin,
                                   AdministrativeRegion = s.AdministrativeRegion,
                                   River = s.River
                               };

            item.Name = queryStation.FirstOrDefault().Name;
            item.Basin = queryStation.FirstOrDefault().Basin;
            item.Administrativeregion = queryStation.FirstOrDefault().AdministrativeRegion;
            item.River = queryStation.FirstOrDefault().River;


            try
            {
                var first = query.First();
                var second = query.Skip(1).Take(1).Single();
                if (first == null || second == null)
                {
                    IsBeyond = false;
                    OverWarningLevel = 0;
                    return 0;
                }
                else
                {
                    item.L = (double)first.L;
                    item.Wl1 = (double)first.WL1;
                    item.Time = (DateTime)first.Time;
                    if(first.L > first.WL1)
                    {
                        IsBeyond = true;
                    }
                    else
                    {
                        IsBeyond = false;
                    }

                    OverWarningLevel = (double)first.L - (double)first.WL1;

                    if ((first.L - second.L) > 0.001)
                    {
                        return 1;
                    }
                    else if ((first.L - second.L) < -0.001)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                IsBeyond = false;
                OverWarningLevel = 0;
                return 0;
            }
        }
    }
}
