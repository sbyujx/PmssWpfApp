using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.DataToMicaps
{
    public class MicapsRecord
    {
        public int Uid { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double ValueL { get; set; }
        public double ValueQ { get; set; }
        public double ValueWL1 { get; set; }
        public double ValueWL2 { get; set; }
    }
}
