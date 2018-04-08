using System;
using System.Text;
using System.Collections.Generic;


namespace PMSS.SqlDataOutput {
    
    public class Geologicaldisaster {
        public virtual int Recordid { get; set; }
        public virtual DateTime? Time { get; set; }
        public virtual string Area { get; set; }
        public virtual string Process { get; set; }
        public virtual string Disastersituation { get; set; }
        public virtual int? Inwarningarea { get; set; }
        public virtual string Comment { get; set; }
        public virtual string Type { get; set; }
    }
}
