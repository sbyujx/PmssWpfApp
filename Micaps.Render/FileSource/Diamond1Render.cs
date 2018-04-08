using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.DataAccess.FileSource;
using Pmss.Micaps.DataEntities.FileSource;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using Pmss.Micaps.Core.Enums;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond1Render : IRender
    {
        public RenderResult GenerateRenderResult(string path)
        {
            var reader = new Diamond1Reader(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity);
            var labelListSetting = GenerateDefaultLabelSetting();
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                Type = DiamondType.Diamond1
            };

            return result;
        }

        private LabelListSetting GenerateDefaultLabelSetting()
        {
            var settings = new LabelListSetting();
            //Temperature
            LabelSetting setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.Temperature}]",
                Name = Diamond1Attributes.TemperatureCn,
                LabelColor = Colors.Red,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveLeft
            };
            settings.LabelSettings.Add(setting);

            //CloudAmount
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.CloudAmount}]",
                Name = Diamond1Attributes.CloudAmountCn,
                Position = LabelPosition.FixedPositionOrRemove,
                IsVisible = false,
                Placement = LabelPlacement.PointCenterLeft
            };
            settings.LabelSettings.Add(setting);

            //WindSpeed
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.WindSpeed}]",
                Name = Diamond1Attributes.WindSpeedCn,
                Placement = LabelPlacement.PointBelowLeft,
                Position = LabelPosition.FixedPositionOrRemove,
                IsVisible = false
            };
            settings.LabelSettings.Add(setting);

            //SixhoursRain
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.SixhoursRain}]",
                Name = Diamond1Attributes.SixhoursRainCn,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveRight
            };
            settings.LabelSettings.Add(setting);

            //AirPressure
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.AirPressure}]",
                Name = Diamond1Attributes.AirPressureCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointCenterRight
            };
            settings.LabelSettings.Add(setting);

            //StationNumber
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.StationNumber}]",
                Name = Diamond1Attributes.StationNumberCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointBelowRight
            };
            settings.LabelSettings.Add(setting);

            //ThreehoursAP
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.ThreehoursAP}]",
                Name = Diamond1Attributes.ThreehoursAPCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveCenter
            };
            settings.LabelSettings.Add(setting);

            //DiYunLiang
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.DiYunLiang}]",
                Name = Diamond1Attributes.DiYunLiangCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointBelowCenter
            };
            settings.LabelSettings.Add(setting);

            //DiYunGao
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.DiYunGao}]",
                Name = Diamond1Attributes.DiYunGaoCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveCenter
            };
            settings.LabelSettings.Add(setting);

            //DewPoint
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.DewPoint}]",
                Name = Diamond1Attributes.DewPointCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointBelowCenter
            };
            settings.LabelSettings.Add(setting);

            //Visibility
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond1Attributes.Visibility}]",
                Name = Diamond1Attributes.VisibilityCn,
                IsVisible = false,
                Position = LabelPosition.FixedPositionOrRemove,
                Placement = LabelPlacement.PointAboveCenter
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

            return labels;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond1Entity entity)
        {
            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.IsValid())
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
        private Graphic GenerateGraphic(Diamond1EntityItem item)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);

            if (Validation.IsValidInt(item.WindAngle))
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
                    Color = Colors.Black,
                    Size = 5
                };
                graphic.Symbol = symbol;
            }

            graphic.Attributes[Diamond1Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond1Attributes.CloudAmount] = Validation.IsValidInt(item.CloudAmount) ? item.CloudAmount.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.WindSpeed] = Validation.IsValidInt(item.WindSpeed) ? item.WindSpeed.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.AirPressure] = Validation.IsValidInt(item.AirPressure) ? item.AirPressure.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.ThreehoursAP] = Validation.IsValidInt(item.ThreehoursAP) ? item.ThreehoursAP.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.SixhoursRain] = Validation.IsValidFloat(item.SixhoursRain) ? item.SixhoursRain.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.DiYunLiang] = Validation.IsValidInt(item.DiYunLiang) ? item.DiYunLiang.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.DiYunGao] = Validation.IsValidInt(item.DiYunGao) ? item.DiYunGao.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.DewPoint] = Validation.IsValidInt(item.DewPoint) ? item.DewPoint.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.Visibility] = Validation.IsValidFloat(item.Visibility) ? item.Visibility.ToString() : string.Empty;
            graphic.Attributes[Diamond1Attributes.Temperature] = Validation.IsValidFloat(item.Temperature) ? item.Temperature.ToString() : string.Empty;

            return graphic;
        }
    }
}
