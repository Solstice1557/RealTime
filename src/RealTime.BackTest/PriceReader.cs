using RealTime.BL.Prices;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RealTime.BackTest
{
    public class PriceReader
    {
        public static List<PriceModel> Read(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var list = new List<PriceModel>(lines.Length);
            foreach (var line in lines)
            {
                var price = ParseString(line);
                if (price != null)
                {
                    list.Add(price);
                }
                else
                {
                    Console.WriteLine($"Failed to parse price from: {line}");
                }
            }

            return list;
        }

        private static PriceModel ParseString(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            var parts = text.Split(new[] { ',' });
            if (parts.Length < 6)
            {
                return null;
            }

            // timestamp,open,high,low,close,volume
            //2014-09-19 12:00:00,99.0,99.49,97.0,97.0,4268874

            try
            {
                return new PriceModel
                {
                    Date = DateTime.Parse(parts[0], CultureInfo.InvariantCulture),
                    Open = decimal.Parse(parts[1], CultureInfo.InvariantCulture),
                    High = decimal.Parse(parts[2], CultureInfo.InvariantCulture),
                    Low = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                    Close = decimal.Parse(parts[4], CultureInfo.InvariantCulture),
                    Volume = decimal.Parse(parts[5], CultureInfo.InvariantCulture),
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
