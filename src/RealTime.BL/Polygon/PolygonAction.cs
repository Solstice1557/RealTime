using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RealTime.BL.Polygon
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PolygonAction
    {
        [EnumMember(Value = "authenticate")]
        Authenticate,

        [EnumMember(Value = "listen")]
        Listen,

        [EnumMember(Value = "auth")]
        PolygonAuthenticate,

        [EnumMember(Value = "subscribe")]
        PolygonSubscribe,

        [EnumMember(Value = "unsubscribe")]
        PolygonUnsubscribe
    }
}
