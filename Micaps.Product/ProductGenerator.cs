using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using Pmss.Micaps.Render.Config;
using Pmss.Micaps.Render.FileSource;
using Esri.ArcGISRuntime.Controls;

namespace Pmss.Micaps.Product
{
    public class ProductGenerator
    {
        //renderResult.Miscellaneous.Add(Diamond14Attributes.LevelSetting, productDialog.LevelSetting);
        //renderResult.Miscellaneous.Add(Diamond14Attributes.ProductType, productDialog.Type);
        public GraphicsLayer Generate(RenderResult renderResult)
        {
            if (renderResult == null || renderResult.Miscellaneous == null || !renderResult.Miscellaneous.Keys.Contains(Diamond14Attributes.LevelSetting))
            {
                throw new Exception("找不到颜色配置");
            }
            if (string.IsNullOrEmpty(renderResult.Miscellaneous[Diamond14Attributes.LevelSetting].ToString()))
            {
                throw new ArgumentNullException("无效的颜色配置");
            }
            var sourceLayer = renderResult.Layer;
            var settings = LevelValueManager.Settings[renderResult.Miscellaneous[Diamond14Attributes.LevelSetting].ToString()];

            var layer = new GraphicsLayer
            {
                DisplayName = sourceLayer.DisplayName
            };
            //layer.Opacity = 0.5;

            var dict = new SortedDictionary<double, Graphic>();
            foreach (var graphic in sourceLayer.Graphics)
            {
                if (graphic.Geometry is Polyline)
                {
                    // Level value will be int. If null, value will be 0
                    int value = Convert.ToInt32(graphic.Attributes[Diamond14Attributes.LineValue]);

                    var polyline = graphic.Geometry as Polyline;
                    var polygon = ToPolygon(polyline);
                    var polygonSymbol = new SimpleFillSymbol
                    {
                        //Color = Color.FromArgb(0x66, 0xff, 0, 0)
                        Color = settings.GetValueColor(value)
                    };

                    var newGraphic = new Graphic
                    {
                        Geometry = polygon,
                        Symbol = polygonSymbol
                    };
                    layer.Graphics.Add(newGraphic);

                    //double area = GeometryEngine.Area(polygon); clock wise will be positive
                    double area = Math.Abs(GeometryEngine.Area(polygon));
                    dict.Add(area, newGraphic);
                }
            }

            int zindex = dict.Keys.Count;
            foreach(var graphic in dict.Values)
            {
                graphic.ZIndex = zindex--;
            }

            return layer;
        }

        public static void ZoomMapView(MapView view)
        {
            var extent = new ViewpointExtent();
            extent.XMin = Constants.XMin;
            extent.XMax = Constants.XMax;
            extent.YMin = Constants.YMin;
            extent.YMax = Constants.YMax;
            extent.SpatialReferenceID = SpatialReferences.WebMercator.Wkid;

            view.SetView(extent);
        }

        private Polygon ToPolygon(Polyline line)
        {
            var builder = new PolygonBuilder(SpatialReferences.WebMercator);
            if (line.Parts != null && line.Parts.Count > 0)
            {
                foreach (var point in line.Parts[0].GetPoints())
                {
                    builder.AddPoint(point);
                }
            }
            return builder.ToGeometry();
        }

    }
}
