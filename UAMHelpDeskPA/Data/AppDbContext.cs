using Microsoft.EntityFrameworkCore;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Laboratory> Laboratories => Set<Laboratory>();

        public DbSet<Equipment> Equipment => Set<Equipment>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Laboratory>(entity =>
            {
                entity.ToTable("Laboratories");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.HasIndex(x => x.Name).IsUnique();
                entity.Property(x => x.Building).HasMaxLength(50).IsRequired();
                entity.Property(x => x.BuildingFloor).IsRequired(); ;
                entity.Property(x => x.Capacity).IsRequired();
                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.CreatedAtUtc).IsRequired();
                entity.Property(x => x.UpdatedAtUtc).IsRequired();
            });
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipment");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
                entity.HasIndex(x => x.Code).IsUnique();
                entity.Property(x => x.Brand).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Model).HasMaxLength(50).IsRequired();
                entity.Property(x => x.SerialNumber).HasMaxLength(50).IsRequired();
                entity.HasIndex(x => x.SerialNumber).IsUnique();
                entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(x => x.PurchaseDate).HasColumnType("date").IsRequired(false);
                entity.Property(x => x.IsActive).HasDefaultValue(true);
                entity.Property(x => x.CreatedAtUtc).IsRequired();
                entity.Property(x => x.UpdatedAtUtc).IsRequired();
                entity.HasOne(x => x.Laboratory).WithMany(x => x.Equipments).HasForeignKey(x => x.LaboratoryId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
