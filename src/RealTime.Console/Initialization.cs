namespace RealTime.Console
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RealTime.BL;
    using RealTime.BL.Common;
    using RealTime.BL.Trading;
    using RealTime.DAL;
    using Serilog;
    using System;
    using System.IO;

    public static class Initialization
    {
        public static ServiceProvider InitServices()
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
                .AddUserSecrets(typeof(Initialization).Assembly);
            IConfigurationRoot configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(
                configure =>
                {
                    var log = new LoggerConfiguration()
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
                    configure.AddSerilog(log);
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
            serviceCollection.AddETradeServices(configuration);
            serviceCollection.AddTdAmeritradeServices(configuration);
            serviceCollection.AddInteractiveBrokersServices(configuration);
            serviceCollection.AddAlpacaServices(configuration);
            serviceCollection.AddTransient<AdditionalStocksImportService>();
            serviceCollection.AddTransient<BrokersService>();

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
