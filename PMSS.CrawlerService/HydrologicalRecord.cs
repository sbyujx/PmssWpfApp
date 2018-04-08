using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.CrawlerService
{
    public class HydrologicalRecord
    {
        public HydrologyStation Station { get; set; }
        public double L { get; set; }
        public double Q { get; set; }
        public double Wl1 { get; set; }
        public double Wl2 { get; set; }
        public DateTime Time { get; set; }
        public bool isKey { get; set; }

        public HydrologicalRecord()
        {
            this.Station = new HydrologyStation();
            this.isKey = false;
        }
    }
}
