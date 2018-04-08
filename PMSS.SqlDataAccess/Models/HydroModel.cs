namespace PMSS.SqlDataAccess.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class HydroModel : DbContext
    {
        public HydroModel()
            : base("name=HydroModel")
        {
        }

        public virtual DbSet<geologicaldisaster> geologicaldisaster { get; set; }
        public virtual DbSet<hydrologicaldata> hydrologicaldata { get; set; }
        public virtual DbSet<hydrologicalstation> hydrologicalstation { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<geologicaldisaster>()
                .Property(e => e.Area)
                .IsUnicode(false);

            modelBuilder.Entity<geologicaldisaster>()
                .Property(e => e.Process)
                .IsUnicode(false);

            modelBuilder.Entity<geologicaldisaster>()
                .Property(e => e.DisasterSituation)
                .IsUnicode(false);

            modelBuilder.Entity<geologicaldisaster>()
                .Property(e => e.Comment)
                .IsUnicode(false);

            modelBuilder.Entity<geologicaldisaster>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicaldata>()
                .Property(e => e.StationId)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.UID)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.River)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.HydrographicNet)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.Basin)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.AdministrativeRegion)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.Address)
                .IsUnicode(false);

            modelBuilder.Entity<hydrologicalstation>()
                .Property(e => e.Type)
                .IsUnicode(false);
        }
    }
}
