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
                .WithMany(f => f.DailyPrices)
                .HasForeignKey(p => p.FundId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fund>().HasData(GetFunds());
        }

        private static Fund[] GetFunds()
        {
            var baseFunds = GetFundsFromFile("funds.csv");
            var funds = baseFunds.ToArray();
            for (var i = 0; i < funds.Length; i++)
            {
                funds[i].FundId = i + 1;
            }

            return funds;
        }

        private static Fund[] GetFundsFromFile(string fileName)
        {
            if (!TryGetSolutionFolder(out var solutionDirectory))
            {
                return new Fund[0];
            }

            var filePath = Path.Combine(
                solutionDirectory,
                "Data",
                fileName);

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
                .Select(
                    x => new Fund
                    {
                        Symbol = x.Symbol,
                        Name = x.Name,
                        Volume = x.Volume
                    })
                .ToArray();
            return funds;
        }

        private static bool TryGetSolutionFolder(out string solutionFolder)
        {
            var assemblyDirectory = Assembly.GetAssembly(typeof(PricesDbContext)).CodeBase;
            if (assemblyDirectory.StartsWith(@"file:"))
            {
                assemblyDirectory = assemblyDirectory.Substring(8);
            }

            solutionFolder = Path.GetDirectoryName(assemblyDirectory);
            if (string.IsNullOrEmpty(solutionFolder))
            {
                return false;
            }

            solutionFolder = Path.GetDirectoryName(solutionFolder);
            if (string.IsNullOrEmpty(solutionFolder))
            {
                return false;
            }

            solutionFolder = Path.GetDirectoryName(solutionFolder);
            if (string.IsNullOrEmpty(solutionFolder))
            {
                return false;
            }

            solutionFolder = Path.GetDirectoryName(solutionFolder);
            if (string.IsNullOrEmpty(solutionFolder))
            {
                return false;
            }

            solutionFolder = Path.GetDirectoryName(solutionFolder);
            if (string.IsNullOrEmpty(solutionFolder))
            {
                return false;
            }

            return true;
        }
    }
}
