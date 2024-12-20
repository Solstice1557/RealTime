using System;

namespace RealTime.PredictionsCheck
{
    public class PredictionModel
    {
        public DateTime Timestamp { get; set; }

        public string Symbol { get; set; }

        public double Score { get; set; }
    }
}
