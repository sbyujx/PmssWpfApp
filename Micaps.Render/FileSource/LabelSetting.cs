using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Esri.ArcGISRuntime.Layers;

namespace Pmss.Micaps.Render.FileSource
{
    public class LabelSetting
    {
        public string TextExpression { get; set; }
        public string Name { get; set; }
        public LabelPlacement Placement { get; set; }
        public LabelPosition Position { get; set; } = LabelPosition.FixedPositionWithOverlaps;
        public bool IsVisible { get; set; } = true;
        public Color LabelColor { get; set; } = Colors.Blue;
        public double FondSize { get; set; } = 12;
        public string WhereClause { get; set; }
        public LabelPriority Priority { get; set; } = LabelPriority.Medium;

        public AttributeLabelClass RelatedAttributeLabelClass { get; set; }
    }
}
