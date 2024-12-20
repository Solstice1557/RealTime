using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealTime.BL.Sync;
using RealTime.DAL;
using RealTime.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.PredictionsCheck
{
    public class Program
    {
        private static ILogger _logger;

        private static readonly Dictionary<string, int> SymbolToFundIdMapping = new Dictionary<string, int>();

        private static readonly HashSet<string> NotFoundSymbols = new HashSet<string>();

        public static async Task Main()
        {
            var serviceProvider = Initialization.InitServices();
            _logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => _logger.LogCritical("Unhandled exception", (Exception)e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (s, e) => _logger.LogError("Unhandled task exception", e.Exception);
            await Sync(serviceProvider);

            var predictions = LoadPredictions();
            var correctCount = 0;
            var incorrectCount = 0;
            var currentProgress = -1; 
            var db = serviceProvider.GetRequiredService<PricesDbContext>();
            for (int i = 0; i < predictions.Count; i++)
            {
                var res = await CheckPrediction(predictions[i], db);
                if (res.HasValue)
                {
                    if (res.Value)
                    {
                        correctCount++;
                    }
                    else
                    {
                        incorrectCount++;
                    }
                }

                var progress = (int)Math.Floor(i * 100m / predictions.Count);
                if (progress != currentProgress)
                {
                    currentProgress = progress;
                    _logger.LogInformation($"Progress {currentProgress}%");
                }
            }

            var total = correctCount + incorrectCount;
            var correctPercent = correctCount * 100m / total;
            _logger.LogInformation($"Correct: {correctCount} ({correctPercent:F02}), Incorrect: {incorrectCount}");
        }

        private static async Task<bool?> CheckPrediction(PredictionModel prediction, PricesDbContext db)
        {
            if (NotFoundSymbols.Contains(prediction.Symbol))
            {
                return null;
            }

            if (!SymbolToFundIdMapping.ContainsKey(prediction.Symbol))
            {
                var fund = await db.Funds.SingleOrDefaultAsync(x => x.Symbol == prediction.Symbol);
                if (fund == null)
                {
                    NotFoundSymbols.Add(prediction.Symbol);
                    return null;
                }

                SymbolToFundIdMapping[prediction.Symbol] = fund.FundId;
            }

            var fundId = SymbolToFundIdMapping[prediction.Symbol];
            var startPrice = await GetDailyPrice(db, fundId, prediction.Timestamp);
            if (startPrice == null)
            {
                return null;
            }

            var endPrice = await GetDailyPrice(db, fundId, prediction.Timestamp.AddMonths(1));
            if (endPrice == null)
            {
                return null;
            }

            var pricesGoesUp = endPrice.Close > startPrice.Close;
            return prediction.Score > 0 ? pricesGoesUp : !pricesGoesUp;
        }

        private static async Task<DailyPrice> GetDailyPrice(PricesDbContext db, int fundId, DateTime timestamp)
        {
            var price = await db.DailyPrices
                .Where(p => p.FundId == fundId && p.Timestamp <= timestamp)
                .OrderByDescending(p => p.Timestamp)
                .FirstOrDefaultAsync();

            if (price == null || price.Timestamp < timestamp.AddDays(-7))
            {
                return null;
            }

            return price;
        }

        private static async Task Sync(IServiceProvider serviceProvider)
        {
            _logger.LogInformation("Start syncing daily data");
            var syncronizer = serviceProvider.GetService<IPricesSyncronizer>();
            await syncronizer.SyncDailyPrices(CancellationToken.None);
            _logger.LogInformation("Finish syncing daily data");
        }

        private static IReadOnlyList<PredictionModel> LoadPredictions()
        {
            _logger.LogInformation("Loading predictions");
            using var reader = new StreamReader("predictions.csv");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.PrepareHeaderForMatch = (string header, int _) => header.ToLower();
            var records = csv.GetRecords<PredictionModel>();
            var result = records.ToArray();
            _logger.LogInformation($"Loaded {result.Length} predictions");
            return result;
        }
    }
}
