using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.FileSource
{
    public interface IMonitorRender
    {
        RenderResult GenerateRenderResult(string path, int monitorValue);
    }
}
