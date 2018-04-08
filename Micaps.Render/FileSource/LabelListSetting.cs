using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;

namespace Pmss.Micaps.Render.FileSource
{
    public class LabelListSetting
    {
        public double FondSize { get; set; } = 12;
        public bool IsVisible { get; set; } = true;
        public List<LabelSetting> LabelSettings { get; set; } = new List<LabelSetting>();
        public LabelProperties RelatedLabelProperties { get; set; }
    }
}
