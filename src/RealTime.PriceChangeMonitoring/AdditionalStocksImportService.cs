using Microsoft.EntityFrameworkCore;
using RealTime.DAL;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RealTime.PriceChangeMonitoring
{
    public class AdditionalStocksImportService
    {
        private readonly PricesDbContext db;

        public AdditionalStocksImportService(PricesDbContext db)
        {
            this.db = db;
        }

        public async Task ImportAdditionalStocks()
        {
            var symbols = File.ReadAllLines("AdditionalSymbols.txt")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
            foreach (var symbol in symbols)
            {
                var alreadyExists = await db.Funds.AnyAsync(x => x.Symbol == symbol);
                if (alreadyExists)
                {
                    continue;
                }

                db.Funds.Add(
                    new DAL.Entities.Fund
                    {
                        Symbol = symbol,
                        Name = symbol
                    });
            }

            await db.SaveChangesAsync();
        }
    }
}
