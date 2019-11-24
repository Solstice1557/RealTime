namespace RealTime.BL.Prices
{
    public class TechAnalysisInfo
    {
        public TechAnalysisInfo()
        {
        }

        public TechAnalysisInfo(TechAnalysisType type, int timePeriod, PriceType priceType)
        {
            this.Type = type;
            this.TimePeriod = timePeriod;
            this.PriceType = priceType;
        }

        public TechAnalysisType Type { get; set; }

        public int TimePeriod { get; set; }

        public PriceType PriceType { get; set; }

        public override string ToString()
        {
            return $"{this.Type.GetDescription()}({this.TimePeriod}, {this.PriceType})";
        }
    }
}
