using RealTime.BL.Trading;
using System.Linq;
using System.Threading.Tasks;

namespace RealTime.Console
{
    using Console = System.Console;

    public static class BrokersServiceExample
    {
        public static async Task Run(BrokersService service)
        {
            service.BrokerType = BrokerType.Alpaca;
            var authResult = await service.Authenticate();
            if (!authResult)
            {
                Console.WriteLine("Failed to authenticate at broker");
            }

            var positions = await service.GetPositions();
            var positionsString =
                string.Join(", ", positions.Select(x => $"{x.Symbol} - {x.Amount} - {x.CostBasis}"));
            Console.WriteLine($"Positions: {positionsString}");

            var balance = await service.GetBalance();
            Console.WriteLine($"Balance: {balance.BuyingPower}");

            return;

            Console.WriteLine("Buying one F");
            var orderId = await service.Buy("F", 1);

            Console.WriteLine($"Cancelling order {orderId}");
            await service.CancelOrder(orderId);
        } 
    }
}
