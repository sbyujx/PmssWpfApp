using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Controls;
using System.ComponentModel;
using Esri.ArcGISRuntime.Geometry;

namespace PmssWpfApp.ViewModel
{
    public class MouseInfoVm : INotifyPropertyChanged
    {
        public void SetMapView(MapView view)
        {
            if (this.pmssMapView == null)
            {
                this.pmssMapView = view;
                this.pmssMapView.MouseMove += PmssMapView_MouseMove;
            }
        }

        public string StatusBarText
        {
            get
            {
                return statusBarText;
            }
            set
            {
                statusBarText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(StatusBarText)));
                }
            }
        }

        private void PmssMapView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (pmssMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry) == null)
                return;

            System.Windows.Point screenPoint = e.GetPosition(pmssMapView);
            //StatusBarText = string.Format("Screen Coords: X = {0}, Y = {1}",
            //    screenPoint.X, screenPoint.Y);

            MapPoint mapPoint = pmssMapView.ScreenToLocation(screenPoint);
            if (pmssMapView.WrapAround)
                mapPoint = GeometryEngine.NormalizeCentralMeridian(mapPoint) as MapPoint;
            mapPoint = GeometryEngine.Project(mapPoint, SpatialReferences.Wgs84) as MapPoint;
            StatusBarText = string.Format("鼠标信息: X = {0}, Y = {1}", Math.Round(mapPoint.X, 4), Math.Round(mapPoint.Y, 4));
        }

        private MapView pmssMapView;
        private string statusBarText = "鼠标信息: ";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
