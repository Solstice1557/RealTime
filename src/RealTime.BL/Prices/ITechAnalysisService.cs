namespace RealTime.BL.Prices
{
    using System;
    using System.Collections.Generic;

    public interface ITechAnalysisService
    {
        Dictionary<DateTime, decimal?> CaclulateMovingAvg(List<(DateTime, decimal)> prices, int timePeriod);

        Dictionary<DateTime, decimal?> CaclulateSmoothedMovingAvg(List<(DateTime, decimal)> prices, int timePeriod);

        Dictionary<DateTime, decimal?> CaclulateExponentalMovingAvg(List<(DateTime, decimal)> prices, int timePeriod);
    }
}