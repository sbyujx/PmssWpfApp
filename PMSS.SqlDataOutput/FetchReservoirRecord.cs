using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq.Expressions;


namespace PMSS.SqlDataOutput
{
    public class FetchReservoirRecord
    {
        private Configuration myConfigration;
        private ISessionFactory mySessionFactory;
        private ISession mySession;
        private bool isSetDuration = false;
        private DateTime from;
        private DateTime to;
        private bool isSetStation = false;
        private List<string> stationList;

        public FetchReservoirRecord()
        {
            reservoirList = new List<ReservoirHydrologyRecord>();
            myConfigration = new Configuration();
            myConfigration.Configure();
            try
            {
                mySessionFactory = myConfigration.BuildSessionFactory();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            mySession = mySessionFactory.OpenSession();
        }

        private List<ReservoirHydrologyRecord> reservoirList;
        public List<ReservoirHydrologyRecord> ReservoirList
        {
            get
            {
                return reservoirList;
            }
        }

        //查询所有水库站点信息
        public void GetAllReservoirStation()
        {
            var collection = mySession.CreateCriteria<Hydrologicalstation>()
                .Add(Restrictions.Eq("Type", "水库站"))
                .List<Hydrologicalstation>();

            foreach (Hydrologicalstation station in collection)
            {
                ReservoirHydrologyRecord rhr = new ReservoirHydrologyRecord();
                rhr.StationId = station.Uid;
                rhr.StationName = station.Name;
                rhr.StationLocation = station.Administrativeregion;
                rhr.RiverName = station.River;
                reservoirList.Add(rhr);
            }
        }

        //设置检索的起止时间
        public void SetDuration(DateTime from, DateTime to)
        {
            isSetDuration = true;
            this.from = from;
            this.to = to;
        }

        //设置检索的站点号
        public void SetStationId(List<string> stationList)
        {
            isSetStation = true;
            this.stationList = stationList;
        }

        //获取数据到List中
        public void FecthData()
        {
            /* Hydrologicaldata data = null;
             Hydrologicalstation station = null;*/

            var datas = mySession.Query<Hydrologicaldata>();
            var stations = mySession.Query<Hydrologicalstation>();
            var query = from d in datas
                        join s in stations
                        on d.Stationid equals s.Uid
                        select new
                        {
                            RecordId = d.Recordid,
                            StationId = d.Stationid,
                            StationName = s.Name,
                            StationLocation = s.Address,
                            RiverName = s.River,
                            Level = d.L,
                            Pondage = d.Q,
                            Time = d.Time,
                            WarningLevel = d.Wl1,
                            Longitude = s.Longitude,
                            Latitude = s.Latitude
                        };


            if (isSetDuration == true)
            {
                query = query.Where(d => d.Time < this.to && d.Time > this.from);
            }

            if (isSetStation == true)
            {
                if (stationList.Count > 0)
                {
                    query = query.Where(s => stationList.Contains(s.StationId));
                }
            }

            foreach (var data in query)
            {
                ReservoirHydrologyRecord record = new ReservoirHydrologyRecord();
                record.RecordId = data.RecordId;
                record.StationId = data.StationId;
                record.StationName = data.StationName;
                record.StationLocation = data.StationLocation;
                record.RiverName = data.RiverName;
                try
                {
                    record.Level = double.Parse(data.Level);
                }
                catch (ArgumentNullException)
                {
                    record.Level = 0.0;
                }
                try
                {
                    record.Pondage = double.Parse(data.Pondage);
                }
                catch (ArgumentNullException)
                {
                    record.Pondage = 0.0;
                }
                record.Time = (DateTime)data.Time;
                try
                {
                    record.WarningLevel = double.Parse(data.WarningLevel);
                }
                catch (ArgumentNullException)
                {
                    record.WarningLevel = 0.0;
                }
                try
                {
                    record.Longitude = double.Parse(data.Longitude);
                }
                catch (ArgumentNullException)
                {
                    record.Longitude = 0.0;
                }
                try
                {
                    record.Latitude = double.Parse(data.Latitude);
                }
                catch (ArgumentNullException)
                {
                    record.Latitude = 0.0;
                }
                reservoirList.Add(record);
            }
        }

        public void SaveToTxTFile(string fileName)
        {
            List<string> ls = new List<string>();
            string str = "";
            str += "记录号\t";
            str += "站点号\t";
            str += "站名\t";
            str += "站址\t";
            str += "河名\t";
            str += "库水位\t";
            str += "蓄水量\t";
            str += "时间\t";
            str += "汛限水位\t";
            str += "经度\t";
            str += "纬度\t" + System.Environment.NewLine;
            ls.Add(str);
            foreach (ReservoirHydrologyRecord rhr in reservoirList)
            {
                str = "";
                str += rhr.RecordId.ToString() + "\t";
                str += rhr.StationId.ToString() + "\t";
                str += rhr.StationName + "\t";
                str += rhr.StationLocation + "\t";
                str += rhr.RiverName + "\t";
                str += rhr.Level.ToString("F2") + "\t";
                str += rhr.Pondage.ToString("F2") + "\t";
                str += rhr.Time.ToString() + "\t";
                str += rhr.WarningLevel.ToString("F2") + "\t";
                str += rhr.Longitude.ToString("F2") + "\t";
                str += rhr.Latitude.ToString("F2") + "\t" + System.Environment.NewLine;
                ls.Add(str);
            }
            string[] lines = ls.ToArray();
            System.IO.File.WriteAllLines(fileName, lines);
        }

        public void SaveToExcelFile(string fileName)
        {
            Excel.Application oXL;
            Excel._Workbook oWB;
            Excel._Worksheet oSheet; ;
            Excel.Range oRng;

            try
            {
                //Start Excel and get Application object.
                oXL = new Excel.Application();
                //oXL.Visible = true;

                //Get a new workbook.
                oWB = (Excel._Workbook)(oXL.Workbooks.Add(Missing.Value));
                oSheet = (Excel._Worksheet)oWB.ActiveSheet;

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "记录号";
                oSheet.Cells[1, 2] = "站点号";
                oSheet.Cells[1, 3] = "站名";
                oSheet.Cells[1, 4] = "站址";
                oSheet.Cells[1, 5] = "河名";
                oSheet.Cells[1, 6] = "库水位";
                oSheet.Cells[1, 7] = "蓄水量";
                oSheet.Cells[1, 8] = "时间\t";
                oSheet.Cells[1, 9] = "汛限水位";
                oSheet.Cells[1, 10] = "经度";
                oSheet.Cells[1, 11] = "纬度";

                //Format A1:D1 as bold, vertical alignment = center.
                oSheet.get_Range("A1", "K1").Font.Bold = true;
                oSheet.get_Range("A1", "K1").VerticalAlignment =
                    Excel.XlVAlign.xlVAlignCenter;

                int row = 2;

                foreach (ReservoirHydrologyRecord rhr in reservoirList)
                {
                    oSheet.Cells[row, 1] = rhr.RecordId.ToString();
                    oSheet.Cells[row, 2] = rhr.StationId.ToString();
                    oSheet.Cells[row, 3] = rhr.StationName;
                    oSheet.Cells[row, 4] = rhr.StationLocation;
                    oSheet.Cells[row, 5] = rhr.RiverName;
                    oSheet.Cells[row, 6] = rhr.Level.ToString("F2");
                    oSheet.Cells[row, 7] = rhr.Pondage.ToString("F2");
                    oSheet.Cells[row, 8] = rhr.Time.ToString();
                    oSheet.Cells[row, 9] = rhr.WarningLevel.ToString("F2");
                    oSheet.Cells[row, 10] = rhr.Longitude.ToString("F2");
                    oSheet.Cells[row, 11] = rhr.Latitude.ToString("F2");
                    row++;
                }

                oRng = oSheet.Range[oSheet.Cells[1, 1], oSheet.Cells[row, 11]];
                oRng.EntireColumn.AutoFit();  //列宽自适应

                oWB.SaveAs(fileName, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                oXL.Quit();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
