namespace RealTime.Console
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using RealTime.BL;
    using RealTime.BL.Common;
    using RealTime.DAL;
    using System;
    using System.IO;

    public static class Initialization
    {
        public static ServiceProvider InitServices()
        {
            var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("realtimesettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"realtimesettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(
                configure =>
                {
                    configure.AddConsole();
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
