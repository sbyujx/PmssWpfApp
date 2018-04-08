using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PMSS.SqlDataAccess;
using System.IO;
using EasyMicaps;

namespace PMSS.DataToMicaps
{
    public class MicapsFile
    {
        public string Type { get; set; }
        public int TimeGap { get; set; }
        public DateTime TimeTo { get; set; }
        public DateTime TimeFrom { get; set; }
        public List<MicapsRecord> ListRecord { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        private string infodir;
        private string v1;
        private int v2;
        private DateTime now;
        private string v3;

        public MicapsFile(string StationType, int TimeGap, DateTime now)
        {
            this.Type = StationType;
            this.TimeGap = TimeGap;
            this.TimeTo = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            this.TimeFrom = this.TimeTo.AddHours(0 - this.TimeGap);
            this.ListRecord = new List<MicapsRecord>();
            this.FileName = now.ToString("yyMMddHH") + ".000";
        }

        public MicapsFile(string v1, int v2, DateTime now, string v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.now = now;
            this.v3 = v3;
        }

        public void GenFile()
        {
            string all_str = "";
            int num = 1;
            string sss = "";
            string tmp = "";
            
            if (this.TimeGap == 24)
            {
                /*最大要素*/
                var entity = new HydrologicReader().RetrieveHighestEntity(TimeFrom, TimeTo, Type);
                foreach (HydrologicEntityItem item in entity.Items)
                {
                    if (!double.IsNaN(item.Latitude) && !double.IsNaN(item.Longitude))
                    {
                        MicapsRecord record = new MicapsRecord();
                        record.Uid = item.Stationid.GetHashCode();
                        record.Latitude = item.Latitude;
                        record.Longitude = item.Longitude;
                        record.ValueL = item.L;
                        record.ValueQ = item.Q;
                        record.ValueWL1 = item.Wl1;
                        record.ValueWL2 = item.Wl2;
                        ListRecord.Add(record);
                        all_str += num + "\t\t";
                        num++;
                        all_str += item.Basin + "\t\t";
                        all_str += item.Administrativeregion + "\t\t";
                        all_str += item.River + "\t\t";
                        all_str += item.Name + "\t\t";
                        all_str += item.Latitude.ToString("F2") + "\t\t";
                        all_str += item.Longitude.ToString("F2") + "\t\t";
                        if (double.IsNaN(item.L))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.L.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Q))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Q.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl1))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Wl1.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl2))
                        {
                            all_str += "9999" + System.Environment.NewLine;
                        }
                        else
                        {
                            all_str += item.Wl2.ToString("F2") + System.Environment.NewLine;
                        }
                    }
                }

                if (Type.Equals("河道站"))
                {
                    sss = "最高水位\t\t最大流量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                if (Type.Equals("水库站"))
                {
                    sss = "最高库水位\t\t最大蓄水量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                all_str = "序号\t\t流域\t\t行政区\t\t河名\t\t站名\t\t纬度\t\t经度\t\t" + sss + all_str;
                /*生成最大全部要素*/
                infodir = this.TimeGap + "小时最大全部要素";
                tmp = TimeGap.ToString("D3");
                this.FilePath = @"D:/products/" + Type + @"/" + tmp + @"/" + this.infodir + @"/" + this.FileName;
                FileInfo fileInfo = new FileInfo(this.FilePath);
                if (!fileInfo.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, false, Encoding.Default))
                {
                    file.Write(all_str);
                }


                /*生成最新要素*/
                all_str = "";
                num = 1;
                entity = new HydrologicReader().RetrieveEntity(TimeFrom, TimeTo, Type);
                foreach (HydrologicEntityItem item in entity.Items)
                {
                    if (!double.IsNaN(item.Latitude) && !double.IsNaN(item.Longitude))
                    {
                        MicapsRecord record = new MicapsRecord();
                        record.Uid = item.Stationid.GetHashCode();
                        record.Latitude = item.Latitude;
                        record.Longitude = item.Longitude;
                        record.ValueL = item.L;
                        record.ValueQ = item.Q;
                        record.ValueWL1 = item.Wl1;
                        record.ValueWL2 = item.Wl2;
                        ListRecord.Add(record);
                        all_str += num + "\t\t";
                        num++;
                        all_str += item.Basin + "\t\t";
                        all_str += item.Administrativeregion + "\t\t";
                        all_str += item.River + "\t\t";
                        all_str += item.Name + "\t\t";
                        all_str += item.Latitude.ToString("F2") + "\t\t";
                        all_str += item.Longitude.ToString("F2") + "\t\t";
                        if (double.IsNaN(item.L))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.L.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Q))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Q.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl1))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Wl1.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl2))
                        {
                            all_str += "9999" + System.Environment.NewLine; ;
                        }
                        else
                        {
                            all_str += item.Wl2.ToString("F2") + System.Environment.NewLine;
                        }
                    }
                }
                sss = "";
                if (Type.Equals("河道站"))
                {
                    sss = "最新水位\t\t最新流量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                if (Type.Equals("水库站"))
                {
                    sss = "最新库水位\t\t最新蓄水量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                all_str = "序号\t\t流域\t\t行政区\t\t河名\t\t站名\t\t纬度\t\t经度\t\t" + sss + all_str;
                /*生成最新全部要素*/
                infodir = this.TimeGap + "小时最新全部要素";
                tmp = TimeGap.ToString("D3");
                this.FilePath = @"D:/products/" + Type + @"/" + tmp + @"/" + this.infodir + @"/" + this.FileName;
                fileInfo = new FileInfo(this.FilePath);
                if (!fileInfo.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, false, Encoding.Default))
                {
                    file.Write(all_str);
                }
            }

            if (this.TimeGap == 1)
            {
                /*生成最新要素*/
                all_str = "";
                num = 1;
                var entity = new HydrologicReader().RetrieveEntity(TimeFrom, TimeTo, Type);
                foreach (HydrologicEntityItem item in entity.Items)
                {
                    if (!double.IsNaN(item.Latitude) && !double.IsNaN(item.Longitude))
                    {
                        MicapsRecord record = new MicapsRecord();
                        record.Uid = item.Stationid.GetHashCode();
                        record.Latitude = item.Latitude;
                        record.Longitude = item.Longitude;
                        record.ValueL = item.L;
                        record.ValueQ = item.Q;
                        record.ValueWL1 = item.Wl1;
                        record.ValueWL2 = item.Wl2;
                        ListRecord.Add(record);
                        all_str += num + "\t\t";
                        num++;
                        all_str += item.Basin + "\t\t";
                        all_str += item.Administrativeregion + "\t\t";
                        all_str += item.River + "\t\t";
                        all_str += item.Name + "\t\t";
                        all_str += item.Latitude.ToString("F2") + "\t\t";
                        all_str += item.Longitude.ToString("F2") + "\t\t";
                        if (double.IsNaN(item.L))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.L.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Q))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Q.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl1))
                        {
                            all_str += "9999\t\t";
                        }
                        else
                        {
                            all_str += item.Wl1.ToString("F2") + "\t\t";
                        }
                        if (double.IsNaN(item.Wl2))
                        {
                            all_str += "9999" + System.Environment.NewLine; ;
                        }
                        else
                        {
                            all_str += item.Wl2.ToString("F2") + System.Environment.NewLine;
                        }
                    }
                }
                sss = "";
                if (Type.Equals("河道站"))
                {
                    sss = "最新水位\t\t最新流量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                if (Type.Equals("水库站"))
                {
                    sss = "最新库水位\t\t最新蓄水量\t\t警戒水位\t\t保证水位" + System.Environment.NewLine;

                }
                all_str = "序号\t\t流域\t\t行政区\t\t河名\t\t站名\t\t纬度\t\t经度\t\t" + sss + all_str;
                /*生成最新全部要素*/
                infodir = this.TimeGap + "小时最新全部要素";
                tmp = TimeGap.ToString("D3");
                this.FilePath = @"D:/products/" + Type + @"/" + tmp + @"/" + this.infodir + @"/" + this.FileName;
                FileInfo fileInfo = new FileInfo(this.FilePath);
                fileInfo = new FileInfo(this.FilePath);
                if (!fileInfo.Exists)
                {
                    Directory.CreateDirectory(fileInfo.Directory.FullName);
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(FilePath, false, Encoding.Default))
                {
                    file.Write(all_str);
                }
            }


        }

        public void Gen1HourFile()
        {

        }

        public void Gen24HourNewFile()
        {

        }

        public void Gen24HourHighFile()
        {

        }


    }
}
