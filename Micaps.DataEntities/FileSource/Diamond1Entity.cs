using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public class Diamond1Entity : BaseEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int StationAmount { get; set; }
        public List<Diamond1EntityItem> Items { get; set; }
    }

    public class Diamond1EntityItem : BaseEntityItem
    {
        public long StationNumber { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Elevation { get; set; }
        public int StationLevel { get; set; }
        public int CloudAmount { get; set; }
        public int WindAngle { get; set; }
        public int WindSpeed { get; set; }
        public int AirPressure { get; set; }
        public int ThreehoursAP { get; set; }
        public int LastWeather1 { get; set; }
        public int LastWeather2 { get; set; }
        public float SixhoursRain { get; set; }
        public int DiYunZhuang { get; set; }
        public int DiYunLiang { get; set; }
        public int DiYunGao { get; set; }
        public int DewPoint { get; set; }
        public float Visibility { get; set; }
        public int CurrentWeather { get; set; }
        public float Temperature { get; set; }
        public int ZhongYunZhuang { get; set; }
        public int GaoYunZhuang { get; set; }
        public int Flag1 { get; set; }
        public int Flag2 { get; set; }
        public int TemperatureDiff24 { get; set; }
        public int PressureDiff24 { get; set; }

        public override bool IsValid()
        {
            return Validation.IsValidMapPoint(Longitude, Latitude);
        }
    }
}
