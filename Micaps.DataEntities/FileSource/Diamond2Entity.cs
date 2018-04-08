using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pmss.Micaps.DataEntities;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public class Diamond2Entity : BaseEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Level { get; set; }
        public int StationAmount { get; set; }
        public List<Diamond2EntityItem> Items { get; set; }
    }

    public class Diamond2EntityItem : BaseEntityItem
    {
        public long StationNumber { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Elevation { get; set; }
        public int StationLevel { get; set; }
        public float Height { get; set; }
        public float Temperature { get; set; }
        public float DewPoint
        {
            get
            {
                if (Validation.IsValidFloat(Temperature) && Validation.IsValidFloat(TemperatureDiff))
                    return Temperature - TemperatureDiff;
                else
                    return 9999;
            }
        }
        public float TemperatureDiff { get; set; }
        public float WindAngle { get; set; }
        public float WindSpeed { get; set; }

        public override bool IsValid()
        {
            return Validation.IsValidMapPoint(Longitude, Latitude);
        }
    }
}
