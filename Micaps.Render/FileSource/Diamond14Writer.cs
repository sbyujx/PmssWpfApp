using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond14Writer
    {
        public void WriteFile(string path, RenderResult renderResult)
        {
            // if OpenMode is New, the layer is new
            if (string.IsNullOrWhiteSpace(path) || renderResult == null)
            {
                throw new ArgumentNullException();
            }
            using (var writer = new StreamWriter(path, false, Encoding.Default))
            {
                bool isNew = renderResult.Miscellaneous[Diamond14Attributes.OpenMode].ToString() == Diamond14Attributes.OpenModeNew;
                string description = renderResult.Layer.DisplayName;
                if (isNew)
                {
                    description = path.Replace(' ', '-');
                }

                writer.WriteLine($"diamond 14 {description}");

                string year, month, day, hour, aging;
                if (isNew)
                {
                    var time = DateTime.Now;
                    year = time.Year.ToString();
                    month = time.Month.ToString();
                    day = time.Day.ToString();
                    hour = time.Hour.ToString();
                    aging = "0";
                }
                else
                {
                    year = renderResult.Miscellaneous[Diamond14Attributes.Year].ToString();
                    month = renderResult.Miscellaneous[Diamond14Attributes.Month].ToString();
                    day = renderResult.Miscellaneous[Diamond14Attributes.Day].ToString();
                    hour = renderResult.Miscellaneous[Diamond14Attributes.Hour].ToString();
                    aging = renderResult.Miscellaneous[Diamond14Attributes.Aging].ToString();
                }
                writer.WriteLine($"{year} {month} {day} {hour} {aging}");

                // Get Lines and Contours
                List<Graphic> lineList = new List<Graphic>();
                List<Graphic> contourList = new List<Graphic>();
                foreach (var graphic in renderResult.Layer.Graphics)
                {
                    if (graphic.Geometry is Polyline)
                    {
                        object type;
                        if (graphic.Attributes.TryGetValue(Diamond14Attributes.LineType, out type))
                        {
                            if (type.ToString() == Diamond14Attributes.LineTypeLine)
                                lineList.Add(graphic);
                            else if (type.ToString() == Diamond14Attributes.LineTypeContour)
                                contourList.Add(graphic);
                        }
                    }
                }

                // Write Lines
                writer.WriteLine($"LINES: {lineList.Count}");
                foreach (var graphic in lineList)
                {
                    string floadFormat = "0.00000";
                    if (graphic.Geometry is Polyline)
                    {
                        var symbol = graphic.Symbol as SimpleLineSymbol;
                        double width = 2;
                        if (symbol != null)
                            width = symbol.Width;
                        int widthInt = Convert.ToInt32(width);
                        if (width - widthInt > 0.5)
                            widthInt++;

                        var polyline = graphic.Geometry as Polyline;
                        var points = polyline.Parts[0].GetPoints();

                        writer.WriteLine($" {widthInt}  {points.Count()} ");
                        foreach (var point in points)
                        {
                            var wgs84Point = GeometryEngine.Project(point, SpatialReferences.Wgs84) as MapPoint;
                            writer.WriteLine($" {wgs84Point.X.ToString(floadFormat)}  {wgs84Point.Y.ToString(floadFormat)}  0 ");
                        }

                        //Label
                        object lineValue = null;
                        if (graphic.Attributes.TryGetValue(Diamond14Attributes.LineValue, out lineValue))
                        {
                            writer.WriteLine($" {lineValue}  {1} ");

                            var startPoint = points.First();
                            var startPointWgs84 = GeometryEngine.Project(startPoint, SpatialReferences.Wgs84) as MapPoint;
                            writer.WriteLine($" {startPointWgs84.X.ToString(floadFormat)}  {startPointWgs84.Y.ToString(floadFormat)}  0 ");
                        }
                        else
                        {
                            writer.WriteLine(" NoLabel  0 ");
                        }
                    }
                }

                // remaining
                if (isNew)
                {
                    writer.WriteLine("LINES_SYMBOL:  0 ");
                    writer.WriteLine("SYMBOLS:  0");
                }
                else
                {
                    var remaining = renderResult.Miscellaneous[Diamond14Attributes.Remaining] as List<string>;
                    foreach (var line in remaining)
                    {
                        writer.WriteLine(line);
                    }
                }

                // Write Contours
                writer.WriteLine($"CLOSED_CONTOURS: {contourList.Count}");
                foreach (var graphic in contourList)
                {
                    string floadFormat = "0.00000";
                    if (graphic.Geometry is Polyline)
                    {
                        var symbol = graphic.Symbol as SimpleLineSymbol;
                        double width = 2;
                        if (symbol != null)
                            width = symbol.Width;
                        int widthInt = Convert.ToInt32(width);
                        if (width - widthInt > 0.5)
                            widthInt++;

                        var polyline = graphic.Geometry as Polyline;
                        var points = polyline.Parts[0].GetPoints();

                        writer.WriteLine($" {widthInt}  {points.Count()} ");
                        foreach (var point in points)
                        {
                            var wgs84Point = GeometryEngine.Project(point, SpatialReferences.Wgs84) as MapPoint;
                            writer.WriteLine($" {wgs84Point.X.ToString(floadFormat)}  {wgs84Point.Y.ToString(floadFormat)}  0 ");
                        }

                        //Label
                        object lineValue = null;
                        if (graphic.Attributes.TryGetValue(Diamond14Attributes.LineValue, out lineValue))
                        {
                            writer.WriteLine($" {lineValue}  {1} ");

                            var startPoint = points.First();
                            var startPointWgs84 = GeometryEngine.Project(startPoint, SpatialReferences.Wgs84) as MapPoint;
                            writer.WriteLine($" {startPointWgs84.X.ToString(floadFormat)}  {startPointWgs84.Y.ToString(floadFormat)}  0 ");
                        }
                        else
                        {
                            writer.WriteLine(" NoLabel  0 ");
                        }
                    }
                }

                // Remaining Post
                if (isNew)
                {
                    writer.WriteLine("STATION_SITUATION");
                    writer.WriteLine("WEATHER_REGION:  0");
                    writer.WriteLine("FILLAREA:  0");
                    writer.WriteLine("NOTES_SYMBOL:  0");
                    writer.WriteLine("WithProp_LINESYMBOLS:  0 ");
                }
                else
                {
                    var remaining = renderResult.Miscellaneous[Diamond14Attributes.RemainingPost] as List<string>;
                    foreach (var line in remaining)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }
    }
}
