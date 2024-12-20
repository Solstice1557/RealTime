using System.Net.Http;
using System.Threading.Tasks;

namespace RealTime.BL.Tdameritrade.Utils
{
    public interface ITdAmeritradeHttpClientService
    {
        Task<T> Execute<T>(
            HttpRequestMessage httpRequest,
            string accessToken = null)
            where T : class;

        public Task<string> Execute(
            HttpRequestMessage httpRequest,
            string accessToken = null);
    }
}
