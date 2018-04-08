namespace PMSS.SqlDataAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pmssdata.hydrologicaldata")]
    public partial class hydrologicaldata
    {
        [Key]
        public int RecordId { get; set; }

        [Required]
        [StringLength(45)]
        public string StationId { get; set; }

        public double? L { get; set; }

        public double? Q { get; set; }

        public double? WL1 { get; set; }

        public double? WL2 { get; set; }

        public DateTime? Time { get; set; }

        public bool? IsSign { get; set; }
    }
}
