using Microsoft.EntityFrameworkCore;
using UamHelpDeskPA.Api.Models;

namespace UamHelpDeskPA.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Laboratory> Laboratories => Set<Laboratory>();

        public DbSet<Equipment> Equipment => Set<Equipment>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

        public DbSet<PendingSession> PendingSessions => Set<PendingSession>();
        public DbSet<PasswordResetRequest> PasswordResetRequests => Set<PasswordResetRequest>();
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
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.HasIndex(x => x.Name)
                    .IsUnique();
                entity.Property(x => x.Description)
                    .HasMaxLength(200)
                    .IsRequired(false);
                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);
                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();
                entity.Property(x => x.UpdatedAtUtc)
                    .IsRequired();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(x => x.LastName)
                    .HasMaxLength(100)
                    .IsRequired();
                entity.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired();
                entity.HasIndex(x => x.Email)
                    .IsUnique();
                entity.Property(x => x.PasswordHash)
                    .HasMaxLength(500)
                    .IsRequired();
                entity.Property(x => x.IsActive)
                    .HasDefaultValue(true);
                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();
                entity.Property(x => x.UpdatedAtUtc)
                    .IsRequired();
                entity.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Token)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.HasIndex(x => x.Token)
                    .IsUnique();

                entity.Property(x => x.ExpiresAtUtc)
                    .IsRequired();

                entity.Property(x => x.IsRevoked)
                    .HasDefaultValue(false);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(x => x.RevokedAtUtc)
                    .IsRequired(false);

                entity.Property(x => x.RevokedReason)
                    .HasMaxLength(200)
                    .IsRequired(false);
            });
            modelBuilder.Entity<OtpCode>(entity =>
            {
                entity.ToTable("OtpCodes");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Code)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.ExpiresAtUtc)
                    .IsRequired();

                entity.Property(x => x.IsUsed)
                    .HasDefaultValue(false);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.OtpCodes)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PendingSession>(entity =>
            {
                entity.ToTable("PendingSessions");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.SessionToken)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.SessionToken)
                    .IsUnique();

                entity.Property(x => x.ExpiresAtUtc)
                    .IsRequired();

                entity.Property(x => x.IsUsed)
                    .HasDefaultValue(false);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.PendingSessions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<PasswordResetRequest>(entity =>
            {
                entity.ToTable("PasswordResetRequests");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.SessionToken)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.SessionToken)
                    .IsUnique();

                entity.Property(x => x.Code)
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.ExpiresAtUtc)
                    .IsRequired();

                entity.Property(x => x.IsUsed)
                    .HasDefaultValue(false);

                entity.Property(x => x.CreatedAtUtc)
                    .IsRequired();

                entity.Property(x => x.UsedAtUtc)
                    .IsRequired(false);

                entity.HasOne(x => x.User)
                    .WithMany(x => x.PasswordResetRequests)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
