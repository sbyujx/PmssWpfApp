using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;

namespace Pmss.Micaps.Render.FileSource
{
    public interface IRender
    {
        //GraphicsLayer GenerateGraphics(string path);
        RenderResult GenerateRenderResult(string path);
    }
}
