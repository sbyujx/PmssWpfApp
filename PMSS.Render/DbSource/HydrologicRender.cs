using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.Render.FileSource;
using PMSS.SqlDataAccess;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using System.Windows.Media;

namespace PMSS.Render.DbSource
{
    public class HydrologicRender
    {
        public GraphicGroup GraphicGroupVM { get; set; } = new GraphicGroup();
        public RenderResult GenerateRenderResult(DateTime from, DateTime to, bool isRealTime)
        {
            GraphicsLayer layer;
            if (isRealTime)
            {
                var entity = new HydrologicReader().RetrieveEntity(from, to);
                //var entity = FakeDataSource();

                layer = GenerateGraphicLayer(entity);
            }
            else
            {
                var entity = new HydrologicReader().RetrieveHisEntity(from, to);
                //var entity = FakeDataSource();

                layer = GenerateGraphicLayer(entity);
            }


            var labelListSetting = GenerateDefaultLabelSetting();
            layer.Labeling = GenerateLabelProperties(labelListSetting);
            layer.DisplayName = $"水情数据 {from} - {to}";

            var result = new RenderResult
            {
                Layer = layer,
                LabelSettings = labelListSetting,
                GraphicGroupVM = GraphicGroupVM
            };

            return result;

        }

        private LabelListSetting GenerateDefaultLabelSetting()
        {
            var settings = new LabelListSetting();

            //Name
            var setting = new LabelSetting
            {
                TextExpression = $"[{HydrologicAttributes.Name}]",
                Name = HydrologicAttributes.NameCn,
                IsVisible = false,
                Placement = LabelPlacement.PointAboveLeft
                //Position = LabelPosition.FixedPositionOrRemove
            };
            settings.LabelSettings.Add(setting);

            //L
            setting = new LabelSetting
            {
                TextExpression = $"[{HydrologicAttributes.L}]",
                Name = HydrologicAttributes.LCn,
                IsVisible = false,
                Placement = LabelPlacement.PointBelowLeft
                //Position = LabelPosition.FixedPositionOrRemove
            };
            settings.LabelSettings.Add(setting);

            //Wl1
            setting = new LabelSetting
            {
                TextExpression = $"[{HydrologicAttributes.Wl1}]",
                Name = HydrologicAttributes.Wl1Cn,
                IsVisible = false,
                Placement = LabelPlacement.PointBelowRight,
                LabelColor = Colors.Brown
                //Position = LabelPosition.FixedPositionOrRemove
            };
            settings.LabelSettings.Add(setting);

            //Type
            setting = new LabelSetting
            {
                TextExpression = $"[{HydrologicAttributes.Type}]",
                Name = HydrologicAttributes.TypeCn,
                IsVisible = false,
                Placement = LabelPlacement.PointAboveRight,
                LabelColor = Colors.Brown
                //Position = LabelPosition.FixedPositionOrRemove
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
                    labels.LabelClasses.Add(labelClass);
                    setting.RelatedAttributeLabelClass = labelClass;
                }
            }

            return labels;
        }

        private GraphicsLayer GenerateGraphicLayer(HydrologicEntity entity)
        {
            var graphics = new List<Graphic>();
            if (entity != null && entity.Items != null)
            {
                foreach (var item in entity.Items)
                {
                    if (!double.IsNaN(item.Longitude) && !double.IsNaN(item.Latitude) && item.Wl1 >= 0)
                    {
                        var graphic = GenerateGraphic(item);
                        graphics.Add(graphic);
                    }
                }
            }

            var layer = new GraphicsLayer
            {
                GraphicsSource = graphics
            };

            return layer;
        }

        private GraphicsLayer GenerateGraphicLayer(HisHydrologicEntity entity)
        {
            var graphics = new List<Graphic>();
            if (entity != null && entity.Items != null)
            {
                foreach (var item in entity.Items)
                {
                    if (!double.IsNaN(item.Longitude) && !double.IsNaN(item.Latitude) && item.Wl1 >= 0)
                    {
                        var graphic = GenerateGraphic(item);
                        graphics.Add(graphic);
                    }
                }
            }

            var layer = new GraphicsLayer
            {
                GraphicsSource = graphics
            };

            return layer;
        }
        private Graphic GenerateGraphic(HydrologicEntityItem item)
        {
            var point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var color = Colors.Black;
            int size = 5;
            if (item.L > item.Wl1 && item.Wl1 > 0.05)
            {
                color = Colors.Red;
                size = 7;
            }
            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = size
            };

