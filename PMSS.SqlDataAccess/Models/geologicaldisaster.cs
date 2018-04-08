namespace PMSS.SqlDataAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pmssdata.geologicaldisaster")]
    public partial class geologicaldisaster
    {
        [Key]
        public int RecordId { get; set; }

        public DateTime? Time { get; set; }

        [StringLength(60)]
        public string Area { get; set; }

        [StringLength(500)]
        public string Process { get; set; }

        [StringLength(1000)]
        public string DisasterSituation { get; set; }

        public bool? InWarningArea { get; set; }

        [StringLength(100)]
        public string Comment { get; set; }

        [StringLength(45)]
        public string Type { get; set; }
    }
}
