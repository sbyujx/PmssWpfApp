using System;
using System.Text;
using System.Collections.Generic;


namespace PMSS.SqlDataOutput {
    
    public class Hydrologicaldata {
        public virtual int Recordid { get; set; }
        public virtual string Stationid { get; set; }
        public virtual string L { get; set; }
        public virtual string Q { get; set; }
        public virtual string Wl1 { get; set; }
        public virtual string Wl2 { get; set; }
        public virtual DateTime? Time { get; set; }
        public virtual int? Issign { get; set; }
    }
}
