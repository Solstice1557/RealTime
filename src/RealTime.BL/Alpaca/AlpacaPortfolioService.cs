using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Alpaca.Models;
using RealTime.BL.Brokers;

namespace RealTime.BL.Alpaca
{
    public class AlpacaPortfolioService : BaseAlpacaMarketService, IAlpacaPortfolioService
    {
        public AlpacaPortfolioService(IOptions<AlpacaConfig> config, ILogger<AlpacaAuthService> logger) 
            : base(config, logger) { }

        public async Task<IReadOnlyCollection<BrokerPosition>> GetPositions(string authToken)
        {
            var token = ParseToken(authToken).Token;
            return await ExecuteAndHandleExceptions(async () =>
            {
                using var client = CreateAlpacaClient(token);

                var positions = await client.ListPositionsAsync();

                return positions.Select(x => new BrokerPosition
                {
                    CostBasis = x.CostBasis,
                    Symbol = x.Symbol,
                    Amount = x.Quantity
                }).ToArray();
            }, nameof(GetPositions));
        }

        public async Task<BrokerBalance> GetBalance(string accessToken)
        {
            var token = ParseToken(accessToken).Token;

            var account = await GetAccount(token);

            return new BrokerBalance
            {
                BuyingPower = account.BuyingPower,
                Cash = account.TradableCash,
                Equity = account.Equity - account.TradableCash
            };
        }
    }
}
