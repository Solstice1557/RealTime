namespace RealTime.BL.Tdameritrade.Utils
{
    public interface ITdAmeritradeTokenService
    {
        string Concatenate(string accountId, string token);
        (string AccountId, string Token) Split(string token);
    }
}
