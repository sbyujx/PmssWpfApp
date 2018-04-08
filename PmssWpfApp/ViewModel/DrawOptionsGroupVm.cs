using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Esri.ArcGISRuntime.Controls;
using System.ComponentModel;
using Esri.ArcGISRuntime.Layers;
using System.Windows.Media;
using Esri.ArcGISRuntime.Symbology;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Microsoft.Win32;
using PmssWpfApp.Dialogs;
using Pmss.Micaps.Render.FileSource;

namespace PmssWpfApp.ViewModel
{
    public class DrawOptionsGroupVm : INotifyPropertyChanged
    {
        public DrawOptionsGroupVm()
        {
            var polylineSymbol = new SimpleLineSymbol
            {
                Color = Colors.Red,
                Width = 2
            };
            DrawPolyLine = new DrawOptionVm(DrawShape.Polyline, polylineSymbol);
            var contourSymbol = new SimpleLineSymbol
            {
                Color = Colors.Purple,
                Width = 2
            };
            DrawContour = new DrawOptionVm(DrawShape.Polyline, contourSymbol);
            DrawPolyLine.Attributes.Add(Diamond14Attributes.LineType, Diamond14Attributes.LineTypeLine);
            DrawContour.Attributes.Add(Diamond14Attributes.LineType, Diamond14Attributes.LineTypeContour);

            var polygonSymbol = new SimpleFillSymbol
            {
                Color = Color.FromArgb(0x66, 0xff, 0, 0)
            };
            DrawPolygon = new DrawOptionVm(DrawShape.Polygon, polygonSymbol);

            var pointSymbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle
            };
            DrawPoint = new DrawOptionVm(DrawShape.Point, pointSymbol);

            drawOptions.Add(DrawPoint);
            drawOptions.Add(DrawPolygon);
            drawOptions.Add(DrawPolyLine);
            drawOptions.Add(DrawContour);

