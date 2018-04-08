using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Xml;
using PMSS.Log;

namespace PMSS.CrawlerService
{

    public class HtmlParse
    {
        private HtmlDocument riverDoc;
        private HtmlDocument reservoirDoc;
        private HtmlDocument bigRiverDoc;
        private HtmlDocument bigReservoirDoc;

        private List<RiverHydrology> listRiver;
        public List<RiverHydrology> ListRiver
        {
            get
            {
                return listRiver;
            }
        }

        private List<RiverHydrology> listBigRiver;
        public List<RiverHydrology> ListBigRiver
        {
            get
            {
                return listBigRiver;
            }
        }

        private List<ReservoirHydrology> listReservoir;
        public List<ReservoirHydrology> ListReservoir
        {
            get
            {
                return listReservoir;
            }
        }

        private List<ReservoirHydrology> listBigReservoir;
        public List<ReservoirHydrology> ListBigReservoir
        {
            get
            {
                return listBigReservoir;
            }
        }

        private List<HydrologicalRecord> listDetailRecord;
        public List<HydrologicalRecord> ListDetailRecord
        {
            get
            {
                return listDetailRecord;
            }
        }

        public HtmlParse()
        {
            listRiver = new List<RiverHydrology>();
            listReservoir = new List<ReservoirHydrology>();
            riverDoc = new HtmlDocument();
            reservoirDoc = new HtmlDocument();
            bigRiverDoc = new HtmlDocument();
            listBigRiver = new List<RiverHydrology>();
            bigReservoirDoc = new HtmlDocument();
            listBigReservoir = new List<ReservoirHydrology>();
            listDetailRecord = new List<HydrologicalRecord>();
        }

