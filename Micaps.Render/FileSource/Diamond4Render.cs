using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.Core.Enums;
using Pmss.Micaps.DataAccess.FileSource;
using Pmss.Micaps.DataEntities.FileSource;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;
using Pmss.Micaps.Render.Config;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond4Render : IRender, ILevelRender
    {
        public RenderResult GenerateRenderResult(string path, string settingKey)
        {
            var reader = new Diamond4Reader(path);
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
        public RenderResult GenerateRenderResult(string path)
        {
            var reader = new Diamond4Reader(path);
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
                    //GridValue
                    setting = new LabelSetting
                    {
                        TextExpression = $"[{Diamond4Attributes.GridValue}]",
                        Name = $"{Diamond4Attributes.GridValueCn} {startValue}~{endValue}",
                        IsVisible = true,
                        Placement = LabelPlacement.PointAboveLeft,
                        WhereClause = $"{Diamond4Attributes.GridValueFloat}>={item.StartValue} and {Diamond4Attributes.GridValueFloat}<{item.EndValue}",
                        LabelColor = item.LevelColor,
                        Position = LabelPosition.FixedPositionOrRemove
                    };
                    settings.LabelSettings.Add(setting);
                }
            }
            else
            {
                //GridValue
                setting = new LabelSetting
                {
                    TextExpression = $"[{Diamond4Attributes.GridValue}]",
                    Name = Diamond4Attributes.GridValueCn,
                    IsVisible = true,
                    Placement = LabelPlacement.PointAboveLeft,
                    Position = LabelPosition.FixedPositionOrRemove
                };
                settings.LabelSettings.Add(setting);
            }

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
                        WhereClause = setting.WhereClause
                    };
                    labels.LabelClasses.Add(labelClass);
                    setting.RelatedAttributeLabelClass = labelClass;
                }
            }

            return labels;
        }

        private GraphicsLayer GenerateGraphicLayer(Diamond4Entity entity, string settingKey = null)
        {
            var graphics = new List<Graphic>();
            for (int i = 0; i < entity.LonGridCount; i++)
            {
                for (int j = 0; j < entity.latGridCount; j++)
                {
                    float lon = entity.LonStart + j * entity.LonGridSize;
                    float lat = entity.LatStart + i * entity.LatGridSize;
                    float value = entity.Items[i, j];

                    if (Validation.IsValidMapPoint(lon, lat) && Math.Abs(value - 0.0) > 0.01)
                    {
                        var graphic = GenerateGraphic(lon, lat, value, settingKey);
                        graphics.Add(graphic);
                    }
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
        private Graphic GenerateGraphic(float lon, float lat, float value, string settingKey = null)
        {
            MapPoint point = new MapPoint(lon, lat, SpatialReferences.Wgs84);
            var graphic = new Graphic(point);
            var color = Colors.Black;

            if (!string.IsNullOrEmpty(settingKey) && LevelValueManager.Settings.Keys.Contains(settingKey))
            {
                var setting = LevelValueManager.Settings[settingKey];
                float itemValue = Convert.ToSingle(value);
                color = setting.GetValueColor(Convert.ToInt32(itemValue));
            }

            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = 2
            };
            graphic.Symbol = symbol;

            graphic.Attributes[Diamond4Attributes.GridValue] = value.ToString();
            graphic.Attributes[Diamond4Attributes.GridValueFloat] = value;

            return graphic;
        }
    }
}
