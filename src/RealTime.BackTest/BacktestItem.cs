using RealTime.BL.Prices;
using System;
using System.Collections.Generic;

namespace RealTime.BackTest
{
    public class BacktestPriceItem
    {
        public BacktestPriceItem(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; private set; }

        public Dictionary<string, PriceModel> OneMinPrices { get; } = new Dictionary<string, PriceModel>();

        public Dictionary<string, PriceModel> FiveMinPrices { get; } = new Dictionary<string, PriceModel>();
    }
}
