namespace RealTime.Console
{
    using Newtonsoft.Json;
    using RealTime.BL.Prices;
    using RealTime.BL.Trading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class HtmlConverter
    {
        public static void SavePricesHtml(
            List<Dictionary<PricesTimeInterval, PriceModel>> prices,
            TradingHistory tradingHistory,
            string symbol,
            PricesTimeInterval[] intervals,
            string filename)
        {
            var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            var result = File.ReadAllText(Path.Combine(baseFolder, "Template.html"));
            var bootstrapCss = File.ReadAllText(Path.Combine(baseFolder, "libs/bootstrap/bootstrap.css"));
            var jquery = File.ReadAllText(Path.Combine(baseFolder, "libs/jquery/jquery.js"));
            var bootstrapJs = File.ReadAllText(Path.Combine(baseFolder, "libs/bootstrap/bootstrap.bundle.js"));
            var momentJs = File.ReadAllText(Path.Combine(baseFolder, "libs/moment/moment.min.js"));
            var amchartsCore = File.ReadAllText(Path.Combine(baseFolder, "libs/amcharts4/core.js"));
            var amchartsCharts = File.ReadAllText(Path.Combine(baseFolder, "libs/amcharts4/charts.js"));
            var amchartsDataviz = File.ReadAllText(Path.Combine(baseFolder, "libs/amcharts4/dataviz.js"));
            var pricesJson = JsonConvert.SerializeObject(prices); 

            var lastPrice = prices.Last().First().Value.Close.Value;
            var amount = tradingHistory?.GetCurrentAmount() ?? 0;
            var currentCost = (amount * lastPrice).ToString("F02");
            var profit = tradingHistory?.GetCurrentProfit(lastPrice).ToString("F02") ?? "0";
            var historyJson = "{}";
            if (tradingHistory != null)
            {
                historyJson = JsonConvert.SerializeObject(tradingHistory.Items.ToDictionary(i => i.Date));
            }

            var intervalStrings = intervals.Select(i => i.ToString()).ToArray();

            result = result
                .Replace("{{BootstrapStyles}}", bootstrapCss)
                .Replace("{{PricesJson}}", pricesJson)
                .Replace("{{PricesIntervals}}", JsonConvert.SerializeObject(intervalStrings))
                .Replace("{{HistoryJson}}", historyJson)
                .Replace("{{symbol}}", symbol)
                .Replace("{{CurrentProfit}}", profit)
                .Replace("{{CurrentCost}}", currentCost)
                .Replace("{{CurrentAmount}}", amount.ToString())
                .Replace("{{JQueryScript}}", jquery)
                .Replace("{{BootstrapScript}}", bootstrapJs)
                .Replace("{{MomentScript}}", momentJs)
                .Replace("{{AmChartsCoreScript}}", amchartsCore)
                .Replace("{{AmChartsChartsScript}}", amchartsCharts)
                .Replace("{{AmChartsDatavizScript}}", amchartsDataviz);

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            File.WriteAllText(filename, result);
        }
    }
}
