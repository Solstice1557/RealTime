using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RealTime.BL.Polygon
{
    public class PolygonConnectionStatus
    {
        [JsonProperty(PropertyName = "status", Required = Required.Always)]
        public ConnectionStatus Status { get; set; }

        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; } = string.Empty;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConnectionStatus
    {
        /// <summary>
        /// Client successfully connected.
        /// </summary>
        [EnumMember(Value = "connected")]
        Connected,

        /// <summary>
        /// Client successfully authorized.
        /// </summary>
        [EnumMember(Value = "auth_success")]
        AuthenticationSuccess,

        /// <summary>
        /// Client authentication required.
        /// </summary>
        [EnumMember(Value = "auth_required")]
        AuthenticationRequired,

        /// <summary>
        /// Client authentication failed.
        /// </summary>
        [EnumMember(Value = "auth_failed")]
        AuthenticationFailed,

        /// <summary>
        /// Requested operation successfully completed.
        /// </summary>
        [EnumMember(Value = "success")]
        Success,

        /// <summary>
        /// Requested operation failed.
        /// </summary>
        [EnumMember(Value = "failed")]
        Failed,

        [EnumMember(Value = "error")]
        Error
    }
}
