namespace RealTime.BL.ETrade.Models.Response
{
    public class ErrorResponse
    {
        public Error Error { get; set; }
    }

    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
