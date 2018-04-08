using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.Core.Enums;

namespace Pmss.Micaps.Render.FileSource
{
    public class RenderResult
    {
        public DiamondType Type { get; set; }
        public GraphicsLayer Layer { get; set; }
        public LabelListSetting LabelSettings { get; set; }
        public Dictionary<string, object> Miscellaneous { get; set; } = new Dictionary<string, object>();
        public GraphicGroup GraphicGroupVM { get; set; }
    }
}
