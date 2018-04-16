using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.DataAccess.FileSource;
using Pmss.Micaps.DataEntities.FileSource;
using Pmss.Micaps.Core.Enums;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond14Render : IRender
    {
        public RenderResult GenerateRenderResult(string path)
        {
            var reader = new Diamond14Reader(path);
            var entity = reader.RetrieveEntity();

            var result = new RenderResult();
            //result.Miscellaneous = new Dictionary<string, object>();
            result.Miscellaneous.Add(Diamond14Attributes.Year, entity.Year.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Month, entity.Month.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Day, entity.Day.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Hour, entity.Hour.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Aging, entity.Aging.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Remaining, entity.Remaining);
            result.Miscellaneous.Add(Diamond14Attributes.RemainingPost, entity.RemainingPost);
            result.Miscellaneous.Add(Diamond14Attributes.OpenMode, Diamond14Attributes.OpenModeFile);

            result.Layer = GenerateLayer(entity);
            result.Type = DiamondType.Diamond14;

            return result;
        }

        public RenderResult GenerateRenderResult(string path,List<double> latList, List<double> lonList)
        {
            var reader = new Diamond14Reader(path);
            var entity = reader.RetrieveEntity();

            var result = new RenderResult();
            //result.Miscellaneous = new Dictionary<string, object>();
            result.Miscellaneous.Add(Diamond14Attributes.Year, entity.Year.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Month, entity.Month.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Day, entity.Day.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Hour, entity.Hour.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Aging, entity.Aging.ToString());
            result.Miscellaneous.Add(Diamond14Attributes.Remaining, entity.Remaining);
            result.Miscellaneous.Add(Diamond14Attributes.RemainingPost, entity.RemainingPost);
            result.Miscellaneous.Add(Diamond14Attributes.OpenMode, Diamond14Attributes.OpenModeFile);

            result.Layer = GenerateLayer(entity, latList, lonList);
            result.Type = DiamondType.Diamond14;

            return result;
        }

        private Graphic GenerateGraphic(double lat, double lon)
        {
            MapPoint point = new MapPoint(lon, lat, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);

            
                var symbol = new SimpleMarkerSymbol
                {
                    Style = SimpleMarkerStyle.Circle,
                    Color = Colors.DarkGreen
                };
                graphic.Symbol = symbol;

            return graphic;
        }

        private GraphicsLayer GenerateLayer(Diamond14Entity entity, List<double> latList, List<double> lonList)
        {
            var graphics = new List<Graphic>();

            foreach (var line in entity.Lines)
            {
                var tmp = GenerateGraphics(line, Diamond14Attributes.LineTypeLine, Colors.Red);
                graphics.AddRange(tmp);
            }
            foreach (var line in entity.Contours)
            {
                var tmp = GenerateGraphics(line, Diamond14Attributes.LineTypeContour, Colors.Purple);
                graphics.AddRange(tmp);
            }

            for(int i = 0; i < latList.Count; i++)
            {
                var tmp = GenerateGraphic(latList[i], lonList[i]);
                graphics.Add(tmp);
            }

            var layer = new GraphicsLayer
            {
                //GraphicsSource = graphics,
                DisplayName = entity.Description,
                ID = new Guid().ToString()
            };
            foreach (var graphic in graphics)
            {
                layer.Graphics.Add(graphic);
            }

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

            return layer;
        }

        private GraphicsLayer GenerateLayer(Diamond14Entity entity)
        {
            var graphics = new List<Graphic>();

            foreach (var line in entity.Lines)
            {
                var tmp = GenerateGraphics(line, Diamond14Attributes.LineTypeLine, Colors.Red);
                graphics.AddRange(tmp);
            }
            foreach (var line in entity.Contours)
            {
                var tmp = GenerateGraphics(line, Diamond14Attributes.LineTypeContour, Colors.Purple);
                graphics.AddRange(tmp);
            }

            var layer = new GraphicsLayer
            {
                //GraphicsSource = graphics,
                DisplayName = entity.Description,
                ID = new Guid().ToString()
            };
            foreach (var graphic in graphics)
            {
                layer.Graphics.Add(graphic);
            }

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

            return layer;
        }

        private List<Graphic> GenerateGraphics(Diamond14EntityLine line, string type, Color color)
        {
            var result = new List<Graphic>();

            var lineBuilder = new PolylineBuilder(SpatialReferences.WebMercator);
            foreach (var point in line.Items)
            {
                if (point.IsValid())
                {
                    var mapPointWgs84 = new MapPoint(point.Longitude, point.Latitude, SpatialReferences.Wgs84);
                    var mapPoint = GeometryEngine.Project(mapPointWgs84, SpatialReferences.WebMercator) as MapPoint;
                    lineBuilder.AddPoint(mapPoint);
                }
            }

            var geometry = lineBuilder.ToGeometry();
            var resultLine = new Graphic
            {
                Geometry = geometry,
                Symbol = new SimpleLineSymbol
                {
                    Color = color,
                    Width = line.Width
                }
            };
            resultLine.Attributes[Diamond14Attributes.LineValue] = line.LabelValue?.ToString() ?? string.Empty;
            resultLine.Attributes[Diamond14Attributes.LineType] = type;

            result.Add(resultLine);

            return result;
        }
    }
}
