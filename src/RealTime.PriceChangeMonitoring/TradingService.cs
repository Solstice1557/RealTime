using RealTime.BL.Tdameritrade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealTime.PriceChangeMonitoring
{
    public class TradingService
    {
        private readonly TdAmeritradeFacade tdameritradeFacade;

        public TradingService(TdAmeritradeFacade tdameritradeFacade)
        {
            this.tdameritradeFacade = tdameritradeFacade;
        }

        public async Task NewPrices(
            IReadOnlyList<Performance> topYesterdayPerformances,
            IReadOnlyList<Performance> topDailyPerformances,
            IReadOnlyList<Performance> top5MinsPerformances)
        {
            // topDailyPerformances[0].StartPrice - start of the day price
            // top5MinsPerformances[0].StartPrice - 5 mins ago price

            // topDailyPerformances[0].CurrentPrice - current price
            // top5MinsPerformances[0].CurrentPrice - current price

            // await tdameritradeFacade.Buy(topDailyPerformances[0].Symbol, 1);
            // await tdameritradeFacade.Buy(top5MinsPerformances[0].Symbol, 1);
        }
    }
}
