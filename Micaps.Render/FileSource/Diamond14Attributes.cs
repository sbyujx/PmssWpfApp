using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.Render.FileSource
{
    public static class Diamond14Attributes
    {
        public static readonly string LineValue = "LineValue";
        public static readonly string Year = "Year";
        public static readonly string Month = "Month";
        public static readonly string Day = "Day";
        public static readonly string Hour = "Hour";
        public static readonly string Aging = "Aging";
        public static readonly string Remaining = "Remaining";
        public static readonly string RemainingPost = "RemainingPost";
        public static readonly string LineType = "LineType";

        public static readonly string LineTypeLine = "Line";
        public static readonly string LineTypeContour = "Contour";

        // When read or new, must decide which setting will be used to show the graphic in product.
        public static readonly string LevelSetting = "LevelSetting";
        public static readonly string ProductType = "ProductType";

        public static readonly string OpenMode = "OpenMode";
        public static readonly string OpenModeNew = "New";
        public static readonly string OpenModeFile = "File";
    }
}
