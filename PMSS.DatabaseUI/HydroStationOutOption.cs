using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMSS.DatabaseUI
{
    public class HydroStationOutOption
    {
        public string StationId { get; set; }
        public string StationName { get; set; }
        public string Address { get; set; }
        public string RiverName { get; set; }
        public bool IsSelected { get; set; }
    }
}
