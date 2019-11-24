namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AlphavantageTechnicalAnalysisResponse
    {
        [JsonProperty("Meta Data")]
        public AlphavantageTechnicalAnalysisMetaData MetaData { get; set; }

        [JsonProperty("Error Message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("Technical Analysis: SMA")]
        public Dictionary<DateTime, AlphavantageTechnicalAnalysis> TechnicalAnalysisSMA { get; set; }

        [JsonProperty("Technical Analysis: EMA")]
        public Dictionary<DateTime, AlphavantageTechnicalAnalysis> TechnicalAnalysisEMA { get; set; }
    }
}
