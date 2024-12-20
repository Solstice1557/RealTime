using System;
using System.Linq;

namespace RealTime.BL.Tdameritrade.Utils
{
    class TdAmeritradeTokenService : ITdAmeritradeTokenService
    {
        private const char TokenDeliminator = '|';

        public string Concatenate(string accountId, string token) =>
            $"{accountId}{TokenDeliminator}{token}";

        public (string AccountId, string Token) Split(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var parts = token.Split(TokenDeliminator);

                if (parts.Length == 2)
                {
                    if (parts.All(x => x.Length > 0))
                    {
                        return (parts[0], parts[1]);
                    }
                }
            }

            throw new Exception("Bad token!");
        }
    }
}
