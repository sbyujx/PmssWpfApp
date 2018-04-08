using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.DataAccess.FileSource;
using Pmss.Micaps.DataEntities.FileSource;
using Esri.ArcGISRuntime.Geometry;
using Pmss.Micaps.Render.Helper;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using Pmss.Micaps.Core.Enums;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond2Render : IRender
    {
        public RenderResult GenerateRenderResult(string path)
        {
            var layer = GenerateGraphics(path);
            var labelSettings = GenerateDefaultLabelSettings();
            var type = DiamondType.Diamond2;
            layer.Labeling = GenerateLabelProperties(labelSettings);

            return new RenderResult
            {
                Layer = layer,
                LabelSettings = labelSettings,
                Type = type
            };
        }

        public GraphicsLayer GenerateGraphics(string path)
        {
            var reader = new Diamond2Reader(path);
            var entity = reader.RetrieveEntity();

            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.IsValid())
                    graphics.Add(GenerateGraphic(item));
            }

            var layer = new GraphicsLayer
            {
                GraphicsSource = graphics,
                DisplayName = entity.Description,
                ID = new Guid().ToString()
            };

            return layer;
        }
        public LabelListSetting GenerateDefaultLabelSettings()
        {
            var settings = new LabelListSetting();
            //Temperature
            LabelSetting setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.Temperature}]",
                Name = Diamond2Attributes.TemperatureCn,
                LabelColor = Colors.Red,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveLeft
            };
            settings.LabelSettings.Add(setting);

            //TemperatureDiff
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.TemperatureDiff}]",
                Name = Diamond2Attributes.TemperatureDiffCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointCenterLeft
            };
            settings.LabelSettings.Add(setting);

            //DewPoint
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.DewPoint}]",
                Name = Diamond2Attributes.DewPointCn,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointBelowLeft
            };
            settings.LabelSettings.Add(setting);

            //WindSpeed
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.WindSpeed}]",
                Name = Diamond2Attributes.WindSpeedCn,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveRight
            };
            settings.LabelSettings.Add(setting);

            //Height
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.Height}]",
                Name = Diamond2Attributes.HeightCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointCenterRight
            };
            settings.LabelSettings.Add(setting);

            //StationNumber
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond2Attributes.StationNumber}]",
                Name = Diamond2Attributes.StationNumberCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointBelowRight
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
                    labels.LabelClasses.Add(labelClass);
                    setting.RelatedAttributeLabelClass = labelClass;
                }
            }

            //AttributeLabelClass label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.Temperature}]",
            //    LabelPlacement = LabelPlacement.PointAboveLeft,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Red
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps,

            //};
            //labels.LabelClasses.Add(label);

            //label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.TemperatureDiff}]",
            //    LabelPlacement = LabelPlacement.PointCenterLeft,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Blue
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps,
            //    IsVisible = false
            //};
            //labels.LabelClasses.Add(label);

            //label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.DewPoint}]",
            //    LabelPlacement = LabelPlacement.PointBelowLeft,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Blue
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps,
            //    IsVisible = false
            //};
            //labels.LabelClasses.Add(label);

            //label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.WindSpeed}]",
            //    LabelPlacement = LabelPlacement.PointAboveRight,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Blue
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps
            //};
            //labels.LabelClasses.Add(label);

            //label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.Height}]",
            //    LabelPlacement = LabelPlacement.PointCenterRight,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Blue
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps,
            //    IsVisible = false
            //};
            //labels.LabelClasses.Add(label);

            //label = new AttributeLabelClass
            //{
            //    TextExpression = $"[{Diamond2Attributes.StationNumber}]",
            //    LabelPlacement = LabelPlacement.PointBelowRight,
            //    Symbol = new TextSymbol
            //    {
            //        Color = Colors.Blue
            //    },
            //    LabelPosition = LabelPosition.FixedPositionWithOverlaps,
            //    IsVisible = false
            //};
            //labels.LabelClasses.Add(label);

            return labels;
        }
        private Graphic GenerateGraphic(Diamond2EntityItem item)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);

            if (Validation.IsValidFloat(item.WindAngle))
            {
                var symbol = new PictureMarkerSymbol();
                symbol.SetSourceAsync(new Uri("pack://application:,,,/Pmss.Micaps.Render;component/Img/Wind.png")).GetAwaiter().GetResult();
                symbol.Angle = item.WindAngle;
                graphic.Symbol = symbol;
            }
            else
            {
                var symbol = new SimpleMarkerSymbol
                {
                    Style = SimpleMarkerStyle.Circle,
                    Color = Colors.Black
                };
                graphic.Symbol = symbol;
            }


            graphic.Attributes[Diamond2Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond2Attributes.Temperature] = Validation.IsValidFloat(item.Temperature) ? item.Temperature.ToString() : string.Empty;
            graphic.Attributes[Diamond2Attributes.TemperatureDiff] = Validation.IsValidFloat(item.TemperatureDiff) ? item.TemperatureDiff.ToString() : string.Empty;
            graphic.Attributes[Diamond2Attributes.Height] = Validation.IsValidFloat(item.Height) ? item.Height.ToString() : string.Empty;
            graphic.Attributes[Diamond2Attributes.DewPoint] = Validation.IsValidFloat(item.DewPoint) ? item.DewPoint.ToString() : string.Empty;
            graphic.Attributes[Diamond2Attributes.WindSpeed] = Validation.IsValidFloat(item.WindSpeed) ? item.WindSpeed.ToString() : string.Empty;

            return graphic;
        }
    }
}
