using Alpaca.Markets;
using RealTime.BL.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace RealTime.BL.Prices
{
    public class CalendarService
    {
        private readonly TimeZoneInfo marketTimeZone;
        private readonly Lazy<CalendarModel[]> items;

        public CalendarService()
        {
            this.marketTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Eastern Standard Time"
                : "America/New_York");
            items =  new Lazy<CalendarModel[]>(() => GetItems());
        }

        public IClock GetClock()
        {
            var utcNow = DateTime.UtcNow;
            var marketDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, this.marketTimeZone);
            var today = items.Value.FirstOrDefault(x => x.Date == marketDateTime.Date);
            var nextDay = items.Value.Where(x => x.Date > marketDateTime.Date).OrderBy(x => x.Date).First();
            var result = new Clock
            {
                TimestampUtc = utcNow
            };

            if (today != null)
            {
                result.IsOpen = utcNow < today.CloseUtc && utcNow > today.OpenUtc;
                result.NextOpenUtc = utcNow < today.OpenUtc ? today.OpenUtc : nextDay.OpenUtc;
                result.NextCloseUtc = utcNow < today.CloseUtc ? today.CloseUtc : nextDay.CloseUtc;
            }
            else
            {
                result.IsOpen = false;
                result.NextOpenUtc = nextDay.OpenUtc;
                result.NextCloseUtc = nextDay.CloseUtc;
            }

            return result;
        }

        private CalendarModel[] GetItems()
        {
            var calendarJson = GetCalendarJson();
            var calendarItems = calendarJson.FromJson<CalendarItem[]>();
            return calendarItems.Select(ConvertToModel).ToArray();
        }

        private static string GetCalendarJson()
        {
            var assembly = typeof(CalendarService).Assembly;
            var all = assembly.GetManifestResourceNames();


            using (var stream = assembly.GetManifestResourceStream("RealTime.BL.Prices.Data.calendar.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private CalendarModel ConvertToModel(CalendarItem item)
        {
            var date = DateTime.ParseExact(
                item.Date,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal);

            return new CalendarModel
            {
                Date = date,
                OpenUtc = GetUtcDateTime(date, item.Open),
                CloseUtc = GetUtcDateTime(date, item.Close)
            };
        }

        private DateTime GetUtcDateTime(DateTime date, string timespanStr)
        {
            var timeSpan = TimeSpan.ParseExact(timespanStr, @"hh\:mm", CultureInfo.InvariantCulture);
            return TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(date + timeSpan, DateTimeKind.Unspecified),
                this.marketTimeZone);
        }

        public class CalendarModel
        {
            public DateTime Date { get; set; }

            public DateTime OpenUtc { get; set; }

            public DateTime CloseUtc { get; set; }
        }

        public class Clock : IClock
        {
            public DateTime TimestampUtc { get; set; }

            public bool IsOpen { get; set; }

            public DateTime NextOpenUtc { get; set; }

            public DateTime NextCloseUtc { get; set; }
        }

        public class CalendarItem
        {
            [JsonPropertyName("date")]
            public string Date { get; set; }

            [JsonPropertyName("open")]
            public string Open { get; set; }

            [JsonPropertyName("close")]
            public string Close { get; set; }

            [JsonPropertyName("session_open")]
            public string SessionOpen { get; set; }

            [JsonPropertyName("session_close")]
            public string SessionClose { get; set; }
        }
    }
}
