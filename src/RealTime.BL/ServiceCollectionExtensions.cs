namespace RealTime.BL
{
    using AutoMapper;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using RealTime.BL.Alpaca;
    using RealTime.BL.Alpaca.Models;
    using RealTime.BL.Alphavantage;
    using RealTime.BL.Encryption;
    using RealTime.BL.ETrade;
    using RealTime.BL.ETrade.Models;
    using RealTime.BL.InteractiveBroker;
    using RealTime.BL.Polygon;
    using RealTime.BL.Prices;
    using RealTime.BL.Sync;
    using RealTime.BL.Tdameritrade;
    using RealTime.BL.Tdameritrade.Mappers;
    using RealTime.BL.Tdameritrade.Utils;
    using RealTime.BL.Trading;
    using System;

    public static class ServiceCollectionExtensions
    {
        public static void RegisterBusinessLogicServices(this IServiceCollection services)
        {
            services.AddSingleton<CalendarService>();
            services.AddSingleton<IAlphavantageService, AlphavantageService>();
            services.AddSingleton<ITechAnalysisService, TechAnalysisService>();
            services.AddScoped<IPricesService, DBPricesService>();
            services.AddSingleton<IPricesSyncronizer, AlpacaPricesSyncronizer>();
            services.AddSingleton<ICustomPolygonStreamingClient, CustomPolygonStreamingClient>();
            services.AddTransient<IWebSocket, WebSocket>();
            services.AddTransient<IKeyVaultService, LocalKeyVaultService>();
            services.AddTransient<ExchangeSymbolService>();
        }

        public static void AddTdAmeritradeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TdAmeritradeConfig>(configuration.GetSection(nameof(TdAmeritradeConfig)));
            services.AddAutoMapper(typeof(TdAmeritradeMappingProfile));

            services.AddHttpClient(
                Constant.TdAmeritradeHttpClientName,
                client =>
                {
                    client.BaseAddress = new Uri(
                        configuration
                        .GetSection(nameof(TdAmeritradeConfig))
                        .Get<TdAmeritradeConfig>()
                        .TdAmeritradeApiEndpoint);

                });

            services.AddScoped<ITdAmeritradeHttpClientService, TdAmeritradeHttpClientService>();
            services.AddScoped<ITdAmeritradeTokenService, TdAmeritradeTokenService>();
            services.AddTransient<TdAmeritradeAuthService>();
            services.AddTransient<TdAmeritradePortfolioService>();
            services.AddSingleton<TdAmeritradeFacade>();
        }

        public static void AddETradeServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ETradeConfig>(configuration.GetSection(nameof(ETradeConfig)));

            services.AddHttpClient(
                "etrade",
                (sp, client) =>
                {
                    var config = sp.GetService<IOptions<ETradeConfig>>();
                    client.BaseAddress = config.Value.Endpoint;
                });

            services.AddTransient<ETradeAuthService>();
            services.AddTransient<ETradeOrderService>();
            services.AddTransient<ETradePortfolioService>();
            services.AddTransient<ETradeFacade>();
        }

        public static IServiceCollection AddInteractiveBrokersServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<InteractiveBrokersConfig>(configuration.GetSection(nameof(InteractiveBrokersConfig)));

            services.AddHttpClient<InteractiveBrokersAuthService>(
                (sp, client) =>
                {
                    var config = sp.GetService<IOptions<InteractiveBrokersConfig>>();
                    client.BaseAddress = config.Value.Endpoint;
                });

            services.AddHttpClient<InteractiveBrokersOrderService>(
                (sp, client) =>
                {
                    var config = sp.GetService<IOptions<InteractiveBrokersConfig>>();
                    client.BaseAddress = config.Value.Endpoint;
                });

            services.AddHttpClient<InteractiveBrokersPortfolioService>(
                (sp, client) =>
                {
                    var config = sp.GetService<IOptions<InteractiveBrokersConfig>>();
                    client.BaseAddress = config.Value.Endpoint;
                });
            services.AddTransient<InteractiveBrokersFacade>();
            return services;
        }

        public static IServiceCollection AddAlpacaServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AlpacaConfig>(configuration.GetSection(nameof(AlpacaConfig)));
            services.AddTransient<IAlpacaPortfolioService, AlpacaPortfolioService>();
            services.AddTransient<IAlpacaOrderService, AlpacaOrderService>();
            services.AddHttpClient<IAlpacaAuthService, AlpacaAuthService>();
            services.AddTransient<AlpacaBrokersFacade>();
            return services;
        }
    }
}
