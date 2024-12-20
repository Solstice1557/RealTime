using System.Text.Json.Serialization;

namespace RealTime.BL.Tdameritrade.Models
{
    /// <summary>
    /// Response of access token request
    /// doc found in https://developer.tdameritrade.com/authentication/apis/post/token-0
    /// </summary>
    public class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("refresh_token_expires_in")]
        public int? RefreshTokenExpiresIn { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}