            StartDrawCommand = new DelegateCommand(StartDraw);
            StartEditCommand = new DelegateCommand(StartEdit);
            DeleteGraphicCommand = new DelegateCommand(DeleteGraphic);
            UpdateValueCommand = new DelegateCommand(UpdateValue);
            //IsEnabledChangedCommand = new DelegateCommand<bool?>(IsEnabledChanged);
            //CompleteCommand = new DelegateCommand(Complete, CanComplete);
            //CancelCommand = new DelegateCommand(Cancel, CanCancel);
        }
        public void SetLayer(GraphicsLayer layer)
        {
            this.layer = layer;
            if (layer == null)
            {
                Reset();
                IsEnable = false;
            }
            else
            {
                IsEnable = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public DrawOptionVm DrawPolyLine { get; set; }
        public DrawOptionVm DrawContour { get; set; }
        public DrawOptionVm DrawPolygon { get; set; }
        public DrawOptionVm DrawPoint { get; set; }
        public bool IsEnable
        {
            get
            {
                return this.isEnable;
            }
            set
            {
                isEnable = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsEnable)));
                }
            }
        }
        public DelegateCommand StartDrawCommand { get; set; }
        public Editor MapEditor
        {
            get
            {
                return mapEditor;
            }
            set
            {
                mapEditor = value;
                MapEditor.Cancel.CanExecuteChanged += HandleCancelCanExecuteChanged;
            }
        }
        public MapView PmssMapView
        {
            get
            {
                return mapView;
            }
            set
            {
                this.mapView = value;
                this.mapView.MapViewTapped += HandleMapViewTapped;
            }
        }

        public bool IsGraphicSelected
        {
            get
            {
                return isGraphicSelected;
            }

            set
            {
                isGraphicSelected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsGraphicSelected)));
                }
            }
        }
        public Graphic CurrentGraphic
        {
            get
            {
                return currentGraphic;
            }

            set
            {
                currentGraphic = value;
                if (currentGraphic == null)
                    IsGraphicSelected = false;
                else
                    IsGraphicSelected = true;
            }
        }
        public DelegateCommand StartEditCommand { get; set; }
        public DelegateCommand DeleteGraphicCommand { get; set; }
        public DelegateCommand UpdateValueCommand { get; set; }
        //public DelegateCommand<bool?> IsEnabledChangedCommand { get; set; }
        //public DelegateCommand CompleteCommand { get; set; }
        //public DelegateCommand CancelCommand { get; set; }

        private List<DrawOptionVm> drawOptions = new List<DrawOptionVm>();
        private bool isEnable = false;
        private GraphicsLayer layer;
        private Editor mapEditor;
        private MapView mapView;
        private Graphic currentGraphic;
        private bool isGraphicSelected = false;


        private void Reset()
        {
            if (MapEditor != null && MapEditor.IsActive)
            {
                if (MapEditor.Complete.CanExecute(null))
                {
                    MapEditor.Complete.Execute(null);
                }
                else if (MapEditor.Cancel.CanExecute(null))
                {
                    MapEditor.Cancel.Execute(null);
                }
            }
            foreach (var opt in drawOptions)
            {
                opt.IsChecked = false;
            }
            if (CurrentGraphic != null)
            {
                CurrentGraphic.IsSelected = false;
                CurrentGraphic = null;
            }
        }
        private async void StartDraw()
        {
            if (!IsEnable && MapEditor == null && layer == null)
                return;
            GraphicsLayer drawLayer = layer; // Fix Bug, change layer during drawing
            DrawOptionVm optionVm = null;
            foreach (var opt in drawOptions)
            {
                if (opt.IsChecked)
                {
                    optionVm = opt;
                    break;
                }
            }

            if (optionVm != null)
            {
                try
                {
                    var progress = new Progress<GeometryEditStatus>();

                    var result = await MapEditor.RequestShapeAsync(optionVm.Shape, null, progress);
                    var graphic = new Graphic
                    {
                        Geometry = result,
                        Symbol = optionVm.Symbol
                    };
                    foreach (var item in optionVm.Attributes)
                    {
                        graphic.Attributes.Add(item.Key, item.Value);
                    }
                    drawLayer.Graphics.Add(graphic);

                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception ex)
                {
                    Reset();
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void HandleCancelCanExecuteChanged(object sender, EventArgs e)
        {
            if (!MapEditor.IsActive)
            {
                foreach (var opt in drawOptions)
                {
                    opt.IsChecked = false;
                }
            }
        }
        private async void HandleMapViewTapped(object sender, MapViewInputEventArgs e)
        {
            if (layer == null || MapEditor.IsActive)
                return;
            var graphic = await layer.HitTestAsync(PmssMapView, e.Position);
            if (graphic != null)
            {
                if (graphic == CurrentGraphic)
                {
                    CurrentGraphic.IsSelected = false;
                    CurrentGraphic = null;
                }
                else
                {
                    if (CurrentGraphic != null)
                    {
                        CurrentGraphic.IsSelected = false;
                    }
                    CurrentGraphic = graphic;
                    CurrentGraphic.IsSelected = true;
                }
            }
        }
        private async void StartEdit()
        {
            if (CurrentGraphic == null || MapEditor.IsActive)
                return;
            if (CurrentGraphic.Geometry.GeometryType == GeometryType.Point)
            {
                MessageBox.Show("当前图形不支持编辑");
                return;
            }
            var editingGraphic = CurrentGraphic;// Fix Bug, change layer during Editing
            editingGraphic.IsVisible = false;

            try
            {
                var progress = new Progress<GeometryEditStatus>();
                var result = await MapEditor.EditGeometryAsync(editingGraphic.Geometry, null, progress);
                editingGraphic.Geometry = result ?? editingGraphic.Geometry;
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (editingGraphic != null)
                {
                    editingGraphic.IsVisible = true;
                    editingGraphic.IsSelected = false;
                    editingGraphic = null;
                    CurrentGraphic = null;// Fix Bug, Edit->Cancel, still can edit but not selected
                }
            }
        }
        private void DeleteGraphic()
        {
            if (MapEditor.IsActive)
            {
                MessageBox.Show("无法在编辑状态删除该元素");
                return;
            }
            if (layer != null && CurrentGraphic != null)
            {
                if (layer.Graphics.Contains(CurrentGraphic))
                {
                    layer.Graphics.Remove(CurrentGraphic);
                    CurrentGraphic = null;
                }
            }
        }
        private void UpdateValue()
        {
            if (layer != null && CurrentGraphic != null)
            {
                var dialog = new LineValueInput();

                object oldValue = null;
                if (CurrentGraphic.Attributes.TryGetValue(Diamond14Attributes.LineValue, out oldValue))
                {
                    if (oldValue.ToString() != string.Empty)
                    {
                        dialog.LineValue = Convert.ToInt32(oldValue);
                    }
                }

                dialog.ShowDialog();

                if (dialog.LineValue == null && oldValue != null)// remove
                {
                    CurrentGraphic.Attributes.Remove(Diamond14Attributes.LineValue);
                }
                else
                {
                    CurrentGraphic.Attributes[Diamond14Attributes.LineValue] = dialog.LineValue.ToString();
                }
            }
        }

        //private void IsEnabledChanged(bool? obj)
        //{
        //    int a = 1;
        //}
        //private void Complete()
        //{
        //    MapEditor.Complete.Execute(null);
        //    Reset();
        //}
        //private bool CanComplete()
        //{
        //    if (MapEditor == null)
        //        return false;
        //    return MapEditor.Complete.CanExecute(null);
        //}
        //private void Cancel()
        //{
        //    MapEditor.Cancel.Execute(null);
        //    Reset();
        //}
        //private bool CanCancel()
        //{
        //    if (MapEditor == null)
        //        return false;
        //    return MapEditor.Cancel.CanExecute(null);
        //}
    }
}
