namespace RealTime.BL.Tdameritrade
{
    public class TdAmeritradeConfig
    {
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
        public string TdAmeritradeApiEndpoint { get; set; }
        public string TdAmeritradeAuthWebAddressPattern { get; set; }

        public string TdAmeritradeAuthWebAddress =>
            TdAmeritradeAuthWebAddressPattern
                    .Replace($"{{{nameof(ClientId)}}}", ClientId)
                    .Replace($"{{{nameof(RedirectUrl)}}}", RedirectUrl); 
    }
}
