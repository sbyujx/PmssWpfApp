using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.FileSource
{
    public interface ILevelRender
    {
        RenderResult GenerateRenderResult(string path, string settingKey);
    }
}
