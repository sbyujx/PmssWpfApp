using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;
using System.IO;
using PMSS.WordProduct;
using PMSS.Configure;
using PMSS.Log;
using PMSS.DataToMicaps;

namespace PMSS.CrawlerService
{
    public partial class ServiceCrawler : ServiceBase
    {
        private static System.Timers.Timer timerDownload;
        private string allHydrUrl;
        private List<OldData> listOldData;
        private DateTime last1 = new DateTime(2000, 1, 1, 1, 0, 0);
        private DateTime last6 = new DateTime(2000, 1, 1, 1, 0, 0);
        private DateTime last24 = new DateTime(2000, 1, 1, 1, 0, 0);

        public ServiceCrawler()
        {
            InitializeComponent();
            timerDownload = new System.Timers.Timer();
            allHydrUrl = CrawlerConfig.AllHydrUrl;
            listOldData = new List<OldData>();
        }

        protected override void OnStart(string[] args)
        {
            timerDownload.Interval = 20000;
            timerDownload.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timerDownload.Enabled = true;
        }

        protected override void OnStop()
        {
            timerDownload.Enabled = false;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            timerDownload.Interval = CrawlerConfig.CrawlIntervalMS;
            LogHelper.WriteLog(typeof(ServiceCrawler), "爬虫服务启动！");
            List <OldData> listNewData = new List<OldData>();
            List<WarningData> listKeyResWarningData = new List<WarningData>();
            List<WarningData> listRiverWarningData = new List<WarningData>();
            List<WarningData> listResWarningData = new List<WarningData>();
            int maxIdStart = 1;
            //LogHelper.WriteLog(typeof(ServiceCrawler), "服务开始时数据库最大ID为" + maxIdStart + "！");           

            HtmlParse hp = new HtmlParse();
            try
            {
                /*获取重点站水情信息*/
                hp.LoadHTML_River();
                hp.ParseRiverHydrology();
                hp.LoadHTML_Reservoir();
                hp.ParseReservoirHydrology();

                /*更新重点河流站点*/
                List<OldData> listTempData = DBHydro.UpdateKeyRiver(hp.ListRiver, listOldData);
                listNewData.AddRange(listTempData);
                listOldData.AddRange(listTempData);
                /*更新重点水库站点*/
                listTempData = DBHydro.UpdateKeyReservoir(hp.ListReservoir, listOldData, ref listKeyResWarningData);
                listNewData.AddRange(listTempData);
                listOldData.AddRange(listTempData);
                LogHelper.WriteLog(typeof(ServiceCrawler), "发现超警重点水库信息" + listKeyResWarningData.Count + "条！");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(ServiceCrawler), ex);
            }

            //重点水库超警信息去重
            Duplication(ref listKeyResWarningData);
            LogHelper.WriteLog(typeof(ServiceCrawler), "存储过程获得重点关注水库站实时超警水情" + listKeyResWarningData.Count() + "条！");
            List<ReservoirWarningRecord> keyReservoir = HydroMonitor.ResGetByWarningList(listKeyResWarningData);
            LogHelper.WriteLog(typeof(ServiceCrawler), "生成全国重点关注水库站实时水情" + keyReservoir.Count() + "条！");

            //更新逐小时水情
            try
            {
                hp.ParseTodayDetail(DBHydro.GetAllNumsStation());
                LogHelper.WriteLog(typeof(ServiceCrawler), "共解析成功逐小时水情数据" + hp.ListDetailRecord.Count + "条！");
                DBHydro.AddAllRecord(hp.ListDetailRecord, listOldData, ref listRiverWarningData, ref listResWarningData);
                foreach (var item in hp.ListDetailRecord)
                {
                    OldData od = new OldData(item.Station.Uid, item.Time);
                    listNewData.Add(od);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(ServiceCrawler), ex);
            }

            try
            {
                //获取大江大河实时水情
                hp.LoadBigRiver();
                hp.ParseBigRiver();
                //更新大江大河实时水情
                List<OldData> listTempData = DBHydro.UpdateBigRiver(hp.ListBigRiver, listOldData, ref listRiverWarningData);
                listNewData.AddRange(listTempData);
                listOldData.AddRange(listTempData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(ServiceCrawler), ex);
            }

            try
            {
                //获取大型水库水情
                hp.LoadBigReservoir();
                hp.ParseBigReservoir();

                //更新大型水库水情
                List<OldData> listTempData = DBHydro.UpdateBigReservoir(hp.ListBigReservoir, listOldData, ref listResWarningData);
                listNewData.AddRange(listTempData);
                listOldData.AddRange(listTempData);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(ServiceCrawler), ex);
            }

