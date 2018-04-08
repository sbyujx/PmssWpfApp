namespace PMSS.SqlDataAccess.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pmssdata.hydrologicalstation")]
    public partial class hydrologicalstation
    {
        [Key]
        [StringLength(45)]
        public string UID { get; set; }

        [Required]
        [StringLength(45)]
        public string Name { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

        [StringLength(45)]
        public string River { get; set; }

        [StringLength(45)]
        public string HydrographicNet { get; set; }

        [StringLength(45)]
        public string Basin { get; set; }

        [StringLength(45)]
        public string AdministrativeRegion { get; set; }

        [StringLength(45)]
        public string Address { get; set; }

        [StringLength(45)]
        public string Type { get; set; }
    }
}
