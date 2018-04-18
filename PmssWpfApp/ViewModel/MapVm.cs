using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using Microsoft.Win32;
using Pmss.Micaps.Render.FileSource;
using Pmss.Micaps.Core.Enums;
using System.Windows;
using Prism.Commands;
using System.ComponentModel;
using System.IO;
using PMSS.Render.FileSource;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using Pmss.Micaps.Product;
using Pmss.Micaps.Render.Config;
using PmssWpfApp.Dialogs;
using Pmss.Micaps.DataChecking;
using Pmss.Micaps.DataAccess.FileSource;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using Pmss.Micaps.DataEntities.FileSource;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace PmssWpfApp.ViewModel
{
    public static class MapVmConst
    {
        public static readonly string BaseGroupLayer0Name = "底图";
        public static readonly string BaseGroupLayer0Id = "BaseGroupLayer0";
        public static readonly string BaseGroupLayerName = "基础地理信息";
        public static readonly string BaseGroupLayerId = "BaseGroupLayer";
        public static readonly string DataGroupLayerName = "只读地理数据";
        public static readonly string DataGroupLayerId = "DataGroupLayer";
        public static readonly string EditableGroupLayerName = "可编辑地理数据";
        public static readonly string EditableGroupLayerId = "EditableGroupLayer";
        public static readonly string ProductGroupLayerName = "产品";
        public static readonly string ProductGroupLayerId = "ProductGroupLayer";
    }
    public class MapVm : INotifyPropertyChanged
    {
        public MapVm()
        {
            InitializeGrouplayers();
            InitializeBaseMap();

            this.OpenMicapsFileCommand = new DelegateCommand(OpenMicapsFile);
            this.AddEditableLayerCommand = new DelegateCommand(AddEditableLayer);
            this.DeleteReadonlyLayerCommand = new DelegateCommand(DeleteReadonlyLayer);
            this.DeleteEditableLayerCommand = new DelegateCommand(DeleteEditableLayer);
            this.SelectedLayerChangedCommand = new DelegateCommand<LayersTreeViewItem>(SelectedLayperChanged);
            this.SaveEditableLayerCommand = new DelegateCommand(SaveToDiamond14File);
            this.LoadTopographicMapCommand = new DelegateCommand(LoadTopographicMap);
            this.PreviewProductLayerCommand = new DelegateCommand(PreviewProductLayer);
            this.ZoomAndPreviewProductLayerCommand = new DelegateCommand(ZoomAndPreviewProductLayer);
            this.DeleteProductLayerCommand = new DelegateCommand(DeleteProductLayer);
            this.ExportProductLayerCommand = new DelegateCommand(ExportProductLayer);
            this.ExportScreenShotCommand = new DelegateCommand(ExportScreenShot);
            this.ShowEditFloodCommand = new DelegateCommand(ShowEditFlood);
            this.CheckFloodCommand = new DelegateCommand(CheckFloodWorker);
            this.CheckDizhiCommand = new DelegateCommand(CheckDisasterWorker);
            this.CheckOutCommand = new DelegateCommand(CheckOut);
            this.ShowEditZilaoCommand = new DelegateCommand(ShowEditZilao);
            this.ShowEditDisasterCommand = new DelegateCommand(ShowEditDisaster);
            this.ShowEditRiverCommand = new DelegateCommand(ShowEditRiver);
            this.ShowNextFileCommand = new DelegateCommand(ShowNextFile);
            this.ShowLastFileCommand = new DelegateCommand(ShowLastFile);
            this.currentProgress = 0;
            this.worker.DoWork += worker_DoWork_CheckFlood;
            this.worker.ProgressChanged += ProgressChangedFlood;
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted_CheckFlood;
            this.disasterworker.DoWork += worker_DoWork_CheckDisaster;
            this.disasterworker.ProgressChanged += ProgressChangedDisaster;
            this.disasterworker.RunWorkerCompleted += worker_RunWorkerCompleted_CheckDisaster;
        }
        public int CurrentProgress
        {
            get
            {
                return currentProgress;
            }
            set
            {
                currentProgress = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentProgress)));
                }
            }
        }
        private int currentProgress;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker disasterworker = new BackgroundWorker();
        private void ProgressChangedFlood(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            CurrentProgress = e.ProgressPercentage;
        }

        private void CheckFloodWorker()
        {
            this.worker.WorkerReportsProgress = true;
            this.worker.RunWorkerAsync();
        }

        private void CheckDisasterWorker()
        {
            this.disasterworker.WorkerReportsProgress = true;
            this.disasterworker.RunWorkerAsync();
        }

        private void worker_DoWork_CheckDisaster(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            string folderPath = Properties.Settings.Default.EditDisasterFolder;
            string settingName = LevelValueManager.LevelNameDisaster;
            ProductTypeEnum productType = ProductTypeEnum.Disaster;
            //ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;
            List<double> latList = new List<double>();
            List<double> lonList = new List<double>();


            if (dialog.ShowDialog() == true)
            {
                this.disasterworker.ReportProgress(20);
                CheckDisaster(dialog.FileName, settingName, productType, out latList, out lonList);
                this.disasterworker.ReportProgress(80);
                ThrResult thr = new ThrResult();
                thr.FileName = dialog.FileName;
                thr.LatList = latList;
                thr.LonList = lonList;
                thr.SettingName = settingName;
                thr.ProductType = productType;
                e.Result = thr;
                //Sleep(100);
                this.disasterworker.ReportProgress(100);

            }
        }
        private void worker_RunWorkerCompleted_CheckDisaster(object sender,
                                       RunWorkerCompletedEventArgs e)
        {
            ThrResult thr = (ThrResult)e.Result;
            if (thr == null)
                return;
            var renderResult = new Diamond14Render().GenerateRenderResult(thr.FileName, thr.LatList, thr.LonList);
            if (renderResult.Type != DiamondType.Diamond14)
            {
                throw new Exception("错误的文件格式.");
            }
            renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, thr.SettingName);
            renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, thr.ProductType);
            //this.PmssMapView.Dispatcher.Invoke((Action)delegate ()
            //{
            try
            {
                if (this.PmssMapView != null)
                {
                    //this.PmssMapView.SetView(new Viewpoint(new MapPoint(106, 38, SpatialReferences.Wgs84), 30000000));
                    ProductGenerator.ZoomMapView(this.PmssMapView);
                }
                //this.PreviewProductLayerHelper(ProductRegionEnum.Country);
                //if (!IsEditableLayerSelected)
                //    return;
                if (drawOptionsVm.MapEditor.IsActive)
                {
                    throw new Exception("请结束图层编辑");
                }

                try
                {
                    var generator = new ProductGenerator();
                    //var setting = LevelValueManager.Settings[LevelValueManager.LevelNameFlood];
                    //selectedLayer.IsVisible = false;
                    //var renderResult = diamond14SettingDic[selectedLayer as GraphicsLayer];
                    //var layer = renderResult.Layer;
                    var layer = generator.Generate(renderResult);
                    //productGroupLayer.ChildLayers.Add(layer);
                    this.TreeViewVm.AddProductLayer(layer);
                    var saveOption = new ProductSaveOption
                    {
                        ProductType = (ProductTypeEnum)renderResult.Miscellaneous[Diamond14Attributes.ProductType],
                        ProductRegion = ProductRegionEnum.Country
                    };


                    // Set editable layer's IsVisible to false, select the new product layer
                    //this.TreeViewVm.SetVisibility(selectedLayer, false);
                    ProductSaveSettingDic.Add(layer, saveOption);
                    this.TreeViewVm.SelectLayer(layer);
                    if (saveOption.ProductType == ProductTypeEnum.River)
                    {
                        this.TreeViewVm.SetVisibility(this.districtLayer, false);
                        this.TreeViewVm.SetVisibility(this.basinLayer, true);
                        this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                    }
                    else
                    {
                        this.TreeViewVm.SetVisibility(this.districtLayer, true);
                        this.TreeViewVm.SetVisibility(this.basinLayer, false);
                        this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //});

            MessageBox.Show("地质灾害检验完成");
            this.CurrentProgress = 0;
            //update ui once worker complete his work
        }
        private void ProgressChangedDisaster(object sender, ProgressChangedEventArgs e)
        {
            // This is called on the UI thread when ReportProgress method is called
            CurrentProgress = e.ProgressPercentage;
        }

        private void worker_DoWork_CheckFlood(object sender, DoWorkEventArgs e)
        {
            // run all background tasks here
            string folderPath = Properties.Settings.Default.EditFloodFolder;
            string settingName = LevelValueManager.LevelNameFlood;
            ProductTypeEnum productType = ProductTypeEnum.Flood;
            //ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;
            List<double> latList = new List<double>();
            List<double> lonList = new List<double>();
            

            if (dialog.ShowDialog() == true)
            {
                this.worker.ReportProgress(20);
                CheckFlood(dialog.FileName, settingName, productType, out latList, out lonList);
                this.worker.ReportProgress(80);
                ThrResult thr = new ThrResult();
                thr.FileName = dialog.FileName;
                thr.LatList = latList;
                thr.LonList = lonList;
                thr.SettingName = settingName;
                thr.ProductType = productType;
                e.Result = thr;
                //Sleep(100);
                this.worker.ReportProgress(100);

            }
        }

        public class ThrResult
        {
            private List<double> latList;
            public List<double> LatList
            {
                get
                {
                    return latList;
                }
                set
                {
                    latList = value;
                }
            }

            private List<double> lonList;
            public List<double> LonList
            {
                get
                {
                    return lonList;
                }
                set
                {
                    lonList = value;
                }
            }

            private string fileName;
            public string FileName
            {
                get
                {
                    return fileName;
                }
                set
                {
                    fileName = value;
                }
            }

            private string settingName;
            public string SettingName
            {
                get
                {
                    return settingName;
                }
                set
                {
                    settingName = value;
                }
            }

            private ProductTypeEnum productType;
            public ProductTypeEnum ProductType
            {
                get
                {
                    return productType;
                }
                set
                {
                    productType = value;
                }
            }
        }

        private void worker_RunWorkerCompleted_CheckFlood(object sender,
                                       RunWorkerCompletedEventArgs e)
        {
            ThrResult thr = (ThrResult)e.Result;
            if (thr == null)
                return;
            var renderResult = new Diamond14Render().GenerateRenderResult(thr.FileName, thr.LatList, thr.LonList);
            if (renderResult.Type != DiamondType.Diamond14)
            {
                throw new Exception("错误的文件格式.");
            }
            renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, thr.SettingName);
            renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, thr.ProductType);
            //this.PmssMapView.Dispatcher.Invoke((Action)delegate ()
            //{
            try
                {
                    if (this.PmssMapView != null)
                    {
                        //this.PmssMapView.SetView(new Viewpoint(new MapPoint(106, 38, SpatialReferences.Wgs84), 30000000));
                        ProductGenerator.ZoomMapView(this.PmssMapView);
                    }
                    //this.PreviewProductLayerHelper(ProductRegionEnum.Country);
                    //if (!IsEditableLayerSelected)
                    //    return;
                    if (drawOptionsVm.MapEditor.IsActive)
                    {
                        throw new Exception("请结束图层编辑");
                    }

                    try
                    {
                        var generator = new ProductGenerator();
                    //var setting = LevelValueManager.Settings[LevelValueManager.LevelNameFlood];
                    //selectedLayer.IsVisible = false;
                    //var renderResult = diamond14SettingDic[selectedLayer as GraphicsLayer];
                    //var layer = renderResult.Layer;
                        var layer = generator.Generate(renderResult);
                        //productGroupLayer.ChildLayers.Add(layer);
                        this.TreeViewVm.AddProductLayer(layer);
                        var saveOption = new ProductSaveOption
                        {
                            ProductType = (ProductTypeEnum)renderResult.Miscellaneous[Diamond14Attributes.ProductType],
                            ProductRegion = ProductRegionEnum.Country
                        };


                        // Set editable layer's IsVisible to false, select the new product layer
                        //this.TreeViewVm.SetVisibility(selectedLayer, false);
                        ProductSaveSettingDic.Add(layer, saveOption);
                        this.TreeViewVm.SelectLayer(layer);
                        if (saveOption.ProductType == ProductTypeEnum.River)
                        {
                            this.TreeViewVm.SetVisibility(this.districtLayer, false);
                            this.TreeViewVm.SetVisibility(this.basinLayer, true);
                            this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                        }
                        else
                        {
                            this.TreeViewVm.SetVisibility(this.districtLayer, true);
                            this.TreeViewVm.SetVisibility(this.basinLayer, false);
                            this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            //});

            MessageBox.Show("山洪检验完成");
            this.CurrentProgress = 0;
            //update ui once worker complete his work
        }
        public void SetDrawOptionsVm(DrawOptionsGroupVm vm)
        {
            this.drawOptionsVm = vm;
        }
        public void SetDataCheckingVm(DataCheckingVm vm)
        {
            this.dataCheckingVm = vm;
        }
        public void SetDataMonitoringVm(DataMonitoringVm vm)
        {
            this.dataMonitoringVm = vm;
        }

        //Child View Model
        public LabelListSettingVm LabelListOfSelectedLayer
        {
            get
            {
                return this.labelListOfSelectedLayer;
            }
            set
            {
                this.labelListOfSelectedLayer = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(LabelListOfSelectedLayer)));
                }
            }
        }

        public GraphicGroup GraphicGroupOfSelectedLayer
        {
            get
            {
                return this.graphicGroupOfSelectedLayer;
            }
            set
            {
                this.graphicGroupOfSelectedLayer = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(GraphicGroupOfSelectedLayer)));

                }
            }
        }


        public Map PmssMap { get; set; }
        public LayersTreeViewVm TreeViewVm { get; set; }
        public DelegateCommand OpenMicapsFileCommand { get; set; }
        public DelegateCommand AddEditableLayerCommand { get; set; }
        public DelegateCommand<LayersTreeViewItem> SelectedLayerChangedCommand { get; set; }
        public DelegateCommand DeleteReadonlyLayerCommand { get; set; }
        public DelegateCommand DeleteEditableLayerCommand { get; set; }
        public DelegateCommand DeleteProductLayerCommand { get; set; }
        public DelegateCommand SaveEditableLayerCommand { get; set; }
        public DelegateCommand LoadTopographicMapCommand { get; set; }
        public DelegateCommand PreviewProductLayerCommand { get; set; }
        public DelegateCommand ExportProductLayerCommand { get; set; }
        public DelegateCommand ExportScreenShotCommand { get; set; }
        public DelegateCommand ZoomAndPreviewProductLayerCommand { get; set; }
        public DelegateCommand ShowEditFloodCommand { get; set; }
        public DelegateCommand CheckFloodCommand { get; set; }
        public DelegateCommand CheckDizhiCommand { get; set; }
        public DelegateCommand CheckOutCommand { get; set; }
        public DelegateCommand ShowEditZilaoCommand { get; set; }
        public DelegateCommand ShowEditDisasterCommand { get; set; }
        public DelegateCommand ShowEditRiverCommand { get; set; }
        public DelegateCommand ShowNextFileCommand { get; set; }
        public DelegateCommand ShowLastFileCommand { get; set; }
        public bool IsReadonlyLayerSelected
        {
            get
            {
                return isReadonlyLayerSelected;
            }
            set
            {
                isReadonlyLayerSelected = value;
                // select the points of Hydrologic
                if (this.dataMonitoringVm != null)
                {
                    if (value)
                    {
                        var layer = this.selectedLayer as GraphicsLayer;
                        if (layer != null && layer.DisplayName != null && layer.DisplayName.StartsWith("水情数据"))
                        {
                            dataMonitoringVm.SetCurrentLayer(layer);
                        }
                        else
                        {
                            dataMonitoringVm.SetCurrentLayer(null);
                        }
                    }
                    else
                    {
                        dataMonitoringVm.SetCurrentLayer(null);
                    }
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsReadonlyLayerSelected)));
                }
            }
        }
        public bool IsEditableLayerSelected
        {
            get
            {
                return this.isEditableLayerSelected;
            }
            set
            {
                this.isEditableLayerSelected = value;
                // Draw or edit in this layer
                if (this.drawOptionsVm != null)
                {
                    if (value)
                    {
                        drawOptionsVm.SetLayer(this.selectedLayer as GraphicsLayer);
                    }
                    else
                    {
                        drawOptionsVm.SetLayer(null);
                    }
                }
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsEditableLayerSelected)));
                }
            }
        }
        public bool IsProductLayerSelected
        {
            get
            {
                return this.isProductLayerSelected;
            }
            set
            {
                this.isProductLayerSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsProductLayerSelected)));
                }
            }
        }
        public bool IsTopographicMapShown { get; set; } = false;

        public MapView PmssMapView { get; set; }

        public void AddReadonlyLayer(RenderResult renderResult)
        {
            //if (this.selectedLayer == null)
            //{
            //    this.TreeViewVm.AddReadonlyLayer(renderResult.Layer);
            //    this.selectedLayer = renderResult.Layer;
            //}
            //else
            //{
            //    this.TreeViewVm.RemoveReadonlyLayer(this.selectedLayer);
            //    this.selectedLayer = null;
            //}
            this.TreeViewVm.AddReadonlyLayer(renderResult.Layer);
            labelListSettingDict[renderResult.Layer] = new LabelListSettingVm(renderResult.LabelSettings);

            if (renderResult.GraphicGroupVM != null)
            {
                graphicGroupDict[renderResult.Layer] = renderResult.GraphicGroupVM;
            }
        }
        private void DeleteReadonlyLayer()
        {

            if (IsReadonlyLayerSelected && selectedLayer != null)
            {
                if (this.navMetaData != null)
                {
                    this.dataCheckingVm.RemoveMapping(this.navMetaData.CurrentLayer);
                }

                labelListSettingDict.Remove(selectedLayer as GraphicsLayer);

                if (graphicGroupDict.ContainsKey(selectedLayer as GraphicsLayer))
                {
                    graphicGroupDict.Remove(selectedLayer as GraphicsLayer);
                }

                this.TreeViewVm.RemoveReadonlyLayer(selectedLayer);
            }
        }
        public void DeleteReadonlyLayer(Layer layer)
        {
            if (IsReadonlyLayerSelected && layer != null)
            {
                labelListSettingDict.Remove(layer as GraphicsLayer);
                this.TreeViewVm.RemoveReadonlyLayer(layer);
            }
        }

        private void InitializeGrouplayers()
        {
            this.PmssMap = new Map
            {
                InitialViewpoint = new Viewpoint(new MapPoint(106, 35, SpatialReferences.Wgs84), 40000000)
            };

            this.baseGroupLayer0 = new GroupLayer
            {
                ID = MapVmConst.BaseGroupLayer0Id,
                DisplayName = MapVmConst.BaseGroupLayer0Name
            };
            this.baseGroupLayer = new GroupLayer
            {
                ID = MapVmConst.BaseGroupLayerId,
                DisplayName = MapVmConst.BaseGroupLayerName
            };
            this.readonlyGroupLayer = new GroupLayer
            {
                ID = MapVmConst.DataGroupLayerId,
                DisplayName = MapVmConst.DataGroupLayerName
            };
            this.editableGroupLayer = new GroupLayer
            {
                ID = MapVmConst.EditableGroupLayerId,
                DisplayName = MapVmConst.EditableGroupLayerName
            };
            this.productGroupLayer = new GroupLayer
            {
                ID = MapVmConst.ProductGroupLayerId,
                DisplayName = MapVmConst.ProductGroupLayerName
            };

            this.PmssMap.Layers.Add(baseGroupLayer0);
            this.PmssMap.Layers.Add(productGroupLayer);
            this.PmssMap.Layers.Add(baseGroupLayer);
            this.PmssMap.Layers.Add(readonlyGroupLayer);
            this.PmssMap.Layers.Add(editableGroupLayer);

            this.TreeViewVm = new LayersTreeViewVm(baseGroupLayer0, baseGroupLayer, readonlyGroupLayer, editableGroupLayer, productGroupLayer);
        }
        private void InitializeBaseMap()
        {
            this.AddBaseLayer0("Data/BaseMaps/PMSS-Base-Asia.tpk", "基础数据");

            this.AddBaseLayer("Data/BaseMaps/ProductMask.tpk", "产品蒙版", false);
            //this.AddBaseLayer("Data/BaseMaps/PMSS-Base-District.tpk", "县", false);
            this.AddBasinBaseLayer();
            this.AddDistrictBaseLayer();
            this.AddBaseLayer("Data/BaseMaps/PMSS-Base-City.tpk", "市", false);
            this.AddBaseLayer("Data/BaseMaps/PMSS-Base-Province.tpk", "省");
            this.AddBaseLayer("Data/BaseMaps/PMSS-Base-Country.tpk", "国界");
            this.AddBaseLayer("Data/BaseMaps/Rivers2.tpk", "长江黄河");
            this.AddBaseLayer("Data/BaseMaps/PMSS-Base-River.tpk", "河流", false);
            this.AddBaseLayer("Data/BaseMaps/RiverLevel1.tpk", "一级河流", false);
            this.AddBaseLayer("Data/BaseMaps/RiverLevel2.tpk", "二级河流", false);
            this.AddBaseLayer("Data/BaseMaps/RiverLevel3.tpk", "三级河流", false);
            this.AddBaseLayer("Data/BaseMaps/RiverLevel4.tpk", "四级河流", false);
            this.AddBaseLayer("Data/BaseMaps/RiverLevel5.tpk", "五级河流", false);

            var render = new CitynamesRender();
            var renderResult = render.GenerateRenderResult("Data/BaseMaps/ProvinceNames.txt", Colors.Black);
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);
            this.provinceNameLayer = renderResult.Layer;

            renderResult = render.GenerateRenderResult("Data/BaseMaps/DistrictNames.txt", Colors.Black);
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);

            renderResult = render.GenerateRenderResult("Data/BaseMaps/FloodHazardPoint.txt", Colors.Red);
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);

            renderResult = render.GenerateRenderResult("Data/BaseMaps/MSRiverHazardPoint.txt", Colors.Red);
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);
        }
        private void LoadTopographicMap()
        {
            //baseGroupLayer.ChildLayers.RemoveAt(0);
            this.TreeViewVm.RemoveFirstBaseLayer0();
            Layer layer;
            if (IsTopographicMapShown)
                layer = new ArcGISTiledMapServiceLayer(new Uri("http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer"));
            else
                layer = new ArcGISLocalTiledLayer("Data/BaseMaps/PMSS-Base-Asia.tpk");
            layer.DisplayName = "基础数据";
            //baseGroupLayer.ChildLayers.Insert(0, layer);
            this.TreeViewVm.AddFirstBaseLayer0(layer);
        }
        private void AddBaseLayer0(string path, string layerName, bool isVisible = true)
        {
            var layer = new ArcGISLocalTiledLayer(path)
            {
                DisplayName = layerName,
                IsVisible = isVisible
            };
            //baseGroupLayer.ChildLayers.Add(layer);
            this.TreeViewVm.AddBaseLayer0(layer);
        }

        private void AddBaseLayer(string path, string layerName, bool isVisible = true)
        {
            var layer = new ArcGISLocalTiledLayer(path)
            {
                DisplayName = layerName,
                IsVisible = isVisible
            };
            //baseGroupLayer.ChildLayers.Add(layer);
            this.TreeViewVm.AddBaseLayer(layer);
        }
        private void AddDistrictBaseLayer()
        {
            string path = "Data/BaseMaps/PMSS-Base-District.tpk";
            string layerName = "县";
            bool isVisible = false;
            var layer = new ArcGISLocalTiledLayer(path)
            {
                DisplayName = layerName,
                IsVisible = isVisible
            };
            this.TreeViewVm.AddBaseLayer(layer);

            this.districtLayer = layer;
        }
        private void AddBasinBaseLayer()
        {
            string path = "Data/BaseMaps/RiverRegion.tpk";
            string layerName = "流域";
            bool isVisible = false;
            var layer = new ArcGISLocalTiledLayer(path)
            {
                DisplayName = layerName,
                IsVisible = isVisible
            };
            this.TreeViewVm.AddBaseLayer(layer);

            this.basinLayer = layer;
        }
        private void AddEditableLayer()
        {
            var layer = new GraphicsLayer()
            {
                DisplayName = $"新建交互图层{number++}"
            };
            // Add Label
            var labelClass = new AttributeLabelClass
            {
                TextExpression = $"[{Diamond14Attributes.LineValue}]",
                LabelPlacement = LabelPlacement.LineAboveStart,
                LabelPosition = LabelPosition.FixedPositionWithOverlaps,
                IsVisible = true,
                Symbol = new TextSymbol
                {
                    Color = Colors.Blue
                }
            };
            layer.Labeling.LabelClasses.Add(labelClass);

            //editableGroupLayer.ChildLayers.Add(layer);
            var renderResult = new RenderResult
            {
                Layer = layer,
                Type = DiamondType.Diamond14
            };
            // Ask user about the level setting
            //var dialog = new ProductChooseLevel();
            //dialog.ShowDialog();
            //renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, dialog.LevelSetting);
            renderResult.Miscellaneous.Add(Diamond14Attributes.OpenMode, Diamond14Attributes.OpenModeNew);
            //renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, dialog.Type);

            //diamond14SettingDic.Add(layer, renderResult);
            this.AddEditableLayer(renderResult);
        }
        public void AddEditableLayer(RenderResult renderResult)
        {
            if (renderResult != null)
            {
                if (!renderResult.Miscellaneous.Keys.Contains(Diamond14Attributes.LevelSetting) || !renderResult.Miscellaneous.Keys.Contains(Diamond14Attributes.ProductType))
                {
                    // Ask use about the level setting
                    var productDialog = new ProductChooseLevel();
                    productDialog.ShowDialog();
                    renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, productDialog.LevelSetting);
                    renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, productDialog.Type);
                }

                //editableGroupLayer.ChildLayers.Add(renderResult.Layer);
                this.TreeViewVm.AddEditableLayer(renderResult.Layer);
                diamond14SettingDic.Add(renderResult.Layer, renderResult);
            }
        }
        private void OpenMicapsFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            string path = string.Empty;

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    path = dialog.FileName;
                    //var layer = GraphicsLayerFactory.GenerateGraphicsLayerFromFile(path);
                    //this.AddDataLayer(layer);
                    var renderResult = GraphicsLayerFactory.GenerateRenderResultFromFile(path);
                    if (renderResult.Type == DiamondType.Diamond14)
                    {
                        // Ask use about the level setting
                        //var productDialog = new ProductChooseLevel();
                        //productDialog.ShowDialog();
                        //renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, productDialog.LevelSetting);
                        //renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, productDialog.Type);

                        this.AddEditableLayer(renderResult);
                    }
                    else
                    {
                        this.AddReadonlyLayer(renderResult);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        //private void SelectedLayperChanged(Layer layer)
        //{
        //    if (layer != null)
        //    {
        //        this.selectedLayer = layer;
        //        this.IsReadonlyLayerSelected = false;
        //        this.IsEditableLayerSelected = false;
        //        this.IsProductLayerSelected = false;
        //        this.LabelListOfSelectedLayer = null;

        //        this.navMetaData = null;
        //        this.HasLastFile = false;
        //        this.HasNextFile = false;
        //        if (this.dataCheckingVm.NavDict.Keys.Contains(layer))
        //        {
        //            this.navMetaData = this.dataCheckingVm.NavDict[layer];
        //            this.HasLastFile = this.navMetaData.HasLastFile;
        //            this.HasNextFile = this.navMetaData.HasNextFile;
        //        }

        //        if (readonlyGroupLayer.ChildLayers.Contains(layer))
        //        {
        //            this.IsReadonlyLayerSelected = true;
        //            this.LabelListOfSelectedLayer = this.labelListSettingDict[layer as GraphicsLayer];
        //        }
        //        else if (editableGroupLayer.ChildLayers.Contains(layer))
        //        {
        //            this.IsEditableLayerSelected = true;
        //        }
        //        else if (productGroupLayer.ChildLayers.Contains(layer))
        //        {
        //            this.IsProductLayerSelected = true;
        //        }
        //    }
        //}
        private void SelectedLayperChanged(LayersTreeViewItem item)
        {
            if (item != null)
            {
                var layer = item.RelatedLayer;

                this.selectedLayer = layer;
                this.IsReadonlyLayerSelected = false;
                this.IsEditableLayerSelected = false;
                this.IsProductLayerSelected = false;
                this.LabelListOfSelectedLayer = null;
                this.GraphicGroupOfSelectedLayer = null;

                this.navMetaData = null;
                this.HasLastFile = false;
                this.HasNextFile = false;
                if (this.dataCheckingVm.NavDict.Keys.Contains(layer))
                {
                    this.navMetaData = this.dataCheckingVm.NavDict[layer];
                    this.HasLastFile = this.navMetaData.HasLastFile;
                    this.HasNextFile = this.navMetaData.HasNextFile;
                }

                if (readonlyGroupLayer.ChildLayers.Contains(layer))
                {
                    this.IsReadonlyLayerSelected = true;
                    this.LabelListOfSelectedLayer = this.labelListSettingDict[layer as GraphicsLayer];

                    if (this.graphicGroupDict.ContainsKey(layer as GraphicsLayer))
                    {
                        this.GraphicGroupOfSelectedLayer = this.graphicGroupDict[layer as GraphicsLayer];
                    }
                }
                else if (editableGroupLayer.ChildLayers.Contains(layer))
                {
                    this.IsEditableLayerSelected = true;
                }
                else if (productGroupLayer.ChildLayers.Contains(layer))
                {
                    this.IsProductLayerSelected = true;
                }
            }
        }

        private void DeleteEditableLayer()
        {
            if (IsEditableLayerSelected && selectedLayer != null)
            {
                if (this.drawOptionsVm != null && this.drawOptionsVm.MapEditor != null && !this.drawOptionsVm.MapEditor.IsActive)
                {
                    if (this.navMetaData != null)
                    {
                        this.dataCheckingVm.RemoveMapping(this.navMetaData.CurrentLayer);
                    }
                    diamond14SettingDic.Remove(selectedLayer as GraphicsLayer);
                    //editableGroupLayer.ChildLayers.Remove(selectedLayer);
                    this.TreeViewVm.RemoveEditableLayer(selectedLayer);
                }
                else
                {
                    MessageBox.Show("处于编辑状态，无法删除！");
                }
            }
        }
        public void DeleteEditableLayer(Layer layer)
        {
            if (IsEditableLayerSelected && layer != null)
            {
                if (this.drawOptionsVm != null && this.drawOptionsVm.MapEditor != null && !this.drawOptionsVm.MapEditor.IsActive)
                {
                    diamond14SettingDic.Remove(selectedLayer as GraphicsLayer);
                    //editableGroupLayer.ChildLayers.Remove(selectedLayer);
                    this.TreeViewVm.RemoveEditableLayer(selectedLayer);
                }
                else
                {
                    MessageBox.Show("处于编辑状态，无法删除！");
                }
            }
        }
        private void DeleteProductLayer()
        {
            if (IsProductLayerSelected && selectedLayer != null)
            {
                ProductSaveSettingDic.Remove(selectedLayer as GraphicsLayer);
                //productGroupLayer.ChildLayers.Remove(selectedLayer);
                this.TreeViewVm.RemoveProductLayer(selectedLayer);
            }
        }
        private void SaveToDiamond14File()
        {
            if (!IsEditableLayerSelected)
                return;
            if (drawOptionsVm.MapEditor.IsActive)
            {
                MessageBox.Show("请结束图层编辑");
            }

            var dialog = new SaveFileDialog();
            string path = string.Empty;

            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;

                try
                {
                    Diamond14Writer writer = new Diamond14Writer();
                    writer.WriteFile(path, diamond14SettingDic[selectedLayer as GraphicsLayer]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void PreviewProductLayerHelper(ProductRegionEnum region)
        {
            if (!IsEditableLayerSelected)
                return;
            if (drawOptionsVm.MapEditor.IsActive)
            {
                MessageBox.Show("请结束图层编辑");
            }

            try
            {
                var generator = new ProductGenerator();
                //var setting = LevelValueManager.Settings[LevelValueManager.LevelNameFlood];
                //selectedLayer.IsVisible = false;
                var renderResult = diamond14SettingDic[selectedLayer as GraphicsLayer];
                var layer = generator.Generate(renderResult);
                //productGroupLayer.ChildLayers.Add(layer);
                this.TreeViewVm.AddProductLayer(layer);
                var saveOption = new ProductSaveOption
                {
                    ProductType = (ProductTypeEnum)renderResult.Miscellaneous[Diamond14Attributes.ProductType],
                    ProductRegion = region
                };
                ProductSaveSettingDic.Add(layer, saveOption);

                // Set editable layer's IsVisible to false, select the new product layer
                this.TreeViewVm.SetVisibility(selectedLayer, false);
                this.TreeViewVm.SelectLayer(layer);
                if (saveOption.ProductType == ProductTypeEnum.River)
                {
                    this.TreeViewVm.SetVisibility(this.districtLayer, false);
                    this.TreeViewVm.SetVisibility(this.basinLayer, true);
                    this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                }
                else
                {
                    this.TreeViewVm.SetVisibility(this.districtLayer, true);
                    this.TreeViewVm.SetVisibility(this.basinLayer, false);
                    this.TreeViewVm.SetVisibility(this.provinceNameLayer, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void PreviewProductLayer()
        {
            this.PreviewProductLayerHelper(ProductRegionEnum.Region);
        }
        private void ZoomAndPreviewProductLayer()
        {
            try
            {
                if (this.PmssMapView != null)
                {
                    //this.PmssMapView.SetView(new Viewpoint(new MapPoint(106, 38, SpatialReferences.Wgs84), 30000000));
                    ProductGenerator.ZoomMapView(this.PmssMapView);
                }
                this.PreviewProductLayerHelper(ProductRegionEnum.Country);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ExportProductLayer()
        {
            try
            {
                if (!IsProductLayerSelected)
                    return;
                var saveOption = this.ProductSaveSettingDic[selectedLayer as GraphicsLayer];

                // dialog
                var dialog = new ProductChooseSaveOption(saveOption);
                if (dialog.ShowDialog() == true)
                {
                    var folderPath = Properties.Settings.Default.ProductExportFolder;
                    var imgWriter = new ImageWriter(saveOption, dialog.ProductTitle, dialog.ProductStartDate, dialog.ProductEndDate, dialog.ProductPublishDate, dialog.TemplateType);
                    imgWriter.Write(folderPath, PmssMapView);
                    MessageBox.Show("已保存至" + folderPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void ExportScreenShot()
        {
            try
            {
                // dialog
                var productDialog = new ProductChooseLevel();
                productDialog.AddUnknownOption();
                productDialog.ShowDialog();

                var saveOption = new ProductSaveOption
                {
                    //ProductType = ProductTypeEnum.Unknown,
                    ProductType = productDialog.Type,
                    ProductRegion = ProductRegionEnum.Region
                };
                var dialog = new ProductChooseSaveOption(saveOption);
                if (dialog.ShowDialog() == true)
                {
                    var folderPath = Properties.Settings.Default.ProductExportFolder;
                    var imgWriter = new ImageWriter(saveOption, dialog.ProductTitle, dialog.ProductStartDate, dialog.ProductEndDate, dialog.ProductPublishDate);

                    bool drawImage = false;
                    if (saveOption.ProductType != ProductTypeEnum.Unknown)
                    {
                        drawImage = true;
                    }

                    imgWriter.Write(folderPath, PmssMapView, drawImage);
                    MessageBox.Show("已保存至" + folderPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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

                    this.AddEditableLayer(renderResult);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void ShowEditFlood()
        {
            ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
        }
        private void CheckOut()
        {
            try
            {
                if (!IsProductLayerSelected)
                    return;
                var saveOption = this.ProductSaveSettingDic[selectedLayer as GraphicsLayer];


                var foldersPath = Properties.Settings.Default.ProductExportFolder;
                var imgWriter = new ImageWriter(saveOption, this.checkTitle, this.checkLine);
                imgWriter.WriteCheckProduct(foldersPath, PmssMapView);
                MessageBox.Show("已保存至" + foldersPath);
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        private void CheckFlood(string filename, string settingName, ProductTypeEnum productType, out List<double> latList, out List<double> lonList)
        {
            /*string folderPath = Properties.Settings.Default.EditFloodFolder;
            string settingName = LevelValueManager.LevelNameFlood;
            ProductTypeEnum productType = ProductTypeEnum.Flood;
            //ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;

            if (dialog.ShowDialog() == true)
            {*/
            latList = new List<double>();
            lonList = new List<double>();
            try
                {
                    //string filename = dialog.FileName;

                    var reader = new Diamond14Reader(filename);
                    var entity = reader.RetrieveEntity();
                    List<List<GeoCoordinate>> level1List = new List<List<GeoCoordinate>>();
                    List<List<GeoCoordinate>> level2List = new List<List<GeoCoordinate>>();
                    List<List<GeoCoordinate>> level3List = new List<List<GeoCoordinate>>();
                    List<List<GeoCoordinate>> level4List = new List<List<GeoCoordinate>>();
                    foreach (Diamond14EntityLine line in entity.Contours)
                    {
                        if (line.LabelValue == 1)
                        {
                            List<GeoCoordinate> lg = new List<GeoCoordinate>();
                            foreach (Diamond14EntityLineItem item in line.Items)
                            {
                                GeoCoordinate gd = new GeoCoordinate();
                                gd.Lat = item.Latitude;
                                gd.Lon = item.Longitude;
                                lg.Add(gd);
                            }
                            level1List.Add(lg);
                        }
                        else if (line.LabelValue == 2)
                        {
                            List<GeoCoordinate> lg = new List<GeoCoordinate>();
                            foreach (Diamond14EntityLineItem item in line.Items)
                            {
                                GeoCoordinate gd = new GeoCoordinate();
                                gd.Lat = item.Latitude;
                                gd.Lon = item.Longitude;
                                lg.Add(gd);
                            }
                            level2List.Add(lg);
                        }
                        else if (line.LabelValue == 3)
                        {
                            List<GeoCoordinate> lg = new List<GeoCoordinate>();
                            foreach (Diamond14EntityLineItem item in line.Items)
                            {
                                GeoCoordinate gd = new GeoCoordinate();
                                gd.Lat = item.Latitude;
                                gd.Lon = item.Longitude;
                                lg.Add(gd);
                            }
                            level3List.Add(lg);
                        }
                        else if (line.LabelValue == 4)
                        {
                            List<GeoCoordinate> lg = new List<GeoCoordinate>();
                            foreach (Diamond14EntityLineItem item in line.Items)
                            {
                                GeoCoordinate gd = new GeoCoordinate();
                                gd.Lat = item.Latitude;
                                gd.Lon = item.Longitude;
                                lg.Add(gd);
                            }
                            level4List.Add(lg);
                        }
                    }

                    string file = Path.GetFileName(filename);
                    if (!file.StartsWith("sh"))
                    {
                        throw new Exception("文件头错误.");
                    }

                    DateTime timeFrom = new DateTime();
                    string timeTitle;
                    try
                    {
                        timeTitle = file.Substring(2, 14);
                        string time = file.Substring(2, 10);
                        timeFrom = DateTime.ParseExact(time, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception)
                    {
                        throw new Exception("文件名解析错误.");
                    }

                    DateTime timeTo = timeFrom.AddDays(1);
                this.worker.ReportProgress(40);

                List<GeoCoordinate> chekcList = getAllPoints(timeFrom, timeTo, "山洪");
                this.worker.ReportProgress(60);
                int hitLevel1 = 0;
                    int hitLevel2 = 0;
                    int hitLevel3 = 0;
                    int hitLevel4 = 0;                    
                    int missLevel1 = 0;
                    int missLevel2 = 0;
                    int missLevel3 = 0;
                    int missLevel4 = 0;
                    int fallLevel1 = 0;
                    int fallLevel2 = 0;
                    int fallLevel3 = 0;
                    int fallLevel4 = 0;
                    double TSR1 = 0;
                    double PO1 = 0;
                    double FAR1 = 0;
                    double TS1 = 0;
                    double TSR2 = 0;
                    double PO2 = 0;
                    double FAR2 = 0;
                    double TS2 = 0;
                    double TSR3 = 0;
                    double PO3 = 0;
                    double FAR3 = 0;
                    double TS3 = 0;
                    double TSR4 = 0;
                    double PO4 = 0;
                    double FAR4 = 0;
                    double TS4 = 0;
                    calTS(chekcList, level1List, out hitLevel1, out missLevel1, out fallLevel1, out TSR1, out PO1, out FAR1, out TS1);
                    calTS(chekcList, level2List, out hitLevel2, out missLevel2, out fallLevel2, out TSR2, out PO2, out FAR2, out TS2);
                    calTS(chekcList, level3List, out hitLevel3, out missLevel3, out fallLevel3, out TSR3, out PO3, out FAR3, out TS3);
                    calTS(chekcList, level4List, out hitLevel4, out missLevel4, out fallLevel4, out TSR4, out PO4, out FAR4, out TS4);

                    //var renderResult = GraphicsLayerFactory.GenerateRenderResultFromFile(filename);
                    
                    foreach (GeoCoordinate gd in chekcList)
                    {
                        latList.Add(gd.Lat);
                        lonList.Add(gd.Lon);
                    }

                    

                //this.AddEditableLayer(renderResult);
                string title = "山洪检验" + timeTitle;
                string line1 = "";
                if (level1List.Count > 0)
                {
                    line1 += "RED  HIT:" + hitLevel1 + " " + "MISS:" + missLevel1 + " " + "FALL:" + fallLevel1
                        + " " + "TSR:" + TSR1.ToString("P1") + " " + "PO:" + PO1.ToString("P1") + " " + "FAR:" + FAR1.ToString("P1") + " " + "TS:" + TS1.ToString("F2") + "\n";
                }
                if (level2List.Count > 0)
                {
                    line1 = line1 + "ORANGE  HIT:" + hitLevel2 + " " + "MISS:" + missLevel2 + " " + "FALL:" + fallLevel2
                    + " " + "TSR:" + TSR2.ToString("P1") + " " + "PO:" + PO2.ToString("P1") + " " + "FAR:" + FAR2.ToString("P1") + " " + "TS:" + TS2.ToString("F2") + "\n";
                }
                if (level3List.Count > 0)
                {
                    line1 = line1 + "YELLOW  HIT:" + hitLevel3 + " " + "MISS:" + missLevel3 + " " + "FALL:" + fallLevel3
                    + " " + "TSR:" + TSR3.ToString("P1") + " " + "PO:" + PO3.ToString("P1") + " " + "FAR:" + FAR3.ToString("P1") + " " + "TS:" + TS3.ToString("F2") + "\n";
                }
                if (level4List.Count > 0)
                {
                    line1 = line1 + "BLUE  HIT:" + hitLevel4 + " " + "MISS:" + missLevel4 + " " + "FALL:" + fallLevel4
                    + " " + "TSR:" + TSR4.ToString("P1") + " " + "PO:" + PO4.ToString("P1") + " " + "FAR:" + FAR4.ToString("P1") + " " + "TS:" + TS4.ToString("F2") + "\n";
                }
                this.checkTitle = title;
                this.checkLine = line1;

            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            return;
           
            /*}*/

        }

        private void CheckDisaster(string filename, string settingName, ProductTypeEnum productType, out List<double> latList, out List<double> lonList)
        {
            /*string folderPath = Properties.Settings.Default.EditFloodFolder;
            string settingName = LevelValueManager.LevelNameFlood;
            ProductTypeEnum productType = ProductTypeEnum.Flood;
            //ShowEdit(Properties.Settings.Default.EditFloodFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.Flood);
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"找不到路径{folderPath}");
                return;
            }

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = folderPath;

            if (dialog.ShowDialog() == true)
            {*/
            latList = new List<double>();
            lonList = new List<double>();
            try
            {
                //string filename = dialog.FileName;

                var reader = new Diamond14Reader(filename);
                var entity = reader.RetrieveEntity();
                List<List<GeoCoordinate>> level1List = new List<List<GeoCoordinate>>();
                List<List<GeoCoordinate>> level2List = new List<List<GeoCoordinate>>();
                List<List<GeoCoordinate>> level3List = new List<List<GeoCoordinate>>();
               // List<List<GeoCoordinate>> level4List = new List<List<GeoCoordinate>>();
                foreach (Diamond14EntityLine line in entity.Contours)
                {
                    if (line.LabelValue == 5)
                    {
                        List<GeoCoordinate> lg = new List<GeoCoordinate>();
                        foreach (Diamond14EntityLineItem item in line.Items)
                        {
                            GeoCoordinate gd = new GeoCoordinate();
                            gd.Lat = item.Latitude;
                            gd.Lon = item.Longitude;
                            lg.Add(gd);
                        }
                        level1List.Add(lg);
                    }
                    else if (line.LabelValue == 4)
                    {
                        List<GeoCoordinate> lg = new List<GeoCoordinate>();
                        foreach (Diamond14EntityLineItem item in line.Items)
                        {
                            GeoCoordinate gd = new GeoCoordinate();
                            gd.Lat = item.Latitude;
                            gd.Lon = item.Longitude;
                            lg.Add(gd);
                        }
                        level2List.Add(lg);
                    }
                    else if (line.LabelValue == 3)
                    {
                        List<GeoCoordinate> lg = new List<GeoCoordinate>();
                        foreach (Diamond14EntityLineItem item in line.Items)
                        {
                            GeoCoordinate gd = new GeoCoordinate();
                            gd.Lat = item.Latitude;
                            gd.Lon = item.Longitude;
                            lg.Add(gd);
                        }
                        level3List.Add(lg);
                    }
                }

                string file = Path.GetFileName(filename);
                if (!file.StartsWith("hv"))
                {
                    throw new Exception("文件头错误.");
                }

                DateTime timeFrom = new DateTime();
                string timeTitle;
                try
                {
                    timeTitle = "20" + file.Substring(2, 12);
                    string time = "20" + file.Substring(2, 8);
                    timeFrom = DateTime.ParseExact(time, "yyyyMMddHH", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    throw new Exception("文件名解析错误.");
                }

                DateTime timeTo = timeFrom.AddDays(1);
                this.disasterworker.ReportProgress(40);

                List<GeoCoordinate> chekcList = getAllPoints(timeFrom, timeTo, "地质灾害");
                this.disasterworker.ReportProgress(60);
                int hitLevel1 = 0;
                int hitLevel2 = 0;
                int hitLevel3 = 0;
                int missLevel1 = 0;
                int missLevel2 = 0;
                int missLevel3 = 0;
                int fallLevel1 = 0;
                int fallLevel2 = 0;
                int fallLevel3 = 0;
                double TSR1 = 0;
                double PO1 = 0;
                double FAR1 = 0;
                double TS1 = 0;
                double TSR2 = 0;
                double PO2 = 0;
                double FAR2 = 0;
                double TS2 = 0;
                double TSR3 = 0;
                double PO3 = 0;
                double FAR3 = 0;
                double TS3 = 0;
                calTS(chekcList, level1List, out hitLevel1, out missLevel1, out fallLevel1, out TSR1, out PO1, out FAR1, out TS1);
                calTS(chekcList, level2List, out hitLevel2, out missLevel2, out fallLevel2, out TSR2, out PO2, out FAR2, out TS2);
                calTS(chekcList, level3List, out hitLevel3, out missLevel3, out fallLevel3, out TSR3, out PO3, out FAR3, out TS3);

                //var renderResult = GraphicsLayerFactory.GenerateRenderResultFromFile(filename);

                foreach (GeoCoordinate gd in chekcList)
                {
                    latList.Add(gd.Lat);
                    lonList.Add(gd.Lon);
                }



                //this.AddEditableLayer(renderResult);
                string title = "地质灾害检验" + timeTitle;
                string line1 = "";
                if (level1List.Count > 0)
                {
                    line1 += "RED  HIT:" + hitLevel1 + " " + "MISS:" + missLevel1 + " " + "FALL:" + fallLevel1
                        + " " + "TSR:" + TSR1.ToString("P1") + " " + "PO:" + PO1.ToString("P1") + " " + "FAR:" + FAR1.ToString("P1") + " " + "TS:" + TS1.ToString("F2") + "\n";
                }
                if (level2List.Count > 0)
                {
                    line1 = line1 + "ORANGE  HIT:" + hitLevel2 + " " + "MISS:" + missLevel2 + " " + "FALL:" + fallLevel2
                    + " " + "TSR:" + TSR2.ToString("P1") + " " + "PO:" + PO2.ToString("P1") + " " + "FAR:" + FAR2.ToString("P1") + " " + "TS:" + TS2.ToString("F2") + "\n";
                }
                if (level3List.Count > 0)
                {
                    line1 = line1 + "YELLOW  HIT:" + hitLevel3 + " " + "MISS:" + missLevel3 + " " + "FALL:" + fallLevel3
                    + " " + "TSR:" + TSR3.ToString("P1") + " " + "PO:" + PO3.ToString("P1") + " " + "FAR:" + FAR3.ToString("P1") + " " + "TS:" + TS3.ToString("F2") + "\n";
                }
                this.checkTitle = title;
                this.checkLine = line1;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;

            /*}*/

        }

        private string checkTitle;
        private string checkLine;

        private void ShowEditZilao()
        {
            ShowEdit(Properties.Settings.Default.EditZilaoFolder, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Zilao);
        }
        private void ShowEditDisaster()
        {
            ShowEdit(Properties.Settings.Default.EditDisasterFolder, LevelValueManager.LevelNameDisaster, ProductTypeEnum.Disaster);
        }
        private void ShowEditRiver()
        {
            ShowEdit(Properties.Settings.Default.EditRiverFolder, LevelValueManager.LevelNameFlood, ProductTypeEnum.River);
        }

        private GroupLayer baseGroupLayer0;
        private GroupLayer baseGroupLayer;
        private GroupLayer readonlyGroupLayer;
        private GroupLayer editableGroupLayer;
        private GroupLayer productGroupLayer;
        private Layer selectedLayer;
        private bool isReadonlyLayerSelected = false;
        private bool isEditableLayerSelected = false;
        private bool isProductLayerSelected = false;
        private Dictionary<GraphicsLayer, LabelListSettingVm> labelListSettingDict = new Dictionary<GraphicsLayer, LabelListSettingVm>();
        private Dictionary<GraphicsLayer, RenderResult> diamond14SettingDic = new Dictionary<GraphicsLayer, RenderResult>();
        private Dictionary<GraphicsLayer, ProductSaveOption> ProductSaveSettingDic = new Dictionary<GraphicsLayer, ProductSaveOption>();
        private Dictionary<GraphicsLayer, GraphicGroup> graphicGroupDict = new Dictionary<GraphicsLayer, GraphicGroup>();
        private LabelListSettingVm labelListOfSelectedLayer;
        private GraphicGroup graphicGroupOfSelectedLayer;
        private int number = 1;
        private DrawOptionsGroupVm drawOptionsVm;
        private DataCheckingVm dataCheckingVm;
        private DataMonitoringVm dataMonitoringVm;
        private Layer districtLayer;
        private Layer basinLayer;
        private Layer provinceNameLayer;

        public bool HasNextFile
        {
            get
            {
                return this.hasNextFile;
            }
            set
            {
                this.hasNextFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasNextFile)));
                }
            }
        }
        public bool HasLastFile
        {
            get
            {
                return this.hasLastFile;
            }
            set
            {
                this.hasLastFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasLastFile)));
                }
            }
        }
        private NavMetaData navMetaData;
        private bool hasNextFile = false;
        private bool hasLastFile = false;
        private void ShowNextFile()
        {
            try
            {
                this.navMetaData.ShowNextFile();
            }
            catch (Exception ex)
            {
                this.HasNextFile = this.navMetaData.HasNextFile;
                string nextFile = "";
                if (this.HasNextFile)
                {
                    nextFile = " 自动跳至下一文件";
                }
                MessageBox.Show("当前文件数据错误：" + ex.Message + nextFile);
                if (this.HasNextFile)
                {
                    ShowNextFile();
                }
            }
        }
        private void ShowLastFile()
        {
            try
            {
                this.navMetaData.ShowLastFile();
            }
            catch (Exception ex)
            {
                this.HasLastFile = this.navMetaData.HasLastFile;
                string lastFile = "";
                if (this.HasLastFile)
                {
                    lastFile = " 自动跳至上一文件";
                }
                MessageBox.Show("当前文件数据错误：" + ex.Message + lastFile);
                if (this.HasLastFile)
                {
                    ShowLastFile();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class GeoCoordinate
        {
            private double lat;
            public double Lat
            {
                get
                {
                    return lat;
                }
                set
                {
                    lat = value;
                }
            }

            private double lon;
            public double Lon
            {
                get
                {
                    return lon;
                }
                set
                {
                    lon = value;
                }
            }

            public GeoCoordinate()
            {
                this.Lat = 0;
                this.lon = 0;
            }

            public GeoCoordinate(double lat, double lon)
            {
                this.Lat = lat;
                this.lon = lon;
            }
        }

        private List<GeoCoordinate> getAllPoints(DateTime from, DateTime to, string type)
        {
            List<String> allUrls = new List<string>();
            int pageCount = 1;
            bool hasNext = true;
            while (hasNext)
            {
                List<String> urls = new List<string>();
                urls = CrawlAllDisaster(from, to, "0", pageCount);
                pageCount++;
                if (urls.Count > 0)
                {
                    allUrls.AddRange(urls);
                }
                else
                {
                    hasNext = false;
                }
            }

            List<GeoCoordinate> coodList = new List<GeoCoordinate>();
            //int count = 0;

            foreach (String url in allUrls)
            {
                GeoCoordinate cood = new GeoCoordinate();
                bool bSh = Parse(url, type, out cood);
                if (bSh)
                {
                    //count++;
                    coodList.Add(cood);
                    Debug.WriteLine(url);
                    //Console.WriteLine(url);
                   // Console.WriteLine("Lat:" + cood.Lat + " ,Lon:" + cood.Lon);
                }
            }

            return coodList;
        }

        private void calTS(List<GeoCoordinate> checkPointList, List<List<GeoCoordinate>> ringsList, out int hit, out int miss, out int fall,
            out double TSR, out double PO, out double FAR, out double TS)
        {
            var allStationList = new List<GeoCoordinate>
            {
                new GeoCoordinate{Lat=34.27     , Lon=116.20},
new GeoCoordinate{Lat=34.11     , Lon=116.58},
new GeoCoordinate{Lat=33.47     , Lon=115.44},
new GeoCoordinate{Lat=33.01     , Lon=115.17},
new GeoCoordinate{Lat=33.20 , Lon=115.22},
new GeoCoordinate{Lat=33.08     , Lon=115.39},
new GeoCoordinate{Lat=30.44     , Lon=116.27},
new GeoCoordinate{Lat=33.56     , Lon=116.45},
new GeoCoordinate{Lat=33.29 , Lon=116.11},
new GeoCoordinate{Lat=34.02     , Lon=116.52},
new GeoCoordinate{Lat=33.08     , Lon=116.12},
new GeoCoordinate{Lat=33.16     , Lon=116.31},
new GeoCoordinate{Lat=33.38     , Lon=116.59},
new GeoCoordinate{Lat=33.35 , Lon=117.31},
new GeoCoordinate{Lat=33.28     , Lon=117.52},
new GeoCoordinate{Lat=32.59     , Lon=117.12},
new GeoCoordinate{Lat=33.20 , Lon=117.21},
new GeoCoordinate{Lat=33.09 , Lon=117.50},
new GeoCoordinate{Lat=32.40 , Lon=115.34},
new GeoCoordinate{Lat=32.52     , Lon=115.44},
new GeoCoordinate{Lat=32.39     , Lon=116.14},
new GeoCoordinate{Lat=32.43 , Lon=116.46},
new GeoCoordinate{Lat=32.22     , Lon=116.18},
new GeoCoordinate{Lat=32.26 , Lon=116.47},
new GeoCoordinate{Lat=32.28     , Lon=117.09},
new GeoCoordinate{Lat=32.51     , Lon=117.18},
new GeoCoordinate{Lat=32.51     , Lon=117.33},
new GeoCoordinate{Lat=32.48 , Lon=117.59},
new GeoCoordinate{Lat=32.39     , Lon=117.01},
new GeoCoordinate{Lat=32.32     , Lon=117.40},
new GeoCoordinate{Lat=32.07     , Lon=118.17},
new GeoCoordinate{Lat=32.28     , Lon=118.25},
new GeoCoordinate{Lat=32.21     , Lon=118.15},
new GeoCoordinate{Lat=32.41     , Lon=119.01},
new GeoCoordinate{Lat=31.41     , Lon=115.53},
new GeoCoordinate{Lat=31.44     , Lon=116.30},
new GeoCoordinate{Lat=31.24     , Lon=116.19},
new GeoCoordinate{Lat=31.28     , Lon=116.55},
new GeoCoordinate{Lat=30.52     , Lon=116.22},
new GeoCoordinate{Lat=31.04     , Lon=116.57},
new GeoCoordinate{Lat=31.44     , Lon=117.08},
new GeoCoordinate{Lat=31.47     , Lon=117.18},
new GeoCoordinate{Lat=31.53     , Lon=117.28},
new GeoCoordinate{Lat=31.35 , Lon=117.50},
new GeoCoordinate{Lat=31.16 , Lon=117.17},
new GeoCoordinate{Lat=31.20     , Lon=117.52},
new GeoCoordinate{Lat=31.45     , Lon=118.05},
new GeoCoordinate{Lat=31.44     , Lon=118.22},
new GeoCoordinate{Lat=31.23     , Lon=118.25},
new GeoCoordinate{Lat=31.33     , Lon=118.34},
new GeoCoordinate{Lat=31.42     , Lon=118.34},
new GeoCoordinate{Lat=31.05     , Lon=118.11},
new GeoCoordinate{Lat=31.07     , Lon=118.36},
new GeoCoordinate{Lat=30.26     , Lon=116.19},
new GeoCoordinate{Lat=30.40 , Lon=116.33},
new GeoCoordinate{Lat=30.45     , Lon=116.49},
new GeoCoordinate{Lat=30.10     , Lon=116.08},
new GeoCoordinate{Lat=30.08     , Lon=116.40},
new GeoCoordinate{Lat=30.06     , Lon=117.01},
new GeoCoordinate{Lat=30.43     , Lon=117.14},
new GeoCoordinate{Lat=30.38     , Lon=117.52},
new GeoCoordinate{Lat=30.29     , Lon=117.47},
new GeoCoordinate{Lat=30.37     , Lon=116.58},
new GeoCoordinate{Lat=30.18     , Lon=118.08},
new GeoCoordinate{Lat=30.39 , Lon=117.30},
new GeoCoordinate{Lat=30.13     , Lon=117.30},
new GeoCoordinate{Lat=30.59 , Lon=117.51},
new GeoCoordinate{Lat=30.54 , Lon=118.19},
new GeoCoordinate{Lat=30.41     , Lon=118.24},
new GeoCoordinate{Lat=30.56     , Lon=118.45},
new GeoCoordinate{Lat=30.18     , Lon=118.32},
new GeoCoordinate{Lat=30.37     , Lon=118.59},
new GeoCoordinate{Lat=30.08     , Lon=118.09},
new GeoCoordinate{Lat=30.05     , Lon=118.35},
new GeoCoordinate{Lat=30.52     , Lon=119.25},
new GeoCoordinate{Lat=31.10 , Lon=119.11},
new GeoCoordinate{Lat=29.51     , Lon=117.43},
new GeoCoordinate{Lat=29.56     , Lon=117.56},
new GeoCoordinate{Lat=29.52     , Lon=118.26},
new GeoCoordinate{Lat=29.43     , Lon=118.17},
new GeoCoordinate{Lat=29.47     , Lon=118.11},
new GeoCoordinate{Lat=40.08     , Lon=116.37},
new GeoCoordinate{Lat=39.59     , Lon=116.17},
new GeoCoordinate{Lat=40.27     , Lon=115.58},
new GeoCoordinate{Lat=40.36     , Lon=116.08},
new GeoCoordinate{Lat=40.44     , Lon=116.38},
new GeoCoordinate{Lat=40.23     , Lon=116.52},
new GeoCoordinate{Lat=40.22     , Lon=116.38},
new GeoCoordinate{Lat=40.39     , Lon=117.07},
new GeoCoordinate{Lat=40.10     , Lon=117.07},
new GeoCoordinate{Lat=39.51     , Lon=116.45},
new GeoCoordinate{Lat=39.57     , Lon=116.30},
new GeoCoordinate{Lat=40.13     , Lon=116.13},
new GeoCoordinate{Lat=39.58     , Lon=115.41},
new GeoCoordinate{Lat=39.56 , Lon=116.06},
new GeoCoordinate{Lat=39.48     , Lon=116.28},
new GeoCoordinate{Lat=39.57     , Lon=116.12},
new GeoCoordinate{Lat=39.52     , Lon=116.15},
new GeoCoordinate{Lat=39.43     , Lon=116.21},
new GeoCoordinate{Lat=39.46 , Lon=116.12},
new GeoCoordinate{Lat=39.44     , Lon=115.44},
new GeoCoordinate{Lat=27.32 , Lon=117.19},
new GeoCoordinate{Lat=27.20     , Lon=117.28},
new GeoCoordinate{Lat=27.46     , Lon=118.02},
new GeoCoordinate{Lat=27.55     , Lon=118.32},
new GeoCoordinate{Lat=27.20     , Lon=118.07},
new GeoCoordinate{Lat=27.31 , Lon=118.45},
new GeoCoordinate{Lat=27.23 , Lon=118.49},
new GeoCoordinate{Lat=27.03     , Lon=118.19},
new GeoCoordinate{Lat=27.27     , Lon=119.25},
new GeoCoordinate{Lat=27.09     , Lon=119.21},
new GeoCoordinate{Lat=27.06     , Lon=119.39},
new GeoCoordinate{Lat=27.14     , Lon=119.53},
new GeoCoordinate{Lat=27.20     , Lon=120.12},
new GeoCoordinate{Lat=26.14     , Lon=116.38},
new GeoCoordinate{Lat=26.11     , Lon=116.49},
new GeoCoordinate{Lat=26.54     , Lon=117.10},
new GeoCoordinate{Lat=26.44     , Lon=117.28},
new GeoCoordinate{Lat=26.50     , Lon=116.51},
new GeoCoordinate{Lat=26.48     , Lon=117.48},
new GeoCoordinate{Lat=26.24     , Lon=117.09},
new GeoCoordinate{Lat=26.24     , Lon=117.48},
new GeoCoordinate{Lat=26.16     , Lon=117.37},
new GeoCoordinate{Lat=26.38     , Lon=118.10},
new GeoCoordinate{Lat=26.35     , Lon=118.44},
new GeoCoordinate{Lat=26.09 , Lon=118.09},
new GeoCoordinate{Lat=26.14     , Lon=118.51},
new GeoCoordinate{Lat=26.53     , Lon=120.01},
new GeoCoordinate{Lat=26.09     , Lon=119.09},
new GeoCoordinate{Lat=26.30     , Lon=119.33},
new GeoCoordinate{Lat=26.40     , Lon=119.31},
new GeoCoordinate{Lat=26.05     , Lon=119.17},
new GeoCoordinate{Lat=26.12     , Lon=119.33},
new GeoCoordinate{Lat=26.55     , Lon=120.14},
new GeoCoordinate{Lat=25.51     , Lon=116.22},
new GeoCoordinate{Lat=25.43 , Lon=116.44},
new GeoCoordinate{Lat=25.05     , Lon=116.06},
new GeoCoordinate{Lat=25.03     , Lon=116.25},
new GeoCoordinate{Lat=25.58     , Lon=117.21},
new GeoCoordinate{Lat=25.42     , Lon=117.50},
new GeoCoordinate{Lat=25.18     , Lon=117.25},
new GeoCoordinate{Lat=25.03     , Lon=117.01},
new GeoCoordinate{Lat=25.01     , Lon=117.32},
new GeoCoordinate{Lat=25.04     , Lon=118.12},
new GeoCoordinate{Lat=25.43     , Lon=118.06},
new GeoCoordinate{Lat=25.52     , Lon=118.56},
new GeoCoordinate{Lat=26.55     , Lon=118.59},
new GeoCoordinate{Lat=25.20     , Lon=118.16},
new GeoCoordinate{Lat=25.29     , Lon=118.14},
new GeoCoordinate{Lat=25.22     , Lon=118.42},
new GeoCoordinate{Lat=25.14     , Lon=118.59},
new GeoCoordinate{Lat=26.05     , Lon=119.20},
new GeoCoordinate{Lat=25.58     , Lon=119.30},
new GeoCoordinate{Lat=25.43     , Lon=119.23},
new GeoCoordinate{Lat=25.31     , Lon=119.47},
new GeoCoordinate{Lat=25.27     , Lon=119.00},
new GeoCoordinate{Lat=24.44     , Lon=116.43},
new GeoCoordinate{Lat=24.37     , Lon=117.45},
new GeoCoordinate{Lat=24.31     , Lon=117.23},
new GeoCoordinate{Lat=24.23     , Lon=117.18},
new GeoCoordinate{Lat=24.30     , Lon=117.39},
new GeoCoordinate{Lat=24.26     , Lon=117.50},
new GeoCoordinate{Lat=24.08     , Lon=117.35},
new GeoCoordinate{Lat=24.44     , Lon=118.09},
new GeoCoordinate{Lat=24.58     , Lon=118.22},
new GeoCoordinate{Lat=24.54     , Lon=118.55},
new GeoCoordinate{Lat=24.29     , Lon=118.04},
new GeoCoordinate{Lat=24.49     , Lon=118.34},
new GeoCoordinate{Lat=23.45 , Lon=117.08},
new GeoCoordinate{Lat=23.47     , Lon=117.30},
new GeoCoordinate{Lat=23.59     , Lon=117.22},
new GeoCoordinate{Lat=40.09 , Lon= 94.41},
new GeoCoordinate{Lat=39.46 , Lon= 98.29},
new GeoCoordinate{Lat=41.48 , Lon= 97.02},
new GeoCoordinate{Lat=40.32 , Lon= 95.47},
new GeoCoordinate{Lat=40.16 , Lon= 97.02},
new GeoCoordinate{Lat=40.18 , Lon= 99.31},
new GeoCoordinate{Lat=40.00 , Lon= 98.53},
new GeoCoordinate{Lat=39.31     , Lon= 94.52},
new GeoCoordinate{Lat=38.38 , Lon=103.05},
new GeoCoordinate{Lat=37.55 , Lon=102.40},
new GeoCoordinate{Lat=38.13 , Lon=101.56},
new GeoCoordinate{Lat=37.12 , Lon=102.52},
new GeoCoordinate{Lat=37.28     , Lon=102.54},
new GeoCoordinate{Lat=36.59 , Lon=103.11},
new GeoCoordinate{Lat=35.52     , Lon=104.09},
new GeoCoordinate{Lat=36.21     , Lon=103.57},
new GeoCoordinate{Lat=36.45     , Lon=103.15},
new GeoCoordinate{Lat=35.44 , Lon=107.38},
new GeoCoordinate{Lat=36.34 , Lon=107.18},
new GeoCoordinate{Lat=36.27 , Lon=107.59},
new GeoCoordinate{Lat=35.47     , Lon=107.59},
new GeoCoordinate{Lat=35.29 , Lon=108.24},
new GeoCoordinate{Lat=35.41 , Lon=107.11},
new GeoCoordinate{Lat=35.59     , Lon=107.54},
new GeoCoordinate{Lat=35.31     , Lon=107.55},
new GeoCoordinate{Lat=34.26     , Lon=104.01},
new GeoCoordinate{Lat=35.22     , Lon=103.52},
new GeoCoordinate{Lat=35.23     , Lon=105.01},
new GeoCoordinate{Lat=35.35     , Lon=104.37},
new GeoCoordinate{Lat=35.13     , Lon=105.14},
new GeoCoordinate{Lat=35.00     , Lon=104.39},
new GeoCoordinate{Lat=34.51     , Lon=104.28},
new GeoCoordinate{Lat=35.08     , Lon=104.12},
new GeoCoordinate{Lat=39.05 , Lon=100.17},
new GeoCoordinate{Lat=39.22 , Lon= 99.50},
new GeoCoordinate{Lat=38.48 , Lon=101.05},
new GeoCoordinate{Lat=38.26 , Lon=100.49},
new GeoCoordinate{Lat=39.09 , Lon=100.10},
new GeoCoordinate{Lat=38.50 , Lon= 99.37},
new GeoCoordinate{Lat=37.11 , Lon=104.03},
new GeoCoordinate{Lat=36.34 , Lon=104.41},
new GeoCoordinate{Lat=35.41 , Lon=105.04},
new GeoCoordinate{Lat=36.33 , Lon=104.09},
new GeoCoordinate{Lat=34.00 , Lon=102.05},
new GeoCoordinate{Lat=35.00 , Lon=102.54},
new GeoCoordinate{Lat=35.11 , Lon=102.30},
new GeoCoordinate{Lat=34.36 , Lon=102.30},
new GeoCoordinate{Lat=34.42 , Lon=103.21},
new GeoCoordinate{Lat=34.35 , Lon=103.30},
new GeoCoordinate{Lat=34.04 , Lon=103.14},
new GeoCoordinate{Lat=33.47 , Lon=104.22},
new GeoCoordinate{Lat=33.24 , Lon=104.55},
new GeoCoordinate{Lat=32.57 , Lon=104.40},
new GeoCoordinate{Lat=33.20 , Lon=105.36},
new GeoCoordinate{Lat=34.11 , Lon=105.11},
new GeoCoordinate{Lat=33.45 , Lon=105.43},
new GeoCoordinate{Lat=33.47 , Lon=106.05},
new GeoCoordinate{Lat=33.55 , Lon=106.18},
new GeoCoordinate{Lat=34.02 , Lon=104.23},
new GeoCoordinate{Lat=34.02 , Lon=105.18},
new GeoCoordinate{Lat=34.34 , Lon=105.52},
new GeoCoordinate{Lat=34.44 , Lon=104.53},
new GeoCoordinate{Lat=34.45 , Lon=105.20},
new GeoCoordinate{Lat=34.51 , Lon=105.39},
new GeoCoordinate{Lat=34.35 , Lon=105.45},
new GeoCoordinate{Lat=34.45 , Lon=106.09},
new GeoCoordinate{Lat=34.59 , Lon=106.12},
new GeoCoordinate{Lat=35.33 , Lon=106.40},
new GeoCoordinate{Lat=35.13 , Lon=106.04},
new GeoCoordinate{Lat=35.04 , Lon=107.37},
new GeoCoordinate{Lat=35.21 , Lon=107.21},
new GeoCoordinate{Lat=35.31 , Lon=105.43},
new GeoCoordinate{Lat=35.12 , Lon=106.37},
new GeoCoordinate{Lat=35.18 , Lon=107.01},
new GeoCoordinate{Lat=35.35 , Lon=103.11},
new GeoCoordinate{Lat=35.58 , Lon=103.18},
new GeoCoordinate{Lat=35.41 , Lon=103.23},
new GeoCoordinate{Lat=35.29 , Lon=103.33},
new GeoCoordinate{Lat=35.25 , Lon=103.20},
new GeoCoordinate{Lat=35.22 , Lon=103.43},
new GeoCoordinate{Lat=36.03     , Lon=103.53},
new GeoCoordinate{Lat=25.07     , Lon=113.21},
new GeoCoordinate{Lat=25.04 , Lon=113.46},
new GeoCoordinate{Lat=25.05 , Lon=114.15},
new GeoCoordinate{Lat=24.44 , Lon=112.17},
new GeoCoordinate{Lat=24.49 , Lon=112.22},
new GeoCoordinate{Lat=24.34     , Lon=112.04},
new GeoCoordinate{Lat=24.28     , Lon=112.38},
new GeoCoordinate{Lat=24.47 , Lon=113.17},
new GeoCoordinate{Lat=24.40 , Lon=113.36},
new GeoCoordinate{Lat=23.53 , Lon=113.31},
new GeoCoordinate{Lat=24.11     , Lon=113.25},
new GeoCoordinate{Lat=24.58     , Lon=114.03},
new GeoCoordinate{Lat=24.21     , Lon=114.07},
new GeoCoordinate{Lat=24.22     , Lon=114.29},
new GeoCoordinate{Lat=24.03     , Lon=114.12},
new GeoCoordinate{Lat=24.27     , Lon=114.56},
new GeoCoordinate{Lat=24.35     , Lon=115.54},
new GeoCoordinate{Lat=24.07     , Lon=115.17},
new GeoCoordinate{Lat=24.10     , Lon=115.43},
new GeoCoordinate{Lat=24.39     , Lon=116.10},
new GeoCoordinate{Lat=24.21     , Lon=116.42},
new GeoCoordinate{Lat=24.17 , Lon=116.04},
new GeoCoordinate{Lat=23.24 , Lon=111.30},
new GeoCoordinate{Lat=23.16     , Lon=111.33},
new GeoCoordinate{Lat=23.10     , Lon=111.47},
new GeoCoordinate{Lat=23.56     , Lon=112.11},
new GeoCoordinate{Lat=23.38     , Lon=112.25},
new GeoCoordinate{Lat=23.21     , Lon=112.39},
new GeoCoordinate{Lat=22.59     , Lon=112.29},
new GeoCoordinate{Lat=23.12     , Lon=112.53},
new GeoCoordinate{Lat=23.43 , Lon=113.05},
new GeoCoordinate{Lat=23.25     , Lon=113.14},
new GeoCoordinate{Lat=23.34 , Lon=113.37},
new GeoCoordinate{Lat=23.13     , Lon=113.29},
new GeoCoordinate{Lat=23.09 , Lon=113.01},
new GeoCoordinate{Lat=22.58 , Lon=113.44},
new GeoCoordinate{Lat=23.47 , Lon=114.16},
new GeoCoordinate{Lat=23.48 , Lon=114.44},
new GeoCoordinate{Lat=23.20     , Lon=113.50},
new GeoCoordinate{Lat=23.11     , Lon=114.15},
new GeoCoordinate{Lat=23.04 , Lon=114.22},
new GeoCoordinate{Lat=23.55 , Lon=115.45},
new GeoCoordinate{Lat=23.38     , Lon=115.11},
new GeoCoordinate{Lat=23.27 , Lon=115.51},
new GeoCoordinate{Lat=23.46     , Lon=116.10},
new GeoCoordinate{Lat=23.40     , Lon=116.42},
new GeoCoordinate{Lat=23.41     , Lon=116.59},
new GeoCoordinate{Lat=23.18     , Lon=116.08},
new GeoCoordinate{Lat=23.35 , Lon=116.24},
new GeoCoordinate{Lat=23.23     , Lon=116.41},
new GeoCoordinate{Lat=22.59     , Lon=116.18},
new GeoCoordinate{Lat=23.15     , Lon=116.35},
new GeoCoordinate{Lat=23.26     , Lon=116.50},
new GeoCoordinate{Lat=23.26     , Lon=117.02},
new GeoCoordinate{Lat=22.21 , Lon=110.56},
new GeoCoordinate{Lat=22.43 , Lon=111.36},
new GeoCoordinate{Lat=22.10     , Lon=111.47},
new GeoCoordinate{Lat=22.42 , Lon=112.13},
new GeoCoordinate{Lat=22.56     , Lon=112.03},
new GeoCoordinate{Lat=22.44     , Lon=112.59},
new GeoCoordinate{Lat=22.24 , Lon=112.39},
new GeoCoordinate{Lat=22.32     , Lon=113.02},
new GeoCoordinate{Lat=22.16 , Lon=112.14},
new GeoCoordinate{Lat=22.15     , Lon=112.47},
new GeoCoordinate{Lat=22.51     , Lon=113.15},
new GeoCoordinate{Lat=22.56     , Lon=113.19},
new GeoCoordinate{Lat=22.31 , Lon=113.24},
new GeoCoordinate{Lat=22.14 , Lon=113.18},
new GeoCoordinate{Lat=22.17     , Lon=113.34},
new GeoCoordinate{Lat=22.58     , Lon=114.44},
new GeoCoordinate{Lat=22.32     , Lon=114.00},
new GeoCoordinate{Lat=23.01     , Lon=115.19},
new GeoCoordinate{Lat=22.48     , Lon=115.22},
new GeoCoordinate{Lat=22.57     , Lon=115.39},
new GeoCoordinate{Lat=21.24     , Lon=110.15},
new GeoCoordinate{Lat=21.55     , Lon=110.50},
new GeoCoordinate{Lat=21.38     , Lon=110.18},
new GeoCoordinate{Lat=21.39     , Lon=110.37},
new GeoCoordinate{Lat=21.24     , Lon=110.49},
new GeoCoordinate{Lat=21.09     , Lon=110.18},
new GeoCoordinate{Lat=21.45     , Lon=110.55},
new GeoCoordinate{Lat=21.51     , Lon=111.59},
new GeoCoordinate{Lat=21.33 , Lon=110.59},
new GeoCoordinate{Lat=21.44     , Lon=112.46},
new GeoCoordinate{Lat=20.58 , Lon=110.04},
new GeoCoordinate{Lat=20.20     , Lon=110.11},
new GeoCoordinate{Lat=26.02     , Lon=110.38},
new GeoCoordinate{Lat=24.58     , Lon=107.11},
new GeoCoordinate{Lat=25.47     , Lon=109.36},
new GeoCoordinate{Lat=25.48     , Lon=110.00},
new GeoCoordinate{Lat=25.13     , Lon=109.24},
new GeoCoordinate{Lat=25.05     , Lon=109.15},
new GeoCoordinate{Lat=24.59     , Lon=110.00},
new GeoCoordinate{Lat=25.14     , Lon=110.11},
new GeoCoordinate{Lat=25.37     , Lon=110.40},
new GeoCoordinate{Lat=25.25     , Lon=110.19},
new GeoCoordinate{Lat=25.19     , Lon=110.18},
new GeoCoordinate{Lat=25.05     , Lon=110.19},
new GeoCoordinate{Lat=25.56     , Lon=111.04},
new GeoCoordinate{Lat=25.30     , Lon=111.09},
new GeoCoordinate{Lat=24.47     , Lon=105.20},
new GeoCoordinate{Lat=24.30     , Lon=105.06},
new GeoCoordinate{Lat=24.47     , Lon=106.33},
new GeoCoordinate{Lat=24.21     , Lon=106.34},
new GeoCoordinate{Lat=24.16     , Lon=106.16},
new GeoCoordinate{Lat=24.33     , Lon=107.02},
new GeoCoordinate{Lat=24.59     , Lon=107.33},
new GeoCoordinate{Lat=24.42     , Lon=108.02},
new GeoCoordinate{Lat=24.32     , Lon=107.23},
new GeoCoordinate{Lat=24.08     , Lon=107.15},
new GeoCoordinate{Lat=24.49 , Lon=108.16},
new GeoCoordinate{Lat=24.47     , Lon=108.52},
new GeoCoordinate{Lat=24.29     , Lon=108.40},
new GeoCoordinate{Lat=23.56     , Lon=108.06},
new GeoCoordinate{Lat=24.04     , Lon=108.39},
new GeoCoordinate{Lat=24.39     , Lon=109.15},
new GeoCoordinate{Lat=24.28     , Lon=109.23},
new GeoCoordinate{Lat=24.29     , Lon=109.45},
new GeoCoordinate{Lat=24.21     , Lon=109.24},
new GeoCoordinate{Lat=24.15     , Lon=109.20},
new GeoCoordinate{Lat=24.46     , Lon=110.30},
new GeoCoordinate{Lat=24.50     , Lon=110.49},
new GeoCoordinate{Lat=24.39     , Lon=110.38},
new GeoCoordinate{Lat=24.30     , Lon=110.24},
new GeoCoordinate{Lat=24.08     , Lon=110.11},
new GeoCoordinate{Lat=24.12 , Lon=110.31},
new GeoCoordinate{Lat=24.11     , Lon=110.48},
new GeoCoordinate{Lat=24.49     , Lon=111.16},
new GeoCoordinate{Lat=24.33     , Lon=111.19},
new GeoCoordinate{Lat=24.25     , Lon=111.30},
new GeoCoordinate{Lat=23.25     , Lon=105.50},
new GeoCoordinate{Lat=23.54     , Lon=106.36},
new GeoCoordinate{Lat=23.44     , Lon=106.55},
new GeoCoordinate{Lat=23.20 , Lon=106.38},
new GeoCoordinate{Lat=23.08     , Lon=106.25},
new GeoCoordinate{Lat=23.36     , Lon=107.07},
new GeoCoordinate{Lat=23.07 , Lon=107.08},
new GeoCoordinate{Lat=23.19     , Lon=107.35},
new GeoCoordinate{Lat=23.10     , Lon=107.42},
new GeoCoordinate{Lat=23.43     , Lon=108.07},
new GeoCoordinate{Lat=23.26     , Lon=108.37},
new GeoCoordinate{Lat=23.10     , Lon=108.17},
new GeoCoordinate{Lat=23.15     , Lon=108.50},
new GeoCoordinate{Lat=23.58     , Lon=109.41},
new GeoCoordinate{Lat=23.27     , Lon=109.05},
new GeoCoordinate{Lat=23.36     , Lon=109.40},
new GeoCoordinate{Lat=23.07     , Lon=109.37},
new GeoCoordinate{Lat=23.24     , Lon=110.05},
new GeoCoordinate{Lat=23.33     , Lon=110.23},
new GeoCoordinate{Lat=23.22     , Lon=110.54},
new GeoCoordinate{Lat=23.29     , Lon=111.18},
new GeoCoordinate{Lat=23.25     , Lon=111.14},
new GeoCoordinate{Lat=22.20     , Lon=106.51},
new GeoCoordinate{Lat=22.04     , Lon=106.44},
new GeoCoordinate{Lat=22.52     , Lon=107.10},
new GeoCoordinate{Lat=22.23     , Lon=107.23},
new GeoCoordinate{Lat=22.37 , Lon=107.55},
new GeoCoordinate{Lat=22.07     , Lon=107.04},
new GeoCoordinate{Lat=22.11 , Lon=108.01},
new GeoCoordinate{Lat=22.38     , Lon=108.13},
new GeoCoordinate{Lat=22.46     , Lon=108.29},
new GeoCoordinate{Lat=22.42     , Lon=109.15},
new GeoCoordinate{Lat=22.25     , Lon=109.18},
new GeoCoordinate{Lat=22.16     , Lon=109.33},
new GeoCoordinate{Lat=22.18     , Lon=109.59},
new GeoCoordinate{Lat=22.42     , Lon=110.21},
new GeoCoordinate{Lat=22.50     , Lon=110.32},
new GeoCoordinate{Lat=22.40     , Lon=110.07},
new GeoCoordinate{Lat=22.57 , Lon=111.00},
new GeoCoordinate{Lat=22.19     , Lon=110.16},
new GeoCoordinate{Lat=21.34 , Lon=107.57},
new GeoCoordinate{Lat=21.47     , Lon=108.21},
new GeoCoordinate{Lat=21.59     , Lon=108.36},
new GeoCoordinate{Lat=21.37     , Lon=108.21},
new GeoCoordinate{Lat=21.40     , Lon=109.11},
new GeoCoordinate{Lat=21.27     , Lon=109.08},
new GeoCoordinate{Lat=21.02     , Lon=109.06},
new GeoCoordinate{Lat=27.08     , Lon=104.44},
new GeoCoordinate{Lat=26.52     , Lon=104.17},
new GeoCoordinate{Lat=26.35     , Lon=104.52},
new GeoCoordinate{Lat=25.47     , Lon=104.58},
new GeoCoordinate{Lat=25.43 , Lon=104.28},
new GeoCoordinate{Lat=28.08     , Lon=106.50},
new GeoCoordinate{Lat=28.35     , Lon=105.42},
new GeoCoordinate{Lat=28.20     , Lon=106.13},
new GeoCoordinate{Lat=28.53     , Lon=107.36},
new GeoCoordinate{Lat=28.33     , Lon=107.27},
new GeoCoordinate{Lat=28.31     , Lon=107.53},
new GeoCoordinate{Lat=28.34     , Lon=108.30},
new GeoCoordinate{Lat=28.15 , Lon=108.06},
new GeoCoordinate{Lat=28.09     , Lon=109.11},
new GeoCoordinate{Lat=27.18     , Lon=105.17},
new GeoCoordinate{Lat=27.09     , Lon=105.36},
new GeoCoordinate{Lat=27.48 , Lon=106.24},
new GeoCoordinate{Lat=27.44     , Lon=106.57},
new GeoCoordinate{Lat=27.28     , Lon=106.14},
new GeoCoordinate{Lat=27.32     , Lon=106.50},
new GeoCoordinate{Lat=27.06 , Lon=106.43},
new GeoCoordinate{Lat=27.04     , Lon=106.58},
new GeoCoordinate{Lat=27.53     , Lon=107.10},
new GeoCoordinate{Lat=27.46     , Lon=107.28},
new GeoCoordinate{Lat=27.59     , Lon=107.43},
new GeoCoordinate{Lat=27.03     , Lon=107.28},
new GeoCoordinate{Lat=27.14     , Lon=107.53},
new GeoCoordinate{Lat=27.57     , Lon=108.15},
new GeoCoordinate{Lat=28.01     , Lon=108.24},
new GeoCoordinate{Lat=27.34     , Lon=108.14},
new GeoCoordinate{Lat=27.11     , Lon=108.49},
new GeoCoordinate{Lat=27.42     , Lon=108.51},
new GeoCoordinate{Lat=27.02     , Lon=108.07},
new GeoCoordinate{Lat=27.03     , Lon=108.25},
new GeoCoordinate{Lat=27.14     , Lon=108.55},
new GeoCoordinate{Lat=27.44     , Lon=109.11},
new GeoCoordinate{Lat=27.31     , Lon=109.12},
new GeoCoordinate{Lat=26.46     , Lon=105.22},
new GeoCoordinate{Lat=27.02     , Lon=106.01},
new GeoCoordinate{Lat=26.41     , Lon=105.46},
new GeoCoordinate{Lat=26.15     , Lon=105.54},
new GeoCoordinate{Lat=26.12     , Lon=105.28},
new GeoCoordinate{Lat=26.19     , Lon=105.45},
new GeoCoordinate{Lat=26.04 , Lon=105.45},
new GeoCoordinate{Lat=26.50 , Lon=106.36},
new GeoCoordinate{Lat=26.34     , Lon=106.28},
new GeoCoordinate{Lat=26.25     , Lon=106.16},
new GeoCoordinate{Lat=26.35     , Lon=106.44},
new GeoCoordinate{Lat=26.02     , Lon=106.27},
new GeoCoordinate{Lat=26.42     , Lon=107.30},
new GeoCoordinate{Lat=26.54     , Lon=107.54},
new GeoCoordinate{Lat=26.35     , Lon=107.14},
new GeoCoordinate{Lat=26.36     , Lon=107.59},
new GeoCoordinate{Lat=26.19 , Lon=107.32},
new GeoCoordinate{Lat=26.30     , Lon=107.35},
new GeoCoordinate{Lat=26.12     , Lon=107.48},
new GeoCoordinate{Lat=26.58     , Lon=108.40},
new GeoCoordinate{Lat=26.41     , Lon=108.18},
new GeoCoordinate{Lat=26.44 , Lon=108.26},
new GeoCoordinate{Lat=26.24     , Lon=108.04},
new GeoCoordinate{Lat=26.14     , Lon=109.08},
new GeoCoordinate{Lat=26.54 , Lon=109.12},
new GeoCoordinate{Lat=26.41     , Lon=109.11},
new GeoCoordinate{Lat=25.50     , Lon=105.13},
new GeoCoordinate{Lat=25.26     , Lon=105.11},
new GeoCoordinate{Lat=25.56     , Lon=105.36},
new GeoCoordinate{Lat=25.24     , Lon=105.37},
new GeoCoordinate{Lat=25.11     , Lon=106.05},
new GeoCoordinate{Lat=25.05     , Lon=104.54},
new GeoCoordinate{Lat=25.07     , Lon=105.29},
new GeoCoordinate{Lat=24.59     , Lon=105.49},
new GeoCoordinate{Lat=25.46     , Lon=106.05},
new GeoCoordinate{Lat=26.40     , Lon=106.39},
new GeoCoordinate{Lat=26.08     , Lon=106.38},
new GeoCoordinate{Lat=26.27     , Lon=106.59},
new GeoCoordinate{Lat=26.25     , Lon=106.40},
new GeoCoordinate{Lat=26.38     , Lon=106.46},
new GeoCoordinate{Lat=25.26     , Lon=106.46},
new GeoCoordinate{Lat=25.51     , Lon=107.21},
new GeoCoordinate{Lat=25.50     , Lon=107.33},
new GeoCoordinate{Lat=25.59     , Lon=107.52},
new GeoCoordinate{Lat=25.25     , Lon=107.53},
new GeoCoordinate{Lat=25.56     , Lon=108.31},
new GeoCoordinate{Lat=25.45 , Lon=108.55},
new GeoCoordinate{Lat=20.00     , Lon=110.22},
new GeoCoordinate{Lat=20.00 , Lon=110.15},
new GeoCoordinate{Lat=19.06     , Lon=108.37},
new GeoCoordinate{Lat=19.54 , Lon=109.41},
new GeoCoordinate{Lat=19.44     , Lon=110.00},
new GeoCoordinate{Lat=19.31     , Lon=109.35},
new GeoCoordinate{Lat=19.16     , Lon=109.03},
new GeoCoordinate{Lat=19.14     , Lon=109.26},
new GeoCoordinate{Lat=19.02     , Lon=109.50},
new GeoCoordinate{Lat=19.40 , Lon=110.20},
new GeoCoordinate{Lat=19.22     , Lon=110.06},
new GeoCoordinate{Lat=19.14     , Lon=110.28},
new GeoCoordinate{Lat=19.37     , Lon=110.45},
new GeoCoordinate{Lat=18.45     , Lon=109.10},
new GeoCoordinate{Lat=18.46     , Lon=109.31},
new GeoCoordinate{Lat=18.39     , Lon=109.42},
new GeoCoordinate{Lat=18.13 , Lon=109.35},
new GeoCoordinate{Lat=18.48 , Lon=110.20},
new GeoCoordinate{Lat=18.33 , Lon=110.02},
new GeoCoordinate{Lat=16.50     , Lon=112.20},
new GeoCoordinate{Lat=41.51     , Lon=114.35},
new GeoCoordinate{Lat=41.06     , Lon=113.59},
new GeoCoordinate{Lat=41.09     , Lon=114.42},
new GeoCoordinate{Lat=40.40 , Lon=114.23},
new GeoCoordinate{Lat=40.06     , Lon=114.09},
new GeoCoordinate{Lat=40.34     , Lon=115.02},
new GeoCoordinate{Lat=40.46     , Lon=114.44},
new GeoCoordinate{Lat=39.50     , Lon=114.34},
new GeoCoordinate{Lat=41.40     , Lon=115.39},
new GeoCoordinate{Lat=40.57 , Lon=115.16},
new GeoCoordinate{Lat=40.46 , Lon=114.55},
new GeoCoordinate{Lat=40.53     , Lon=115.50},
new GeoCoordinate{Lat=40.25     , Lon=115.30},
new GeoCoordinate{Lat=40.23     , Lon=115.13},
new GeoCoordinate{Lat=36.52     , Lon=114.28},
new GeoCoordinate{Lat=37.31     , Lon=114.40},
new GeoCoordinate{Lat=37.27     , Lon=114.29},
new GeoCoordinate{Lat=37.21     , Lon=114.45},
new GeoCoordinate{Lat=37.37     , Lon=114.53},
new GeoCoordinate{Lat=37.17     , Lon=114.23},
new GeoCoordinate{Lat=37.11     , Lon=114.22},
new GeoCoordinate{Lat=37.14 , Lon=114.59},
new GeoCoordinate{Lat=37.08     , Lon=114.42},
new GeoCoordinate{Lat=37.01 , Lon=114.41},
new GeoCoordinate{Lat=37.04     , Lon=115.09},
new GeoCoordinate{Lat=37.04     , Lon=115.02},
new GeoCoordinate{Lat=37.30     , Lon=115.16},
new GeoCoordinate{Lat=37.22     , Lon=115.23},
new GeoCoordinate{Lat=37.05     , Lon=115.40},
new GeoCoordinate{Lat=37.00     , Lon=115.15},
new GeoCoordinate{Lat=36.51     , Lon=115.29},
new GeoCoordinate{Lat=40.12     , Lon=117.57},
new GeoCoordinate{Lat=40.10 , Lon=118.20},
new GeoCoordinate{Lat=39.30 , Lon=118.37},
new GeoCoordinate{Lat=40.01     , Lon=118.43},
new GeoCoordinate{Lat=39.53     , Lon=117.44},
new GeoCoordinate{Lat=39.44 , Lon=118.43},
new GeoCoordinate{Lat=39.48 , Lon=118.05},
new GeoCoordinate{Lat=39.35 , Lon=118.06},
new GeoCoordinate{Lat=39.39     , Lon=118.06},
new GeoCoordinate{Lat=39.17     , Lon=118.28},
new GeoCoordinate{Lat=39.26     , Lon=118.53},
new GeoCoordinate{Lat=38.18     , Lon=114.23},
new GeoCoordinate{Lat=38.25 , Lon=114.33},
new GeoCoordinate{Lat=38.01     , Lon=115.04},
new GeoCoordinate{Lat=38.09     , Lon=114.34},
new GeoCoordinate{Lat=38.02     , Lon=114.08},
new GeoCoordinate{Lat=38.16     , Lon=114.13},
new GeoCoordinate{Lat=38.21     , Lon=114.41},
new GeoCoordinate{Lat=38.02     , Lon=114.50},
new GeoCoordinate{Lat=38.04 , Lon=114.21},
new GeoCoordinate{Lat=38.11     , Lon=114.57},
new GeoCoordinate{Lat=37.45     , Lon=114.47},
new GeoCoordinate{Lat=37.53     , Lon=114.38},
new GeoCoordinate{Lat=37.38 , Lon=114.36},
new GeoCoordinate{Lat=37.48     , Lon=114.30},
new GeoCoordinate{Lat=37.39     , Lon=114.24},
new GeoCoordinate{Lat=38.11     , Lon=115.11},
new GeoCoordinate{Lat=37.56     , Lon=115.12},
new GeoCoordinate{Lat=40.25 , Lon=118.57},
new GeoCoordinate{Lat=39.53     , Lon=118.53},
new GeoCoordinate{Lat=39.51     , Lon=119.31},
new GeoCoordinate{Lat=39.44     , Lon=119.12},
new GeoCoordinate{Lat=39.54     , Lon=119.14},
new GeoCoordinate{Lat=39.55 , Lon=116.57},
new GeoCoordinate{Lat=39.25 , Lon=116.17},
new GeoCoordinate{Lat=39.30     , Lon=116.42},
new GeoCoordinate{Lat=39.10 , Lon=116.24},
new GeoCoordinate{Lat=39.18     , Lon=116.29},
new GeoCoordinate{Lat=39.58     , Lon=117.05},
new GeoCoordinate{Lat=39.42     , Lon=116.59},
new GeoCoordinate{Lat=38.51 , Lon=116.27},
new GeoCoordinate{Lat=38.42     , Lon=116.37},
new GeoCoordinate{Lat=38.13     , Lon=115.45},
new GeoCoordinate{Lat=38.00     , Lon=115.33},
new GeoCoordinate{Lat=38.14 , Lon=115.35},
new GeoCoordinate{Lat=38.02     , Lon=115.56},
new GeoCoordinate{Lat=37.44     , Lon=115.42},
new GeoCoordinate{Lat=37.48     , Lon=115.53},
new GeoCoordinate{Lat=37.31 , Lon=115.34},
new GeoCoordinate{Lat=37.21     , Lon=115.59},
new GeoCoordinate{Lat=37.30     , Lon=115.44},
new GeoCoordinate{Lat=37.52     , Lon=116.10},
new GeoCoordinate{Lat=37.42     , Lon=116.17},
new GeoCoordinate{Lat=36.20 , Lon=114.37},
new GeoCoordinate{Lat=36.34 , Lon=113.40},
new GeoCoordinate{Lat=36.40 , Lon=114.06},
new GeoCoordinate{Lat=36.37 , Lon=114.28},
new GeoCoordinate{Lat=36.46 , Lon=114.57},
new GeoCoordinate{Lat=36.24 , Lon=114.12},
new GeoCoordinate{Lat=36.45 , Lon=114.27},
new GeoCoordinate{Lat=36.20 , Lon=114.57},
new GeoCoordinate{Lat=36.24 , Lon=114.21},
new GeoCoordinate{Lat=36.29 , Lon=114.58},
new GeoCoordinate{Lat=36.33 , Lon=114.48},
new GeoCoordinate{Lat=36.27 , Lon=114.39},
new GeoCoordinate{Lat=36.55 , Lon=114.52},
new GeoCoordinate{Lat=36.18 , Lon=115.11},
new GeoCoordinate{Lat=36.33 , Lon=115.17},
new GeoCoordinate{Lat=36.49 , Lon=115.12},
new GeoCoordinate{Lat=41.12 , Lon=116.38},
new GeoCoordinate{Lat=41.58 , Lon=117.46},
new GeoCoordinate{Lat=41.19 , Lon=117.44},
new GeoCoordinate{Lat=41.00 , Lon=118.40},
new GeoCoordinate{Lat=40.56 , Lon=117.20},
new GeoCoordinate{Lat=40.58 , Lon=117.55},
new GeoCoordinate{Lat=40.24 , Lon=117.28},
new GeoCoordinate{Lat=40.47 , Lon=118.13},
new GeoCoordinate{Lat=40.37     , Lon=118.28},
new GeoCoordinate{Lat=38.44     , Lon=116.06},
new GeoCoordinate{Lat=38.25 , Lon=116.03},
new GeoCoordinate{Lat=38.35 , Lon=116.51},
new GeoCoordinate{Lat=38.21     , Lon=116.51},
new GeoCoordinate{Lat=38.12     , Lon=116.10},
new GeoCoordinate{Lat=38.05     , Lon=116.33},
new GeoCoordinate{Lat=38.24 , Lon=117.19},
new GeoCoordinate{Lat=38.25     , Lon=115.49},
new GeoCoordinate{Lat=38.02     , Lon=117.14},
new GeoCoordinate{Lat=38.09     , Lon=117.29},
new GeoCoordinate{Lat=38.03     , Lon=117.07},
new GeoCoordinate{Lat=37.53     , Lon=116.32},
new GeoCoordinate{Lat=37.38     , Lon=116.24},
new GeoCoordinate{Lat=38.02     , Lon=116.41},
new GeoCoordinate{Lat=38.51 , Lon=115.08},
new GeoCoordinate{Lat=39.22 , Lon=114.41},
new GeoCoordinate{Lat=38.38 , Lon=114.41},
new GeoCoordinate{Lat=38.51 , Lon=114.11},
new GeoCoordinate{Lat=38.44 , Lon=114.59},
new GeoCoordinate{Lat=38.33 , Lon=115.01},
new GeoCoordinate{Lat=39.29 , Lon=116.02},
new GeoCoordinate{Lat=39.04 , Lon=115.49},
new GeoCoordinate{Lat=39.19 , Lon=115.57},
new GeoCoordinate{Lat=39.20 , Lon=115.31},
new GeoCoordinate{Lat=38.59 , Lon=115.39},
new GeoCoordinate{Lat=38.44 , Lon=115.29},
new GeoCoordinate{Lat=38.43 , Lon=115.46},
new GeoCoordinate{Lat=38.25 , Lon=115.20},
new GeoCoordinate{Lat=38.56 , Lon=115.56},
new GeoCoordinate{Lat=38.43 , Lon=115.07},
new GeoCoordinate{Lat=38.56 , Lon=115.19},
new GeoCoordinate{Lat=38.29 , Lon=115.34},
new GeoCoordinate{Lat=39.01 , Lon=116.06},
new GeoCoordinate{Lat=36.04     , Lon=113.52},
new GeoCoordinate{Lat=36.03 , Lon=114.08},
new GeoCoordinate{Lat=35.03 , Lon=112.54},
new GeoCoordinate{Lat=35.37 , Lon=114.14},
new GeoCoordinate{Lat=35.05     , Lon=112.38},
new GeoCoordinate{Lat=35.09     , Lon=113.05},
new GeoCoordinate{Lat=35.14     , Lon=113.16},
new GeoCoordinate{Lat=35.02     , Lon=114.25},
new GeoCoordinate{Lat=35.14     , Lon=113.28},
new GeoCoordinate{Lat=35.30 , Lon=113.49},
new GeoCoordinate{Lat=35.19     , Lon=113.53},
new GeoCoordinate{Lat=35.06 , Lon=113.21},
new GeoCoordinate{Lat=35.16     , Lon=113.40},
new GeoCoordinate{Lat=35.05 , Lon=113.59},
new GeoCoordinate{Lat=35.55 , Lon=114.20},
new GeoCoordinate{Lat=35.39     , Lon=114.31},
new GeoCoordinate{Lat=35.56     , Lon=114.55},
new GeoCoordinate{Lat=35.23     , Lon=114.04},
new GeoCoordinate{Lat=35.33 , Lon=114.30},
new GeoCoordinate{Lat=35.09     , Lon=114.11},
new GeoCoordinate{Lat=35.12     , Lon=114.39},
new GeoCoordinate{Lat=35.59     , Lon=115.52},
new GeoCoordinate{Lat=35.42     , Lon=115.01},
new GeoCoordinate{Lat=36.05     , Lon=115.10},
new GeoCoordinate{Lat=35.57     , Lon=115.09},
new GeoCoordinate{Lat=35.51     , Lon=115.29},
new GeoCoordinate{Lat=34.48     , Lon=111.12},
new GeoCoordinate{Lat=34.32 , Lon=110.51},
new GeoCoordinate{Lat=34.46     , Lon=111.46},
new GeoCoordinate{Lat=34.33     , Lon=112.08},
new GeoCoordinate{Lat=34.24 , Lon=111.40},
new GeoCoordinate{Lat=34.05 , Lon=111.04},
new GeoCoordinate{Lat=34.44     , Lon=112.07},
new GeoCoordinate{Lat=34.49     , Lon=112.26},
new GeoCoordinate{Lat=34.55     , Lon=112.45},
new GeoCoordinate{Lat=34.25     , Lon=112.24},
new GeoCoordinate{Lat=34.10     , Lon=112.52},
new GeoCoordinate{Lat=34.44 , Lon=112.47},
new GeoCoordinate{Lat=33.47     , Lon=111.39},
new GeoCoordinate{Lat=34.09     , Lon=112.28},
new GeoCoordinate{Lat=34.57     , Lon=113.02},
new GeoCoordinate{Lat=34.44     , Lon=112.58},
new GeoCoordinate{Lat=34.48     , Lon=113.26},
new GeoCoordinate{Lat=34.28     , Lon=113.01},
new GeoCoordinate{Lat=34.43     , Lon=113.39},
new GeoCoordinate{Lat=34.30     , Lon=113.03},
new GeoCoordinate{Lat=34.32 , Lon=113.20},
new GeoCoordinate{Lat=34.23     , Lon=113.43},
new GeoCoordinate{Lat=34.16 , Lon=113.44},
new GeoCoordinate{Lat=34.11 , Lon=113.30},
new GeoCoordinate{Lat=34.04 , Lon=113.56},
new GeoCoordinate{Lat=34.43     , Lon=113.58},
new GeoCoordinate{Lat=34.47     , Lon=114.18},
new GeoCoordinate{Lat=34.51     , Lon=114.49},
new GeoCoordinate{Lat=34.24     , Lon=114.13},
new GeoCoordinate{Lat=34.07     , Lon=114.09},
new GeoCoordinate{Lat=34.32     , Lon=114.47},
new GeoCoordinate{Lat=34.05     , Lon=114.24},
new GeoCoordinate{Lat=34.04     , Lon=114.51},
new GeoCoordinate{Lat=33.18     , Lon=111.30},
new GeoCoordinate{Lat=34.08 , Lon=112.04},
new GeoCoordinate{Lat=33.09     , Lon=111.53},
new GeoCoordinate{Lat=33.45     , Lon=112.53},
new GeoCoordinate{Lat=33.05     , Lon=112.15},
new GeoCoordinate{Lat=33.29     , Lon=112.25},
new GeoCoordinate{Lat=33.20     , Lon=113.32},
new GeoCoordinate{Lat=33.06 , Lon=112.29},
new GeoCoordinate{Lat=33.17     , Lon=113.00},
new GeoCoordinate{Lat=33.59     , Lon=113.12},
new GeoCoordinate{Lat=33.53     , Lon=113.03},
new GeoCoordinate{Lat=33.51 , Lon=113.31},
new GeoCoordinate{Lat=33.48     , Lon=113.55},
new GeoCoordinate{Lat=33.38 , Lon=113.18},
new GeoCoordinate{Lat=33.27     , Lon=113.35},
new GeoCoordinate{Lat=33.36     , Lon=114.03},
new GeoCoordinate{Lat=33.04     , Lon=112.56},
new GeoCoordinate{Lat=33.22     , Lon=114.02},
new GeoCoordinate{Lat=33.08     , Lon=113.57},
new GeoCoordinate{Lat=33.45     , Lon=114.24},
new GeoCoordinate{Lat=34.31 , Lon=114.28},
new GeoCoordinate{Lat=33.44     , Lon=114.51},
new GeoCoordinate{Lat=33.47     , Lon=114.31},
new GeoCoordinate{Lat=33.17     , Lon=114.16},
new GeoCoordinate{Lat=33.37     , Lon=114.37},
new GeoCoordinate{Lat=33.28 , Lon=114.52},
new GeoCoordinate{Lat=32.58     , Lon=114.21},
new GeoCoordinate{Lat=33.30 , Lon=114.34},
new GeoCoordinate{Lat=33.07 , Lon=111.31},
new GeoCoordinate{Lat=32.31 , Lon=112.23},
new GeoCoordinate{Lat=32.41     , Lon=112.51},
new GeoCoordinate{Lat=32.42     , Lon=112.07},
new GeoCoordinate{Lat=32.42     , Lon=113.18},
new GeoCoordinate{Lat=32.23     , Lon=113.25},
new GeoCoordinate{Lat=32.56 , Lon=113.55},
new GeoCoordinate{Lat=32.58     , Lon=114.38},
new GeoCoordinate{Lat=32.44     , Lon=114.59},
new GeoCoordinate{Lat=32.50     , Lon=114.01},
new GeoCoordinate{Lat=32.37     , Lon=114.21},
new GeoCoordinate{Lat=32.21     , Lon=114.44},
new GeoCoordinate{Lat=32.08     , Lon=114.03},
new GeoCoordinate{Lat=32.13     , Lon=114.33},
new GeoCoordinate{Lat=32.01     , Lon=114.54},
new GeoCoordinate{Lat=31.48     , Lon=114.04},
new GeoCoordinate{Lat=31.38     , Lon=114.51},
new GeoCoordinate{Lat=34.26     , Lon=115.06},
new GeoCoordinate{Lat=34.39     , Lon=115.09},
new GeoCoordinate{Lat=34.26 , Lon=115.32},
new GeoCoordinate{Lat=34.23     , Lon=115.53},
new GeoCoordinate{Lat=34.04     , Lon=115.18},
new GeoCoordinate{Lat=34.28     , Lon=115.20},
new GeoCoordinate{Lat=34.15     , Lon=116.08},
new GeoCoordinate{Lat=33.39     , Lon=115.10},
new GeoCoordinate{Lat=33.52 , Lon=115.29},
new GeoCoordinate{Lat=33.24     , Lon=115.04},
new GeoCoordinate{Lat=33.58     , Lon=116.27},
new GeoCoordinate{Lat=32.28     , Lon=115.26},
new GeoCoordinate{Lat=32.10     , Lon=115.03},
new GeoCoordinate{Lat=32.10     , Lon=115.37},
new GeoCoordinate{Lat=31.49     , Lon=115.23},
new GeoCoordinate{Lat=52.58 , Lon=122.31},
new GeoCoordinate{Lat=53.28     , Lon=122.22},
new GeoCoordinate{Lat=52.21 , Lon=124.43},
new GeoCoordinate{Lat=52.02 , Lon=123.34},
new GeoCoordinate{Lat=51.40     , Lon=124.24},
new GeoCoordinate{Lat=51.44     , Lon=126.38},
new GeoCoordinate{Lat=50.24     , Lon=124.07},
new GeoCoordinate{Lat=50.15     , Lon=127.27},
new GeoCoordinate{Lat=49.10     , Lon=125.14},
new GeoCoordinate{Lat=49.26     , Lon=127.21},
new GeoCoordinate{Lat=49.33 , Lon=128.28},
new GeoCoordinate{Lat=48.29     , Lon=124.51},
new GeoCoordinate{Lat=48.30     , Lon=126.11},
new GeoCoordinate{Lat=48.15 , Lon=126.30},
new GeoCoordinate{Lat=48.03     , Lon=125.53},
new GeoCoordinate{Lat=48.02     , Lon=126.15},
new GeoCoordinate{Lat=48.53     , Lon=130.24},
new GeoCoordinate{Lat=48.34     , Lon=129.26},
new GeoCoordinate{Lat=47.18     , Lon=123.15},
new GeoCoordinate{Lat=47.56     , Lon=123.30},
new GeoCoordinate{Lat=47.48     , Lon=124.29},
new GeoCoordinate{Lat=47.23     , Lon=123.55},
new GeoCoordinate{Lat=47.11     , Lon=124.50},
new GeoCoordinate{Lat=47.54     , Lon=125.18},
new GeoCoordinate{Lat=47.37     , Lon=126.04},
new GeoCoordinate{Lat=47.27 , Lon=126.52},
new GeoCoordinate{Lat=47.11     , Lon=125.54},
new GeoCoordinate{Lat=47.14     , Lon=127.06},
new GeoCoordinate{Lat=48.06     , Lon=129.14},
new GeoCoordinate{Lat=47.42     , Lon=128.50},
new GeoCoordinate{Lat=47.20 , Lon=130.12},
new GeoCoordinate{Lat=47.34     , Lon=130.50},
new GeoCoordinate{Lat=47.40     , Lon=132.32},
new GeoCoordinate{Lat=48.22     , Lon=134.17},
new GeoCoordinate{Lat=47.19 , Lon=131.50},
new GeoCoordinate{Lat=47.14     , Lon=131.59},
new GeoCoordinate{Lat=46.50     , Lon=124.25},
new GeoCoordinate{Lat=46.24 , Lon=123.27},
new GeoCoordinate{Lat=46.34     , Lon=125.08},
new GeoCoordinate{Lat=46.41     , Lon=126.05},
new GeoCoordinate{Lat=46.50 , Lon=126.27},
new GeoCoordinate{Lat=46.37     , Lon=126.58},
new GeoCoordinate{Lat=46.23     , Lon=125.19},
new GeoCoordinate{Lat=46.01     , Lon=125.48},
new GeoCoordinate{Lat=46.17 , Lon=126.17},
new GeoCoordinate{Lat=46.53 , Lon=127.29},
new GeoCoordinate{Lat=46.59 , Lon=127.59},
new GeoCoordinate{Lat=46.05     , Lon=127.21},
new GeoCoordinate{Lat=46.44     , Lon=129.53},
new GeoCoordinate{Lat=46.47     , Lon=130.18},
new GeoCoordinate{Lat=46.18     , Lon=129.35},
new GeoCoordinate{Lat=47.00     , Lon=130.43},
new GeoCoordinate{Lat=46.13     , Lon=130.33},
new GeoCoordinate{Lat=46.44     , Lon=131.07},
new GeoCoordinate{Lat=46.47 , Lon=131.47},
new GeoCoordinate{Lat=46.38     , Lon=131.09},
new GeoCoordinate{Lat=46.23     , Lon=132.10},
new GeoCoordinate{Lat=46.48     , Lon=134.00},
new GeoCoordinate{Lat=45.42     , Lon=125.15},
new GeoCoordinate{Lat=45.56     , Lon=126.34},
new GeoCoordinate{Lat=45.30     , Lon=125.05},
new GeoCoordinate{Lat=45.24 , Lon=126.19},
new GeoCoordinate{Lat=46.05     , Lon=126.46},
new GeoCoordinate{Lat=45.32 , Lon=126.56},
new GeoCoordinate{Lat=45.44 , Lon=127.23},
new GeoCoordinate{Lat=45.57     , Lon=128.02},
new GeoCoordinate{Lat=45.58     , Lon=128.44},
new GeoCoordinate{Lat=45.50     , Lon=128.48},
new GeoCoordinate{Lat=45.26 , Lon=128.16},
new GeoCoordinate{Lat=45.13     , Lon=127.58},
new GeoCoordinate{Lat=45.46     , Lon=131.01},
new GeoCoordinate{Lat=45.45 , Lon=130.36},
new GeoCoordinate{Lat=45.18     , Lon=130.55},
new GeoCoordinate{Lat=45.16     , Lon=130.14},
new GeoCoordinate{Lat=45.46     , Lon=132.58},
new GeoCoordinate{Lat=45.33 , Lon=131.52},
new GeoCoordinate{Lat=45.16 , Lon=131.06},
new GeoCoordinate{Lat=44.54     , Lon=127.09},
new GeoCoordinate{Lat=44.36     , Lon=129.24},
new GeoCoordinate{Lat=44.56 , Lon=130.33},
new GeoCoordinate{Lat=44.30 , Lon=129.40},
new GeoCoordinate{Lat=44.23     , Lon=131.10},
new GeoCoordinate{Lat=44.20     , Lon=129.28},
new GeoCoordinate{Lat=44.05     , Lon=131.08},
new GeoCoordinate{Lat=32.19     , Lon=109.41},
new GeoCoordinate{Lat=33.00     , Lon=110.24},
new GeoCoordinate{Lat=32.50     , Lon=110.52},
new GeoCoordinate{Lat=32.39     , Lon=110.47},
new GeoCoordinate{Lat=32.14     , Lon=110.14},
new GeoCoordinate{Lat=32.03 , Lon=110.45},
new GeoCoordinate{Lat=32.34 , Lon=111.31},
new GeoCoordinate{Lat=32.26 , Lon=111.44},
new GeoCoordinate{Lat=32.16 , Lon=111.37},
new GeoCoordinate{Lat=32.00 , Lon=112.05},
new GeoCoordinate{Lat=32.09 , Lon=112.45},
new GeoCoordinate{Lat=31.02 , Lon=110.22},
new GeoCoordinate{Lat=30.50 , Lon=110.58},
new GeoCoordinate{Lat=31.21     , Lon=110.44},
new GeoCoordinate{Lat=31.53     , Lon=111.16},
new GeoCoordinate{Lat=31.45     , Lon=110.40},
new GeoCoordinate{Lat=31.48 , Lon=111.50},
new GeoCoordinate{Lat=31.03 , Lon=111.38},
new GeoCoordinate{Lat=31.44     , Lon=112.13},
new GeoCoordinate{Lat=31.00     , Lon=112.13},
new GeoCoordinate{Lat=31.12     , Lon=112.38},
new GeoCoordinate{Lat=31.37     , Lon=113.20},
new GeoCoordinate{Lat=31.37 , Lon=113.49},
new GeoCoordinate{Lat=31.15 , Lon=114.00},
new GeoCoordinate{Lat=31.00 , Lon=113.08},
new GeoCoordinate{Lat=31.16 , Lon=113.42},
new GeoCoordinate{Lat=31.02     , Lon=113.45},
new GeoCoordinate{Lat=31.34     , Lon=114.07},
new GeoCoordinate{Lat=31.17 , Lon=114.37},
new GeoCoordinate{Lat=31.08 , Lon=114.57},
new GeoCoordinate{Lat=30.17     , Lon=108.56},
new GeoCoordinate{Lat=30.36     , Lon=109.43},
new GeoCoordinate{Lat=30.17     , Lon=109.28},
new GeoCoordinate{Lat=30.46 , Lon=111.19},
new GeoCoordinate{Lat=30.12 , Lon=110.40},
new GeoCoordinate{Lat=30.49 , Lon=111.47},
new GeoCoordinate{Lat=30.44 , Lon=111.22},
new GeoCoordinate{Lat=30.52     , Lon=111.05},
new GeoCoordinate{Lat=30.28 , Lon=111.11},
new GeoCoordinate{Lat=30.22     , Lon=111.26},
new GeoCoordinate{Lat=30.26 , Lon=111.46},
new GeoCoordinate{Lat=30.11     , Lon=111.46},
new GeoCoordinate{Lat=30.24 , Lon=112.54},
new GeoCoordinate{Lat=30.21     , Lon=112.09},
new GeoCoordinate{Lat=30.04     , Lon=112.13},
new GeoCoordinate{Lat=30.57     , Lon=113.34},
new GeoCoordinate{Lat=30.54     , Lon=113.57},
new GeoCoordinate{Lat=30.40     , Lon=113.08},
new GeoCoordinate{Lat=30.44 , Lon=112.33},
new GeoCoordinate{Lat=30.18 , Lon=113.29},
new GeoCoordinate{Lat=30.39     , Lon=113.51},
new GeoCoordinate{Lat=30.31     , Lon=114.00},
new GeoCoordinate{Lat=30.52     , Lon=114.24},
new GeoCoordinate{Lat=30.50 , Lon=114.50},
new GeoCoordinate{Lat=30.21 , Lon=114.20},
new GeoCoordinate{Lat=30.36 , Lon=114.03},
new GeoCoordinate{Lat=30.40 , Lon=114.53},
new GeoCoordinate{Lat=30.15     , Lon=114.37},
new GeoCoordinate{Lat=30.31     , Lon=114.56},
new GeoCoordinate{Lat=30.04 , Lon=114.53},
new GeoCoordinate{Lat=29.41     , Lon=109.09},
new GeoCoordinate{Lat=30.00     , Lon=109.29},
new GeoCoordinate{Lat=29.54 , Lon=110.02},
new GeoCoordinate{Lat=29.32 , Lon=109.25},
new GeoCoordinate{Lat=29.40     , Lon=112.25},
new GeoCoordinate{Lat=29.50     , Lon=112.54},
new GeoCoordinate{Lat=29.49     , Lon=113.27},
new GeoCoordinate{Lat=29.43 , Lon=113.53},
new GeoCoordinate{Lat=29.55     , Lon=113.58},
new GeoCoordinate{Lat=29.32     , Lon=114.02},
new GeoCoordinate{Lat=29.16 , Lon=113.53},
new GeoCoordinate{Lat=29.51 , Lon=114.22},
new GeoCoordinate{Lat=29.36     , Lon=114.30},
new GeoCoordinate{Lat=29.38     , Lon=114.12},
new GeoCoordinate{Lat=30.45     , Lon=115.24},
new GeoCoordinate{Lat=30.44     , Lon=115.40},
new GeoCoordinate{Lat=30.28 , Lon=115.13},
new GeoCoordinate{Lat=30.14     , Lon=115.02},
new GeoCoordinate{Lat=30.14 , Lon=115.26},
new GeoCoordinate{Lat=30.06     , Lon=115.56},
new GeoCoordinate{Lat=29.54 , Lon=115.13},
new GeoCoordinate{Lat=29.55 , Lon=115.38},
new GeoCoordinate{Lat=29.28     , Lon=109.27},
new GeoCoordinate{Lat=29.24     , Lon=110.10},
new GeoCoordinate{Lat=29.08     , Lon=110.28},
new GeoCoordinate{Lat=29.35     , Lon=111.22},
new GeoCoordinate{Lat=29.26     , Lon=111.08},
new GeoCoordinate{Lat=29.40 , Lon=111.43},
new GeoCoordinate{Lat=29.27     , Lon=111.39},
new GeoCoordinate{Lat=29.22     , Lon=112.24},
new GeoCoordinate{Lat=29.31     , Lon=112.33},
new GeoCoordinate{Lat=29.24 , Lon=112.10},
new GeoCoordinate{Lat=29.23     , Lon=113.05},
new GeoCoordinate{Lat=29.29     , Lon=113.27},
new GeoCoordinate{Lat=28.35     , Lon=109.27},
new GeoCoordinate{Lat=28.42     , Lon=109.39},
new GeoCoordinate{Lat=29.00 , Lon=109.50},
new GeoCoordinate{Lat=28.37     , Lon=109.57},
new GeoCoordinate{Lat=28.14 , Lon=109.41},
new GeoCoordinate{Lat=28.28 , Lon=110.24},
new GeoCoordinate{Lat=28.13 , Lon=110.12},
new GeoCoordinate{Lat=28.00 , Lon=110.12},
new GeoCoordinate{Lat=28.55 , Lon=111.29},
new GeoCoordinate{Lat=29.07 , Lon=111.41},
new GeoCoordinate{Lat=28.55 , Lon=111.57},
new GeoCoordinate{Lat=28.30 , Lon=112.10},
new GeoCoordinate{Lat=28.23 , Lon=111.13},
new GeoCoordinate{Lat=28.51 , Lon=112.22},
new GeoCoordinate{Lat=28.41 , Lon=112.53},
new GeoCoordinate{Lat=28.34 , Lon=112.23},
new GeoCoordinate{Lat=28.15 , Lon=112.33},
new GeoCoordinate{Lat=28.13 , Lon=113.12},
new GeoCoordinate{Lat=28.51 , Lon=113.06},
new GeoCoordinate{Lat=28.43 , Lon=113.34},
new GeoCoordinate{Lat=28.07 , Lon=112.47},
new GeoCoordinate{Lat=28.09 , Lon=113.38},
new GeoCoordinate{Lat=27.57 , Lon=109.36},
new GeoCoordinate{Lat=27.52 , Lon=109.48},
new GeoCoordinate{Lat=27.22 , Lon=109.10},
new GeoCoordinate{Lat=27.27 , Lon=109.41},
new GeoCoordinate{Lat=27.34 , Lon=110.00},
new GeoCoordinate{Lat=27.55 , Lon=110.36},
new GeoCoordinate{Lat=27.13 , Lon=109.50},
new GeoCoordinate{Lat=27.04 , Lon=110.35},
new GeoCoordinate{Lat=27.42 , Lon=111.26},
new GeoCoordinate{Lat=27.45 , Lon=111.18},
new GeoCoordinate{Lat=27.41 , Lon=111.42},
new GeoCoordinate{Lat=27.41 , Lon=112.00},
new GeoCoordinate{Lat=27.11 , Lon=111.27},
new GeoCoordinate{Lat=27.07 , Lon=111.00},
new GeoCoordinate{Lat=27.19 , Lon=111.27},
new GeoCoordinate{Lat=27.14 , Lon=111.44},
new GeoCoordinate{Lat=27.56 , Lon=112.32},
new GeoCoordinate{Lat=27.45 , Lon=112.30},
new GeoCoordinate{Lat=27.53 , Lon=112.50},
new GeoCoordinate{Lat=27.27 , Lon=112.10},
new GeoCoordinate{Lat=27.18 , Lon=112.42},
new GeoCoordinate{Lat=27.14 , Lon=112.52},
new GeoCoordinate{Lat=27.06 , Lon=112.57},
new GeoCoordinate{Lat=27.00 , Lon=113.21},
new GeoCoordinate{Lat=27.52 , Lon=113.10},
new GeoCoordinate{Lat=27.39 , Lon=113.30},
new GeoCoordinate{Lat=26.34 , Lon=109.40},
new GeoCoordinate{Lat=26.53 , Lon=109.43},
new GeoCoordinate{Lat=26.10 , Lon=109.47},
new GeoCoordinate{Lat=26.35 , Lon=110.09},
new GeoCoordinate{Lat=26.28 , Lon=110.50},
new GeoCoordinate{Lat=26.44 , Lon=110.38},
new GeoCoordinate{Lat=26.22 , Lon=110.19},
new GeoCoordinate{Lat=27.00 , Lon=111.17},
new GeoCoordinate{Lat=26.30 , Lon=111.37},
new GeoCoordinate{Lat=26.14 , Lon=111.37},
new GeoCoordinate{Lat=26.24 , Lon=111.17},
new GeoCoordinate{Lat=26.36 , Lon=111.52},
new GeoCoordinate{Lat=26.48 , Lon=112.06},
new GeoCoordinate{Lat=26.59 , Lon=112.22},
new GeoCoordinate{Lat=26.54 , Lon=112.36},
new GeoCoordinate{Lat=26.25 , Lon=112.24},
new GeoCoordinate{Lat=26.45 , Lon=112.41},
new GeoCoordinate{Lat=26.26 , Lon=112.50},
new GeoCoordinate{Lat=26.43 , Lon=113.26},
new GeoCoordinate{Lat=26.47 , Lon=113.33},
new GeoCoordinate{Lat=26.29 , Lon=113.48},
new GeoCoordinate{Lat=26.08 , Lon=113.07},
new GeoCoordinate{Lat=26.05 , Lon=113.57},
new GeoCoordinate{Lat=25.58 , Lon=111.39},
new GeoCoordinate{Lat=25.32 , Lon=111.36},
new GeoCoordinate{Lat=25.36 , Lon=111.58},
new GeoCoordinate{Lat=25.17 , Lon=111.19},
new GeoCoordinate{Lat=25.55 , Lon=112.12},
new GeoCoordinate{Lat=25.44 , Lon=112.58},
new GeoCoordinate{Lat=25.45 , Lon=112.43},
new GeoCoordinate{Lat=25.35 , Lon=112.22},
new GeoCoordinate{Lat=25.23 , Lon=112.12},
new GeoCoordinate{Lat=25.24 , Lon=112.56},
new GeoCoordinate{Lat=25.16 , Lon=112.33},
new GeoCoordinate{Lat=25.58 , Lon=113.13},
new GeoCoordinate{Lat=25.33 , Lon=113.41},
new GeoCoordinate{Lat=25.11 , Lon=111.34},
new GeoCoordinate{Lat=45.38     , Lon=122.50},
new GeoCoordinate{Lat=45.20     , Lon=122.49},
new GeoCoordinate{Lat=45.51     , Lon=123.10},
new GeoCoordinate{Lat=45.30     , Lon=124.14},
new GeoCoordinate{Lat=45.11     , Lon=124.50},
new GeoCoordinate{Lat=45.00     , Lon=124.01},
new GeoCoordinate{Lat=45.05     , Lon=124.52},
new GeoCoordinate{Lat=44.48 , Lon=123.04},
new GeoCoordinate{Lat=44.15     , Lon=123.58},
new GeoCoordinate{Lat=44.58     , Lon=126.00},
new GeoCoordinate{Lat=44.23     , Lon=125.09},
new GeoCoordinate{Lat=44.32     , Lon=125.39},
new GeoCoordinate{Lat=44.07     , Lon=125.48},
new GeoCoordinate{Lat=44.51     , Lon=126.31},
new GeoCoordinate{Lat=44.23     , Lon=126.56},
new GeoCoordinate{Lat=43.30     , Lon=123.32},
new GeoCoordinate{Lat=43.21     , Lon=124.18},
new GeoCoordinate{Lat=43.40     , Lon=124.14},
new GeoCoordinate{Lat=43.31     , Lon=124.48},
new GeoCoordinate{Lat=43.07     , Lon=124.23},
new GeoCoordinate{Lat=43.54     , Lon=125.13},
new GeoCoordinate{Lat=43.21     , Lon=125.17},
new GeoCoordinate{Lat=43.33     , Lon=125.38},
new GeoCoordinate{Lat=43.18     , Lon=126.01},
new GeoCoordinate{Lat=43.42     , Lon=126.31},
new GeoCoordinate{Lat=43.52     , Lon=126.32},
new GeoCoordinate{Lat=43.42     , Lon=127.20},
new GeoCoordinate{Lat=43.22     , Lon=128.12},
new GeoCoordinate{Lat=43.07     , Lon=128.55},
new GeoCoordinate{Lat=43.42     , Lon=130.16},
new GeoCoordinate{Lat=43.18     , Lon=129.47},
new GeoCoordinate{Lat=42.55     , Lon=125.05},
new GeoCoordinate{Lat=42.40     , Lon=125.28},
new GeoCoordinate{Lat=42.58     , Lon=126.05},
new GeoCoordinate{Lat=42.32     , Lon=125.38},
new GeoCoordinate{Lat=42.15     , Lon=125.44},
new GeoCoordinate{Lat=42.59     , Lon=126.45},
new GeoCoordinate{Lat=42.41     , Lon=125.59},
new GeoCoordinate{Lat=42.24 , Lon=126.48},
new GeoCoordinate{Lat=42.03     , Lon=126.35},
new GeoCoordinate{Lat=42.09 , Lon=127.30},
new GeoCoordinate{Lat=42.25     , Lon=128.07},
new GeoCoordinate{Lat=42.32     , Lon=129.00},
new GeoCoordinate{Lat=42.46     , Lon=129.24},
new GeoCoordinate{Lat=42.54     , Lon=130.17},
new GeoCoordinate{Lat=42.52     , Lon=129.30},
new GeoCoordinate{Lat=42.57     , Lon=129.50},
new GeoCoordinate{Lat=41.40     , Lon=125.44},
new GeoCoordinate{Lat=41.41     , Lon=125.54},
new GeoCoordinate{Lat=41.56     , Lon=126.26},
new GeoCoordinate{Lat=41.48     , Lon=126.53},
new GeoCoordinate{Lat=41.09 , Lon=126.13},
new GeoCoordinate{Lat=41.25     , Lon=128.11},
new GeoCoordinate{Lat=34.40 , Lon=116.39},
new GeoCoordinate{Lat=34.45     , Lon=116.54},
new GeoCoordinate{Lat=34.24     , Lon=118.01},
new GeoCoordinate{Lat=34.17     , Lon=117.09},
new GeoCoordinate{Lat=34.20     , Lon=118.21},
new GeoCoordinate{Lat=34.33 , Lon=118.43},
new GeoCoordinate{Lat=34.05     , Lon=118.47},
new GeoCoordinate{Lat=34.51     , Lon=119.08},
new GeoCoordinate{Lat=34.47     , Lon=119.26},
new GeoCoordinate{Lat=34.32     , Lon=119.14},
new GeoCoordinate{Lat=34.12 , Lon=119.36},
new GeoCoordinate{Lat=34.15 , Lon=119.14},
new GeoCoordinate{Lat=34.07     , Lon=119.21},
new GeoCoordinate{Lat=34.01 , Lon=119.48},
new GeoCoordinate{Lat=33.56     , Lon=117.57},
new GeoCoordinate{Lat=33.58 , Lon=118.13},
new GeoCoordinate{Lat=33.43     , Lon=118.41},
new GeoCoordinate{Lat=33.29     , Lon=118.13},
new GeoCoordinate{Lat=32.59     , Lon=118.31},
new GeoCoordinate{Lat=33.18 , Lon=118.55},
new GeoCoordinate{Lat=33.46     , Lon=119.18},
new GeoCoordinate{Lat=33.38 , Lon=118.56},
new GeoCoordinate{Lat=33.48     , Lon=119.51},
new GeoCoordinate{Lat=33.30     , Lon=119.09},
new GeoCoordinate{Lat=33.28     , Lon=119.46},
new GeoCoordinate{Lat=32.59     , Lon=118.58},
new GeoCoordinate{Lat=33.16 , Lon=119.21},
new GeoCoordinate{Lat=33.45 , Lon=120.18},
new GeoCoordinate{Lat=33.19     , Lon=120.06},
new GeoCoordinate{Lat=33.10     , Lon=120.27},
new GeoCoordinate{Lat=32.22     , Lon=118.51},
new GeoCoordinate{Lat=32.04 , Lon=118.35},
new GeoCoordinate{Lat=31.56     , Lon=118.54},
new GeoCoordinate{Lat=32.48     , Lon=119.27},
new GeoCoordinate{Lat=32.18 , Lon=119.10},
new GeoCoordinate{Lat=32.57 , Lon=119.49},
new GeoCoordinate{Lat=32.27     , Lon=119.35},
new GeoCoordinate{Lat=32.25     , Lon=119.25},
new GeoCoordinate{Lat=32.33 , Lon=120.00},
new GeoCoordinate{Lat=32.16 , Lon=119.48},
new GeoCoordinate{Lat=32.10     , Lon=120.00},
new GeoCoordinate{Lat=32.31     , Lon=120.09},
new GeoCoordinate{Lat=32.51     , Lon=120.17},
new GeoCoordinate{Lat=32.11     , Lon=119.28},
new GeoCoordinate{Lat=32.31     , Lon=120.27},
new GeoCoordinate{Lat=32.22     , Lon=120.34},
new GeoCoordinate{Lat=31.59     , Lon=120.15},
new GeoCoordinate{Lat=32.05     , Lon=120.59},
new GeoCoordinate{Lat=32.20     , Lon=121.11},
new GeoCoordinate{Lat=32.04     , Lon=121.36},
new GeoCoordinate{Lat=32.06     , Lon=121.05},
new GeoCoordinate{Lat=31.47     , Lon=121.39},
new GeoCoordinate{Lat=31.20     , Lon=118.54},
new GeoCoordinate{Lat=31.36     , Lon=119.04},
new GeoCoordinate{Lat=31.59     , Lon=119.36},
new GeoCoordinate{Lat=31.43     , Lon=119.33},
new GeoCoordinate{Lat=31.53     , Lon=119.59},
new GeoCoordinate{Lat=31.58     , Lon=119.12},
new GeoCoordinate{Lat=31.26     , Lon=119.29},
new GeoCoordinate{Lat=31.20     , Lon=119.49},
new GeoCoordinate{Lat=31.25 , Lon=120.34},
new GeoCoordinate{Lat=31.54     , Lon=120.18},
new GeoCoordinate{Lat=31.39     , Lon=120.46},
new GeoCoordinate{Lat=31.52 , Lon=120.34},
new GeoCoordinate{Lat=31.37     , Lon=120.21},
new GeoCoordinate{Lat=31.24     , Lon=121.00},
new GeoCoordinate{Lat=31.04     , Lon=120.26},
new GeoCoordinate{Lat=31.08     , Lon=120.37},
new GeoCoordinate{Lat=31.55     , Lon=121.12},
new GeoCoordinate{Lat=31.31 , Lon=121.06},
new GeoCoordinate{Lat=29.02     , Lon=114.35},
new GeoCoordinate{Lat=28.32     , Lon=114.23},
new GeoCoordinate{Lat=28.24     , Lon=114.47},
new GeoCoordinate{Lat=28.06     , Lon=114.26},
new GeoCoordinate{Lat=28.14     , Lon=114.55},
new GeoCoordinate{Lat=27.38 , Lon=113.51},
new GeoCoordinate{Lat=27.08     , Lon=113.57},
new GeoCoordinate{Lat=27.49     , Lon=114.41},
new GeoCoordinate{Lat=27.48     , Lon=114.23},
new GeoCoordinate{Lat=27.50 , Lon=114.53},
new GeoCoordinate{Lat=27.24     , Lon=114.36},
new GeoCoordinate{Lat=27.03     , Lon=114.55},
new GeoCoordinate{Lat=26.45     , Lon=114.17},
new GeoCoordinate{Lat=26.56     , Lon=114.15},
new GeoCoordinate{Lat=26.35     , Lon=114.10},
new GeoCoordinate{Lat=26.28     , Lon=114.47},
new GeoCoordinate{Lat=26.20     , Lon=114.30},
new GeoCoordinate{Lat=26.48     , Lon=114.55},
new GeoCoordinate{Lat=25.42     , Lon=114.18},
new GeoCoordinate{Lat=25.48     , Lon=114.33},
new GeoCoordinate{Lat=25.40     , Lon=114.45},
new GeoCoordinate{Lat=25.52     , Lon=115.00},
new GeoCoordinate{Lat=25.24     , Lon=114.21},
new GeoCoordinate{Lat=25.21     , Lon=114.52},
new GeoCoordinate{Lat=29.40     , Lon=116.01},
new GeoCoordinate{Lat=29.42 , Lon=115.40},
new GeoCoordinate{Lat=29.35     , Lon=115.59},
new GeoCoordinate{Lat=29.15     , Lon=115.07},
new GeoCoordinate{Lat=29.20     , Lon=115.46},
new GeoCoordinate{Lat=29.03     , Lon=115.49},
new GeoCoordinate{Lat=29.44     , Lon=116.14},
new GeoCoordinate{Lat=29.54     , Lon=116.33},
new GeoCoordinate{Lat=29.27     , Lon=116.03},
new GeoCoordinate{Lat=29.16     , Lon=116.12},
new GeoCoordinate{Lat=29.00     , Lon=116.41},
new GeoCoordinate{Lat=29.18     , Lon=117.12},
new GeoCoordinate{Lat=29.17     , Lon=117.51},
new GeoCoordinate{Lat=28.52     , Lon=115.22},
new GeoCoordinate{Lat=28.42     , Lon=115.23},
new GeoCoordinate{Lat=28.51     , Lon=115.33},
new GeoCoordinate{Lat=28.25     , Lon=115.23},
new GeoCoordinate{Lat=28.36     , Lon=115.55},
new GeoCoordinate{Lat=28.33     , Lon=115.57},
new GeoCoordinate{Lat=28.04     , Lon=115.33},
new GeoCoordinate{Lat=28.13     , Lon=115.49},
new GeoCoordinate{Lat=28.42     , Lon=116.41},
new GeoCoordinate{Lat=28.24     , Lon=116.17},
new GeoCoordinate{Lat=28.41     , Lon=117.05},
new GeoCoordinate{Lat=28.13     , Lon=116.48},
new GeoCoordinate{Lat=28.16 , Lon=116.36},
new GeoCoordinate{Lat=27.53     , Lon=116.15},
new GeoCoordinate{Lat=29.01 , Lon=117.06},
new GeoCoordinate{Lat=28.57     , Lon=117.35},
new GeoCoordinate{Lat=28.28     , Lon=117.55},
new GeoCoordinate{Lat=28.24     , Lon=117.26},
new GeoCoordinate{Lat=28.25     , Lon=117.36},
new GeoCoordinate{Lat=28.18     , Lon=117.14},
new GeoCoordinate{Lat=28.19     , Lon=117.03},
new GeoCoordinate{Lat=28.17 , Lon=117.42},
new GeoCoordinate{Lat=28.41     , Lon=118.15},
new GeoCoordinate{Lat=28.27     , Lon=118.12},
new GeoCoordinate{Lat=28.27     , Lon=117.59},
new GeoCoordinate{Lat=28.42     , Lon=115.50},
new GeoCoordinate{Lat=27.46     , Lon=115.24},
new GeoCoordinate{Lat=27.35     , Lon=115.21},
new GeoCoordinate{Lat=27.20     , Lon=115.25},
new GeoCoordinate{Lat=27.26     , Lon=115.50},
new GeoCoordinate{Lat=27.13     , Lon=115.08},
new GeoCoordinate{Lat=27.46     , Lon=116.03},
new GeoCoordinate{Lat=27.55     , Lon=116.47},
new GeoCoordinate{Lat=27.43     , Lon=117.04},
new GeoCoordinate{Lat=27.33     , Lon=116.14},
new GeoCoordinate{Lat=27.35     , Lon=116.39},
new GeoCoordinate{Lat=27.13     , Lon=116.32},
new GeoCoordinate{Lat=27.18     , Lon=116.56},
new GeoCoordinate{Lat=26.17     , Lon=115.21},
new GeoCoordinate{Lat=26.29     , Lon=116.01},
new GeoCoordinate{Lat=26.51     , Lon=116.20},
new GeoCoordinate{Lat=26.21     , Lon=116.21},
new GeoCoordinate{Lat=25.54     , Lon=116.02},
new GeoCoordinate{Lat=25.58     , Lon=115.25},
new GeoCoordinate{Lat=25.36     , Lon=115.48},
new GeoCoordinate{Lat=25.09     , Lon=115.24},
new GeoCoordinate{Lat=24.45 , Lon=114.31},
new GeoCoordinate{Lat=24.52     , Lon=114.48},
new GeoCoordinate{Lat=24.47     , Lon=115.02},
new GeoCoordinate{Lat=24.57     , Lon=115.39},
new GeoCoordinate{Lat=29.22     , Lon=117.13},
new GeoCoordinate{Lat=27.38     , Lon=114.01},
new GeoCoordinate{Lat=27.53     , Lon=113.49},
new GeoCoordinate{Lat=29.37     , Lon=115.54},
new GeoCoordinate{Lat=42.23     , Lon=122.33},
new GeoCoordinate{Lat=42.04     , Lon=121.45},
new GeoCoordinate{Lat=42.47     , Lon=124.06},
new GeoCoordinate{Lat=42.44     , Lon=123.21},
new GeoCoordinate{Lat=42.30     , Lon=123.24},
new GeoCoordinate{Lat=42.02 , Lon=123.32},
new GeoCoordinate{Lat=42.18     , Lon=123.52},
new GeoCoordinate{Lat=42.44     , Lon=124.44},
new GeoCoordinate{Lat=42.32     , Lon=124.03},
new GeoCoordinate{Lat=42.04 , Lon=124.52},
new GeoCoordinate{Lat=41.54     , Lon=119.43},
new GeoCoordinate{Lat=41.49     , Lon=120.47},
new GeoCoordinate{Lat=41.33     , Lon=120.26},
new GeoCoordinate{Lat=41.11     , Lon=120.22},
new GeoCoordinate{Lat=41.25 , Lon=119.37},
new GeoCoordinate{Lat=41.15     , Lon=119.23},
new GeoCoordinate{Lat=41.07     , Lon=119.45},
new GeoCoordinate{Lat=41.08 , Lon=121.20},
new GeoCoordinate{Lat=41.36 , Lon=121.45},
new GeoCoordinate{Lat=41.31     , Lon=122.42},
new GeoCoordinate{Lat=41.58     , Lon=122.51},
new GeoCoordinate{Lat=41.31     , Lon=121.15},
new GeoCoordinate{Lat=41.41     , Lon=122.05},
new GeoCoordinate{Lat=41.25     , Lon=122.26},
new GeoCoordinate{Lat=41.08     , Lon=121.07},
new GeoCoordinate{Lat=41.16     , Lon=122.02},
new GeoCoordinate{Lat=41.05     , Lon=123.00},
new GeoCoordinate{Lat=41.40     , Lon=123.19},
new GeoCoordinate{Lat=41.44     , Lon=123.31},
new GeoCoordinate{Lat=41.12     , Lon=123.04},
new GeoCoordinate{Lat=41.18     , Lon=123.47},
new GeoCoordinate{Lat=41.15     , Lon=123.11},
new GeoCoordinate{Lat=41.25     , Lon=123.22},
new GeoCoordinate{Lat=41.18     , Lon=124.08},
new GeoCoordinate{Lat=41.55     , Lon=124.04},
new GeoCoordinate{Lat=41.44     , Lon=125.03},
new GeoCoordinate{Lat=41.17     , Lon=125.21},
new GeoCoordinate{Lat=40.48     , Lon=119.48},
new GeoCoordinate{Lat=40.52     , Lon=120.58},
new GeoCoordinate{Lat=40.20     , Lon=120.18},
new GeoCoordinate{Lat=40.37     , Lon=120.39},
new GeoCoordinate{Lat=41.01     , Lon=122.03},
new GeoCoordinate{Lat=40.40     , Lon=122.10},
new GeoCoordinate{Lat=40.53     , Lon=122.45},
new GeoCoordinate{Lat=40.25     , Lon=122.21},
new GeoCoordinate{Lat=40.34     , Lon=122.27},
new GeoCoordinate{Lat=40.11     , Lon=122.09},
new GeoCoordinate{Lat=40.53     , Lon=123.54},
new GeoCoordinate{Lat=40.20     , Lon=123.17},
new GeoCoordinate{Lat=40.43     , Lon=124.47},
new GeoCoordinate{Lat=40.28     , Lon=124.03},
new GeoCoordinate{Lat=40.02     , Lon=124.20},
new GeoCoordinate{Lat=39.38     , Lon=122.01},
new GeoCoordinate{Lat=39.04 , Lon=121.45},
new GeoCoordinate{Lat=39.25     , Lon=121.57},
new GeoCoordinate{Lat=39.24     , Lon=122.20},
new GeoCoordinate{Lat=39.16     , Lon=122.35},
new GeoCoordinate{Lat=39.43     , Lon=122.57},
new GeoCoordinate{Lat=39.52     , Lon=124.09},
new GeoCoordinate{Lat=38.49     , Lon=121.14},
new GeoCoordinate{Lat=38.54     , Lon=121.38},
new GeoCoordinate{Lat=39.36 , Lon=121.28},
new GeoCoordinate{Lat=50.15     , Lon=120.11},
new GeoCoordinate{Lat=50.47     , Lon=121.31},
new GeoCoordinate{Lat=50.29     , Lon=121.41},
new GeoCoordinate{Lat=50.35     , Lon=123.44},
new GeoCoordinate{Lat=49.35     , Lon=117.19},
new GeoCoordinate{Lat=49.19     , Lon=119.26},
new GeoCoordinate{Lat=49.09     , Lon=119.45},
new GeoCoordinate{Lat=49.17     , Lon=120.42},
new GeoCoordinate{Lat=49.15     , Lon=119.42},
new GeoCoordinate{Lat=49.12     , Lon=123.43},
new GeoCoordinate{Lat=48.41     , Lon=116.49},
new GeoCoordinate{Lat=48.13     , Lon=118.16},
new GeoCoordinate{Lat=48.46     , Lon=121.55},
new GeoCoordinate{Lat=48.00     , Lon=122.44},
new GeoCoordinate{Lat=48.29     , Lon=124.29},
new GeoCoordinate{Lat=48.08     , Lon=123.29},
new GeoCoordinate{Lat=47.10     , Lon=119.56},
new GeoCoordinate{Lat=46.43     , Lon=122.05},
new GeoCoordinate{Lat=46.43     , Lon=122.54},
new GeoCoordinate{Lat=46.36     , Lon=121.13},
new GeoCoordinate{Lat=46.05     , Lon=122.03},
new GeoCoordinate{Lat=45.43     , Lon=118.50},
new GeoCoordinate{Lat=45.31     , Lon=116.58},
new GeoCoordinate{Lat=45.33     , Lon=119.39},
new GeoCoordinate{Lat=45.04     , Lon=120.20},
new GeoCoordinate{Lat=45.23     , Lon=121.35},
new GeoCoordinate{Lat=45.04 , Lon=121.29},
new GeoCoordinate{Lat=41.57     , Lon=101.04},
new GeoCoordinate{Lat=40.10     , Lon=104.48},
new GeoCoordinate{Lat=39.25     , Lon=102.47},
new GeoCoordinate{Lat=39.13     , Lon=101.41},
new GeoCoordinate{Lat=39.26     , Lon=106.39},
new GeoCoordinate{Lat=43.38 , Lon=111.56},
new GeoCoordinate{Lat=44.37     , Lon=114.09},
new GeoCoordinate{Lat=42.32     , Lon=110.08},
new GeoCoordinate{Lat=44.01     , Lon=115.00},
new GeoCoordinate{Lat=43.51 , Lon=113.38},
new GeoCoordinate{Lat=42.45     , Lon=112.38},
new GeoCoordinate{Lat=42.24     , Lon=112.54},
new GeoCoordinate{Lat=42.14     , Lon=113.50},
new GeoCoordinate{Lat=41.04     , Lon=107.03},
new GeoCoordinate{Lat=41.34     , Lon=108.31},
new GeoCoordinate{Lat=41.05     , Lon=108.28},
new GeoCoordinate{Lat=41.46     , Lon=109.58},
new GeoCoordinate{Lat=41.01     , Lon=109.08},
new GeoCoordinate{Lat=41.42     , Lon=110.26},
new GeoCoordinate{Lat=41.02 , Lon=110.03},
new GeoCoordinate{Lat=41.32     , Lon=111.41},
new GeoCoordinate{Lat=41.19     , Lon=111.14},
new GeoCoordinate{Lat=41.05     , Lon=111.28},
new GeoCoordinate{Lat=41.17     , Lon=112.37},
new GeoCoordinate{Lat=41.27     , Lon=113.11},
new GeoCoordinate{Lat=41.35     , Lon=113.32},
new GeoCoordinate{Lat=41.54     , Lon=114.00},
new GeoCoordinate{Lat=40.20     , Lon=107.00},
new GeoCoordinate{Lat=40.51     , Lon=107.07},
new GeoCoordinate{Lat=40.44     , Lon=108.39},
new GeoCoordinate{Lat=40.32 , Lon=109.53},
new GeoCoordinate{Lat=40.33     , Lon=110.32},
new GeoCoordinate{Lat=40.24     , Lon=110.02},
new GeoCoordinate{Lat=40.51     , Lon=111.34},
new GeoCoordinate{Lat=40.43     , Lon=111.10},
new GeoCoordinate{Lat=40.45     , Lon=111.42},
new GeoCoordinate{Lat=40.15     , Lon=111.15},
new GeoCoordinate{Lat=40.24     , Lon=111.49},
new GeoCoordinate{Lat=40.52     , Lon=112.34},
new GeoCoordinate{Lat=40.31     , Lon=112.28},
new GeoCoordinate{Lat=41.02     , Lon=113.04},
new GeoCoordinate{Lat=40.48     , Lon=113.13},
new GeoCoordinate{Lat=40.52     , Lon=113.50},
new GeoCoordinate{Lat=40.27     , Lon=113.09},
new GeoCoordinate{Lat=39.47     , Lon=105.45},
new GeoCoordinate{Lat=37.53     , Lon=105.24},
new GeoCoordinate{Lat=39.48     , Lon=106.48},
new GeoCoordinate{Lat=40.44     , Lon=107.22},
new GeoCoordinate{Lat=40.03     , Lon=107.50},
new GeoCoordinate{Lat=39.05 , Lon=107.58},
new GeoCoordinate{Lat=39.49 , Lon=108.43},
new GeoCoordinate{Lat=39.50     , Lon=109.59},
new GeoCoordinate{Lat=39.34 , Lon=109.43},
new GeoCoordinate{Lat=39.06     , Lon=109.02},
new GeoCoordinate{Lat=39.52     , Lon=111.13},
new GeoCoordinate{Lat=39.55     , Lon=111.40},
new GeoCoordinate{Lat=38.50     , Lon=105.40},
new GeoCoordinate{Lat=38.36     , Lon=108.50},
new GeoCoordinate{Lat=38.11     , Lon=107.29},
new GeoCoordinate{Lat=37.51     , Lon=108.43},
new GeoCoordinate{Lat=44.34     , Lon=117.38},
new GeoCoordinate{Lat=44.27     , Lon=119.18},
new GeoCoordinate{Lat=44.34     , Lon=120.54},
new GeoCoordinate{Lat=43.59     , Lon=119.24},
new GeoCoordinate{Lat=44.54     , Lon=121.49},
new GeoCoordinate{Lat=44.02     , Lon=122.01},
new GeoCoordinate{Lat=44.08     , Lon=123.17},
new GeoCoordinate{Lat=43.57     , Lon=116.07},
new GeoCoordinate{Lat=43.32     , Lon=118.39},
new GeoCoordinate{Lat=43.38     , Lon=118.02},
new GeoCoordinate{Lat=43.15     , Lon=117.32},
new GeoCoordinate{Lat=43.52     , Lon=120.02},
new GeoCoordinate{Lat=42.24     , Lon=121.04},
new GeoCoordinate{Lat=43.36     , Lon=121.17},
new GeoCoordinate{Lat=43.36     , Lon=122.16},
new GeoCoordinate{Lat=42.18     , Lon=115.00},
new GeoCoordinate{Lat=42.14     , Lon=116.00},
new GeoCoordinate{Lat=42.11     , Lon=116.28},
new GeoCoordinate{Lat=42.56 , Lon=119.01},
new GeoCoordinate{Lat=42.35     , Lon=118.25},
new GeoCoordinate{Lat=42.18     , Lon=118.50},
new GeoCoordinate{Lat=42.51     , Lon=120.39},
new GeoCoordinate{Lat=42.18 , Lon=119.57},
new GeoCoordinate{Lat=42.20     , Lon=120.42},
new GeoCoordinate{Lat=42.58     , Lon=122.21},
new GeoCoordinate{Lat=42.44     , Lon=121.45},
new GeoCoordinate{Lat=41.53     , Lon=115.16},
new GeoCoordinate{Lat=41.56     , Lon=118.42},
new GeoCoordinate{Lat=41.31     , Lon=118.45},
new GeoCoordinate{Lat=41.36     , Lon=119.18},
new GeoCoordinate{Lat=41.24     , Lon=106.24},
new GeoCoordinate{Lat=39.16     , Lon=106.20},
new GeoCoordinate{Lat=39.02     , Lon=106.21},
new GeoCoordinate{Lat=39.13     , Lon=106.46},
new GeoCoordinate{Lat=38.50     , Lon=106.22},
new GeoCoordinate{Lat=38.34     , Lon=106.21},
new GeoCoordinate{Lat=38.53 , Lon=106.33},
new GeoCoordinate{Lat=37.59     , Lon=106.11},
new GeoCoordinate{Lat=38.28     , Lon=106.12},
new GeoCoordinate{Lat=38.48     , Lon=106.42},
new GeoCoordinate{Lat=38.02     , Lon=106.05},
new GeoCoordinate{Lat=38.17     , Lon=106.15},
new GeoCoordinate{Lat=38.07     , Lon=106.18},
new GeoCoordinate{Lat=37.32     , Lon=105.11},
new GeoCoordinate{Lat=37.29     , Lon=105.41},
new GeoCoordinate{Lat=36.56     , Lon=105.15},
new GeoCoordinate{Lat=37.48     , Lon=107.23},
new GeoCoordinate{Lat=37.10     , Lon=107.07},
new GeoCoordinate{Lat=36.34     , Lon=105.39},
new GeoCoordinate{Lat=36.58     , Lon=105.54},
new GeoCoordinate{Lat=36.00     , Lon=106.16},
new GeoCoordinate{Lat=37.17     , Lon=106.29},
new GeoCoordinate{Lat=35.58     , Lon=105.43},
new GeoCoordinate{Lat=35.40     , Lon=106.12},
new GeoCoordinate{Lat=35.37     , Lon=106.07},
new GeoCoordinate{Lat=35.30     , Lon=106.19},
new GeoCoordinate{Lat=38.15     , Lon= 90.51},
new GeoCoordinate{Lat=38.45     , Lon= 93.20},
new GeoCoordinate{Lat=38.49 , Lon= 98.25},
new GeoCoordinate{Lat=38.26 , Lon= 99.36},
new GeoCoordinate{Lat=38.11     , Lon=100.15},
new GeoCoordinate{Lat=36.48     , Lon= 93.41},
new GeoCoordinate{Lat=37.51     , Lon= 95.21},
new GeoCoordinate{Lat=37.22     , Lon= 97.23},
new GeoCoordinate{Lat=37.18 , Lon= 99.01},
new GeoCoordinate{Lat=37.20     , Lon=100.08},
new GeoCoordinate{Lat=37.23     , Lon=101.37},
new GeoCoordinate{Lat=36.25     , Lon= 94.55},
new GeoCoordinate{Lat=36.26     , Lon= 96.26},
new GeoCoordinate{Lat=36.56 , Lon= 98.29},
new GeoCoordinate{Lat=36.18     , Lon= 98.06},
new GeoCoordinate{Lat=36.47     , Lon= 99.05},
new GeoCoordinate{Lat=36.54 , Lon=100.59},
new GeoCoordinate{Lat=36.41     , Lon=101.15},
new GeoCoordinate{Lat=36.16     , Lon=100.37},
new GeoCoordinate{Lat=36.57     , Lon=101.41},
new GeoCoordinate{Lat=36.49     , Lon=101.57},
new GeoCoordinate{Lat=36.44 , Lon=101.45},
new GeoCoordinate{Lat=36.01 , Lon=101.22},
new GeoCoordinate{Lat=36.30 , Lon=101.35},
new GeoCoordinate{Lat=36.29     , Lon=102.25},
new GeoCoordinate{Lat=36.30     , Lon=102.06},
new GeoCoordinate{Lat=36.20 , Lon=102.50},
new GeoCoordinate{Lat=36.06 , Lon=102.15},
new GeoCoordinate{Lat=35.13     , Lon= 93.05},
new GeoCoordinate{Lat=35.35     , Lon= 99.59},
new GeoCoordinate{Lat=35.35     , Lon=100.44},
new GeoCoordinate{Lat=35.15     , Lon=100.36},
new GeoCoordinate{Lat=35.56     , Lon=102.01},
new GeoCoordinate{Lat=35.02     , Lon=101.28},
new GeoCoordinate{Lat=35.51 , Lon=102.27},
new GeoCoordinate{Lat=35.33     , Lon=102.02},
new GeoCoordinate{Lat=34.13     , Lon= 92.26},
new GeoCoordinate{Lat=33.51     , Lon= 95.37},
new GeoCoordinate{Lat=32.53 , Lon= 95.17},
new GeoCoordinate{Lat=34.07 , Lon= 95.48},
new GeoCoordinate{Lat=33.00 , Lon= 96.58},
new GeoCoordinate{Lat=34.55     , Lon= 98.13},
new GeoCoordinate{Lat=33.48     , Lon= 97.08},
new GeoCoordinate{Lat=34.29 , Lon=100.14},
new GeoCoordinate{Lat=33.58     , Lon= 99.54},
new GeoCoordinate{Lat=33.45     , Lon= 99.39},
new GeoCoordinate{Lat=34.44     , Lon=101.36},
new GeoCoordinate{Lat=33.26     , Lon=101.29},
new GeoCoordinate{Lat=32.12     , Lon= 96.28},
new GeoCoordinate{Lat=32.56     , Lon=100.45},
new GeoCoordinate{Lat=37.14     , Lon=116.04},
new GeoCoordinate{Lat=37.12     , Lon=116.49},
new GeoCoordinate{Lat=37.27     , Lon=116.20},
new GeoCoordinate{Lat=37.19     , Lon=116.31},
new GeoCoordinate{Lat=37.40     , Lon=116.48},
new GeoCoordinate{Lat=37.09 , Lon=116.28},
new GeoCoordinate{Lat=37.45     , Lon=117.38},
new GeoCoordinate{Lat=37.39     , Lon=117.34},
new GeoCoordinate{Lat=37.19     , Lon=117.08},
new GeoCoordinate{Lat=37.30     , Lon=117.32},
new GeoCoordinate{Lat=37.42     , Lon=117.11},
new GeoCoordinate{Lat=36.41     , Lon=117.33},
new GeoCoordinate{Lat=37.46     , Lon=117.21},
new GeoCoordinate{Lat=37.10     , Lon=117.52},
new GeoCoordinate{Lat=37.42     , Lon=118.08},
new GeoCoordinate{Lat=37.29     , Lon=118.13},
new GeoCoordinate{Lat=37.52     , Lon=118.31},
new GeoCoordinate{Lat=37.21     , Lon=118.00},
new GeoCoordinate{Lat=37.26     , Lon=118.40},
new GeoCoordinate{Lat=37.09     , Lon=118.09},
new GeoCoordinate{Lat=37.03     , Lon=118.24},
new GeoCoordinate{Lat=37.35     , Lon=118.33},
new GeoCoordinate{Lat=37.11     , Lon=119.57},
new GeoCoordinate{Lat=37.56     , Lon=120.44},
new GeoCoordinate{Lat=37.48     , Lon=120.46},
new GeoCoordinate{Lat=37.38     , Lon=120.20},
new GeoCoordinate{Lat=37.20 , Lon=120.23},
new GeoCoordinate{Lat=37.19 , Lon=120.51},
new GeoCoordinate{Lat=37.29     , Lon=121.14},
new GeoCoordinate{Lat=37.29     , Lon=121.26},
new GeoCoordinate{Lat=37.26 , Lon=121.35},
new GeoCoordinate{Lat=37.28     , Lon=122.08},
new GeoCoordinate{Lat=37.24     , Lon=122.42},
new GeoCoordinate{Lat=37.12     , Lon=122.04},
new GeoCoordinate{Lat=37.10     , Lon=122.29},
new GeoCoordinate{Lat=36.49     , Lon=115.44},
new GeoCoordinate{Lat=36.59     , Lon=116.01},
new GeoCoordinate{Lat=36.27 , Lon=115.28},
new GeoCoordinate{Lat=36.25     , Lon=115.58},
new GeoCoordinate{Lat=36.09 , Lon=115.46},
new GeoCoordinate{Lat=36.14     , Lon=115.38},
new GeoCoordinate{Lat=36.52     , Lon=116.16},
new GeoCoordinate{Lat=36.56     , Lon=116.37},
new GeoCoordinate{Lat=36.44     , Lon=116.43},
new GeoCoordinate{Lat=36.32     , Lon=116.15},
new GeoCoordinate{Lat=36.23     , Lon=116.16},
new GeoCoordinate{Lat=36.31     , Lon=116.48},
new GeoCoordinate{Lat=36.15     , Lon=116.25},
new GeoCoordinate{Lat=36.11     , Lon=116.48},
new GeoCoordinate{Lat=37.00     , Lon=117.12},
new GeoCoordinate{Lat=36.52     , Lon=117.44},
new GeoCoordinate{Lat=36.36     , Lon=117.00},
new GeoCoordinate{Lat=36.38     , Lon=117.57},
new GeoCoordinate{Lat=36.31     , Lon=117.51},
new GeoCoordinate{Lat=36.15     , Lon=117.06},
new GeoCoordinate{Lat=36.10     , Lon=117.09},
new GeoCoordinate{Lat=36.14     , Lon=117.41},
new GeoCoordinate{Lat=36.46     , Lon=117.52},
new GeoCoordinate{Lat=36.49     , Lon=117.56},
new GeoCoordinate{Lat=36.39     , Lon=118.30},
new GeoCoordinate{Lat=36.52     , Lon=118.48},
new GeoCoordinate{Lat=36.58     , Lon=118.05},
new GeoCoordinate{Lat=36.52     , Lon=118.17},
new GeoCoordinate{Lat=36.30     , Lon=118.33},
new GeoCoordinate{Lat=36.11     , Lon=118.10},
new GeoCoordinate{Lat=36.41     , Lon=118.50},
new GeoCoordinate{Lat=36.52     , Lon=119.25},
new GeoCoordinate{Lat=36.47     , Lon=120.00},
new GeoCoordinate{Lat=36.45     , Lon=119.12},
new GeoCoordinate{Lat=36.25     , Lon=119.11},
new GeoCoordinate{Lat=36.23     , Lon=119.44},
new GeoCoordinate{Lat=35.59     , Lon=119.25},
new GeoCoordinate{Lat=36.13     , Lon=120.02},
new GeoCoordinate{Lat=36.54     , Lon=120.34},
new GeoCoordinate{Lat=36.58     , Lon=120.44},
new GeoCoordinate{Lat=36.10     , Lon=120.25},
new GeoCoordinate{Lat=36.24     , Lon=120.23},
new GeoCoordinate{Lat=36.04     , Lon=120.20},
new GeoCoordinate{Lat=36.56     , Lon=121.32},
new GeoCoordinate{Lat=36.47     , Lon=121.11},
new GeoCoordinate{Lat=36.56     , Lon=122.29},
new GeoCoordinate{Lat=35.35     , Lon=115.31},
new GeoCoordinate{Lat=35.35     , Lon=115.56},
new GeoCoordinate{Lat=35.15     , Lon=115.25},
new GeoCoordinate{Lat=34.59     , Lon=116.39},
new GeoCoordinate{Lat=35.16     , Lon=115.06},
new GeoCoordinate{Lat=35.05     , Lon=115.31},
new GeoCoordinate{Lat=35.49     , Lon=116.03},
new GeoCoordinate{Lat=35.56 , Lon=116.24},
new GeoCoordinate{Lat=35.46     , Lon=116.30},
new GeoCoordinate{Lat=35.45     , Lon=116.49},
new GeoCoordinate{Lat=35.25     , Lon=116.05},
new GeoCoordinate{Lat=35.26     , Lon=116.36},
new GeoCoordinate{Lat=35.34     , Lon=116.51},
new GeoCoordinate{Lat=35.08     , Lon=116.20},
new GeoCoordinate{Lat=35.36     , Lon=116.58},
new GeoCoordinate{Lat=35.23 , Lon=116.59},
new GeoCoordinate{Lat=35.39     , Lon=117.17},
new GeoCoordinate{Lat=35.25     , Lon=116.20},
new GeoCoordinate{Lat=35.52     , Lon=117.45},
new GeoCoordinate{Lat=35.43     , Lon=117.56},
new GeoCoordinate{Lat=35.30     , Lon=117.41},
new GeoCoordinate{Lat=35.06     , Lon=117.12},
new GeoCoordinate{Lat=35.16     , Lon=117.58},
new GeoCoordinate{Lat=35.46     , Lon=118.39},
new GeoCoordinate{Lat=35.32     , Lon=118.26},
new GeoCoordinate{Lat=35.34     , Lon=118.50},
new GeoCoordinate{Lat=35.03     , Lon=118.24},
new GeoCoordinate{Lat=35.10     , Lon=118.49},
new GeoCoordinate{Lat=35.45     , Lon=119.12},
new GeoCoordinate{Lat=35.53 , Lon=120.00},
new GeoCoordinate{Lat=35.28     , Lon=119.33},
new GeoCoordinate{Lat=34.51     , Lon=115.35},
new GeoCoordinate{Lat=34.57     , Lon=115.53},
new GeoCoordinate{Lat=34.48     , Lon=116.04},
new GeoCoordinate{Lat=34.48     , Lon=117.08},
new GeoCoordinate{Lat=34.47     , Lon=117.17},
new GeoCoordinate{Lat=34.45     , Lon=117.34},
new GeoCoordinate{Lat=34.52     , Lon=117.35},
new GeoCoordinate{Lat=34.34     , Lon=117.44},
new GeoCoordinate{Lat=34.53 , Lon=118.01},
new GeoCoordinate{Lat=34.57     , Lon=118.39},
new GeoCoordinate{Lat=34.36     , Lon=118.19},
new GeoCoordinate{Lat=40.00     , Lon=112.27},
new GeoCoordinate{Lat=40.22     , Lon=113.46},
new GeoCoordinate{Lat=40.05 , Lon=113.25},
new GeoCoordinate{Lat=40.02     , Lon=113.35},
new GeoCoordinate{Lat=40.26     , Lon=114.03},
new GeoCoordinate{Lat=39.22     , Lon=111.13},
new GeoCoordinate{Lat=39.26     , Lon=111.30},
new GeoCoordinate{Lat=40.00     , Lon=112.42},
new GeoCoordinate{Lat=39.31     , Lon=112.16},
new GeoCoordinate{Lat=39.06     , Lon=112.12},
new GeoCoordinate{Lat=39.30     , Lon=112.49},
new GeoCoordinate{Lat=39.00     , Lon=112.18},
new GeoCoordinate{Lat=39.22     , Lon=112.26},
new GeoCoordinate{Lat=39.01     , Lon=112.54},
new GeoCoordinate{Lat=39.49     , Lon=113.06},
new GeoCoordinate{Lat=39.43     , Lon=113.40},
new GeoCoordinate{Lat=39.34     , Lon=113.10},
new GeoCoordinate{Lat=39.10     , Lon=113.16},
new GeoCoordinate{Lat=38.57     , Lon=113.31},
new GeoCoordinate{Lat=39.45     , Lon=114.16},
new GeoCoordinate{Lat=39.27     , Lon=114.11},
new GeoCoordinate{Lat=37.58     , Lon=111.00},
new GeoCoordinate{Lat=39.01     , Lon=111.05},
new GeoCoordinate{Lat=38.43     , Lon=111.35},
new GeoCoordinate{Lat=38.55     , Lon=111.49},
new GeoCoordinate{Lat=38.28     , Lon=111.08},
new GeoCoordinate{Lat=38.17     , Lon=111.39},
new GeoCoordinate{Lat=38.21     , Lon=111.56},
new GeoCoordinate{Lat=38.05     , Lon=111.48},
new GeoCoordinate{Lat=38.44     , Lon=112.43},
new GeoCoordinate{Lat=38.24 , Lon=112.42},
new GeoCoordinate{Lat=38.30     , Lon=112.59},
new GeoCoordinate{Lat=37.56     , Lon=112.29},
new GeoCoordinate{Lat=38.04     , Lon=112.39},
new GeoCoordinate{Lat=37.45 , Lon=112.33},
new GeoCoordinate{Lat=38.50     , Lon=113.22},
new GeoCoordinate{Lat=38.04     , Lon=113.25},
new GeoCoordinate{Lat=37.47     , Lon=113.38},
new GeoCoordinate{Lat=37.26     , Lon=110.53},
new GeoCoordinate{Lat=36.59     , Lon=110.50},
new GeoCoordinate{Lat=37.53     , Lon=111.14},
new GeoCoordinate{Lat=37.55     , Lon=112.10},
new GeoCoordinate{Lat=37.30     , Lon=111.06},
new GeoCoordinate{Lat=37.20     , Lon=111.10},
new GeoCoordinate{Lat=37.09     , Lon=111.45},
new GeoCoordinate{Lat=37.15     , Lon=111.47},
new GeoCoordinate{Lat=37.21     , Lon=112.21},
new GeoCoordinate{Lat=37.25     , Lon=112.03},
new GeoCoordinate{Lat=37.37 , Lon=112.35},
new GeoCoordinate{Lat=37.35 , Lon=112.22},
new GeoCoordinate{Lat=37.25 , Lon=112.36},
new GeoCoordinate{Lat=37.41     , Lon=112.46},
new GeoCoordinate{Lat=37.31     , Lon=112.09},
new GeoCoordinate{Lat=37.10     , Lon=112.13},
new GeoCoordinate{Lat=37.54     , Lon=113.09},
new GeoCoordinate{Lat=37.56     , Lon=113.37},
new GeoCoordinate{Lat=37.36     , Lon=113.43},
new GeoCoordinate{Lat=37.05 , Lon=113.20},
new GeoCoordinate{Lat=37.04     , Lon=112.59},
new GeoCoordinate{Lat=37.20     , Lon=113.34},
new GeoCoordinate{Lat=36.46     , Lon=110.38},
new GeoCoordinate{Lat=36.42     , Lon=110.57},
new GeoCoordinate{Lat=36.28     , Lon=110.45},
new GeoCoordinate{Lat=36.06     , Lon=110.40},
new GeoCoordinate{Lat=36.59     , Lon=111.10},
new GeoCoordinate{Lat=35.53     , Lon=111.23},
new GeoCoordinate{Lat=36.51 , Lon=111.48},
new GeoCoordinate{Lat=37.04     , Lon=111.57},
new GeoCoordinate{Lat=36.24     , Lon=111.06},
new GeoCoordinate{Lat=36.39     , Lon=111.33},
new GeoCoordinate{Lat=36.14     , Lon=111.40},
new GeoCoordinate{Lat=36.04     , Lon=111.30},
new GeoCoordinate{Lat=36.35     , Lon=111.42},
new GeoCoordinate{Lat=36.51     , Lon=112.52},
new GeoCoordinate{Lat=36.46     , Lon=112.41},
new GeoCoordinate{Lat=36.06     , Lon=112.52},
new GeoCoordinate{Lat=36.15     , Lon=111.54},
new GeoCoordinate{Lat=36.31     , Lon=112.21},
new GeoCoordinate{Lat=36.10     , Lon=112.15},
new GeoCoordinate{Lat=36.31     , Lon=113.23},
new GeoCoordinate{Lat=36.19     , Lon=112.53},
new GeoCoordinate{Lat=36.20     , Lon=113.14},
new GeoCoordinate{Lat=36.04 , Lon=113.02},
new GeoCoordinate{Lat=36.31     , Lon=113.02},
new GeoCoordinate{Lat=36.07     , Lon=113.12},
new GeoCoordinate{Lat=36.12     , Lon=113.26},
new GeoCoordinate{Lat=35.58     , Lon=110.51},
new GeoCoordinate{Lat=35.37     , Lon=110.58},
new GeoCoordinate{Lat=35.24     , Lon=110.50},
new GeoCoordinate{Lat=35.37     , Lon=110.43},
new GeoCoordinate{Lat=35.10 , Lon=110.47},
new GeoCoordinate{Lat=35.07     , Lon=111.04},
new GeoCoordinate{Lat=35.39     , Lon=111.29},
new GeoCoordinate{Lat=35.44     , Lon=111.42},
new GeoCoordinate{Lat=35.39     , Lon=111.22},
new GeoCoordinate{Lat=35.37     , Lon=111.13},
new GeoCoordinate{Lat=35.31     , Lon=111.34},
new GeoCoordinate{Lat=35.59     , Lon=111.50},
new GeoCoordinate{Lat=35.20     , Lon=111.12},
new GeoCoordinate{Lat=35.17 , Lon=111.40},
new GeoCoordinate{Lat=35.42     , Lon=112.12},
new GeoCoordinate{Lat=35.46     , Lon=112.57},
new GeoCoordinate{Lat=35.29     , Lon=112.24},
new GeoCoordinate{Lat=35.30     , Lon=112.52},
new GeoCoordinate{Lat=35.47     , Lon=113.16},
new GeoCoordinate{Lat=34.53     , Lon=110.27},
new GeoCoordinate{Lat=34.42     , Lon=110.43},
new GeoCoordinate{Lat=35.10     , Lon=111.14},
new GeoCoordinate{Lat=34.50     , Lon=111.12},
new GeoCoordinate{Lat=39.01 , Lon=111.00},
new GeoCoordinate{Lat=38.16     , Lon=109.47},
new GeoCoordinate{Lat=38.49     , Lon=110.28},
new GeoCoordinate{Lat=38.02     , Lon=110.29},
new GeoCoordinate{Lat=37.35     , Lon=107.35},
new GeoCoordinate{Lat=37.37     , Lon=108.48},
new GeoCoordinate{Lat=36.55     , Lon=108.10},
new GeoCoordinate{Lat=37.56     , Lon=109.14},
new GeoCoordinate{Lat=37.11     , Lon=109.42},
new GeoCoordinate{Lat=37.46     , Lon=110.11},
new GeoCoordinate{Lat=37.36     , Lon=110.03},
new GeoCoordinate{Lat=37.30     , Lon=110.13},
new GeoCoordinate{Lat=37.31     , Lon=110.43},
new GeoCoordinate{Lat=37.07     , Lon=110.07},
new GeoCoordinate{Lat=36.46     , Lon=108.46},
new GeoCoordinate{Lat=36.53     , Lon=109.19},
new GeoCoordinate{Lat=36.35     , Lon=109.27},
new GeoCoordinate{Lat=36.16     , Lon=109.21},
new GeoCoordinate{Lat=36.53     , Lon=110.11},
new GeoCoordinate{Lat=36.35     , Lon=110.04},
new GeoCoordinate{Lat=36.04     , Lon=110.11},
new GeoCoordinate{Lat=35.12     , Lon=107.48},
new GeoCoordinate{Lat=36.00     , Lon=109.22},
new GeoCoordinate{Lat=35.10     , Lon=108.18},
new GeoCoordinate{Lat=35.11     , Lon=109.35},
new GeoCoordinate{Lat=35.46     , Lon=109.25},
new GeoCoordinate{Lat=35.34     , Lon=109.15},
new GeoCoordinate{Lat=35.26     , Lon=109.04},
new GeoCoordinate{Lat=35.36     , Lon=109.49},
new GeoCoordinate{Lat=35.05     , Lon=109.04},
new GeoCoordinate{Lat=34.57     , Lon=109.35},
new GeoCoordinate{Lat=35.11     , Lon=109.55},
new GeoCoordinate{Lat=35.14     , Lon=110.09},
new GeoCoordinate{Lat=35.28     , Lon=110.27},
new GeoCoordinate{Lat=34.54     , Lon=106.50},
new GeoCoordinate{Lat=34.21     , Lon=107.08},
new GeoCoordinate{Lat=34.22     , Lon=107.24},
new GeoCoordinate{Lat=34.39     , Lon=107.08},
new GeoCoordinate{Lat=34.41     , Lon=107.47},
new GeoCoordinate{Lat=35.01     , Lon=108.07},
new GeoCoordinate{Lat=34.27     , Lon=107.39},
new GeoCoordinate{Lat=34.31     , Lon=107.23},
new GeoCoordinate{Lat=34.22     , Lon=107.53},
new GeoCoordinate{Lat=34.18     , Lon=107.44},
new GeoCoordinate{Lat=34.02     , Lon=107.19},
new GeoCoordinate{Lat=34.30     , Lon=108.29},
new GeoCoordinate{Lat=34.42     , Lon=108.09},
new GeoCoordinate{Lat=34.49     , Lon=108.33},
new GeoCoordinate{Lat=34.08 , Lon=108.12},
new GeoCoordinate{Lat=34.33     , Lon=108.49},
new GeoCoordinate{Lat=34.19 , Lon=108.14},
new GeoCoordinate{Lat=34.33     , Lon=108.14},
new GeoCoordinate{Lat=34.56     , Lon=108.59},
new GeoCoordinate{Lat=34.17     , Lon=108.27},
new GeoCoordinate{Lat=34.05 , Lon=108.53},
new GeoCoordinate{Lat=34.33     , Lon=109.04},
new GeoCoordinate{Lat=34.38     , Lon=108.55},
new GeoCoordinate{Lat=34.47     , Lon=109.11},
new GeoCoordinate{Lat=34.48     , Lon=109.58},
new GeoCoordinate{Lat=34.24     , Lon=109.14},
new GeoCoordinate{Lat=34.24     , Lon=109.30},
new GeoCoordinate{Lat=34.29     , Lon=110.05},
new GeoCoordinate{Lat=34.10     , Lon=109.19},
new GeoCoordinate{Lat=34.24     , Lon=108.43},
new GeoCoordinate{Lat=34.31     , Lon=109.44},
new GeoCoordinate{Lat=34.33     , Lon=110.14},
new GeoCoordinate{Lat=34.36     , Lon=110.08},
new GeoCoordinate{Lat=34.06     , Lon=110.09},
new GeoCoordinate{Lat=33.19     , Lon=106.09},
new GeoCoordinate{Lat=33.56     , Lon=106.33},
new GeoCoordinate{Lat=33.10     , Lon=106.42},
new GeoCoordinate{Lat=34.17     , Lon=108.04},
new GeoCoordinate{Lat=33.38     , Lon=106.56},
new GeoCoordinate{Lat=33.13     , Lon=107.33},
new GeoCoordinate{Lat=33.04     , Lon=107.02},
new GeoCoordinate{Lat=33.10     , Lon=107.20},
new GeoCoordinate{Lat=32.59     , Lon=107.43},
new GeoCoordinate{Lat=34.26     , Lon=108.58},
new GeoCoordinate{Lat=34.08 , Lon=108.35},
new GeoCoordinate{Lat=33.31     , Lon=107.59},
new GeoCoordinate{Lat=33.19     , Lon=108.19},
new GeoCoordinate{Lat=33.40     , Lon=109.07},
new GeoCoordinate{Lat=33.52     , Lon=109.58},
new GeoCoordinate{Lat=33.26     , Lon=109.09},
new GeoCoordinate{Lat=33.42     , Lon=110.20},
new GeoCoordinate{Lat=33.32     , Lon=110.54},
new GeoCoordinate{Lat=33.33     , Lon=109.52},
new GeoCoordinate{Lat=32.50     , Lon=106.15},
new GeoCoordinate{Lat=33.00     , Lon=106.56},
new GeoCoordinate{Lat=32.32     , Lon=108.32},
new GeoCoordinate{Lat=33.03     , Lon=108.16},
new GeoCoordinate{Lat=32.54     , Lon=108.30},
new GeoCoordinate{Lat=32.32     , Lon=107.54},
new GeoCoordinate{Lat=32.51     , Lon=109.22},
new GeoCoordinate{Lat=32.43     , Lon=109.02},
new GeoCoordinate{Lat=32.19     , Lon=108.54},
new GeoCoordinate{Lat=32.24     , Lon=109.20},
new GeoCoordinate{Lat=32.49     , Lon=110.07},
new GeoCoordinate{Lat=31.54     , Lon=109.32},
new GeoCoordinate{Lat=31.06     , Lon=121.22},
new GeoCoordinate{Lat=31.24 , Lon=121.27},
new GeoCoordinate{Lat=31.23     , Lon=121.12},
new GeoCoordinate{Lat=31.40 , Lon=121.30},
new GeoCoordinate{Lat=31.12     , Lon=121.26},
new GeoCoordinate{Lat=31.03     , Lon=121.47},
new GeoCoordinate{Lat=31.14 , Lon=121.32},
new GeoCoordinate{Lat=30.44     , Lon=121.21},
new GeoCoordinate{Lat=31.08     , Lon=121.07},
new GeoCoordinate{Lat=31.02     , Lon=121.14},
new GeoCoordinate{Lat=30.53     , Lon=121.30},
new GeoCoordinate{Lat=32.59     , Lon=980.6 },
new GeoCoordinate{Lat=33.35     , Lon=102.58},
new GeoCoordinate{Lat=33.16     , Lon=104.15},
new GeoCoordinate{Lat=31.48     , Lon=983.5 },
new GeoCoordinate{Lat=31.37     , Lon=100.00},
new GeoCoordinate{Lat=31.13     , Lon=985.0 },
new GeoCoordinate{Lat=32.17     , Lon=100.20},
new GeoCoordinate{Lat=31.24     , Lon=100.40},
new GeoCoordinate{Lat=32.16     , Lon=100.58},
new GeoCoordinate{Lat=30.59     , Lon=101.07},
new GeoCoordinate{Lat=31.29     , Lon=102.04},
new GeoCoordinate{Lat=32.54     , Lon=101.42},
new GeoCoordinate{Lat=31.54     , Lon=102.14},
new GeoCoordinate{Lat=32.48     , Lon=102.33},
new GeoCoordinate{Lat=31.00 , Lon=102.21},
new GeoCoordinate{Lat=31.41     , Lon=103.51},
new GeoCoordinate{Lat=30.41     , Lon=103.42},
new GeoCoordinate{Lat=32.40     , Lon=103.36},
new GeoCoordinate{Lat=31.30 , Lon=103.37},
new GeoCoordinate{Lat=31.26     , Lon=103.10},
new GeoCoordinate{Lat=32.05     , Lon=102.59},
new GeoCoordinate{Lat=31.20     , Lon=104.12},
new GeoCoordinate{Lat=30.45     , Lon=103.52},
new GeoCoordinate{Lat=31.00     , Lon=103.40},
new GeoCoordinate{Lat=30.59     , Lon=103.56},
new GeoCoordinate{Lat=31.33 , Lon=104.33},
new GeoCoordinate{Lat=32.25 , Lon=104.31},
new GeoCoordinate{Lat=31.38 , Lon=104.27},
new GeoCoordinate{Lat=31.48 , Lon=104.44},
new GeoCoordinate{Lat=31.27 , Lon=104.44},
new GeoCoordinate{Lat=31.09     , Lon=104.10},
new GeoCoordinate{Lat=31.19     , Lon=104.30},
new GeoCoordinate{Lat=31.02     , Lon=104.41},
new GeoCoordinate{Lat=30.00     , Lon= 99.06},
new GeoCoordinate{Lat=30.56     , Lon=100.19},
new GeoCoordinate{Lat=30.00     , Lon=100.16},
new GeoCoordinate{Lat=30.53     , Lon=101.53},
new GeoCoordinate{Lat=30.02     , Lon=101.01},
new GeoCoordinate{Lat=30.49     , Lon=103.53},
new GeoCoordinate{Lat=30.23 , Lon=102.49},
new GeoCoordinate{Lat=30.27     , Lon=103.49},
new GeoCoordinate{Lat=30.04     , Lon=102.46},
new GeoCoordinate{Lat=30.10     , Lon=102.57},
new GeoCoordinate{Lat=30.05     , Lon=103.07},
new GeoCoordinate{Lat=30.11     , Lon=103.30},
new GeoCoordinate{Lat=30.27     , Lon=103.26},
new GeoCoordinate{Lat=30.36     , Lon=103.31},
new GeoCoordinate{Lat=30.37     , Lon=104.16},
new GeoCoordinate{Lat=29.59     , Lon=103.00},
new GeoCoordinate{Lat=30.35     , Lon=103.55},
new GeoCoordinate{Lat=30.12     , Lon=103.52},
new GeoCoordinate{Lat=30.47     , Lon=104.11},
new GeoCoordinate{Lat=30.56 , Lon=104.17},
new GeoCoordinate{Lat=30.23     , Lon=104.33},
new GeoCoordinate{Lat=30.49     , Lon=104.26},
new GeoCoordinate{Lat=30.01     , Lon=104.09},
new GeoCoordinate{Lat=30.08     , Lon=104.36},
new GeoCoordinate{Lat=29.03     , Lon=100.18},
new GeoCoordinate{Lat=29.53     , Lon=102.13},
new GeoCoordinate{Lat=29.47     , Lon=102.51},
new GeoCoordinate{Lat=30.03     , Lon=101.58},
new GeoCoordinate{Lat=29.20     , Lon=102.38},
new GeoCoordinate{Lat=29.14     , Lon=102.21},
new GeoCoordinate{Lat=29.53     , Lon=103.21},
new GeoCoordinate{Lat=30.04     , Lon=103.30},
new GeoCoordinate{Lat=29.45     , Lon=103.33},
new GeoCoordinate{Lat=29.50     , Lon=103.52},
new GeoCoordinate{Lat=29.36     , Lon=103.29},
new GeoCoordinate{Lat=29.31     , Lon=103.20},
new GeoCoordinate{Lat=29.34     , Lon=103.45},
new GeoCoordinate{Lat=29.14 , Lon=103.16},
new GeoCoordinate{Lat=29.12     , Lon=103.57},
new GeoCoordinate{Lat=29.40     , Lon=104.04},
new GeoCoordinate{Lat=30.05     , Lon=103.49},
new GeoCoordinate{Lat=29.46     , Lon=104.51},
new GeoCoordinate{Lat=29.27     , Lon=104.26},
new GeoCoordinate{Lat=29.31     , Lon=104.40},
new GeoCoordinate{Lat=29.21     , Lon=104.46},
new GeoCoordinate{Lat=29.11     , Lon=104.59},
new GeoCoordinate{Lat=28.43     , Lon= 99.17},
new GeoCoordinate{Lat=28.56     , Lon= 99.48},
new GeoCoordinate{Lat=27.56     , Lon=101.16},
new GeoCoordinate{Lat=29.00     , Lon=101.30},
new GeoCoordinate{Lat=28.57     , Lon=102.46},
new GeoCoordinate{Lat=28.33     , Lon=102.10},
new GeoCoordinate{Lat=28.39     , Lon=102.31},
new GeoCoordinate{Lat=28.18     , Lon=102.26},
new GeoCoordinate{Lat=28.00     , Lon=102.51},
new GeoCoordinate{Lat=28.49     , Lon=103.32},
new GeoCoordinate{Lat=28.16     , Lon=103.35},
new GeoCoordinate{Lat=28.20     , Lon=103.08},
new GeoCoordinate{Lat=28.57     , Lon=103.54},
new GeoCoordinate{Lat=28.42     , Lon=104.34},
new GeoCoordinate{Lat=28.48     , Lon=104.36},
new GeoCoordinate{Lat=28.51     , Lon=104.58},
new GeoCoordinate{Lat=28.50     , Lon=104.20},
new GeoCoordinate{Lat=28.18     , Lon=105.14},
new GeoCoordinate{Lat=28.09     , Lon=104.30},
new GeoCoordinate{Lat=28.23     , Lon=104.47},
new GeoCoordinate{Lat=27.26     , Lon=101.31},
new GeoCoordinate{Lat=27.25     , Lon=102.11},
new GeoCoordinate{Lat=27.54     , Lon=102.16},
new GeoCoordinate{Lat=27.22     , Lon=102.33},
new GeoCoordinate{Lat=27.04     , Lon=102.45},
new GeoCoordinate{Lat=27.43     , Lon=102.48},
new GeoCoordinate{Lat=27.42     , Lon=103.15},
new GeoCoordinate{Lat=28.26     , Lon=104.31},
new GeoCoordinate{Lat=28.35     , Lon=104.55},
new GeoCoordinate{Lat=26.41     , Lon=101.51},
new GeoCoordinate{Lat=26.34     , Lon=101.43},
new GeoCoordinate{Lat=26.55     , Lon=102.07},
new GeoCoordinate{Lat=26.39     , Lon=102.15},
new GeoCoordinate{Lat=26.30     , Lon=101.44},
new GeoCoordinate{Lat=26.39     , Lon=102.35},
new GeoCoordinate{Lat=32.34     , Lon=105.13},
new GeoCoordinate{Lat=32.25     , Lon=105.54},
new GeoCoordinate{Lat=32.17 , Lon=105.31},
new GeoCoordinate{Lat=32.21     , Lon=106.50},
new GeoCoordinate{Lat=32.14     , Lon=106.17},
new GeoCoordinate{Lat=32.04     , Lon=108.02},
new GeoCoordinate{Lat=31.44     , Lon=105.55},
new GeoCoordinate{Lat=31.40 , Lon=105.10},
new GeoCoordinate{Lat=31.35     , Lon=105.59},
new GeoCoordinate{Lat=31.06 , Lon=105.05},
new GeoCoordinate{Lat=31.13 , Lon=105.23},
new GeoCoordinate{Lat=30.59     , Lon=105.53},
new GeoCoordinate{Lat=31.52     , Lon=106.46},
new GeoCoordinate{Lat=31.21     , Lon=106.04},
new GeoCoordinate{Lat=31.31     , Lon=106.24},
new GeoCoordinate{Lat=31.02     , Lon=106.25},
new GeoCoordinate{Lat=31.05     , Lon=106.34},
new GeoCoordinate{Lat=31.56     , Lon=107.13},
new GeoCoordinate{Lat=31.35     , Lon=107.05},
new GeoCoordinate{Lat=31.22     , Lon=107.43},
new GeoCoordinate{Lat=31.12     , Lon=107.30},
new GeoCoordinate{Lat=31.06     , Lon=107.51},
new GeoCoordinate{Lat=30.52     , Lon=105.22},
new GeoCoordinate{Lat=30.46     , Lon=105.42},
new GeoCoordinate{Lat=30.30     , Lon=105.33},
new GeoCoordinate{Lat=30.17     , Lon=105.02},
new GeoCoordinate{Lat=30.07     , Lon=105.19},
new GeoCoordinate{Lat=30.45     , Lon=106.08},
new GeoCoordinate{Lat=30.51     , Lon=106.58},
new GeoCoordinate{Lat=30.31     , Lon=106.27},
new GeoCoordinate{Lat=30.32     , Lon=106.38},
new GeoCoordinate{Lat=30.20     , Lon=106.56},
new GeoCoordinate{Lat=30.21     , Lon=106.17},
new GeoCoordinate{Lat=30.45     , Lon=107.11},
new GeoCoordinate{Lat=29.37     , Lon=105.07},
new GeoCoordinate{Lat=29.20 , Lon=105.18},
new GeoCoordinate{Lat=29.09     , Lon=105.22},
new GeoCoordinate{Lat=28.43 , Lon=105.04},
new GeoCoordinate{Lat=28.49     , Lon=105.50},
new GeoCoordinate{Lat=28.47     , Lon=105.23},
new GeoCoordinate{Lat=28.02     , Lon=105.49},
new GeoCoordinate{Lat=28.10     , Lon=105.26},
new GeoCoordinate{Lat=40.02     , Lon=117.24},
new GeoCoordinate{Lat=39.26     , Lon=117.04},
new GeoCoordinate{Lat=39.44     , Lon=117.17},
new GeoCoordinate{Lat=39.05     , Lon=117.20},
new GeoCoordinate{Lat=39.05     , Lon=117.03},
new GeoCoordinate{Lat=39.14     , Lon=117.08},
new GeoCoordinate{Lat=39.21     , Lon=117.49},
new GeoCoordinate{Lat=39.14     , Lon=117.46},
new GeoCoordinate{Lat=38.55     , Lon=116.55},
new GeoCoordinate{Lat=38.59     , Lon=117.22},
new GeoCoordinate{Lat=39.03     , Lon=117.43},
new GeoCoordinate{Lat=38.51     , Lon=117.28},
new GeoCoordinate{Lat=32.30     , Lon= 80.05},
new GeoCoordinate{Lat=32.09     , Lon= 84.25},
new GeoCoordinate{Lat=31.23 , Lon= 90.01},
new GeoCoordinate{Lat=32.21     , Lon= 91.06},
new GeoCoordinate{Lat=31.29     , Lon= 92.04},
new GeoCoordinate{Lat=30.17     , Lon= 81.15},
new GeoCoordinate{Lat=30.57     , Lon= 88.38},
new GeoCoordinate{Lat=30.29     , Lon= 91.06},
new GeoCoordinate{Lat=29.05     , Lon= 87.36},
new GeoCoordinate{Lat=29.41     , Lon= 89.06},
new GeoCoordinate{Lat=29.15     , Lon= 88.53},
new GeoCoordinate{Lat=29.26     , Lon= 90.10},
new GeoCoordinate{Lat=29.18     , Lon= 90.59},
new GeoCoordinate{Lat=29.40     , Lon= 91.08},
new GeoCoordinate{Lat=29.51     , Lon= 91.44},
new GeoCoordinate{Lat=29.02     , Lon= 91.41},
new GeoCoordinate{Lat=29.16     , Lon= 91.46},
new GeoCoordinate{Lat=28.11 , Lon= 85.58},
new GeoCoordinate{Lat=28.38     , Lon= 87.05},
new GeoCoordinate{Lat=28.55     , Lon= 89.36},
new GeoCoordinate{Lat=28.58     , Lon= 90.24},
new GeoCoordinate{Lat=27.59     , Lon= 91.57},
new GeoCoordinate{Lat=28.25     , Lon= 92.28},
new GeoCoordinate{Lat=27.44     , Lon= 89.05},
new GeoCoordinate{Lat=31.53     , Lon= 93.47},
new GeoCoordinate{Lat=31.29     , Lon= 93.47},
new GeoCoordinate{Lat=31.25     , Lon= 95.36},
new GeoCoordinate{Lat=31.13     , Lon= 96.36},
new GeoCoordinate{Lat=31.09     , Lon= 97.10},
new GeoCoordinate{Lat=30.40     , Lon= 93.17},
new GeoCoordinate{Lat=30.45     , Lon= 95.50},
new GeoCoordinate{Lat=29.52     , Lon= 95.46},
new GeoCoordinate{Lat=30.03     , Lon= 96.55},
new GeoCoordinate{Lat=29.09     , Lon= 92.35},
new GeoCoordinate{Lat=29.40     , Lon= 94.20},
new GeoCoordinate{Lat=29.13     , Lon= 94.13},
new GeoCoordinate{Lat=29.40     , Lon= 97.50},
new GeoCoordinate{Lat=29.41     , Lon= 98.36},
new GeoCoordinate{Lat=28.39     , Lon= 97.28},
new GeoCoordinate{Lat=48.03 , Lon= 86.24},
new GeoCoordinate{Lat=47.06 , Lon= 87.58},
new GeoCoordinate{Lat=47.27 , Lon= 85.53},
new GeoCoordinate{Lat=47.42 , Lon= 86.52},
new GeoCoordinate{Lat=47.07 , Lon= 87.29},
new GeoCoordinate{Lat=47.44 , Lon= 88.05},
new GeoCoordinate{Lat=46.59 , Lon= 89.31},
new GeoCoordinate{Lat=46.44 , Lon= 83.00},
new GeoCoordinate{Lat=46.12 , Lon= 82.56},
new GeoCoordinate{Lat=46.33 , Lon= 83.39},
new GeoCoordinate{Lat=46.49 , Lon= 85.45},
new GeoCoordinate{Lat=46.40 , Lon= 90.23},
new GeoCoordinate{Lat=45.11 , Lon= 82.34},
new GeoCoordinate{Lat=44.54 , Lon= 82.04},
new GeoCoordinate{Lat=45.56 , Lon= 83.36},
new GeoCoordinate{Lat=45.37 , Lon= 84.51},
new GeoCoordinate{Lat=45.22 , Lon= 90.32},
new GeoCoordinate{Lat=44.10 , Lon= 80.25},
new GeoCoordinate{Lat=44.03 , Lon= 80.51},
new GeoCoordinate{Lat=44.58 , Lon= 81.01},
new GeoCoordinate{Lat=44.34 , Lon= 82.49},
new GeoCoordinate{Lat=44.26 , Lon= 84.40},
new GeoCoordinate{Lat=44.51 , Lon= 85.15},
new GeoCoordinate{Lat=45.01 , Lon= 86.06},
new GeoCoordinate{Lat=44.19 , Lon= 86.03},
new GeoCoordinate{Lat=44.20 , Lon= 85.37},
new GeoCoordinate{Lat=44.17 , Lon= 85.49},
new GeoCoordinate{Lat=44.19 , Lon= 86.12},
new GeoCoordinate{Lat=44.12 , Lon= 87.32},
new GeoCoordinate{Lat=44.10 , Lon= 86.51},
new GeoCoordinate{Lat=44.07 , Lon= 87.19},
new GeoCoordinate{Lat=43.58 , Lon= 87.39},
new GeoCoordinate{Lat=44.10 , Lon= 87.55},
new GeoCoordinate{Lat=44.00 , Lon= 89.10},
new GeoCoordinate{Lat=44.01 , Lon= 89.34},
new GeoCoordinate{Lat=43.50 , Lon= 81.09},
new GeoCoordinate{Lat=43.57 , Lon= 81.20},
new GeoCoordinate{Lat=43.48 , Lon= 82.31},
new GeoCoordinate{Lat=43.58 , Lon= 81.32},
new GeoCoordinate{Lat=43.28 , Lon= 82.14},
new GeoCoordinate{Lat=43.27 , Lon= 83.18},
new GeoCoordinate{Lat=43.09 , Lon= 81.08},
new GeoCoordinate{Lat=43.11 , Lon= 81.46},
new GeoCoordinate{Lat=43.47 , Lon= 87.39},
new GeoCoordinate{Lat=43.29 , Lon= 87.06},
new GeoCoordinate{Lat=42.46 , Lon= 86.20},
new GeoCoordinate{Lat=43.06 , Lon= 86.50},
new GeoCoordinate{Lat=43.27 , Lon= 87.11},
new GeoCoordinate{Lat=43.53 , Lon= 88.07},
new GeoCoordinate{Lat=43.21 , Lon= 88.19},
new GeoCoordinate{Lat=43.50 , Lon= 90.17},
new GeoCoordinate{Lat=43.13 , Lon= 91.44},
new GeoCoordinate{Lat=42.14 , Lon= 88.13},
new GeoCoordinate{Lat=43.02 , Lon= 84.09},
new GeoCoordinate{Lat=42.19 , Lon= 86.24},
new GeoCoordinate{Lat=42.05 , Lon= 86.34},
new GeoCoordinate{Lat=42.15 , Lon= 86.48},
new GeoCoordinate{Lat=42.46 , Lon= 88.36},
new GeoCoordinate{Lat=42.50 , Lon= 89.15},
new GeoCoordinate{Lat=42.57 , Lon= 89.14},
new GeoCoordinate{Lat=42.51 , Lon= 90.14},
new GeoCoordinate{Lat=41.13 , Lon= 79.14},
new GeoCoordinate{Lat=41.07 , Lon= 80.23},
new GeoCoordinate{Lat=41.16 , Lon= 80.14},
new GeoCoordinate{Lat=41.47 , Lon= 81.54},
new GeoCoordinate{Lat=41.33 , Lon= 82.39},
new GeoCoordinate{Lat=41.14 , Lon= 82.47},
new GeoCoordinate{Lat=41.49 , Lon= 84.16},
new GeoCoordinate{Lat=41.43 , Lon= 82.58},
new GeoCoordinate{Lat=41.21 , Lon= 86.16},
new GeoCoordinate{Lat=41.44 , Lon= 85.49},
new GeoCoordinate{Lat=40.31 , Lon= 75.24},
new GeoCoordinate{Lat=39.43 , Lon= 76.10},
new GeoCoordinate{Lat=39.43 , Lon= 75.15},
new GeoCoordinate{Lat=39.30 , Lon= 76.47},
new GeoCoordinate{Lat=39.09 , Lon= 75.57},
new GeoCoordinate{Lat=39.29 , Lon= 75.45},
new GeoCoordinate{Lat=40.56 , Lon= 78.27},
new GeoCoordinate{Lat=39.48 , Lon= 78.34},
new GeoCoordinate{Lat=39.15 , Lon= 76.47},
new GeoCoordinate{Lat=40.30 , Lon= 79.03},
new GeoCoordinate{Lat=40.39 , Lon= 80.24},
new GeoCoordinate{Lat=40.33 , Lon= 81.16},
new GeoCoordinate{Lat=39.00 , Lon= 83.40},
new GeoCoordinate{Lat=40.38 , Lon= 87.42},
new GeoCoordinate{Lat=39.02 , Lon= 88.10},
new GeoCoordinate{Lat=38.56 , Lon= 76.10},
new GeoCoordinate{Lat=37.46 , Lon= 75.14},
new GeoCoordinate{Lat=38.55 , Lon= 77.38},
new GeoCoordinate{Lat=38.26 , Lon= 77.16},
new GeoCoordinate{Lat=37.55 , Lon= 77.24},
new GeoCoordinate{Lat=38.12 , Lon= 77.16},
new GeoCoordinate{Lat=37.37 , Lon= 78.17},
new GeoCoordinate{Lat=37.01 , Lon= 80.48},
new GeoCoordinate{Lat=37.10 , Lon= 79.38},
new GeoCoordinate{Lat=37.08 , Lon= 79.56},
new GeoCoordinate{Lat=37.05 , Lon= 80.10},
new GeoCoordinate{Lat=37.04 , Lon= 82.43},
new GeoCoordinate{Lat=38.09 , Lon= 85.33},
new GeoCoordinate{Lat=36.51 , Lon= 81.39},
new GeoCoordinate{Lat=43.36 , Lon= 93.03},
new GeoCoordinate{Lat=43.45 , Lon= 94.59},
new GeoCoordinate{Lat=43.16 , Lon= 94.42},
new GeoCoordinate{Lat=42.49 , Lon= 93.31},
new GeoCoordinate{Lat=41.32 , Lon= 94.40},
new GeoCoordinate{Lat=28.29     , Lon= 98.55},
new GeoCoordinate{Lat=28.35 , Lon=103.57},
new GeoCoordinate{Lat=28.14     , Lon=103.38},
new GeoCoordinate{Lat=28.06     , Lon=104.14},
new GeoCoordinate{Lat=27.45     , Lon= 98.40},
new GeoCoordinate{Lat=27.50     , Lon= 99.45},
new GeoCoordinate{Lat=27.10     , Lon= 99.17},
new GeoCoordinate{Lat=27.15     , Lon=100.51},
new GeoCoordinate{Lat=27.45     , Lon=103.54},
new GeoCoordinate{Lat=27.12     , Lon=103.33},
new GeoCoordinate{Lat=27.21     , Lon=103.43},
new GeoCoordinate{Lat=27.38     , Lon=104.03},
new GeoCoordinate{Lat=27.25 , Lon=104.52},
new GeoCoordinate{Lat=27.51     , Lon=105.03},
new GeoCoordinate{Lat=26.54     , Lon= 98.52},
new GeoCoordinate{Lat=25.52     , Lon= 98.51},
new GeoCoordinate{Lat=26.25     , Lon= 99.25},
new GeoCoordinate{Lat=26.32     , Lon= 99.55},
new GeoCoordinate{Lat=26.06 , Lon= 99.58},
new GeoCoordinate{Lat=26.51 , Lon=100.13},
new GeoCoordinate{Lat=26.41     , Lon=100.45},
new GeoCoordinate{Lat=26.35     , Lon=100.11},
new GeoCoordinate{Lat=26.38     , Lon=101.16},
new GeoCoordinate{Lat=26.03     , Lon=101.40},
new GeoCoordinate{Lat=26.55     , Lon=102.55},
new GeoCoordinate{Lat=26.24 , Lon=103.15},
new GeoCoordinate{Lat=26.06     , Lon=103.10},
new GeoCoordinate{Lat=26.13     , Lon=104.05},
new GeoCoordinate{Lat=24.59 , Lon= 98.30},
new GeoCoordinate{Lat=25.53     , Lon= 99.22},
new GeoCoordinate{Lat=25.41     , Lon= 99.57},
new GeoCoordinate{Lat=25.28     , Lon= 99.31},
new GeoCoordinate{Lat=25.07     , Lon= 99.11},
new GeoCoordinate{Lat=25.42     , Lon=100.11},
new GeoCoordinate{Lat=25.50     , Lon=100.34},
new GeoCoordinate{Lat=25.23     , Lon=100.24},
new GeoCoordinate{Lat=25.29     , Lon=100.35},
new GeoCoordinate{Lat=25.16     , Lon=100.17},
new GeoCoordinate{Lat=25.43     , Lon=101.19},
new GeoCoordinate{Lat=25.44     , Lon=101.52},
new GeoCoordinate{Lat=25.32     , Lon=101.14},
new GeoCoordinate{Lat=25.18     , Lon=101.31},
new GeoCoordinate{Lat=25.11     , Lon=101.17},
new GeoCoordinate{Lat=25.02     , Lon=101.33},
new GeoCoordinate{Lat=25.14     , Lon=102.30},
new GeoCoordinate{Lat=25.32     , Lon=102.25},
new GeoCoordinate{Lat=25.33     , Lon=102.27},
new GeoCoordinate{Lat=25.07     , Lon=102.06},
new GeoCoordinate{Lat=25.00     , Lon=102.39},
new GeoCoordinate{Lat=25.33     , Lon=103.16},
new GeoCoordinate{Lat=25.25     , Lon=103.33},
new GeoCoordinate{Lat=25.30     , Lon=103.48},
new GeoCoordinate{Lat=25.21     , Lon=103.05},
new GeoCoordinate{Lat=25.35     , Lon=103.50},
new GeoCoordinate{Lat=24.59     , Lon=103.37},
new GeoCoordinate{Lat=25.41 , Lon=104.15},
new GeoCoordinate{Lat=24.10     , Lon= 97.49},
new GeoCoordinate{Lat=24.42     , Lon= 97.57},
new GeoCoordinate{Lat=24.00 , Lon= 97.51},
new GeoCoordinate{Lat=23.46     , Lon= 98.49},
new GeoCoordinate{Lat=24.49     , Lon= 98.18},
new GeoCoordinate{Lat=24.36     , Lon= 98.41},
new GeoCoordinate{Lat=24.44     , Lon= 99.11},
new GeoCoordinate{Lat=24.50     , Lon= 99.37},
new GeoCoordinate{Lat=24.26     , Lon= 98.35},
new GeoCoordinate{Lat=24.35     , Lon= 99.56},
new GeoCoordinate{Lat=24.02     , Lon= 99.14},
new GeoCoordinate{Lat=25.02 , Lon=100.31},
new GeoCoordinate{Lat=24.27     , Lon=100.08},
new GeoCoordinate{Lat=24.28     , Lon=100.52},
new GeoCoordinate{Lat=24.41     , Lon=101.36},
new GeoCoordinate{Lat=24.55     , Lon=102.30},
new GeoCoordinate{Lat=23.59     , Lon=101.07},
new GeoCoordinate{Lat=24.04     , Lon=101.58},
new GeoCoordinate{Lat=24.39 , Lon=102.11},
new GeoCoordinate{Lat=24.39     , Lon=102.36},
new GeoCoordinate{Lat=24.57     , Lon=102.37},
new GeoCoordinate{Lat=24.41     , Lon=102.55},
new GeoCoordinate{Lat=24.20     , Lon=102.33},
new GeoCoordinate{Lat=24.17     , Lon=102.46},
new GeoCoordinate{Lat=24.08     , Lon=102.45},
new GeoCoordinate{Lat=24.11     , Lon=102.55},
new GeoCoordinate{Lat=24.55     , Lon=103.10},
new GeoCoordinate{Lat=24.45 , Lon=103.16},
new GeoCoordinate{Lat=24.54     , Lon=102.49},
new GeoCoordinate{Lat=24.49     , Lon=103.59},
new GeoCoordinate{Lat=24.24     , Lon=103.27},
new GeoCoordinate{Lat=24.32     , Lon=103.46},
new GeoCoordinate{Lat=24.03     , Lon=104.10},
new GeoCoordinate{Lat=24.59     , Lon=104.19},
new GeoCoordinate{Lat=24.11     , Lon=102.24},
new GeoCoordinate{Lat=23.09     , Lon= 99.16},
new GeoCoordinate{Lat=23.33     , Lon= 99.24},
new GeoCoordinate{Lat=22.38     , Lon= 99.36},
new GeoCoordinate{Lat=22.19     , Lon= 99.36},
new GeoCoordinate{Lat=23.28     , Lon= 99.48},
new GeoCoordinate{Lat=23.53     , Lon=100.05},
new GeoCoordinate{Lat=23.30     , Lon=100.42},
new GeoCoordinate{Lat=22.34     , Lon= 99.56},
new GeoCoordinate{Lat=21.58     , Lon=100.28},
new GeoCoordinate{Lat=22.00     , Lon=100.47},
new GeoCoordinate{Lat=23.02 , Lon=101.03},
new GeoCoordinate{Lat=23.25 , Lon=101.40},
new GeoCoordinate{Lat=22.47     , Lon=100.58},
new GeoCoordinate{Lat=23.36     , Lon=101.59},
new GeoCoordinate{Lat=21.28 , Lon=101.34},
new GeoCoordinate{Lat=23.42     , Lon=102.29},
new GeoCoordinate{Lat=23.37     , Lon=102.50},
new GeoCoordinate{Lat=23.22     , Lon=102.26},
new GeoCoordinate{Lat=23.13     , Lon=102.51},
new GeoCoordinate{Lat=22.35     , Lon=101.51},
new GeoCoordinate{Lat=23.00     , Lon=102.25},
new GeoCoordinate{Lat=23.42     , Lon=103.17},
new GeoCoordinate{Lat=23.23     , Lon=103.09},
new GeoCoordinate{Lat=23.27     , Lon=103.20},
new GeoCoordinate{Lat=22.59     , Lon=103.41},
new GeoCoordinate{Lat=22.47     , Lon=103.14},
new GeoCoordinate{Lat=22.30     , Lon=103.57},
new GeoCoordinate{Lat=23.37     , Lon=104.20},
new GeoCoordinate{Lat=23.27     , Lon=104.42},
new GeoCoordinate{Lat=23.20     , Lon=104.17},
new GeoCoordinate{Lat=23.02     , Lon=104.25},
new GeoCoordinate{Lat=23.08     , Lon=104.42},
new GeoCoordinate{Lat=24.04     , Lon=105.04},
new GeoCoordinate{Lat=23.39     , Lon=105.38},
new GeoCoordinate{Lat=31.01     , Lon=119.54},
new GeoCoordinate{Lat=30.38 , Lon=119.42},
new GeoCoordinate{Lat=30.13 , Lon=119.42},
new GeoCoordinate{Lat=30.03     , Lon=119.57},
new GeoCoordinate{Lat=30.52     , Lon=120.03},
new GeoCoordinate{Lat=30.50     , Lon=120.56},
new GeoCoordinate{Lat=30.44     , Lon=120.46},
new GeoCoordinate{Lat=30.04     , Lon=120.30},
new GeoCoordinate{Lat=30.32 , Lon=119.59},
new GeoCoordinate{Lat=30.29     , Lon=120.30},
new GeoCoordinate{Lat=30.38     , Lon=120.31},
new GeoCoordinate{Lat=30.14 , Lon=120.10},
new GeoCoordinate{Lat=30.32     , Lon=120.54},
new GeoCoordinate{Lat=30.11     , Lon=120.17},
new GeoCoordinate{Lat=30.39 , Lon=121.07},
new GeoCoordinate{Lat=30.12     , Lon=121.16},
new GeoCoordinate{Lat=30.01     , Lon=121.08},
new GeoCoordinate{Lat=30.44     , Lon=122.27},
new GeoCoordinate{Lat=30.02     , Lon=122.06},
new GeoCoordinate{Lat=30.15     , Lon=122.12},
new GeoCoordinate{Lat=29.08     , Lon=118.24},
new GeoCoordinate{Lat=29.49     , Lon=119.41},
new GeoCoordinate{Lat=29.37 , Lon=119.01},
new GeoCoordinate{Lat=29.29 , Lon=119.16},
new GeoCoordinate{Lat=29.28     , Lon=119.53},
new GeoCoordinate{Lat=29.02     , Lon=119.11},
new GeoCoordinate{Lat=29.13     , Lon=119.28},
new GeoCoordinate{Lat=29.07     , Lon=119.39},
new GeoCoordinate{Lat=29.42     , Lon=120.15},
new GeoCoordinate{Lat=30.03     , Lon=120.49},
new GeoCoordinate{Lat=29.31     , Lon=120.53},
new GeoCoordinate{Lat=29.36     , Lon=120.49},
new GeoCoordinate{Lat=29.20     , Lon=120.05},
new GeoCoordinate{Lat=29.16     , Lon=120.13},
new GeoCoordinate{Lat=29.09     , Lon=120.58},
new GeoCoordinate{Lat=29.03     , Lon=120.26},
new GeoCoordinate{Lat=29.59 , Lon=121.36},
new GeoCoordinate{Lat=29.48     , Lon=121.30},
new GeoCoordinate{Lat=29.53     , Lon=121.50},
new GeoCoordinate{Lat=29.42 , Lon=121.23},
new GeoCoordinate{Lat=29.28     , Lon=121.53},
new GeoCoordinate{Lat=29.19     , Lon=121.26},
new GeoCoordinate{Lat=29.07     , Lon=121.23},
new GeoCoordinate{Lat=29.12     , Lon=121.57},
new GeoCoordinate{Lat=29.57     , Lon=122.18},
new GeoCoordinate{Lat=28.54     , Lon=118.30},
new GeoCoordinate{Lat=28.43     , Lon=118.36},
new GeoCoordinate{Lat=29.00     , Lon=118.54},
new GeoCoordinate{Lat=28.53     , Lon=119.48},
new GeoCoordinate{Lat=28.54     , Lon=120.02},
new GeoCoordinate{Lat=28.36     , Lon=119.17},
new GeoCoordinate{Lat=28.27     , Lon=119.55},
new GeoCoordinate{Lat=28.05     , Lon=119.08},
new GeoCoordinate{Lat=28.52 , Lon=120.43},
new GeoCoordinate{Lat=28.40     , Lon=120.05},
new GeoCoordinate{Lat=28.04     , Lon=120.58},
new GeoCoordinate{Lat=28.09     , Lon=120.17},
new GeoCoordinate{Lat=28.09     , Lon=120.41},
new GeoCoordinate{Lat=28.02     , Lon=120.39},
new GeoCoordinate{Lat=28.52 , Lon=121.12},
new GeoCoordinate{Lat=28.22     , Lon=121.22},
new GeoCoordinate{Lat=28.37     , Lon=121.25},
new GeoCoordinate{Lat=28.27     , Lon=121.54},
new GeoCoordinate{Lat=28.05     , Lon=121.16},
new GeoCoordinate{Lat=28.07     , Lon=119.33},
new GeoCoordinate{Lat=27.37     , Lon=119.05},
new GeoCoordinate{Lat=27.33     , Lon=119.42},
new GeoCoordinate{Lat=27.47     , Lon=120.05},
new GeoCoordinate{Lat=27.40 , Lon=120.34},
new GeoCoordinate{Lat=27.47     , Lon=120.39},
new GeoCoordinate{Lat=27.50     , Lon=121.09},
new GeoCoordinate{Lat=28.00 , Lon=119.38},
new GeoCoordinate{Lat=31.57     , Lon=108.40},
new GeoCoordinate{Lat=31.11     , Lon=108.25},
new GeoCoordinate{Lat=30.57     , Lon=108.41},
new GeoCoordinate{Lat=31.24     , Lon=109.37},
new GeoCoordinate{Lat=31.01     , Lon=109.32},
new GeoCoordinate{Lat=31.05     , Lon=109.52},
new GeoCoordinate{Lat=30.11     , Lon=105.48},
new GeoCoordinate{Lat=30.20     , Lon=107.20},
new GeoCoordinate{Lat=30.41     , Lon=107.48},
new GeoCoordinate{Lat=30.50     , Lon=108.21},
new GeoCoordinate{Lat=30.46     , Lon=108.24},
new GeoCoordinate{Lat=30.18     , Lon=108.02},
new GeoCoordinate{Lat=29.59     , Lon=108.07},
new GeoCoordinate{Lat=29.42     , Lon=105.42},
new GeoCoordinate{Lat=29.25     , Lon=105.35},
new GeoCoordinate{Lat=29.22     , Lon=105.54},
new GeoCoordinate{Lat=28.59     , Lon=106.55},
new GeoCoordinate{Lat=29.51     , Lon=106.04},
new GeoCoordinate{Lat=29.51     , Lon=106.27},
new GeoCoordinate{Lat=29.58     , Lon=106.16},
new GeoCoordinate{Lat=29.44     , Lon=106.37},
new GeoCoordinate{Lat=29.35     , Lon=106.13},
new GeoCoordinate{Lat=29.35     , Lon=106.28},
new GeoCoordinate{Lat=29.17     , Lon=106.15},
new GeoCoordinate{Lat=29.20     , Lon=106.30},
new GeoCoordinate{Lat=29.10     , Lon=107.07},
new GeoCoordinate{Lat=29.50     , Lon=107.04},
new GeoCoordinate{Lat=29.44     , Lon=107.16},
new GeoCoordinate{Lat=29.51     , Lon=107.44},
new GeoCoordinate{Lat=29.19     , Lon=107.45},
new GeoCoordinate{Lat=29.31     , Lon=108.46},
new GeoCoordinate{Lat=29.18     , Lon=108.10},
new GeoCoordinate{Lat=29.00     , Lon=106.39},
new GeoCoordinate{Lat=28.49     , Lon=108.46},
new GeoCoordinate{Lat=28.22     , Lon=109.01},
            };
            hit = 0;
            miss = 0;
            fall = 0;
           
            foreach (GeoCoordinate test in checkPointList)
            {
                foreach(List<GeoCoordinate> ring in ringsList)
                {
                    bool isIn = isInRing(test, ring);
                    if (isIn)
                    {
                        hit++;
                    }
                }               
            }

            miss = checkPointList.Count - hit;
            int nd = 0;

            foreach (GeoCoordinate test in allStationList)
            {
                foreach (List<GeoCoordinate> ring in ringsList)
                {
                    bool isIn = isInRing(test, ring);
                    if (isIn)
                    {
                        fall++;
                    }
                    else
                    {
                        nd++;
                    }
                }
            }
            fall = fall - hit;
            int na = hit;
            int nb = fall;
            int nc = miss;
            nd = nd - miss;
            TSR = (double)(na + nd) / (double)(na + nb + nc + nd);
            PO = (double)nc / (double)(na + nc);
            FAR = (double)nb / (double)(na + nb);
            TS = (double)na / (double)(na + nb + nc);
        }
              
        private List<String> CrawlAllDisaster(DateTime from, DateTime to, String type, int pageCount)
        {
            /*按时间类型爬取全部链接*/
            String url = "http://10.1.64.146/disaster/webDirectReport/directList";
            String rtnStr = "";

            var data = new List<KeyValuePair<string, string>>(new[]
        {
            new KeyValuePair<string, string>("pageCount", pageCount.ToString()),
            new KeyValuePair<string, string>("newExportId", ""),
            new KeyValuePair<string, string>("pageFirst", "1"),
            new KeyValuePair<string, string>("renameFileName", ""),
            new KeyValuePair<string, string>("provinceCheck", "1"),
            new KeyValuePair<string, string>("cityCheck", "1"),
            new KeyValuePair<string, string>("countyCheck", "1"),
            new KeyValuePair<string, string>("province", "0"),
            new KeyValuePair<string, string>("city", "0"),
            new KeyValuePair<string, string>("county", "0"),
            new KeyValuePair<string, string>("startTime", from.ToString("yyyy-MM-dd HH") + ":00:00"),   
            new KeyValuePair<string, string>("endTime",  to.ToString("yyyy-MM-dd HH") + ":00:00"),
            new KeyValuePair<string, string>("vCategory", type),
            new KeyValuePair<string, string>("peopledeadStr", "0"),
            new KeyValuePair<string, string>("vRzDpop", ""),
            new KeyValuePair<string, string>("generallossStr", "0"),
            new KeyValuePair<string, string>("vGeneralLoss", ""),
        });

            string boundary = "----WebKitFormBoundaryhtBmrmyVTCdwfeHH";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";

            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                foreach (var item in data)
                {
                    writer.WriteLine("--" + boundary);
                    writer.WriteLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", item.Key));
                    writer.WriteLine();
                    writer.WriteLine(item.Value);
                }
                writer.WriteLine("--" + boundary + "--");

            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                rtnStr = reader.ReadToEnd();
            }

            // 定义正则表达式用来匹配  
            Regex regImg = new Regex("href=\"(?<Url>.*?)\" title=\"查看\"", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串   
            MatchCollection matches = regImg.Matches(rtnStr);
            List<String> sUrlList = new List<string>();

            // 取得匹配项列表   
            foreach (Match match in matches)
            {
                string strUrl = match.Groups["Url"].Value;
                sUrlList.Add(strUrl);
            }

            return sUrlList;
        }

        private bool Parse(String url, String type, out GeoCoordinate coord)
        {
            bool isTheType = false;
            url = "http://10.1.64.146" + url;
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            // 定义正则表达式用来匹配  
            //Regex regImg = new Regex("href=\"(?<Url>.*?)\" title=\"查看\"", RegexOptions.IgnoreCase);

            Regex regImg = new Regex("var mapData = '\\[\\{\"x\":(?<lon>.*?),\"y\":(?<lat>.*?),", RegexOptions.IgnoreCase);

            // 搜索匹配的字符串   
            MatchCollection matches = regImg.Matches(responseString);
            string lon = "";
            string lat = "";

            // 取得匹配项列表   
            foreach (Match match in matches)
            {
                lon = match.Groups["lon"].Value;
                lat = match.Groups["lat"].Value;
            }
            if (lon.Equals(String.Empty) || lat.Equals(String.Empty))
            {
                Regex regImg1 = new Regex("var mapData = '\\[\\{\"rings\":\\[\\[\\[(?<lon>.*?),(?<lat>.*?)\\],\\[", RegexOptions.IgnoreCase);

                // 搜索匹配的字符串   
                MatchCollection matches1 = regImg1.Matches(responseString);

                // 取得匹配项列表   
                foreach (Match match in matches1)
                {
                    lon = match.Groups["lon"].Value;
                    lat = match.Groups["lat"].Value;
                }
            }

            if ((!lon.Equals(String.Empty)) && (!lat.Equals(String.Empty)))
            {
                Regex reg = new Regex("<th>灾害影响描述<\\/th>(?<catStr>.*?)<\\/td>", RegexOptions.Singleline);

                // 搜索匹配的字符串   
                MatchCollection matches2 = reg.Matches(responseString);
                String catStr = "";

                // 取得匹配项列表   
                foreach (Match match in matches2)
                {
                    catStr = match.Groups["catStr"].Value;
                }
                if (type.Equals("山洪"))
                {
                    if (catStr.Contains("山洪"))
                    {
                        isTheType = true;
                    }
                }

                if (type.Equals("地质灾害"))
                {
                    //排除关键词
                    catStr.Replace("房屋倒塌", "");
                    catStr.Replace("倒塌房屋", "");
                    catStr.Replace("倒塌房间", "");
                    catStr.Replace("房间倒塌", "");
                    catStr.Replace("倒塌农房", "");
                    catStr.Replace("农房倒塌", "");
                    if (catStr.Contains("滑坡") || catStr.Contains("塌方") || catStr.Contains("垮塌") || 
                        (catStr.Contains("倒塌") 
                        && (!catStr.Contains("房屋倒塌")) && (!catStr.Contains("倒塌房屋")) 
                        && (!catStr.Contains("倒塌房间")) && (!catStr.Contains("房间倒塌"))
                        && (!catStr.Contains("倒塌农房")) && (!catStr.Contains("农房倒塌"))))
                    {
                        isTheType = true;
                    }
                }
            }
            GeoCoordinate cd = new GeoCoordinate();

            if (isTheType == true)
            {
                cd.Lat = double.Parse(lat);
                cd.Lon = double.Parse(lon);
            }

            coord = cd;

            return isTheType;
        }

        private bool isInRing(GeoCoordinate checkPoint, List<GeoCoordinate> coordList)
        {
            bool inRing = false;
            int crossings = 0;
            for (int i = 0; i < coordList.Count; i++)
            {
                double slope = (coordList[(i + 1) % coordList.Count].Lat - coordList[i].Lat) / (coordList[(i + 1) % coordList.Count].Lon - coordList[i].Lon);
                bool cond1 = (coordList[i].Lon <= checkPoint.Lon) && (checkPoint.Lon < coordList[(i + 1) % coordList.Count].Lon);
                bool cond2 = (coordList[(i + 1) % coordList.Count].Lon <= checkPoint.Lon) && (checkPoint.Lon < coordList[i].Lon);
                bool above = checkPoint.Lat < (slope * (checkPoint.Lon - coordList[i].Lon) + coordList[i].Lat);
                if ((cond1 || cond2) && above)
                {
                    crossings++;
                }
            }

            if (crossings % 2 == 0)
            {
                inRing = false;
            }
            else
            {
                inRing = true;
            }

            return inRing;
        }
    }
}
