using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Geometry;

namespace Pmss.Micaps.Product
{
    public enum ProductTypeEnum
    {
        Unknown,
        Flood,
        River,
        Disaster,
        Zilao
    }
    public enum ProductRegionEnum
    {
        Country,
        Region
    }
    public static class Constants
    {
        static Constants()
        {
            MapPoint minLonLat = new MapPoint(Constants.LonMin, Constants.LatMin, SpatialReferences.Wgs84);
            MapPoint maxLonLat = new MapPoint(Constants.LonMax, Constants.LatMax, SpatialReferences.Wgs84);

            MapPoint minXY = GeometryEngine.Project(minLonLat, SpatialReferences.WebMercator) as MapPoint;
            MapPoint maxXY = GeometryEngine.Project(maxLonLat, SpatialReferences.WebMercator) as MapPoint;

            XMin = minXY.X;
            YMin = minXY.Y;
            XMax = maxXY.X;
            YMax = maxXY.Y;

            // make sure the snapshot can be ImageWidth / ImageHeight
            if ((maxXY.X - minXY.X) / (maxXY.Y - minXY.Y) > ImageWidth / ImageHeight)
            {
                double height = (maxXY.X - minXY.X) * ImageHeight / ImageWidth;
                YMin -= (height - (maxXY.Y - minXY.Y)) / 2;
                YMax += (height - (maxXY.Y - minXY.Y)) / 2;
            }
            else
            {
                double width = (maxXY.Y - minXY.Y) * ImageWidth / ImageHeight;
                XMin -= (width - (maxXY.X - minXY.X)) / 2;
                XMax += (width - (maxXY.X - minXY.X)) / 2;
            }


        }
        public static readonly double LonMin = 70;
        public static readonly double LonMax = 137;
        public static readonly double LatMin = 17;
        public static readonly double LatMax = 55;

        public static readonly double XMin;
        public static readonly double YMin;
        public static readonly double XMax;
        public static readonly double YMax;

        public static readonly int ImageWidth = 2087;
        public static readonly int ImageHeight = 1613; //1693-80*2 -80??????
    }
}