            /*获取地图水情信息*/
            try
            {
                AllHydroParse allHydroParse = new AllHydroParse(allHydrUrl);
                allHydroParse.Parse();


                /*地图水情入数据库*/
                //DBHydro.AddAllStation(allHydroParse.ListHydr);
                DBHydro.AddAllRecord(allHydroParse.ListHydr, listOldData, ref listRiverWarningData, ref listResWarningData);

                foreach (var item in allHydroParse.ListHydr)
                {
                    OldData od = new OldData(item.Station.Uid, item.Time);
                    listNewData.Add(od);
                    listOldData.Add(od);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(ServiceCrawler), ex);
            }



            int maxIdEnd = 2;
            //LogHelper.WriteLog(typeof(ServiceCrawler), "爬取入库完成后数据库最大ID为" + maxIdEnd + "！");

            if (maxIdEnd > maxIdStart)
            {
                //启动存储过程
                try
                {
                    //水库超警信息去重
                    Duplication(ref listResWarningData);
                    LogHelper.WriteLog(typeof(ServiceCrawler), "存储过程获得主要水库实时超汛限水情" + listResWarningData.Count() + "条！");
                    //foreach(WarningData data in listResWarningData)
                    //{
                    //    LogHelper.WriteLog(typeof(ServiceCrawler), data.Uid+"L"+data.L+"WL"+ data.WL1+"T"+data.Time);
                    //}
                    //河道超警信息去重
                    Duplication(ref listRiverWarningData);
                    LogHelper.WriteLog(typeof(ServiceCrawler), "存储过程获得主要水文站实时超警水情" + listRiverWarningData.Count() + "条！");


                    List<RiverWarningRecord> groupsRiver = HydroMonitor.RiverGetByWarningList(listRiverWarningData);
                    LogHelper.WriteLog(typeof(ServiceCrawler), "生成主要水文站实时超警水情" + groupsRiver.Count() + "条！");
                    List<ReservoirWarningRecord> groupsReservoir = HydroMonitor.ResGetByWarningList(listResWarningData);
                    LogHelper.WriteLog(typeof(ServiceCrawler), "生成主要水库实时超汛限水情" + groupsReservoir.Count() + "条！");
                    List<RiverWarningRecord> lakes = HydroMonitor.LakeGetByWarningList();


                    MonitorDataSave dataSave = new MonitorDataSave();
                    //LogHelper.WriteLog(typeof(ServiceCrawler), "存储位置为" + dataSave.fullPathRi);
                    dataSave.groupsRiver = groupsRiver;
                    dataSave.groupsReservoir = groupsReservoir;
                    dataSave.keyReservoir = keyReservoir;
                    dataSave.lakes = lakes;
                    dataSave.WriteDataToFile();
                    LogHelper.WriteLog(typeof(ServiceCrawler), "存储过程结束");
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(typeof(ServiceCrawler), ex);
                }
            }

            listOldData = listNewData;
            LogHelper.WriteLog(typeof(ServiceCrawler), "爬虫线程结束！");
            StartMicaps();
            LogHelper.WriteLog(typeof(ServiceCrawler), "写入Micaps结束！");
        }

        public void StartMicaps()
        {
            DateTime now = DateTime.Now;
            if(now.Hour != last1.Hour)
            {
                last1 = now;

                MicapsFile fileShuiku1 = new MicapsFile("水库站", 1, now);
                fileShuiku1.GenFile();
                MicapsFile fileHedao1 = new MicapsFile("河道站", 1, now);
                fileHedao1.GenFile();

                MicapsFile fileShuiku24 = new MicapsFile("水库站", 24, now);
                fileShuiku24.GenFile();
                MicapsFile fileHedao24 = new MicapsFile("河道站", 24, now);
                fileHedao24.GenFile();
            }
        }

        public void Duplication(ref List<WarningData> list)  //去重
        {
            list = list.Distinct(new WarningDataNoComparer()).ToList();
        }
    }

    class WarningDataNoComparer : IEqualityComparer<WarningData>
    {
        public bool Equals(WarningData d1, WarningData d2)
        {
            if (d1 == null)
                return d2 == null;
            return d1.Uid.Equals(d2.Uid);
        }

        public int GetHashCode(WarningData d)
        {
            if (d == null)
                return 0;
            return d.Uid.GetHashCode();
        }
    }
}
