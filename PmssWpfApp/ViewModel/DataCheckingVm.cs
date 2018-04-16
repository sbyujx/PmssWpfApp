using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Pmss.Micaps.Render.Config;
using System.Windows;
using System.IO;
using Pmss.Micaps.Render.FileSource;
using Microsoft.Win32;
using Pmss.Micaps.Core.Enums;
using Pmss.Micaps.Product;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.DataChecking;

namespace PmssWpfApp.ViewModel
{
    public class DataCheckingVm
    {
        public DataCheckingVm()
        {
            ShowRealtimeRain01ZdCommand = new DelegateCommand(ShowRealtimeRain01Zd);
            ShowRealtimeRain01Command = new DelegateCommand(ShowRealtimeRain01);
            ShowRealtimeRain01JmCommand = new DelegateCommand(ShowRealtimeRain01Jm);
            ShowRealtimeRain03Command = new DelegateCommand(ShowRealtimeRain03);
            ShowRealtimeRain03JmCommand = new DelegateCommand(ShowRealtimeRain03Jm);
            ShowRealtimeRain06Command = new DelegateCommand(ShowRealtimeRain06);
            ShowRealtimeRain06JmCommand = new DelegateCommand(ShowRealtimeRain06Jm);
            ShowRealtimeRain2405Command = new DelegateCommand(ShowRealtimeRain2405);
            ShowRealtimeRain2408Command = new DelegateCommand(ShowRealtimeRain2408);
            ShowRealtimeRain2408JmCommand = new DelegateCommand(ShowRealtimeRain2408Jm);
            ShowRealtimeRain2408AllCommand = new DelegateCommand(ShowRealtimeRain2408All);
            ShowRealtimeRain2414Command = new DelegateCommand(ShowRealtimeRain2414);
            ShowRealtimeRain2414JmCommand = new DelegateCommand(ShowRealtimeRain2414Jm);
            ShowRealtimeRain2414AllCommand = new DelegateCommand(ShowRealtimeRain2414All);
            ShowRealtimeRain2420Command = new DelegateCommand(ShowRealtimeRain2420);
            ShowRealtimeRain2420JmCommand = new DelegateCommand(ShowRealtimeRain2420Jm);
            ShowRealtimeRain2420AllCommand = new DelegateCommand(ShowRealtimeRain2420All);
            ShowRealtimeRain05DaysCommand = new DelegateCommand(ShowRealtimeRain05Days);
            ShowRealtimeRain14DaysCommand = new DelegateCommand(ShowRealtimeRain14Days);

            ShowEditFloodCommand = new DelegateCommand(ShowEditFlood);
            ShowEditZilaoCommand = new DelegateCommand(ShowEditZilao);
            ShowEditDisasterCommand = new DelegateCommand(ShowEditDisaster);
            ShowEditRiverCommand = new DelegateCommand(ShowEditRiver);
            ShowEditGTBReferCommand = new DelegateCommand(ShowEditGTBRefer);

            ShowCheckingECMWF03Command = new DelegateCommand(ShowCheckingECMWF03);
            ShowCheckingECMWF06Command = new DelegateCommand(ShowCheckingECMWF06);
            ShowCheckingECMWF12Command = new DelegateCommand(ShowCheckingECMWF12);
            ShowCheckingECMWF24Command = new DelegateCommand(ShowCheckingECMWF24);
            ShowCheckingT63903Command = new DelegateCommand(ShowCheckingT63903);
            ShowCheckingT63906Command = new DelegateCommand(ShowCheckingT63906);
            ShowCheckingT63912Command = new DelegateCommand(ShowCheckingT63912);
            ShowCheckingT63924Command = new DelegateCommand(ShowCheckingT63924);

            ShowCheckingModelDisaster1Command = new DelegateCommand(ShowCheckingModelDisaster1);
            ShowCheckingModelDisaster2Command = new DelegateCommand(ShowCheckingModelDisaster2);
            ShowCheckingModelZilaoCommand = new DelegateCommand(ShowCheckingModelZilao);
            ShowCheckingModelRiverCommand = new DelegateCommand(ShowCheckingModelRiver);
            ShowCheckingModelFloodCommand = new DelegateCommand(ShowCheckingModelFlood);
            ShowCheckingModelAreaRainCommand = new DelegateCommand(ShowCheckingModelAreaRain);

            // Micaps 14
            ShowCheckingForcastWeather24Command = new DelegateCommand(ShowCheckingForcastWeather24);
            ShowCheckingForcastWeatherProCommand = new DelegateCommand(ShowCheckingForcastWeatherPro);
            ShowCheckingForcastRainQPF24Command = new DelegateCommand(ShowCheckingForcastRainQPF24);
            ShowCheckingForcastRainQPF06Command = new DelegateCommand(ShowCheckingForcastRainQPF06);
        }
        public void SetMapVm(MapVm vm)
        {
            mapVm = vm;
        }

