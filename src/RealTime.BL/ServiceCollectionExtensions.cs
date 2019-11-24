namespace RealTime.BL
{
    using Microsoft.Extensions.DependencyInjection;
    using RealTime.BL.Alphavantage;
    using RealTime.BL.Prices;
    using RealTime.BL.Sync;

    public static class ServiceCollectionExtensions
    {
        public static void RegisterBusinessLogicServices(this IServiceCollection services)
        {
            services.AddSingleton<IAlphavantageService, AlphavantageService>();
            services.AddSingleton<ITechAnalysisService, TechAnalysisService>();
            services.AddScoped<IPricesService, DBPricesService>();
            services.AddSingleton<IPricesSyncronizer, PricesSyncronizer>();
        }
    }
}
