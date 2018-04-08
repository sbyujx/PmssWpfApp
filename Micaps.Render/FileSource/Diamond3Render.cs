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
using Pmss.Micaps.Render.Config;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond3Render : IRender, IMonitorRender, ILevelAndMonitorRender, ILevelRender
    {
        public RenderResult GenerateRenderResult(string path, string settingKey)
        {
            var reader = new Diamond3Reader(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity, settingKey);
            var labelListSetting = GenerateDefaultLabelSetting(settingKey);
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                Type = DiamondType.Diamond3
            };

            return result;
        }

        public RenderResult GenerateRenderResult(string path, string settingKey, int value)
        {
            var reader = new Diamond3Reader(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity, settingKey, value);
            var labelListSetting = GenerateDefaultLabelSetting(settingKey);
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                Type = DiamondType.Diamond3
            };

            return result;
        }
        public RenderResult GenerateRenderResult(string path)
        {
            var reader = new Diamond3Reader(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity);
            var labelListSetting = GenerateDefaultLabelSetting();
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                Type = DiamondType.Diamond3
            };

            return result;
        }
        public RenderResult GenerateRenderResult(string path, int monitorValue)
        {
            var reader = new Diamond3Reader(path);
            var entity = reader.RetrieveEntity();

            var layer = GenerateGraphicLayer(entity, monitorValue);
            var labelListSetting = GenerateDefaultLabelSetting();
            layer.Labeling = GenerateLabelProperties(labelListSetting);

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                Type = DiamondType.Diamond3
            };

            return result;
        }
        private LabelListSetting GenerateDefaultLabelSetting(string settingKey = null)
        {
            var settings = new LabelListSetting();
            LabelSetting setting = null;

            if (settingKey != null && LevelValueManager.Settings.Keys.Contains(settingKey))
            {
                var levelSetting = LevelValueManager.Settings[settingKey];
                foreach (var item in levelSetting.LevelSettingVmList)
                {
                    string startValue = item.StartValue.ToString();
                    string endValue = item.EndValue.ToString();
                    if (item.StartValue == int.MinValue)
                    {
                        startValue = "Min";
                    }
                    if (item.EndValue == int.MaxValue)
                    {
                        endValue = "Max";
                    }
                    //StationValue
                    setting = new LabelSetting
                    {
                        TextExpression = $"[{Diamond3Attributes.StationValue}]",
                        Name = $"{Diamond3Attributes.StationValueCn} {startValue}~{endValue}",
                        IsVisible = true,
                        Placement = LabelPlacement.PointAboveLeft,
                        WhereClause = $"{Diamond3Attributes.StationValueFloat}>={item.StartValue} and {Diamond3Attributes.StationValueFloat}<{item.EndValue}",
                        LabelColor = item.LevelColor,
                        Position = LabelPosition.FixedPositionOrRemove
                    };
                    settings.LabelSettings.Add(setting);
                }
            }
            else
            {
                //StationValue
                setting = new LabelSetting
                {
                    TextExpression = $"[{Diamond3Attributes.StationValue}]",
                    Name = Diamond3Attributes.StationValueCn,
                    IsVisible = true,
                    Position = LabelPosition.FixedPositionOrRemove,
                    Placement = LabelPlacement.PointAboveLeft
                };
                settings.LabelSettings.Add(setting);
            }

            //StationNumber
            setting = new LabelSetting
            {
                TextExpression = $"[{Diamond3Attributes.StationNumber}]",
                Name = Diamond3Attributes.StationNumberCn,
                IsVisible = false,
                Position = LabelPosition.RepositionOrRemove,
                Placement = LabelPlacement.PointBelowRight,
                Priority = LabelPriority.High
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
                        },
                        WhereClause = setting.WhereClause,
                        LabelPriority = setting.Priority
                    };
                    labels.LabelClasses.Add(labelClass);
                    setting.RelatedAttributeLabelClass = labelClass;
                }
            }

            return labels;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond3Entity entity)
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
        private Graphic GenerateGraphic(Diamond3EntityItem item)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = Colors.Black,
                Size = 3
            };
            graphic.Symbol = symbol;

            graphic.Attributes[Diamond3Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond3Attributes.StationValue] = item.StationValue;

            return graphic;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond3Entity entity, int value)
        {
            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.IsValid())
                {
                    graphics.Add(GenerateGraphic(item, value));
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
        private Graphic GenerateGraphic(Diamond3EntityItem item, int value)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);
            var color = Colors.Black;
            int size = 3;
            if (Convert.ToSingle(item.StationValue) >= value)// won't monitor level, so StatinValue must be float
            {
                color = Colors.Red;
                size = 5;
            }

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = size
            };
            graphic.Symbol = symbol;

            graphic.Attributes[Diamond3Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond3Attributes.StationValue] = item.StationValue;

            return graphic;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond3Entity entity, string settingKey, int value)
        {
            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.IsValid())
                {
                    graphics.Add(GenerateGraphic(item, settingKey, value));
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
        private Graphic GenerateGraphic(Diamond3EntityItem item, string settingKey, int value)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);
            var color = Colors.Black;
            int size = 3;

            if (!string.IsNullOrEmpty(settingKey) && LevelValueManager.Settings.Keys.Contains(settingKey))
            {
                var setting = LevelValueManager.Settings[settingKey];
                float itemValue = Convert.ToSingle(item.StationValue);
                color = setting.GetValueColor(Convert.ToInt32(itemValue));
            }

            if (Convert.ToSingle(item.StationValue) >= value)// won't monitor level, so StatinValue must be float
            {
                color = Colors.Red;
                size = 5;
            }

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = size
            };
            graphic.Symbol = symbol;

            graphic.Attributes[Diamond3Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond3Attributes.StationValue] = item.StationValue.ToString();
            graphic.Attributes[Diamond3Attributes.StationValueFloat] = Convert.ToSingle(item.StationValue);

            return graphic;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond3Entity entity, string settingKey)
        {
            var graphics = new List<Graphic>();
            foreach (var item in entity.Items)
            {
                if (item.IsValid() && PassCumtomLgic(item))
                {
                    graphics.Add(GenerateGraphic(item, settingKey));
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
        private Graphic GenerateGraphic(Diamond3EntityItem item, string settingKey)
        {
            MapPoint point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);
            var color = Colors.Black;
            int size = 3;

            if (!string.IsNullOrEmpty(settingKey) && LevelValueManager.Settings.Keys.Contains(settingKey))
            {
                var setting = LevelValueManager.Settings[settingKey];
                float itemValue = Convert.ToSingle(item.StationValue);
                color = setting.GetValueColor(Convert.ToInt32(itemValue));
            }

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = size
            };
            graphic.Symbol = symbol;

            graphic.Attributes[Diamond3Attributes.StationNumber] = item.StationNumber;
            graphic.Attributes[Diamond3Attributes.StationValue] = item.StationValue.ToString();
            graphic.Attributes[Diamond3Attributes.StationValueFloat] = Convert.ToSingle(item.StationValue);

            return graphic;
        }

        protected virtual bool PassCumtomLgic(Diamond3EntityItem item)
        {
            return true;
        }
    }
}
