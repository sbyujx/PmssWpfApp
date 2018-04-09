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
            this.ShowEditZilaoCommand = new DelegateCommand(ShowEditZilao);
            this.ShowEditDisasterCommand = new DelegateCommand(ShowEditDisaster);
            this.ShowEditRiverCommand = new DelegateCommand(ShowEditRiver);
            this.ShowNextFileCommand = new DelegateCommand(ShowNextFile);
            this.ShowLastFileCommand = new DelegateCommand(ShowLastFile);
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
            var renderResult = render.GenerateRenderResult("Data/BaseMaps/ProvinceNames.txt");
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);
            this.provinceNameLayer = renderResult.Layer;

            renderResult = render.GenerateRenderResult("Data/BaseMaps/DistrictNames.txt");
            renderResult.Layer.IsVisible = false;
            //this.baseGroupLayer.ChildLayers.Add(renderResult.Layer);
            this.TreeViewVm.AddBaseLayer(renderResult.Layer);

            renderResult = render.GenerateRenderResult("Data/BaseMaps/Flood.txt");
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

                    if(this.graphicGroupDict.ContainsKey(layer as GraphicsLayer))
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
    }
}
