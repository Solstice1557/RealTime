using RealTime.BL.Alpaca;
using RealTime.BL.Brokers;
using RealTime.BL.ETrade;
using RealTime.BL.InteractiveBroker;
using RealTime.BL.Tdameritrade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTime.BL.Trading
{
    public class BrokersService
    {
        private readonly ETradeFacade etradeFacade;
        private readonly TdAmeritradeFacade ameritradeFacade;
        private readonly InteractiveBrokersFacade interactiveBrokerFacade;
        private readonly AlpacaBrokersFacade alpacaBrokersFacade;

        public BrokersService(
            ETradeFacade etradeFacade,
            TdAmeritradeFacade ameritradeFacade,
            InteractiveBrokersFacade interactiveBrokerFacade,
            AlpacaBrokersFacade alpacaBrokersFacade)
        {
            this.etradeFacade = etradeFacade;
            this.ameritradeFacade = ameritradeFacade;
            this.interactiveBrokerFacade = interactiveBrokerFacade;
            this.alpacaBrokersFacade = alpacaBrokersFacade;
        }

        public BrokerType BrokerType { get; set; } = BrokerType.InteractiveBrokers;

        public Task<bool> Authenticate()
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.Authenticate();
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.Authenticate();
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.Authenticate();
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.Authenticate();
            }

            return Task.FromResult(false);
        }

        public Task<BrokerBalance> GetBalance()
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.GetBalance();
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.GetBalance();
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.GetBalance();
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.GetBalance();
            }

            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<BrokerPosition>> GetPositions()
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.GetPositions();
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.GetPositions();
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.GetPositions();
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.GetPositions();
            }

            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<BrokerOrder>> GetOrders()
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.GetOrders();
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.GetOrders();
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.GetOrders();
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.GetOrders();
            }

            throw new NotImplementedException();
        }

        public Task CancelOrder(string id)
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.CancelOrder(id);
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.CancelOrder(id);
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.CancelOrder(id);
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.CancelOrder(id);
            }

            throw new NotImplementedException();
        }

        public Task<string> Buy(string symbol, decimal amount)
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.Buy(symbol, amount);
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.Buy(symbol, amount);
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.Buy(symbol, amount);
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.Buy(symbol, amount);
            }

            throw new NotImplementedException();
        }

        public Task<string> Sell(string symbol, decimal amount)
        {
            switch (BrokerType)
            {
                case BrokerType.Etrade:
                    return etradeFacade.Sell(symbol, amount);
                case BrokerType.TdAmeritrade:
                    return ameritradeFacade.Sell(symbol, amount);
                case BrokerType.InteractiveBrokers:
                    return interactiveBrokerFacade.Sell(symbol, amount);
                case BrokerType.Alpaca:
                    return alpacaBrokersFacade.Sell(symbol, amount);
            }

            throw new NotImplementedException();
        }

        public Task<BrokerOrder> BuyAndWaitForResult(string symbol, decimal amount, int waitMilliseconds = 2000)
            => ExecuteOrderAndWaitForResult(symbol, amount, BrokerOrderType.Buy, waitMilliseconds);

        public Task<BrokerOrder> SellAndWaitForResult(string symbol, decimal amount, int waitMilliseconds = 2000)
            => ExecuteOrderAndWaitForResult(symbol, amount, BrokerOrderType.Sell, waitMilliseconds);

        private async Task<BrokerOrder> ExecuteOrderAndWaitForResult(
            string symbol,
            decimal amount,
            BrokerOrderType orderType,
            int waitMilliseconds)
        {
            if (orderType == BrokerOrderType.Sell)
            {
                var positions = await GetPositions();
                var stockAmount = positions.FirstOrDefault(x => x.Symbol == symbol)?.Amount ?? 0;
                if (stockAmount < amount)
                {
                    return new BrokerOrder
                    {
                        Status = BrokerOrderStatus.Failed,
                        FailedToSellDueToInsufficientAmountOfStocks = true
                    };
                }
            }

            if (orderType == BrokerOrderType.Buy)
            {
                await Buy(symbol, amount);
            }
            else if (orderType == BrokerOrderType.Sell)
            {
                await Sell(symbol, amount);
            }
            else
            {
                throw new NotImplementedException();
            }

            await Task.Delay(waitMilliseconds);
            var orders = await GetOrders();
            var order = orders.Where(x => x.Symbol == symbol && x.Type == orderType)
                .OrderByDescending(x => x.ExecutionTime)
                .FirstOrDefault();
            return order;
        }

    }
}
