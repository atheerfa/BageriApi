using Microsoft.EntityFrameworkCore;
using BageriApi.Models;

namespace BageriApi.Data
{
    public class BageriContext : DbContext
    {
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RawMaterial> RawMaterials { get; set; }
        public DbSet<SupplierRawMaterial> SupplierRawMaterials { get; set; }

        public BageriContext(DbContextOptions<BageriContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SupplierRawMaterial>()
                .HasKey(srm => new { srm.SupplierId, srm.RawMaterialId });

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Kvarnen AB", Address = "Mjölvägen 1", ContactPerson = "Anders Andersson", Phone = "010-111222", Email = "info@kvarnen.se" },
                new Supplier { Id = 2, Name = "Sockerbolaget", Address = "Sötgatan 5", ContactPerson = "Maria Nilsson", Phone = "010-333444", Email = "kontakt@sockerbolaget.se" }
            );

            modelBuilder.Entity<RawMaterial>().HasData(
                new RawMaterial { Id = 1, ArticleNumber = "V-100", Name = "Vetemjöl" },
                new RawMaterial { Id = 2, ArticleNumber = "S-200", Name = "Strösocker" }
            );

            modelBuilder.Entity<SupplierRawMaterial>().HasData(
                new SupplierRawMaterial { SupplierId = 1, RawMaterialId = 1, PricePerKg = 12.50m },
                new SupplierRawMaterial { SupplierId = 2, RawMaterialId = 1, PricePerKg = 14.00m },
                new SupplierRawMaterial { SupplierId = 2, RawMaterialId = 2, PricePerKg = 22.00m }
            );
        }
    }
}