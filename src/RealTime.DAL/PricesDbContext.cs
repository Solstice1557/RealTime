namespace RealTime.DAL
{
    using Microsoft.EntityFrameworkCore;
    using RealTime.DAL.Entities;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class PricesDbContext : DbContext
    {
        public PricesDbContext(DbContextOptions<PricesDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Fund> Funds { get; set; }

        public virtual DbSet<Price> Prices { get; set; }

        public virtual DbSet<DailyPrice> DailyPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fund>()
                .HasIndex(f => f.Symbol)
                .IsUnique()
                .HasName("IX_Funds_Symbol");

            modelBuilder.Entity<Price>()
                .HasIndex(f => new { f.FundId, f.Timestamp })
                .IsUnique()
                .HasName("IX_Prices_FundIdTimestamp");

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Fund)
                .WithMany()
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DailyPrice>()
                .HasIndex(f => new { f.FundId, f.Timestamp })
                .IsUnique()
                .HasName("IX_DailyPrices_FundIdTimestamp");

            modelBuilder.Entity<DailyPrice>()
                .HasOne(p => p.Fund)
                .WithMany()
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fund>().HasData(GetFunds());
        }

        private static Fund[] GetFunds()
        {
            var assemblyDirectory = Assembly.GetAssembly(typeof(PricesDbContext)).CodeBase;
            if (assemblyDirectory.StartsWith(@"file:"))
            {
                assemblyDirectory = assemblyDirectory.Substring(8);
            }

            var solutionDirectory =
                 Path.GetDirectoryName(
                 Path.GetDirectoryName(
                Path.GetDirectoryName((Path.GetDirectoryName(Path.GetDirectoryName(assemblyDirectory))))));
            var filePath = Path.Combine(
                solutionDirectory,
                "Data",
                "funds.csv");

            if (!File.Exists(filePath))
            {
                return new Fund[0];
            }

            var lines = File.ReadAllLines(filePath);
            var funds = lines.Select(x => x.Split(";"))
                .Where(x => x.Length == 3)
                .Select(
                    x => new
                    {
                        Symbol = x[0].Trim(' ', '"'),
                        Name = x[1].Trim(' ', '"'),
                        Volume = int.Parse(x[2])
                    })
                .OrderByDescending(x => x.Volume)
                .Select(
                    (x, i) => new Fund
                    {
                        FundId = i + 1,
                        Symbol = x.Symbol,
                        Name = x.Name,
                        Volume = x.Volume
                    })
                .ToArray();
            return funds;
        }
    }
}
