using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pmss.Micaps.DataEntities.FileSource
{
    public class Diamond4Entity : BaseEntity
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Aging { get; set; }
        public int Level { get; set; }

        public float LonGridSize { get; set; }
        public float LatGridSize { get; set; }
        public float LonStart { get; set; }
        public float LonEnd { get; set; }
        public float LatStart { get; set; }
        public float LatEnd { get; set; }
        public int latGridCount { get; set; }
        public int LonGridCount { get; set; }

        public float ContourInterval { get; set; }
        public float ContourStart { get; set; }
        public float ContourEnd { get; set; }
        public float SmoothFactor { get; set; }
        public float BoldFactor { get; set; }

        public float[,] Items { get; set; }
    }

    //public class Diamond4EntityItem : BaseEntityItem
    //{
    //    public override bool IsValid()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
