using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public class Diamond3Entity : BaseEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Level { get; set; }
        public int StationAmount { get; set; }
        public List<Diamond3EntityItem> Items { get; set; }
    }

    public class Diamond3EntityItem : BaseEntityItem
    {
        public long StationNumber { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Elevation { get; set; }
        public string StationValue { get; set; }

        public override bool IsValid()
        {
            return Validation.IsValidMapPoint(Longitude, Latitude);
        }
    }
}
