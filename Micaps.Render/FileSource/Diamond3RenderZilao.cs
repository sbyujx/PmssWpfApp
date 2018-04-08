using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.DataEntities.FileSource;

namespace Pmss.Micaps.Render.FileSource
{
    public class Diamond3RenderZilao : Diamond3Render
    {
        protected override bool PassCumtomLgic(Diamond3EntityItem item)
        {
            float value = Convert.ToSingle(item.StationValue);
            if (Math.Abs(value - 0.0) > 0.01)
            {
                return true;
            }

            return false;
        }
    }
}
