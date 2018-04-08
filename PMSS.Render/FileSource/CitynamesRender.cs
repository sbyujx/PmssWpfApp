using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Pmss.Micaps.Render.FileSource;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;

namespace PMSS.Render.FileSource
{
    public class CitynamesRender : IRender
    {
        public RenderResult GenerateRenderResult(string path)
        {
            var reader = new CitynamesDataAccess(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity);
            var labelListSetting = GenerateDefaultLabelSetting();
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
            };

            return result;
        }

        private LabelListSetting GenerateDefaultLabelSetting()
        {
            var settings = new LabelListSetting();

            //Name
            var setting = new LabelSetting
            {
                TextExpression = $"[Name]",
                IsVisible = true,
                Placement = LabelPlacement.PointAboveLeft,
                LabelColor = Colors.Black,
                Position = LabelPosition.RepositionOrRemove
            };
            settings.LabelSettings.Add(setting);

            return settings;
        }
        private LabelProperties GenerateLabelProperties(LabelListSetting listSetting)
        {
            LabelProperties labels = new LabelProperties();

            if (listSetting != null)
            {
                listSetting.RelatedLabelProperties = labels;
                foreach (var setting in listSetting.LabelSettings)
                {
                    var labelClass = new AttributeLabelClass
                    {
                        TextExpression = setting.TextExpression,
                        LabelPlacement = setting.Placement,
                        LabelPosition = setting.Position,
                        IsVisible = setting.IsVisible,
                        Symbol = new TextSymbol
                        {
                            Color = setting.LabelColor
                        }
                    };
                    labelClass.Symbol.Font.FontFamily = "微软雅黑";
                    labelClass.Symbol.Font.FontSize = 14; //12 default
                    labels.LabelClasses.Add(labelClass);
                    setting.RelatedAttributeLabelClass = labelClass;
                }
            }

            return labels;
        }

        private GraphicsLayer GenerateGraphicLayer(CitynamesEntity entity)
        {
            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.Longitude >= -180 && item.Longitude <= 180 && item.Latitude >= -90 && item.Latitude <= 90)
                {
                    graphics.Add(GenerateGraphic(item));
                }
            }

            var layer = new GraphicsLayer
            {
                GraphicsSource = graphics,
                DisplayName = entity.Description,
                ID = new Guid().ToString()
            };

            return layer;
        }
        private Graphic GenerateGraphic(CitynamesEntityItem item)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = Colors.Black,
                Size = 4
            };
            graphic.Symbol = symbol;

            graphic.Attributes["Name"] = item.Name;

            return graphic;
        }


    }
    public class CitynamesDataAccess
    {
        public CitynamesDataAccess(string path)
        {
            filePath = path;
        }

        public CitynamesEntity RetrieveEntity()
        {
            if (!File.Exists(this.filePath))
            {
                throw new FileNotFoundException(this.filePath);
            }

            var result = new CitynamesEntity();
            using (var reader = new StreamReader(this.filePath))
            {
                string pattern = @"\s+";
                string line = reader.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    throw new InvalidDataException(this.filePath);
                }

                var array = Regex.Split(line, pattern);
                result.Description = array[0];

                while ((line = reader.ReadLine()?.Trim()) != null)
                {
                    array = Regex.Split(line, pattern);
                    if (array.Length != 3)
                    {
                        throw new InvalidDataException(this.filePath);
                    }

                    var item = new CitynamesEntityItem();
                    item.Longitude = Convert.ToSingle(array[0]);
                    item.Latitude = Convert.ToSingle(array[1]);
                    item.Name = array[2];

                    result.Items.Add(item);
                }
            }

            return result;
        }

        private string filePath;
    }
    public class CitynamesEntity
    {
        public string Description { get; set; }
        public List<CitynamesEntityItem> Items { get; set; } = new List<CitynamesEntityItem>();
    }
    public class CitynamesEntityItem
    {
        public string Name { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}
