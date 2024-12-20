using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL;
using RealTime.BL.Common;
using RealTime.BL.Sync;
using RealTime.BL.Tdameritrade;
using RealTime.DAL;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RealTime.PriceChangeMonitoring
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();
            await using var serviceProvider = GetServiceProvder(configuration);

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            AppDomain.CurrentDomain.UnhandledException += (s, e)
                => logger.LogCritical("Unhandled exception", (Exception)e.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (s, e)
                => logger.LogError("Unhandled task exception", e.Exception);

            var monitorService = serviceProvider.GetRequiredService<PriceMonotoringService>();
            var syncronizer = serviceProvider.GetRequiredService<IPricesSyncronizer>();
            var tdAmeritradeFacade = serviceProvider.GetRequiredService<TdAmeritradeFacade>();
            var additionalStocksService = serviceProvider.GetRequiredService<AdditionalStocksImportService>();

            await additionalStocksService.ImportAdditionalStocks();
            var authResult = await tdAmeritradeFacade.Authenticate();
            if (!authResult)
            {
                logger.LogError("Failed to authenticate at TdAmeritrade");
                return;
            }

            var cancellationTokenSource = new CancellationTokenSource();
            var monitorTask = Task.Run(
                async () =>
                {
                    try
                    {
                        await monitorService.Monitor(cancellationTokenSource.Token);
                        logger.LogInformation("Monitor task finished");
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(e, "Monitor task error");
                    }
                });

            var syncTask = Task.Run(
                async () =>
                {
                    try
                    {
                        await syncronizer.SyncIntradayPrices(cancellationTokenSource.Token);
                        logger.LogInformation("Sync task finished");
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(e, "Monitor task error");
                    }
                });

            Console.WriteLine("Press any key to finish");
            Console.ReadKey();
            cancellationTokenSource.Cancel();
            syncTask.Wait(TimeSpan.FromSeconds(20));
            monitorTask.Wait(TimeSpan.FromSeconds(20));
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            var basePath = Directory.GetCurrentDirectory();
            var projectPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(basePath)));

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("realtimesettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"realtimesettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(projectPath, "realtimesettings.json"), optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(projectPath, $"realtimesettings.{environmentName}.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly);
            return builder.Build();
        }

        private static ServiceProvider GetServiceProvder(IConfigurationRoot configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(
                configure =>
                {
                    Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Warning()
                         .WriteTo.File(
                            "errors.txt",
                            retainedFileCountLimit: 3,
                            fileSizeLimitBytes: 1024 * 1024,
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
                         .CreateLogger();

                    configure.ClearProviders();
                    configure.AddConfiguration(configuration.GetSection("Logging"));
                    configure.AddConsole();
                    configure.AddSerilog(Log.Logger);
                    configure.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                    configure.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
                }
            );

            serviceCollection.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            var sqlFilePath = configuration.GetValue<string>("DBDirPath");

            serviceCollection.AddSingleton(x => x.GetService<IOptions<AppSettings>>().Value);
            serviceCollection.AddDbContext<PricesDbContext>(
                (options) =>
                {
                    options.UseSqlite($"Data Source={sqlFilePath};");
                });
            serviceCollection.RegisterBusinessLogicServices();
            serviceCollection.AddScoped<PriceMonotoringService>();
            serviceCollection.AddScoped<TradingService>();
            serviceCollection.AddScoped<AdditionalStocksImportService>();
            serviceCollection.AddTdAmeritradeServices(configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<PricesDbContext>();
                dbContext.Database.Migrate();
            }

            return serviceProvider;
        }
    }
}
