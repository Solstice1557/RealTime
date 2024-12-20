using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RealTime.BL.Trading
{
    public class ExchangeSymbolService
    {
        private readonly Dictionary<string, string> _symbols;

        public ExchangeSymbolService()
        {
            using var stream = typeof(ExchangeSymbolService).Assembly
                .GetManifestResourceStream("RealTime.BL.StockExhchangeSymbols.csv");
            using var reader = new StreamReader(stream);
            var resource = reader.ReadToEnd();
            _symbols = resource.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(','))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1]);
        }

        public string GetExchangeSymbol(string stockSymbol)
            => _symbols.ContainsKey(stockSymbol) ? _symbols[stockSymbol] : "NYSE";
    }
}
