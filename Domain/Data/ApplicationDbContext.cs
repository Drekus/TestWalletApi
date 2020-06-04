using Microsoft.EntityFrameworkCore;
using TestWalletApi.Models;

namespace TestWalletApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<CurrencyTab> CurrencyTabs { get; set; }
        public DbSet<IdempotentCache> IdempotentCaches { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdempotentCache>().HasKey(ic => new { ic.UserId, ic.IdempotentKey, ic.ParametersHash });
            
            modelBuilder.Entity<CurrencyTab>().HasKey(t => new { t.WalletId, t.Сurrency });
            modelBuilder.Entity<CurrencyTab>().HasOne(t => t.Wallet)
                .WithMany(w => w.CurrencyTabs)
                .HasForeignKey(w => w.WalletId);

            modelBuilder.Entity<CurrencyTab>().Property(t => t.ChangeVersion).IsConcurrencyToken();
        }
    }
}
