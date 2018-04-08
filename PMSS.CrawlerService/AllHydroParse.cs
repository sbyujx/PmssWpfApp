using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using PMSS.Log;

namespace PMSS.CrawlerService
{
    class AllHydroParse
    {
        private string url;
        private int riverCount;  //河道站数量
        public int RiverCount
        {
            get
            {
                return riverCount;
            }
        }
        private int reservoirCount;  //水库站数量
        public int ReservoirCount
        {
            get
            {
                return reservoirCount;
            }
        }

        private int tideCount;  //潮位站数量
        public int TideCount
        {
            get
            {
                return tideCount;
            }
        }

        private List<HydrologicalRecord> listHydr;
        public List<HydrologicalRecord> ListHydr
        {
            get
            {
                return listHydr;
            }
        }

        public AllHydroParse(string url)
        {
            this.url = url;
            listHydr = new List<HydrologicalRecord>();
            riverCount = 0;
            reservoirCount = 0;
            tideCount = 0;
        }

        public void Parse()
        {
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                string text = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                JArray jsonArr = JArray.Parse(text);

                for (int i = 0; i < jsonArr.Count; i++)
                {
                    HydrologicalRecord data = new HydrologicalRecord();
                    JObject obTemp = JObject.Parse(jsonArr[i].ToString());
                    //解析站点
                    data.Station.Uid = obTemp["POI_OID"].Value<string>();
                    data.Station.Name = obTemp["POI_NAME"].Value<string>();
                    data.Station.Longitude = obTemp["POI_LON"].Value<double>();
                    data.Station.Latitude = obTemp["POI_LAT"].Value<double>();
                    data.Station.River = obTemp["POI_RVNM"].Value<string>();
                    data.Station.HydrographicNet = obTemp["POI_HNNM"].Value<string>();
                    data.Station.Basin = obTemp["POI_BSNM"].Value<string>();
                    data.Station.AdministrativeRegion = obTemp["POI_ADDV"].Value<string>();
                    data.Station.Address = obTemp["POI_ADDRESS"].Value<string>();
                    data.Station.Type = obTemp["POI_TYPE"].Value<string>();
                    if (data.Station.Type.Equals("河道站"))
                    {
                        riverCount++;
                    }
                    else if (data.Station.Type.Equals("水库站"))
                    {
                        reservoirCount++;
                    }
                    else if (data.Station.Type.Equals("潮位站"))
                    {
                        tideCount++;
                    }

                    //解析数据
                    try
                    {
                        data.L = obTemp["L"].Value<double>();
                    }
                    catch (InvalidCastException)
                    {
                        data.L = Double.NaN;
                    }

                    try
                    {
                        data.Q = obTemp["Q"].Value<double>();
                    }
                    catch (InvalidCastException)
                    {
                        data.Q = Double.NaN;
                    }

                    try
                    {
                        data.Wl1 = obTemp["WL1"].Value<double>();
                    }
                    catch (InvalidCastException)
                    {
                        data.Wl1 = Double.NaN;
                    }

                    try
                    {
                        data.Wl2 = obTemp["WL2"].Value<double>();
                    }
                    catch (InvalidCastException)
                    {
                        data.Wl2 = Double.NaN;
                    }

                    try
                    {
                        data.Time = obTemp["TM"].Value<DateTime>();
                    }
                    catch (InvalidCastException)
                    {
                        //Ticks 为0
                    }

                    listHydr.Add(data);
                }

                LogHelper.WriteLog(typeof(AllHydroParse), "地图水情数据爬取解析成功,河道站：" + riverCount + " ，水库站：" + reservoirCount + " ，潮位站：" + tideCount);
            }
            catch (WebException ex)
            {
                LogHelper.WriteLog(typeof(AllHydroParse), ex);
            }
        }
    }
}
