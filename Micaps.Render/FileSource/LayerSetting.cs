using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.FileSource
{
    public class LayerSetting
    {
        public LabelListSetting LabelList { get; set; }
        public Dictionary<string,string> Miscellaneous { get; set; }
    }
}
