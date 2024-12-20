namespace RealTime.PredictionsCheck
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RealTime.BL;
    using RealTime.BL.Common;
    using RealTime.DAL;
    using Serilog;
    using Serilog.Events;
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
                .AddEnvironmentVariables();
            IConfigurationRoot configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(
                configure =>
                {
                    var log = new LoggerConfiguration()
                         .MinimumLevel.Debug()
                         .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                         .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Infrastructure", LogEventLevel.Warning)
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                         .MinimumLevel.Override("RealTime.BL.Sync", LogEventLevel.Warning)
                         .WriteTo.Console(
                            outputTemplate: "{Timestamp:HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}")
                         .CreateLogger();

                    configure.ClearProviders();
                    configure.AddSerilog(log);
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
