using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Layers;
using Pmss.Micaps.Core.Enums;
using Pmss.Micaps.DataAccess.FileSource;

namespace Pmss.Micaps.Render.FileSource
{
    public class GraphicsLayerFactory
    {
        private static Dictionary<DiamondType, IRender> renders = new Dictionary<DiamondType, IRender>();
        private static Dictionary<DiamondType, ILevelAndMonitorRender> monitorRenders = new Dictionary<DiamondType, ILevelAndMonitorRender>();
        private static Dictionary<DiamondType, ILevelRender> levelRenders = new Dictionary<DiamondType, ILevelRender>();

        static GraphicsLayerFactory()
        {
            renders.Add(DiamondType.Diamond1, new Diamond1Render());
            renders.Add(DiamondType.Diamond2, new Diamond2Render());
            renders.Add(DiamondType.Diamond3, new Diamond3Render());
            renders.Add(DiamondType.Diamond4, new Diamond4Render());
            renders.Add(DiamondType.Diamond14, new Diamond14Render());

            monitorRenders.Add(DiamondType.Diamond3, new Diamond3Render());

            levelRenders.Add(DiamondType.Diamond3, new Diamond3Render());
            levelRenders.Add(DiamondType.Diamond4, new Diamond4Render());
        }

        public static RenderResult GenerateRenderResultFromFile(string path)
        {
            var type = BaseReader.GetDiamondType(path);
            if (type == DiamondType.NotExist || !renders.Keys.Contains(type))
            {
                throw new NotSupportedException(path);
            }
            return renders[type].GenerateRenderResult(path);
        }

        public static RenderResult GenerateRenderResultFromFile(string path, string settingKey, int value)
        {
            var type = BaseReader.GetDiamondType(path);
            if (type == DiamondType.NotExist || !monitorRenders.Keys.Contains(type))
            {
                throw new NotSupportedException(path);
            }
            return monitorRenders[type].GenerateRenderResult(path, settingKey, value);
        }

        public static RenderResult GenerateRenderResultFromFile(string path, string settingKey)
        {
            var type = BaseReader.GetDiamondType(path);
            if (type == DiamondType.NotExist || !levelRenders.Keys.Contains(type))
            {
                throw new NotSupportedException(path);
            }
            return levelRenders[type].GenerateRenderResult(path, settingKey);
        }

        //public static GraphicsLayer GenerateGraphicsLayerFromFile(string path)
        //{
        //    var diamondType = BaseReader.GetDiamondType(path);
        //    if (diamondType == DiamondType.NotExist)
        //    {
        //        throw new NotSupportedException(path);
        //    }
        //    return renders[diamondType].GenerateGraphics(path);
        //}
    }
}