            var graphic = new Graphic
            {
                Geometry = point,
                Symbol = symbol
            };
            graphic.Attributes[HydrologicAttributes.Name] = item.Name;
            graphic.Attributes[HydrologicAttributes.L] = double.IsNaN(item.L) ? string.Empty : item.L.ToString();
            graphic.Attributes[HydrologicAttributes.Wl1] = double.IsNaN(item.Wl1) ? string.Empty : item.Wl1.ToString();
            graphic.Attributes[HydrologicAttributes.Stationid] = item.Stationid;
            graphic.Attributes[HydrologicAttributes.Recordid] = item.Recordid;
            graphic.Attributes[HydrologicAttributes.Type] = item.Type;
            graphic.Attributes[HydrologicAttributes.Q] = double.IsNaN(item.Q) ? string.Empty : item.Q.ToString();
            graphic.Attributes[HydrologicAttributes.Time] = item.Time.ToString();

            GraphicGroupVM.AddToGroupItem(item.Type, graphic);

            return graphic;
        }

        private Graphic GenerateGraphic(HisHydrologicEntityItem item)
        {
            var point = new MapPoint(item.Longitude, item.Latitude, SpatialReferences.Wgs84);
            var color = Colors.Black;
            int size = 5;
            if (item.MaxL > item.Wl1 && item.Wl1 > 0.05)
            {
                color = Colors.Red;
                size = 7;
            }
            var symbol = new SimpleMarkerSymbol
            {
                Style = SimpleMarkerStyle.Circle,
                Color = color,
                Size = size
            };

            var graphic = new Graphic
            {
                Geometry = point,
                Symbol = symbol
            };
            graphic.Attributes[HydrologicAttributes.Name] = item.Name;
            graphic.Attributes[HydrologicAttributes.L] = double.IsNaN(item.L) ? string.Empty : item.L.ToString();
            graphic.Attributes[HydrologicAttributes.Wl1] = double.IsNaN(item.Wl1) ? string.Empty : item.Wl1.ToString();
            graphic.Attributes[HydrologicAttributes.Stationid] = item.Stationid;
            graphic.Attributes[HydrologicAttributes.Recordid] = item.Recordid;
            graphic.Attributes[HydrologicAttributes.Type] = item.Type;
            graphic.Attributes[HydrologicAttributes.Q] = double.IsNaN(item.Q) ? string.Empty : item.Q.ToString();
            graphic.Attributes[HydrologicAttributes.Time] = item.Time.ToString();

            GraphicGroupVM.AddToGroupItem(item.Type, graphic);

            return graphic;
        }

        private HydrologicEntity FakeDataSource()
        {
            var entity = new HydrologicEntity();
            entity.Items = new List<HydrologicEntityItem>();

            var item = new HydrologicEntityItem
            {
                Latitude = 44.081,
                Longitude = 130.176,
                Wl1 = 10,
                L = 11,
                Name = "库威",
                Stationid = "00100400",
                Recordid = 1,
                Type = "河道站",
                Q = 10,
                Time = DateTime.Now

            };
            entity.Items.Add(item);

            item = new HydrologicEntityItem
            {
                Latitude = 50.224,
                Longitude = 119.53,
                Wl1 = 18,
                L = 11,
                Name = "库威",
                Stationid = "00100400",
                Recordid = 1,
                Type = "水库站",
                Q = 10,
                Time = DateTime.Now

            };
            entity.Items.Add(item);

            item = new HydrologicEntityItem
            {
                Latitude = 45.639,
                Longitude = 130.069,
                Wl1 = 10,
                L = 11,
                Name = "库威",
                Stationid = "00100400",
                Recordid = 1,
                Type = "河道站",
                Q = 10,
                Time = DateTime.Now

            };
            entity.Items.Add(item);

            item = new HydrologicEntityItem
            {
                Latitude = 43.937,
                Longitude = 119.449,
                Wl1 = 15,
                L = 11,
                Name = "库威",
                Stationid = "00100400",
                Recordid = 1,
                Type = "河道站",
                Q = 10,
                Time = DateTime.Now

            };
            entity.Items.Add(item);

            return entity;
        }
    }
}
