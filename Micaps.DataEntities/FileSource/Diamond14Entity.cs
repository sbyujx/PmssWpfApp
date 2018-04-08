using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public class Diamond14Entity : BaseEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Aging { get; set; }
        public List<Diamond14EntityLine> Lines { get; set; } = new List<Diamond14EntityLine>();
        public List<string> Remaining { get; set; } = new List<string>();
        public List<Diamond14EntityLine> Contours { get; set; } = new List<Diamond14EntityLine>();
        public List<string> RemainingPost { get; set; } = new List<string>();
    }
    public class Diamond14EntityLine
    {
        public int Width { get; set; }
        public List<Diamond14EntityLineItem> Items { get; set; } = new List<Diamond14EntityLineItem>();
        public int? LabelValue { get; set; }//NoLabel 0
    }
    public class Diamond14EntityLineItem:BaseEntityItem
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public float Z { get; set; }

        public override bool IsValid()
        {
            return Validation.IsValidMapPoint(Longitude, Latitude);
        }
    }
}
