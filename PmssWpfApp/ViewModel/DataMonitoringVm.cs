using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using PMSS.Render.DbSource;
using System.Windows;
using PmssWpfApp.Dialogs;
using System.IO;
using Pmss.Micaps.Render.FileSource;
using Pmss.Micaps.Render.Config;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using PMSS.DatabaseUI;

namespace PmssWpfApp.ViewModel
{
    public class DataMonitoringVm
    {
        public DataMonitoringVm()
        {
            ShowRealtimeHydrologicCommand = new DelegateCommand(ShowRealtimeHydrologic);
            ShowHistoryHydrologicCommand = new DelegateCommand(ShowHistoryHydrologic);
            ShowRain01Command = new DelegateCommand(ShowRain01);
            ShowRain01JmCommand = new DelegateCommand(ShowRain01Jm);
            ShowRain06Command = new DelegateCommand(ShowRain06);
            ShowRain06JmCommand = new DelegateCommand(ShowRain06Jm);
            ShowRain2405Command = new DelegateCommand(ShowRain2405);
            ShowRain2408Command = new DelegateCommand(ShowRain2408);
            ShowRain2408JmCommand = new DelegateCommand(ShowRain2408Jm);
            ShowRain2408AllCommand = new DelegateCommand(ShowRain2408All);
            ShowRain2414Command = new DelegateCommand(ShowRain2414);
            ShowRain2414JmCommand = new DelegateCommand(ShowRain2414Jm);
            ShowRain2414AllCommand = new DelegateCommand(ShowRain2414All);
            ShowRain2420Command = new DelegateCommand(ShowRain2420);
            ShowRain2420JmCommand = new DelegateCommand(ShowRain2420Jm);
            ShowRain2420AllCommand = new DelegateCommand(ShowRain2420All);
            ShowSettingDialogCommand = new DelegateCommand(ShowSettingDialog);
        }
        public void SetMapVm(MapVm vm)
        {
            mapVm = vm;
        }
        public void SetMapView(MapView view)
        {
            this.mapView = view;
            mapView.MapViewTapped += HandleMapViewTapped;
        }
        public void SetCurrentLayer(GraphicsLayer layer)
        {
            this.layer = layer;
        }

        public DelegateCommand ShowRealtimeHydrologicCommand { get; set; }
        public DelegateCommand ShowHistoryHydrologicCommand { get; set; }
        public DelegateCommand ShowRain01Command { get; set; }
        public DelegateCommand ShowRain01JmCommand { get; set; }
        public DelegateCommand ShowRain06Command { get; set; }
        public DelegateCommand ShowRain06JmCommand { get; set; }
        public DelegateCommand ShowRain2405Command { get; set; }
        public DelegateCommand ShowRain2408Command { get; set; }
        public DelegateCommand ShowRain2408JmCommand { get; set; }
        public DelegateCommand ShowRain2408AllCommand { get; set; }
        public DelegateCommand ShowRain2414Command { get; set; }
        public DelegateCommand ShowRain2414JmCommand { get; set; }
        public DelegateCommand ShowRain2414AllCommand { get; set; }
        public DelegateCommand ShowRain2420Command { get; set; }
        public DelegateCommand ShowRain2420JmCommand { get; set; }
        public DelegateCommand ShowRain2420AllCommand { get; set; }
        public DelegateCommand ShowSettingDialogCommand { get; set; }

        private void ShowRealtimeHydrologic()
        {
            this.ShowHydrologic(DateTime.Now.AddDays(-1), DateTime.Now, true);
        }
        private void ShowHistoryHydrologic()
        {
            var dialog = new MonitorHydrologicHistory();
            if (dialog.ShowDialog() == true)
            {
                this.ShowHydrologic(dialog.StartTime, dialog.EndTime, false);
            }
        }
        private void ShowHydrologic(DateTime from, DateTime to, bool isRealtime)
        {
            try
            {
                var render = new HydrologicRender();
                var renderResult = render.GenerateRenderResult(from, to, isRealtime);
                this.mapVm.AddReadonlyLayer(renderResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain(string path, string settingKey, int monitorValue)
        {
            try
            {
                string folderPath = path;
                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show($"找不到路径{folderPath}");
                    return;
                }

                var file = Directory.GetFiles(folderPath).OrderBy(f => f).Last();
                if (!File.Exists(file))
                {
                    MessageBox.Show($"找不到路径{file}");
                    return;
                }

                var result = GraphicsLayerFactory.GenerateRenderResultFromFile(file, settingKey, monitorValue);
                this.mapVm.AddReadonlyLayer(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain01()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain01Folder;
                int value = Properties.Settings.Default.MonitorRain01Value;
                ShowRain(folderPath, LevelValueManager.RainName1, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain01Jm()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain01jmFolder;
                int value = Properties.Settings.Default.MonitorRain01Value;
                ShowRain(folderPath, LevelValueManager.RainName1, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain06()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain06Folder;
                int value = Properties.Settings.Default.MonitorRain06Value;
                ShowRain(folderPath, LevelValueManager.RainName6, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain06Jm()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain06jmFolder;
                int value = Properties.Settings.Default.MonitorRain06Value;
                ShowRain(folderPath, LevelValueManager.RainName6, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2405()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2405Folder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2408()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2408Folder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2408Jm()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2408jmFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2408All()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2408AllFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2414()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2414Folder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2414Jm()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2414jmFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2414All()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2414AllFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2420()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2420Folder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2420Jm()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2420jmFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRain2420All()
        {
            try
            {
                string folderPath = Properties.Settings.Default.MonitorRain2420AllFolder;
                int value = Properties.Settings.Default.MonitorRain24Value;
                ShowRain(folderPath, LevelValueManager.RainName24, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowSettingDialog()
        {
            var dialog = new MonitorSetting();
            dialog.ShowDialog();
        }
        private async void HandleMapViewTapped(object sender, MapViewInputEventArgs e)
        {
            if (layer == null)
                return;
            var graphic = await layer.HitTestAsync(mapView, e.Position);
            if (graphic != null)
            {
                string stationId = graphic.Attributes[HydrologicAttributes.Stationid].ToString();
                string stationName = graphic.Attributes[HydrologicAttributes.Name].ToString();
                string type = graphic.Attributes[HydrologicAttributes.Type].ToString();
                string l = graphic.Attributes[HydrologicAttributes.L].ToString();
                try
                {
                    double dl = double.Parse(l);
                    if(!(dl > 0))
                    {
                        l = "NAN";
                    }
                }
                catch(Exception)
                {
                    l = "NAN";
                }

                string q = graphic.Attributes[HydrologicAttributes.Q].ToString();
                try
                {
                    double dq = double.Parse(q);
                    if (!(dq > 0))
                    {
                        q = "NAN";
                    }
                }
                catch (Exception)
                {
                    q = "NAN";
                }
                string wl = graphic.Attributes[HydrologicAttributes.Wl1].ToString();
                try
                {
                    double dwl = double.Parse(wl);
                    if (!(dwl > 0))
                    {
                        wl = "NAN";
                    }
                }
                catch (Exception)
                {
                    wl = "NAN";
                }
                string time = graphic.Attributes[HydrologicAttributes.Time].ToString();
                
                WindowMapTrigger wd = new WindowMapTrigger(stationId, stationName, type, l, q, wl, time);
                wd.Show();
            }
        }

        private MapVm mapVm;
        private MapView mapView;
        private GraphicsLayer layer;
    }
}
