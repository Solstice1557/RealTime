using System;

namespace RealTime.BackTest
{
    public class AppConfig
    {
        public string DataPath { get; set; }

        public string[] Tickers { get; set; }

        public DateTime MinPriceDate { get; set; }
    }
}
