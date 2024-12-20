using System.Collections.Generic;
using System.Net.Http;

namespace RealTime.BL.ETrade.Models
{
    internal class ETradeRequest
    {
        public string TokenSecret { get; set; }
        public string RequestPath { get; set; }
        public SortedDictionary<string, string> Parameters { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> QueryParameters { get; set; }
    }
}