        //private Dictionary<string, Layer> LoadedLayerDict { get; } = new Dictionary<string, Layer>();
        private Dictionary<string, NavMetaData> InternalNavDict { get; } = new Dictionary<string, NavMetaData>();
        public Dictionary<Layer, NavMetaData> NavDict { get; } = new Dictionary<Layer, NavMetaData>();

        public DelegateCommand ShowRealtimeRain01ZdCommand { get; set; }
        public DelegateCommand ShowRealtimeRain01Command { get; set; }
        public DelegateCommand ShowRealtimeRain01JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain03Command { get; set; }
        public DelegateCommand ShowRealtimeRain03JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain06Command { get; set; }
        public DelegateCommand ShowRealtimeRain06JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2405Command { get; set; }
        public DelegateCommand ShowRealtimeRain2408Command { get; set; }
        public DelegateCommand ShowRealtimeRain2408JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2408AllCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2414Command { get; set; }
        public DelegateCommand ShowRealtimeRain2414JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2414AllCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2420Command { get; set; }
        public DelegateCommand ShowRealtimeRain2420JmCommand { get; set; }
        public DelegateCommand ShowRealtimeRain2420AllCommand { get; set; }

        public DelegateCommand ShowRealtimeRain05DaysCommand { get; set; }
        public DelegateCommand ShowRealtimeRain14DaysCommand { get; set; }

        public DelegateCommand ShowEditFloodCommand { get; set; }
        public DelegateCommand ShowEditZilaoCommand { get; set; }
        public DelegateCommand ShowEditDisasterCommand { get; set; }
        public DelegateCommand ShowEditRiverCommand { get; set; }
        public DelegateCommand ShowEditGTBReferCommand { get; set; }

        public DelegateCommand ShowCheckingECMWF03Command { get; set; }
        public DelegateCommand ShowCheckingECMWF06Command { get; set; }
        public DelegateCommand ShowCheckingECMWF12Command { get; set; }
        public DelegateCommand ShowCheckingECMWF24Command { get; set; }
        public DelegateCommand ShowCheckingT63903Command { get; set; }
        public DelegateCommand ShowCheckingT63906Command { get; set; }
        public DelegateCommand ShowCheckingT63912Command { get; set; }
        public DelegateCommand ShowCheckingT63924Command { get; set; }

        //ModelDisaster1Folder
        //ModelDisaster2Folder
        //ModelZilaoFolder
        //ModelRiverFolder
        //ModelFloodFolder
        //ModelAreaRainFolder
        //ForcastWeather24Folder
        //ForcastWeatherProFolder
        //ForcastRainQPF24Folder
        //ForcastRainQPF06Floder
        public DelegateCommand ShowCheckingModelDisaster1Command { get; set; }
        public DelegateCommand ShowCheckingModelDisaster2Command { get; set; }
        public DelegateCommand ShowCheckingModelZilaoCommand { get; set; }
        public DelegateCommand ShowCheckingModelRiverCommand { get; set; }
        public DelegateCommand ShowCheckingModelFloodCommand { get; set; }
        public DelegateCommand ShowCheckingModelAreaRainCommand { get; set; }
        public DelegateCommand ShowCheckingForcastWeather24Command { get; set; }
        public DelegateCommand ShowCheckingForcastWeatherProCommand { get; set; }
        public DelegateCommand ShowCheckingForcastRainQPF24Command { get; set; }
        public DelegateCommand ShowCheckingForcastRainQPF06Command { get; set; }


