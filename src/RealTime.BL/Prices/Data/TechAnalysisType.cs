namespace RealTime.BL.Prices
{
    using System.ComponentModel;

    public enum TechAnalysisType : byte
    {
        [Description("MA")]
        MovingAverage,

        [Description("SMA")]
        SmoothedMovingAverage,

        [Description("EMA")]
        ExponentalMovingAverage
    }
}
