using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.Config
{
    public class LevelList<T>
    {
        SortedDictionary<T,Level<T>> List { get; set; }
    }
}