        public void LoadHTML_River()
        {
            string url = @"http://xxfb.hydroinfo.gov.cn/dwr/call/plaincall/IndexDwr.getZDHD_SSSQ.dwr";
            string body = @"callCount=1
page=/index.html
httpSessionId=323C661E34A668B6BBDD9C0E057F33D6.tomcat1
scriptSessionId=2577EEB39202697EF418AB7664783C20252
c0-scriptName=IndexDwr
c0-methodName=getZDHD_SSSQ
c0-id=0
batchId=2";
            string ret = OpenReadWithHttps(url, body);
            string removeStr = @"throw 'allowScriptTagRemoting is false.';
//#DWR-INSERT
//#DWR-REPLY
dwr.engine._remoteHandleCallback('2','0'," + "\"";
            ret = ret.Remove(0, removeStr.Length);
            ret = ret.Replace("\");", "");
            string code = HttpUtility.HtmlDecode(ret);
            this.riverDoc.LoadHtml(code);
        }

        public void LoadHTML_Reservoir()
        {
            string url = @"http://xxfb.hydroinfo.gov.cn/dwr/call/plaincall/IndexDwr.getZDSK_SSSQ.dwr";
            string body = @"callCount=1
page=/
httpSessionId=9E1D938B7DEC323BE8849ABBB9A08A65.tomcat1
scriptSessionId=74FE4ACB8663D58D52D3828ED2226EB093
c0-scriptName=IndexDwr
c0-methodName=getZDSK_SSSQ
c0-id=0
batchId=3";
            string ret = OpenReadWithHttps(url, body);
            string removeStr = @"throw 'allowScriptTagRemoting is false.';
//#DWR-INSERT
//#DWR-REPLY
dwr.engine._remoteHandleCallback('3','0'," + "\"";
            ret = ret.Remove(0, removeStr.Length);
            ret = ret.Replace("\");", "");
            string code = HttpUtility.HtmlDecode(ret);
            this.reservoirDoc.LoadHtml(code);
        }

        //解析全国重点河道站实时水情
        public void ParseRiverHydrology()
        {
            int count = 0;
            HtmlNodeCollection anchors = this.riverDoc.DocumentNode.SelectNodes(@"/table/tr");
            foreach (HtmlNode hn in anchors)
            {
                RiverHydrology rh = new RiverHydrology();

                //解析站名
                HtmlNode nodeStationName = hn.SelectNodes("td/a/font").FindFirst("font");
                rh.StationName = UnicodeToString(nodeStationName.InnerText.Trim());

                //解析站址
                HtmlNode nodeAddress = hn.SelectNodes("td[2]").FindFirst("td");
                rh.StationAddress = UnicodeToString(nodeAddress.InnerText.Trim());

                //解析河名
                HtmlNode nodeRiverName = hn.SelectNodes("td[3]").FindFirst("td");
                rh.RiverName = UnicodeToString(nodeRiverName.InnerText.Trim());

                //解析水位
                HtmlNode nodeLevel = hn.SelectNodes("td[4]/font").FindFirst("font");
                try
                {
                    rh.WaterLevel = Convert.ToDouble(nodeLevel.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.WaterLevel = -1.0;
                }

                //解析流量
                HtmlNode nodeFlow = hn.SelectNodes("td[5]").FindFirst("td");
                try
                {
                    rh.Flow = Convert.ToDouble(nodeFlow.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.Flow = -1.0;
                }

                //解析时间
                HtmlNode nodeTime = hn.SelectNodes("td[6]").FindFirst("td");
                string timeStr = nodeTime.InnerText.Trim();
                rh.Time = StrToTime(timeStr);

                //解析警戒水位
                HtmlNode nodeWarning = hn.SelectNodes("td[7]").FindFirst("td");
                try
                {
                    rh.WarningWaterLevel = Convert.ToDouble(nodeWarning.InnerText.Replace("&nbsp", "").Trim());
                }
                catch (Exception)
                {
                    rh.WarningWaterLevel = -1.0;
                }

                listRiver.Add(rh);
                count++;
            }
            LogHelper.WriteLog(typeof(HtmlParse), "重点水情数据爬取解析成功,河道站：" + count + "条");
        }

        //解析全国重点水库站实时水情
        public void ParseReservoirHydrology()
        {
            int count = 0;
            HtmlNodeCollection anchors = this.reservoirDoc.DocumentNode.SelectNodes(@"/table/tr");
            foreach (HtmlNode hn in anchors)
            {
                ReservoirHydrology rh = new ReservoirHydrology();

                //解析站名
                HtmlNode nodeStationName = hn.SelectNodes("td/a/font").FindFirst("font");
                rh.StationName = UnicodeToString(nodeStationName.InnerText.Trim());

                //解析站址
                HtmlNode nodeAddress = hn.SelectNodes("td[2]").FindFirst("td");
                rh.StationAddress = UnicodeToString(nodeAddress.InnerText.Trim());

                //解析河名
                HtmlNode nodeRiverName = hn.SelectNodes("td[3]").FindFirst("td");
                rh.RiverName = UnicodeToString(nodeRiverName.InnerText.Trim());

                //解析库水位
                HtmlNode nodeLevel = hn.SelectNodes("td[4]/font").FindFirst("font");
                try
                {
                    rh.WaterLevel = Convert.ToDouble(nodeLevel.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.WaterLevel = -1.0;
                }

                //解析蓄水量
                HtmlNode nodePondage = hn.SelectNodes("td[5]").FindFirst("td");
                try
                {
                    rh.Pondage = Convert.ToDouble(nodePondage.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.Pondage = -1.0;
                }

                //解析时间
                HtmlNode nodeTime = hn.SelectNodes("td[6]").FindFirst("td");
                string timeStr = nodeTime.InnerText.Trim();
                rh.Time = StrToTime(timeStr);

                //解析汛限水位
                HtmlNode nodeWarning = hn.SelectNodes("td[7]").FindFirst("td");
                try
                {
                    rh.WarningWaterLevel = Convert.ToDouble(nodeWarning.InnerText.Replace("&nbsp", "").Trim());
                }
                catch (Exception)
                {
                    rh.WarningWaterLevel = -1.0;
                }

                listReservoir.Add(rh);
                count++;
            }

            LogHelper.WriteLog(typeof(HtmlParse), "重点水情数据爬取解析成功,水库站：" + count + "条");
        }

        public DateTime StrToTime(string str)  //格式为 "mm-dd hh:mm"
        {
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '-' };
            string[] words = str.Split(delimiterChars);
            int year = DateTime.Now.Year;
            int month = Convert.ToInt32(words[0]);
            int day = Convert.ToInt32(words[1]);
            int hour = Convert.ToInt32(words[2]);
            int minute = Convert.ToInt32(words[3]);
            int second = 0;
            DateTime time = new DateTime(year, month, day, hour, minute, second);
            return time;
        }

        public string UnicodeToString(string text)
        {
            MatchCollection mc = Regex.Matches(text, "\\\\u([\\w]{4})");
            if (mc != null)
            {
                foreach (Match m in mc)
                {
                    char toChar = (char)Convert.ToInt32(m.Value.Replace("\\u", ""), 16);
                    string toStr = toChar.ToString();
                    text = text.Replace(m.Value, toStr);
                }
            }

            return text;
        }

        public string OpenReadWithHttps(string URL, string strPostdata)
        {
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(URL);
            request.Method = "post";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] buffer = encoding.GetBytes(strPostdata);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        //加载大江大河
        public void LoadBigRiver()
        {
            string url = @"http://xxfb.hydroinfo.gov.cn/dwr/call/plaincall/IndexDwr.getSreachData.dwr";
            string body = @"callCount=1
page=/ssIndex.html
httpSessionId=375A34433A298202F0C4ED257F0F4AC6.tomcat1
scriptSessionId=AB41126DB294D9C57FBEA4A255AE9423193
c0-scriptName=IndexDwr
c0-methodName=getSreachData
c0-id=0
c0-param0=string:hd
c0-param1=string:
c0-param2=string:
batchId=0";
            string ret = OpenReadWithHttps(url, body);
            string removeStr = @"throw 'allowScriptTagRemoting is false.';
//#DWR-INSERT
//#DWR-REPLY
dwr.engine._remoteHandleCallback('0','0'," + "\"";
            ret = ret.Remove(0, removeStr.Length);
            ret = ret.Replace("\");", "");
            string code = HttpUtility.HtmlDecode(ret);
            this.bigRiverDoc.LoadHtml(code);
        }

        //解析大江大河实时水情
        public void ParseBigRiver()
        {
            int count = 0;
            HtmlNodeCollection anchors = this.bigRiverDoc.DocumentNode.SelectNodes(@"/table/tr");

            foreach (HtmlNode hn in anchors)
            {
                RiverHydrology rh = new RiverHydrology();
                //解析流域
                HtmlNode nodeBasin = hn.SelectNodes("td").FindFirst("td");
                string basin = UnicodeToString(nodeBasin.InnerText.Trim());
                rh.Basin = basin;

                //解析行政区
                HtmlNode nodeDistruct = hn.SelectNodes("td[2]").FindFirst("td");
                string distruct = UnicodeToString(nodeDistruct.InnerText.Trim());
                rh.AdministrativeRegion = distruct;

                //解析河名
                HtmlNode nodeRiver = hn.SelectNodes("td[3]").FindFirst("td");
                rh.RiverName = UnicodeToString(nodeRiver.InnerText.Trim());

                //解析站名
                HtmlNode nodeStation = hn.SelectNodes("td[4]/font").FindFirst("font");
                rh.StationName = UnicodeToString(nodeStation.InnerText.Trim());

                //解析时间
                HtmlNode nodeTime = hn.SelectNodes("td[5]").FindFirst("td");
                string timeStr = nodeTime.InnerText.Trim();
                rh.Time = StrToTime(timeStr);

                //解析水位
                HtmlNode nodeLevel = hn.SelectNodes("td[6]/font").FindFirst("font");
                try
                {
                    rh.WaterLevel = Convert.ToDouble(nodeLevel.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.WaterLevel = -1.0;
                }

                //解析流量
                HtmlNode nodeFlow = hn.SelectNodes("td[7]").FindFirst("td");
                try
                {
                    rh.Flow = Convert.ToDouble(nodeFlow.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.Flow = -1.0;
                }

                //解析警戒水位
                HtmlNode nodeWarning = hn.SelectNodes("td[8]").FindFirst("td");
                try
                {
                    rh.WarningWaterLevel = Convert.ToDouble(nodeWarning.InnerText.Replace("&nbsp", "").Trim());
                }
                catch (Exception)
                {
                    rh.WarningWaterLevel = -1.0;
                }

                listBigRiver.Add(rh);
                count++;
            }

            LogHelper.WriteLog(typeof(HtmlParse), "大江大河水情数据爬取解析成功,共：" + count + "条");
        }

        //加载大型水库
        public void LoadBigReservoir()
        {
            string url = @"http://xxfb.hydroinfo.gov.cn/dwr/call/plaincall/IndexDwr.getSreachData.dwr";
            string body = @"callCount=1
page=/ssIndex.html
httpSessionId=99D71ADAF0CAC193368334E8DE48C564.tomcat1
scriptSessionId=9C90127356DB870126B910A2F78089AE657
c0-scriptName=IndexDwr
c0-methodName=getSreachData
c0-id=0
c0-param0=string:sk
c0-param1=string:
c0-param2=string:
batchId=1";
            string ret = OpenReadWithHttps(url, body);
            string removeStr = @"throw 'allowScriptTagRemoting is false.';
//#DWR-INSERT
//#DWR-REPLY
dwr.engine._remoteHandleCallback('1','0'," + "\"";
            ret = ret.Remove(0, removeStr.Length);
            ret = ret.Replace("\");", "");
            string code = HttpUtility.HtmlDecode(ret);
            this.bigReservoirDoc.LoadHtml(code);
        }

        //解析大型水库
        public void ParseBigReservoir()
        {
            int count = 0;
            HtmlNodeCollection anchors = this.bigReservoirDoc.DocumentNode.SelectNodes(@"/table/tr");

            foreach (HtmlNode hn in anchors)
            {
                ReservoirHydrology rh = new ReservoirHydrology();
                //解析流域
                HtmlNode nodeBasin = hn.SelectNodes("td").FindFirst("td");
                string basin = UnicodeToString(nodeBasin.InnerText.Trim());
                rh.Basin = basin;

                //解析行政区
                HtmlNode nodeDistruct = hn.SelectNodes("td[2]").FindFirst("td");
                string distruct = UnicodeToString(nodeDistruct.InnerText.Trim());
                rh.AdministrativeRegion = distruct;

                //解析河名
                HtmlNode nodeRiver = hn.SelectNodes("td[3]").FindFirst("td");
                rh.RiverName = UnicodeToString(nodeRiver.InnerText.Trim());

                //解析站名
                HtmlNode nodeStation = hn.SelectNodes("td[4]/font").FindFirst("font");
                rh.StationName = UnicodeToString(nodeStation.InnerText.Trim());

                //解析时间
                rh.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                //解析库水位
                HtmlNode nodeLevel = hn.SelectNodes("td[5]/font").FindFirst("font");
                char[] delimiterChars = { '\\' };
                string[] words = nodeLevel.InnerText.Trim().Split(delimiterChars);
                try
                {
                    rh.WaterLevel = Convert.ToDouble(words[0].Trim());
                }
                catch (Exception)
                {
                    rh.WaterLevel = -1.0;
                }

                //解析蓄水量
                HtmlNode nodeFlow = hn.SelectNodes("td[6]").FindFirst("td");
                try
                {
                    rh.Pondage = Convert.ToDouble(nodeFlow.InnerText.Trim());
                }
                catch (Exception)
                {
                    rh.Pondage = -1.0;
                }

                //解析警戒水位
                /*HtmlNode nodeWarning = hn.SelectNodes("td[8]").FindFirst("td");
                try
                {
                    rh.WarningWaterLevel = Convert.ToDouble(nodeWarning.InnerText.Replace("&nbsp", "").Trim());
                }
                catch (Exception)
                {
                    rh.WarningWaterLevel = -1.0;
                }*/
                rh.WarningWaterLevel = -1.0;
                listBigReservoir.Add(rh);
                count++;
            }
            LogHelper.WriteLog(typeof(HtmlParse), "大型水库水情数据爬取解析成功,共：" + count + "条");
        }

        //解析今天所有详细水情
        public void ParseTodayDetail(List<PMSS.SqlDataAccess.Models.hydrologicalstation> listStation)
        {
            string yesterday = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd");
            foreach (PMSS.SqlDataAccess.Models.hydrologicalstation s in listStation)
            {

                if (s.Type.Equals("河道站"))
                {
                    string retXml = GetDetailHydro(s.UID, yesterday, 1);
                    ParseDetailRiver(retXml, s.UID, DateTime.Parse(yesterday));
                }

                if (s.Type.Equals("水库站"))
                {
                    string retXml = GetDetailHydro(s.UID, yesterday, 2);
                    ParseDetailReservoir(retXml, s.UID, DateTime.Parse(yesterday));
                }
            }
        }

        public void ParseDetailRiver(string web, string uid, DateTime baseTime)
        {
            if(web.Length < 100)
            {
                return;
            }
            string removeStr = @"


<svg>
	";
            web = web.Remove(0, removeStr.Length);
            web = web.Replace(@"</svg>", "");
            web = web.Replace(@"?>", @"?><root>");
            web = web + @"</root>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(web);
            XmlNodeList listNode = doc.SelectNodes("/root/g/polyline/@points");
            List<HydrologicalRecord> tempList = new List<HydrologicalRecord>();
            if (listNode[0] != null)
            {
                XmlNodeList listNodeWl = doc.SelectNodes("/root/g/text");
                double wl = -1;
                foreach(XmlNode n in listNodeWl)
                {
                    string str = n.InnerText;
                    if(str.Contains("����ˮλ"))
                    {
                        str = str.Remove(0, 6);
                        try
                        {
                            wl = double.Parse(str);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }

                string levelData = listNode[0].Value;
                string[] levelArray = levelData.Split(' ');

                foreach (string l in levelArray)
                {
                    string[] pair = l.Split(',');
                    try
                    {
                        double addHour = double.Parse(pair[0]);
                        double level = ((int)(double.Parse(pair[1]) * 100 + 0.5)) / 100.0;
                        HydrologicalRecord r = new HydrologicalRecord();
                        r.isKey = false;
                        r.Time = baseTime.AddHours(addHour);
                        r.L = level;
                        r.Station = new HydrologyStation();
                        r.Station.Uid = uid;
                        r.Station.Type = "河道站";
                        if(wl > 0)
                        {
                            r.Wl1 = wl;
                        }
                        tempList.Add(r);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            if (listNode[1] != null)
            {
                string levelData = listNode[1].Value;
                string[] levelArray = levelData.Split(' ');
                foreach (string l in levelArray)
                {
                    string[] pair = l.Split(',');
                    try
                    {
                        double addHour = double.Parse(pair[0]);
                        double flow = ((int)(double.Parse(pair[1]) * 100 + 0.5)) / 100.0;
                        HydrologicalRecord r = tempList.Find(item => item.Time.Equals(baseTime.AddHours(addHour)));
                        if (r != null)
                        {
                            r.Q = flow;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            listNode = doc.SelectNodes("/root/g/text");
            if (listNode[1] != null)
            {
                string wl = listNode[1].InnerText;
                wl = wl.Replace(@"����ˮλ", "");
                try
                {
                    double warningLevel = ((int)(double.Parse(wl) * 100 + 0.5)) / 100.0;
                    foreach (HydrologicalRecord r in tempList)
                    {
                        r.Wl1 = warningLevel;
                    }
                }
                catch (Exception)
                {

                }
            }

            this.listDetailRecord.AddRange(tempList);

        }

        public void ParseDetailReservoir(string web, string uid, DateTime baseTime)
        {
            if (web.Length < 100)
            {
                return;
            }
            string removeStr = @"


<svg>
	";
            web = web.Remove(0, removeStr.Length);
            web = web.Replace(@"</svg>", "");
            web = web.Replace(@"?>", @"?><root>");
            web = web + @"</root>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(web);
            XmlNodeList listNode = doc.SelectNodes("/root/g/polyline/@points");
            List<HydrologicalRecord> tempList = new List<HydrologicalRecord>();
            if (listNode[0] != null)
            {
                string levelData = listNode[0].Value;
                string[] levelArray = levelData.Split(' ');

                foreach (string l in levelArray)
                {
                    string[] pair = l.Split(',');
                    try
                    {
                        double addHour = double.Parse(pair[0]);
                        double level = ((int)(double.Parse(pair[1]) * 100 + 0.5)) / 100.0;
                        HydrologicalRecord r = new HydrologicalRecord();
                        r.isKey = false;
                        r.Time = baseTime.AddHours(addHour);
                        r.L = level;
                        r.Station = new HydrologyStation();
                        r.Station.Uid = uid;
                        r.Station.Type = "水库站";
                        tempList.Add(r);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            if (listNode[1] != null)
            {
                string levelData = listNode[1].Value;
                string[] levelArray = levelData.Split(' ');
                foreach (string l in levelArray)
                {
                    string[] pair = l.Split(',');
                    try
                    {
                        double addHour = double.Parse(pair[0]);
                        double flow = ((int)(double.Parse(pair[1]) * 100 + 0.5)) / 100.0;
                        HydrologicalRecord r = tempList.Find(item => item.Time.Equals(baseTime.AddHours(addHour)));
                        if (r != null)
                        {
                            r.Q = flow;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            this.listDetailRecord.AddRange(tempList);
        }

        public string GetDetailHydro(string site, string date, int type)
        {
            string url = null;
            if (type == 1)
            {
                url = @"http://xxfb.hydroinfo.gov.cn/svg/svgwait.jsp?gcxClass=" + type + "&gcxKind=1&DateL=" + date + "&DateM=" + date + "&gcxData=7&site=" + site;
            }

            if (type == 2)
            {
                url = @"http://xxfb.hydroinfo.gov.cn/svg/svgwait.jsp?gcxClass=" + type + "&gcxKind=1&DateL=" + date + "&DateM=" + date + "&gcxData=8&site=" + site;
            }

            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(url);
            request.Method = "get";
            request.Host = "xxfb.hydroinfo.gov.cn";
            request.Referer = @"http://xxfb.hydroinfo.gov.cn/svg/svghtml.html";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