        public DelegateCommand ShowSettingDialogCommand { get; set; }

        public void RemoveMapping(Layer layer)
        {
            NavMetaData metaData = null;
            if (NavDict.TryGetValue(layer, out metaData))
            {
                RemoveMapping(metaData.KeyName);
            }
        }
        private void RemoveMapping(string keyname)
        {
            if (InternalNavDict.Keys.Contains(keyname))
            {
                var layer = InternalNavDict[keyname].CurrentLayer;
                if (layer != null && NavDict.Keys.Contains(layer))
                {
                    NavDict.Remove(layer);
                }

                InternalNavDict.Remove(keyname);
            }
        }

        private void ShowRealtimeRain(string path, string settingKey)
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

                var result = GraphicsLayerFactory.GenerateRenderResultFromFile(file, settingKey);
                this.mapVm.AddReadonlyLayer(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowRealtimeRain(string path)
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

                var result = GraphicsLayerFactory.GenerateRenderResultFromFile(file);
                this.mapVm.AddReadonlyLayer(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowReadonlyDataHelper(string keyName, string folderPath, string pattern, string settingKey = null)
        {
            //string keyName = "ShowRealtimeRain01Zd";
            try
            {
                if (InternalNavDict.Keys.Contains(keyName))
                {
                    var layer = InternalNavDict[keyName].CurrentLayer;
                    this.mapVm.TreeViewVm.SelectLayer(layer);
                }
                else
                {
                    //string folderPath = Properties.Settings.Default.MonitorRain01ZdFolder;
                    //string pattern = @"^\d{8}.000$";
                    var generator = new BaseMetaDataGenerator(folderPath, pattern);
                    var navMetaData = generator.GenerateMetaData();
                    navMetaData.FolderPath = folderPath;
                    navMetaData.ShowLayer = new ShowLayerDelegate(filePath => ShowReadonlyLayerHelper(keyName, filePath, settingKey));
                    navMetaData.KeyName = keyName;

                    InternalNavDict[keyName] = navMetaData;
                    navMetaData.ShowCurrentFile();
                }
            }
            catch (Exception ex)
            {
                RemoveMapping(keyName);
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowReadonlyLayerHelper(string keyName, string filePath, string settingKey = null)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"找不到路径{filePath}");
            }
            if (!InternalNavDict.Keys.Contains(keyName))
            {
                throw new Exception("Interal Mapping Error!");
            }

            RenderResult result = null;
            if (settingKey == null)
                result = GraphicsLayerFactory.GenerateRenderResultFromFile(filePath);
            else
                result = GraphicsLayerFactory.GenerateRenderResultFromFile(filePath, settingKey);

            if (InternalNavDict[keyName].CurrentLayer != null)
            {
                this.mapVm.DeleteReadonlyLayer(InternalNavDict[keyName].CurrentLayer);
                this.NavDict.Remove(InternalNavDict[keyName].CurrentLayer);
            }

            InternalNavDict[keyName].CurrentLayer = result.Layer;
            NavDict[result.Layer] = InternalNavDict[keyName];

            this.mapVm.AddReadonlyLayer(result);
            this.mapVm.TreeViewVm.SelectLayer(result.Layer);
        }
        //private void ShowRealtimeRain()
        //{
        //    string keyName = "ShowRealtimeRain01Zd";
        //    try
        //    {
        //        if (InternalNavDict.Keys.Contains(keyName))
        //        {
        //            var layer = InternalNavDict["ShowRealtimeRain01Zd"].CurrentLayer;
        //            this.mapVm.TreeViewVm.SelectLayer(layer);
        //        }
        //        else
        //        {
        //            string folderPath = Properties.Settings.Default.MonitorRain01ZdFolder;
        //            string pattern = @"^\d{8}.000$";
        //            var generator = new BaseMetaDataGenerator(folderPath, pattern);
        //            var navMetaData = generator.GenerateMetaData();
        //            navMetaData.FolderPath = folderPath;
        //            navMetaData.ShowLayer = ShowRealtimeRain01Zd;
        //            navMetaData.KeyName = keyName;

        //            InternalNavDict[keyName] = navMetaData;
        //            navMetaData.ShowCurrentFile();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RemoveMapping(keyName);
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        //private void ShowRealtimeRain01Zd(string filePath)
        //{
        //    string keyName = "ShowRealtimeRain01Zd";
        //    try
        //    {
        //        if (!File.Exists(filePath))
        //        {
        //            throw new Exception($"找不到路径{filePath}");
        //        }
        //        if (!InternalNavDict.Keys.Contains(keyName))
        //        {
        //            throw new Exception("Interal Mapping Error!");
        //        }

        //        var result = GraphicsLayerFactory.GenerateRenderResultFromFile(filePath);

        //        if (InternalNavDict[keyName].CurrentLayer != null)
        //        {
        //            this.mapVm.DeleteReadonlyLayer(InternalNavDict[keyName].CurrentLayer);
        //            this.NavDict.Remove(InternalNavDict[keyName].CurrentLayer);
        //        }

        //        InternalNavDict[keyName].CurrentLayer = result.Layer;
        //        NavDict[result.Layer] = InternalNavDict[keyName];

        //        this.mapVm.AddReadonlyLayer(result);
        //        this.mapVm.TreeViewVm.SelectLayer(result.Layer);
        //    }
        //    catch (Exception ex)
        //    {
        //        RemoveMapping(keyName);
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void ShowRealtimeRain01Zd()
        {
            string keyName = "ShowRealtimeRain01Zd";
            string folderPath = Properties.Settings.Default.MonitorRain01ZdFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern);
        }
        private void ShowRealtimeRain01()
        {
            string keyName = "ShowRealtimeRain01";
            string folderPath = Properties.Settings.Default.MonitorRain01Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName1);
        }
        private void ShowRealtimeRain01Jm()
        {
            string keyName = "ShowRealtimeRain01Jm";
            string folderPath = Properties.Settings.Default.MonitorRain01jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName1);
        }
        private void ShowRealtimeRain03()
        {
            string keyName = "ShowRealtimeRain03";
            string folderPath = Properties.Settings.Default.MonitorRain03Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern);
        }
        private void ShowRealtimeRain03Jm()
        {
            string keyName = "ShowRealtimeRain03Jm";
            string folderPath = Properties.Settings.Default.MonitorRain03jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern);
        }
        private void ShowRealtimeRain06()
        {
            string keyName = "ShowRealtimeRain06";
            string folderPath = Properties.Settings.Default.MonitorRain06Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName6);
        }
        private void ShowRealtimeRain06Jm()
        {
            string keyName = "ShowRealtimeRain06Jm";
            string folderPath = Properties.Settings.Default.MonitorRain06jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName6);
        }
        private void ShowRealtimeRain2405()
        {
            string keyName = "ShowRealtimeRain2405";
            string folderPath = Properties.Settings.Default.MonitorRain2405Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2408()
        {
            string keyName = "ShowRealtimeRain2408";
            string folderPath = Properties.Settings.Default.MonitorRain2408Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2408Jm()
        {
            string keyName = "ShowRealtimeRain2408Jm";
            string folderPath = Properties.Settings.Default.MonitorRain2408jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2408All()
        {
            string keyName = "ShowRealtimeRain2408All";
            string folderPath = Properties.Settings.Default.MonitorRain2408AllFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2414()
        {
            string keyName = "ShowRealtimeRain2414";
            string folderPath = Properties.Settings.Default.MonitorRain2414Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2414Jm()
        {
            string keyName = "ShowRealtimeRain2414Jm";
            string folderPath = Properties.Settings.Default.MonitorRain2414jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2414All()
        {
            string keyName = "ShowRealtimeRain2414All";
            string folderPath = Properties.Settings.Default.MonitorRain2414AllFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2420()
        {
            string keyName = "ShowRealtimeRain2420";
            string folderPath = Properties.Settings.Default.MonitorRain2420Folder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2420Jm()
        {
            string keyName = "ShowRealtimeRain2420Jm";
            string folderPath = Properties.Settings.Default.MonitorRain2420jmFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain2420All()
        {
            string keyName = "ShowRealtimeRain2420All";
            string folderPath = Properties.Settings.Default.MonitorRain2420AllFolder;
            string pattern = @"^\d{8}.000$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
        }
        private void ShowRealtimeRain05Days()
        {
            string keyName = "ShowRealtimeRain05Days";
            string folderPath = Properties.Settings.Default.MonitorRain05DaysFolder;
            string pattern = @"^\d{8}.005$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName05Days);
        }
        private void ShowRealtimeRain14Days()
        {
            string keyName = "ShowRealtimeRain14Days";
            string folderPath = Properties.Settings.Default.MonitorRain14DaysFolder;
            string pattern = @"^\d{8}.014$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName14Days);
        }

        private void ShowEditDataHelper(string keyName, string folderPath, string pattern, string settingName, ProductTypeEnum productType)
        {
            try
            {
                if (InternalNavDict.Keys.Contains(keyName))
                {
                    var layer = InternalNavDict[keyName].CurrentLayer;
                    this.mapVm.TreeViewVm.SelectLayer(layer);
                }
                else
                {
                    //string folderPath = Properties.Settings.Default.MonitorRain01ZdFolder;
                    //string pattern = @"^\d{8}.000$";
                    var generator = new BaseMetaDataGenerator(folderPath, pattern);
                    var navMetaData = generator.GenerateMetaData();
                    navMetaData.FolderPath = folderPath;
                    navMetaData.ShowLayer = new ShowLayerDelegate(filePath => ShowEditLayerHelper(keyName, filePath, settingName, productType));
                    navMetaData.KeyName = keyName;

                    InternalNavDict[keyName] = navMetaData;
                    navMetaData.ShowCurrentFile();
                }
            }
            catch (Exception ex)
            {
                RemoveMapping(keyName);
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowEditLayerHelper(string keyName, string filePath, string settingName, ProductTypeEnum productType)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"找不到路径{filePath}");
            }
            if (!InternalNavDict.Keys.Contains(keyName))
            {
                throw new Exception("Interal Mapping Error!");
            }

            var renderResult = GraphicsLayerFactory.GenerateRenderResultFromFile(filePath);
            if (renderResult.Type != DiamondType.Diamond14)
            {
                throw new Exception("错误的文件格式.");
            }
            renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, settingName);
            renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, productType);

            if (InternalNavDict[keyName].CurrentLayer != null)
            {
                this.mapVm.DeleteEditableLayer(InternalNavDict[keyName].CurrentLayer);
                this.NavDict.Remove(InternalNavDict[keyName].CurrentLayer);
            }

            InternalNavDict[keyName].CurrentLayer = renderResult.Layer;
            NavDict[renderResult.Layer] = InternalNavDict[keyName];

            mapVm.AddEditableLayer(renderResult);
            this.mapVm.TreeViewVm.SelectLayer(renderResult.Layer);
        }
        private void ShowEdit(string folderPath, string settingName, ProductTypeEnum productType)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string filename = dialog.FileName;
                    var renderResult = GraphicsLayerFactory.GenerateRenderResultFromFile(filename);
                    if (renderResult.Type != DiamondType.Diamond14)
                    {
                        throw new Exception("错误的文件格式.");
                    }
                    renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, settingName);
                    renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, productType);

                    mapVm.AddEditableLayer(renderResult);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        
        private void ShowEditFlood()
        {
            //// hvyymmddhh.024
            //string keyName = "ShowEditFlood";
            //string folderPath = Properties.Settings.Default.EditFloodFolder;
            //string pattern = @"^hv\d{8}.024$";
            //ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);

            ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }
        private void ShowEditZilao()
        {
            //// zlyymmddhh.024
            //string keyName = "ShowEditZilao";
            //string folderPath = Properties.Settings.Default.EditZilaoFolder;
            //string pattern = @"^zl\d{8}.024$";
            //ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Zilao);

            ShowEdit(Properties.Settings.Default.EditZilaoFolder, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Zilao);
        }
        private void ShowEditDisaster()
        {
            //// shyyyymmddhh.024
            //string keyName = "ShowEditDisaster";
            //string folderPath = Properties.Settings.Default.EditDisasterFolder;
            //string pattern = @"^sh\d{10}.024$";
            //ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Disaster);

            ShowEdit(Properties.Settings.Default.EditDisasterFolder, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Disaster);
        }
        private void ShowEditRiver()
        {
            //// fvyymmddhh.024
            //string keyName = "ShowEditRiver";
            //string folderPath = Properties.Settings.Default.EditRiverFolder;
            //string pattern = @"^fv\d{8}.024$";
            //ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.River);

            ShowEdit(Properties.Settings.Default.EditRiverFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.River);
        }
        private void ShowEditGTBRefer()
        {
            //// gtyymmdd.000
            //string keyName = "ShowEditGTBRefer";
            //string folderPath = Properties.Settings.Default.EditGTBReferFolder;
            //string pattern = @"^gt\d{6}.000$";
            //ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);

            ShowEdit(Properties.Settings.Default.EditGTBReferFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }

        private void ShowCheckingData(string folderPath, string settingName = null)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string filename = dialog.FileName;
                    var renderResult = settingName == null ? GraphicsLayerFactory.GenerateRenderResultFromFile(filename) : GraphicsLayerFactory.GenerateRenderResultFromFile(filename, settingName);
                    if (renderResult.Type != DiamondType.Diamond3 && renderResult.Type != DiamondType.Diamond4)
                    {
                        throw new Exception("错误的文件格式.");
                    }

                    mapVm.AddReadonlyLayer(renderResult);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
        private void ShowCheckingDataZilao(string folderPath, string settingName = null)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var render = new Diamond3RenderZilao();
                    string filename = dialog.FileName;
                    var renderResult = settingName == null ? render.GenerateRenderResult(filename) : render.GenerateRenderResult(filename, settingName);
                    if (renderResult.Type != DiamondType.Diamond3 && renderResult.Type != DiamondType.Diamond4)
                    {
                        throw new Exception("错误的文件格式.");
                    }

                    mapVm.AddReadonlyLayer(renderResult);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }
        private void ShowReadonlyDataHelperZilao(string keyName, string folderPath, string pattern, string settingKey = null)
        {
            //string keyName = "ShowRealtimeRain01Zd";
            try
            {
                if (InternalNavDict.Keys.Contains(keyName))
                {
                    var layer = InternalNavDict[keyName].CurrentLayer;
                    this.mapVm.TreeViewVm.SelectLayer(layer);
                }
                else
                {
                    //string folderPath = Properties.Settings.Default.MonitorRain01ZdFolder;
                    //string pattern = @"^\d{8}.000$";
                    var generator = new BaseMetaDataGenerator(folderPath, pattern);
                    var navMetaData = generator.GenerateMetaData();
                    navMetaData.FolderPath = folderPath;
                    navMetaData.ShowLayer = new ShowLayerDelegate(filePath => ShowReadonlyLayerHelperZilao(keyName, filePath, settingKey));
                    navMetaData.KeyName = keyName;

                    InternalNavDict[keyName] = navMetaData;
                    navMetaData.ShowCurrentFile();
                }
            }
            catch (Exception ex)
            {
                RemoveMapping(keyName);
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowReadonlyLayerHelperZilao(string keyName, string filePath, string settingKey = null)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception($"找不到路径{filePath}");
            }
            if (!InternalNavDict.Keys.Contains(keyName))
            {
                throw new Exception("Interal Mapping Error!");
            }

            var render = new Diamond3RenderZilao();
            RenderResult result = null;
            if (settingKey == null)
                result = render.GenerateRenderResult(filePath);
            else
                result = render.GenerateRenderResult(filePath, settingKey);

            if (InternalNavDict[keyName].CurrentLayer != null)
            {
                this.mapVm.DeleteReadonlyLayer(InternalNavDict[keyName].CurrentLayer);
                this.NavDict.Remove(InternalNavDict[keyName].CurrentLayer);
            }

            InternalNavDict[keyName].CurrentLayer = result.Layer;
            NavDict[result.Layer] = InternalNavDict[keyName];

            this.mapVm.AddReadonlyLayer(result);
            this.mapVm.TreeViewVm.SelectLayer(result.Layer);
        }
        private void ShowCheckingECMWF03()
        {
            string keyName = "ShowCheckingECMWF03";
            string folderPath = Properties.Settings.Default.ECMWF03Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName3);
            //ShowCheckingData(Properties.Settings.Default.ECMWF03Folder, LevelValueManager.RainName3);
        }
        private void ShowCheckingECMWF06()
        {
            string keyName = "ShowCheckingECMWF06";
            string folderPath = Properties.Settings.Default.ECMWF06Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName6);
            //ShowCheckingData(Properties.Settings.Default.ECMWF06Folder, LevelValueManager.RainName6);
        }
        private void ShowCheckingECMWF12()
        {
            string keyName = "ShowCheckingECMWF12";
            string folderPath = Properties.Settings.Default.ECMWF12Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName12);
            //ShowCheckingData(Properties.Settings.Default.ECMWF12Folder, LevelValueManager.RainName12);
        }
        private void ShowCheckingECMWF24()
        {
            string keyName = "ShowCheckingECMWF24";
            string folderPath = Properties.Settings.Default.ECMWF24Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
            //ShowCheckingData(Properties.Settings.Default.ECMWF24Folder, LevelValueManager.RainName24);
        }
        private void ShowCheckingT63903()
        {
            string keyName = "ShowCheckingT63903";
            string folderPath = Properties.Settings.Default.T63903Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName3);
            //ShowCheckingData(Properties.Settings.Default.T63903Folder, LevelValueManager.RainName3);
        }
        private void ShowCheckingT63906()
        {
            string keyName = "ShowCheckingT63906";
            string folderPath = Properties.Settings.Default.T63906Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName6);
            //ShowCheckingData(Properties.Settings.Default.T63906Folder, LevelValueManager.RainName6);
        }
        private void ShowCheckingT63912()
        {
            string keyName = "ShowCheckingT63912";
            string folderPath = Properties.Settings.Default.T63912Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName12);
            //ShowCheckingData(Properties.Settings.Default.T63912Folder, LevelValueManager.RainName12);
        }
        private void ShowCheckingT63924()
        {
            string keyName = "ShowCheckingT63924";
            string folderPath = Properties.Settings.Default.T63924Folder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.RainName24);
            //ShowCheckingData(Properties.Settings.Default.T63924Folder, LevelValueManager.RainName24);
        }

        private void ShowCheckingModelDisaster1()
        {
            //yymmddhh.024
            string keyName = "ShowCheckingModelDisaster1";
            string folderPath = Properties.Settings.Default.ModelDisaster1Folder;
            string pattern = @"^\d{8}.024$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameDisaster);
            //ShowCheckingData(Properties.Settings.Default.ModelDisaster1Folder, LevelValueManager.LevelNameDisaster);
        }
        private void ShowCheckingModelDisaster2()
        {
            //yymmddhh.024
            string keyName = "ShowCheckingModelDisaster2";
            string folderPath = Properties.Settings.Default.ModelDisaster2Folder;
            string pattern = @"^\d{8}.024$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameDisaster);
            //ShowCheckingData(Properties.Settings.Default.ModelDisaster2Folder, LevelValueManager.LevelNameDisaster);
        }
        private void ShowCheckingModelZilao()
        {
            //fxyyyymmddhh.hhh
            string keyName = "ShowCheckingModelZilao";
            string folderPath = Properties.Settings.Default.ModelZilaoFolder;
            string pattern = @"^fx\d{10}.\d{3}$";
            ShowReadonlyDataHelperZilao(keyName, folderPath, pattern, LevelValueManager.LevelNameDisaster);
            //ShowCheckingDataZilao(Properties.Settings.Default.ModelZilaoFolder, LevelValueManager.LevelNameDisaster);
        }
        private void ShowCheckingModelRiver()
        {
            // fwyyyymmddhh.hhh
            string keyName = "ShowCheckingModelRiver";
            string folderPath = Properties.Settings.Default.ModelRiverFolder;
            string pattern = @"^fw\d{10}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood);
            //ShowCheckingData(Properties.Settings.Default.ModelRiverFolder, LevelValueManager.LevelNameFlood);
        }
        private void ShowCheckingModelFlood()
        {
            // point_flood_yyyymmddhh.hhh
            string keyName = "ShowCheckingModelFlood";
            string folderPath = Properties.Settings.Default.ModelFloodFolder;
            string pattern = @"^point_flood_\d{10}.\d{3}$";
            ShowReadonlyDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood);
            //ShowCheckingData(Properties.Settings.Default.ModelFloodFolder, LevelValueManager.LevelNameFlood);
        }
        private void ShowCheckingModelAreaRain()
        {
            ShowCheckingData(Properties.Settings.Default.ModelAreaRainFolder);
        }

        private void ShowCheckingForcastWeather24()
        {
            // yymmddhh.024
            string keyName = "ShowCheckingForcastWeather24";
            string folderPath = Properties.Settings.Default.ForcastWeather24Folder;
            string pattern = @"^\d{8}.024$";
            ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            //ShowEdit(Properties.Settings.Default.ForcastWeather24Folder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }
        private void ShowCheckingForcastWeatherPro()
        {
            // yymmdd08.hhh, yymmdd14.hhh, yymmdd20.hhh
            string keyName = "ShowCheckingForcastWeatherPro";
            string folderPath = Properties.Settings.Default.ForcastWeatherProFolder;
            string pattern = @"^\d{8}.\d{3}$";
            ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            //ShowEdit(Properties.Settings.Default.ForcastWeatherProFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }
        private void ShowCheckingForcastRainQPF24()
        {
            // rrmmddhh.hhh
            string keyName = "ShowCheckingForcastRainQPF24";
            string folderPath = Properties.Settings.Default.ForcastRainQPF24Folder;
            string pattern = @"^rr\d{6}.\d{3}$";
            ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            //ShowEdit(Properties.Settings.Default.ForcastRainQPF24Folder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }
        private void ShowCheckingForcastRainQPF06()
        {
            // rrrmmddhh.hhh
            string keyName = "ShowCheckingForcastRainQPF06";
            string folderPath = Properties.Settings.Default.ForcastRainQPF06Folder;
            string pattern = @"^rrr\d{6}.\d{3}$";
            ShowEditDataHelper(keyName, folderPath, pattern, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            //ShowEdit(Properties.Settings.Default.ForcastRainQPF06Folder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }

        private MapVm mapVm;
    }
}
