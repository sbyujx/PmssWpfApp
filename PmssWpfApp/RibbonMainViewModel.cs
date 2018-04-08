using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using System.ComponentModel;
using Esri.ArcGISRuntime.Controls;
using PmssWpfApp.ViewModel;
using System.Windows.Media;

namespace PmssWpfApp
{
    public class RibbonMainViewModel //: INotifyPropertyChanged
    {
        public RibbonMainViewModel()
        {
            SetMapViewCommand = new DelegateCommand<MapView>(SetMapView);
        }

        public Editor MapEditor { get; set; }
        //public DelegateCommand<Editor> SetEditorCommand { get; set; }
        //private void SetEditor(Editor editor)
        //{
        //    drawOptionsVm = App.Current.Resources["DrawOptionsVm"] as DrawOptionsGroupVm;
        //    mapVm = App.Current.Resources["MapVm"] as MapVm;
        //    MapEditor = editor;

        //    drawOptionsVm.MapEditor = editor;
        //    mapVm.SetDrawOptionsVm(drawOptionsVm);
        //}
        public DelegateCommand<MapView> SetMapViewCommand { get; set; }
        private void SetMapView(MapView mapView)
        {
            drawOptionsVm = App.Current.Resources["DrawOptionsVm"] as DrawOptionsGroupVm;
            mapVm = App.Current.Resources["MapVm"] as MapVm;
            monitorVm = App.Current.Resources["MonitorVm"] as DataMonitoringVm;
            checkingVm = App.Current.Resources["CheckingVm"] as DataCheckingVm;
            mouseInfoVm = App.Current.Resources["MouseInfoVm"] as MouseInfoVm;

            drawOptionsVm.MapEditor = mapView.Editor;
            drawOptionsVm.PmssMapView = mapView;
            mapVm.SetDrawOptionsVm(drawOptionsVm);
            mapVm.PmssMapView = mapView;
            monitorVm.SetMapVm(mapVm);
            monitorVm.SetMapView(mapView);
            checkingVm.SetMapVm(mapVm);
            mapVm.SetDataCheckingVm(checkingVm);
            mapVm.SetDataMonitoringVm(monitorVm);
            mouseInfoVm.SetMapView(mapView);

            mapView.MapBackground.Color = Colors.LightBlue;
            mapView.MapBackground.LineWidth = 0;
        }
        //public event PropertyChangedEventHandler PropertyChanged;

        private DrawOptionsGroupVm drawOptionsVm;
        private MapVm mapVm;
        private DataMonitoringVm monitorVm;
        private DataCheckingVm checkingVm;
        private MouseInfoVm mouseInfoVm;
    }
}
