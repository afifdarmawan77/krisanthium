using KrisanthiumCurrency.Models;
using Microsoft.EntityFrameworkCore;

namespace KrisanthiumCurrency.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.ToTable("exchange_rates");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(e => e.Rate)
                    .HasColumnName("rate")
                    .HasColumnType("decimal(18,4)")
                    .IsRequired();

                entity.Property(e => e.RateDate)
                    .HasColumnName("rate_date")
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Source)
                    .HasColumnName("source")
                    .HasMaxLength(100);

                entity.Property(e => e.ErpStatus)
                    .HasColumnName("erp_status")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.SyncedAt).HasColumnName("synced_at");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                // Requirement 2: satu currency hanya boleh punya satu kurs per tanggal
                entity.HasIndex(e => new { e.Currency, e.RateDate }).IsUnique();
            });
        }
    }
}
