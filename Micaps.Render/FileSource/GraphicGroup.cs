using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.FileSource
{
    public class GraphicGroup
    {
        public GraphicGroup()
        {

        }
        public void AddToGroupItem(string name, Graphic graphic)
        {
            if (dict.ContainsKey(name))
            {
                dict[name].AddGraphic(graphic);
            }
            else
            {
                var item = new GraphicGroupItem(name);
                item.AddGraphic(graphic);
                dict.Add(name, item);
                GraphicGroupItems.Add(item);
            }
        }
        public List<GraphicGroupItem> GraphicGroupItems { get; set; } = new List<GraphicGroupItem>();
        private Dictionary<string, GraphicGroupItem> dict = new Dictionary<string, GraphicGroupItem>();
    }

    public class GraphicGroupItem
    {
        public GraphicGroupItem(string name)
        {
            ItemName = name;
        }
        public void AddGraphic(Graphic graphic)
        {
            graphics.Add(graphic);
        }
        public string ItemName { get; set; }
        public bool Visible
        {
            get
            {
                return this.visible;
            }
            set
            {
                this.visible = value;
                if (graphics != null)
                {
                    foreach (var graphic in graphics)
                    {
                        graphic.IsVisible = value;
                    }
                }
            }
        }

        private List<Graphic> graphics = new List<Graphic>();
        private bool visible = true;
    }
}
