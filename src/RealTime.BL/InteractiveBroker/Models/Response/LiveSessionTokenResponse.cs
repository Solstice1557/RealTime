
namespace RealTime.BL.InteractiveBroker.Models.Response
{
    public class LiveSessionTokenResponse
    {
        public string DiffieHellmanResponse { get; set; }
        public string LiveSessionTokenSignature { get; set; }
    }
}
