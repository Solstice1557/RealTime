using System;
using System.Net;
using System.Threading.Tasks;
using Alpaca.Markets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealTime.BL.Alpaca.Models;
using RealTime.BL.Brokers;

namespace RealTime.BL.Alpaca
{
    public class BaseAlpacaMarketService
    {
        protected readonly AlpacaConfig AlpacaConfig;
        protected readonly ILogger Logger;

        public BaseAlpacaMarketService(IOptions<AlpacaConfig> config, ILogger logger)
        {
            AlpacaConfig = config.Value ?? throw new ArgumentNullException(nameof(config));
            Logger = logger;
        }

        protected async Task<T> ExecuteAndHandleExceptions<T>(Func<Task<T>> action, string operationName)
        {
            try
            {
                return await action();
            }

            catch (RestClientErrorException e)
            {
                var statusCode = HttpStatusCode.BadRequest;
                if (Enum.IsDefined(typeof(HttpStatusCode), e.ErrorCode))
                {
                    statusCode = (HttpStatusCode) e.ErrorCode;
                }

                throw BrokerErrorResponseHelper.CreateBrokerException(
                    statusCode,
                    e.Message,
                    e.Message,
                    "Alpaca",
                    Logger);
            }

            catch (RequestValidationException e)
            {
                throw BrokerErrorResponseHelper.CreateBrokerException(
                    HttpStatusCode.BadRequest,
                    e.Message,
                    e.Message,
                    "Alpaca",
                    Logger);
            }

            catch (Exception e)
            {
                var genericMessage = string.Format("Could not {0} using {1}.", operationName, "Alpaca");
                var detailedMessage = $"{genericMessage} Error message: {e.Message}";
                Logger.LogError(genericMessage);
                throw new Exception(detailedMessage);
            }
        }

        protected IAlpacaTradingClient CreateAlpacaClient(string token)
        {
            return Environments.Live.GetAlpacaTradingClient(new OAuthKey(token));
        }

        protected async Task<IAccount> GetAccount(string token)
        {
            return await ExecuteAndHandleExceptions(async () =>
            {
                using var client = CreateAlpacaClient(token);
                return await client.GetAccountAsync();
            }, nameof(GetAccount));
        }

        protected string CreateTokenWithAccountId(TokenModel model)
        {
            if (string.IsNullOrEmpty(model.AccountId) || string.IsNullOrEmpty(model.Token))
            {
                throw new Exception("Alpaca token data is corrupted");
            }

            return $"{model.Token}|{model.AccountId}";
        }

        protected TokenModel ParseToken(string accessToken)
        {
            if (!accessToken.Contains("|"))
            {
                return new TokenModel()
                {
                    Token = accessToken
                };
            }

            var tokenData = accessToken.Split("|");
            return new TokenModel()
            {
                Token = tokenData[0],
                AccountId = tokenData[1]
            };
        }
    }
}
