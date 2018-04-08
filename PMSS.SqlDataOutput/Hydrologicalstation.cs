using System;
using System.Text;
using System.Collections.Generic;


namespace PMSS.SqlDataOutput {
    
    public class Hydrologicalstation {
        public virtual string Uid { get; set; }
        public virtual string Name { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Latitude { get; set; }
        public virtual string River { get; set; }
        public virtual string Hydrographicnet { get; set; }
        public virtual string Basin { get; set; }
        public virtual string Administrativeregion { get; set; }
        public virtual string Address { get; set; }
        public virtual string Type { get; set; }
    }
}
