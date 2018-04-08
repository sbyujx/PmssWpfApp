using Hardcodet.Wpf.TaskbarNotification;
using PMSS.Log;
using PMSS.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace PmssWpfNotificationApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        private DispatcherTimer timer;
        private int interval;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
                notifyIcon = (TaskbarIcon)FindResource("TaskbarIcon");

                interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("NotifyInterval"));

                timer = new DispatcherTimer();
                timer.Tick += ShowNotification;
                timer.Interval = new TimeSpan(interval, 0, 0);
                timer.Start();

                ShowNotification(null, null);
                LogHelper.WriteLog(typeof(App), "OnStartup");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(App), ex.ToString());
                Application.Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            timer.Stop();

            base.OnExit(e);
        }
        private void ShowNotification(object sender, EventArgs e)
        {
            LogHelper.WriteLog(typeof(App), "ShowNotification");
            try
            {
                bool hasWarning = false;

                var from = DateTime.Now.AddHours(-1 * interval);
                var to = DateTime.Now;
                var entity = new HydrologicReader().RetrieveEntity(from, to);

                if (entity != null && entity.Items != null)
                {
                    foreach (var item in entity.Items)
                    {
                        if (item.L > item.Wl1 && item.Wl1 > 0.05)
                        {
                            hasWarning = true;
                            break;
                        }
                    }
                }

                if (hasWarning)
                {
                    var customBalloon = new CustomBalloon();
                    customBalloon.BalloonText = "水情监控警报";
                    customBalloon.BalloonDescription = "发现水情警报，请在 [监控 -> 水情监控 -> 实时数据] 查看详情。";

                    notifyIcon.ShowCustomBalloon(customBalloon, PopupAnimation.Slide, 4000);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(typeof(App), ex.ToString());
            }
        }
    }
}
