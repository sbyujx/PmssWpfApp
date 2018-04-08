using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public static class Validation
    {
        public static bool IsValidInt(int value)
        {
            return value != 9999;
        }

        public static bool IsValidFloat(float value)
        {
            return Math.Abs(value - 9999) > 0.01;
        }

        public static bool IsValidMapPoint(float lon, float lat)
        {
            //15,60  70,135
            return (lon > 70) && (lon < 135) && (lat > 15) && (lat < 60);
        }
    }
}
