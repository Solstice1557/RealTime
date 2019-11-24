namespace RealTime.BL.Alphavantage
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class AlphavantageTechnicalAnalysisResponse
    {
        [JsonPropertyName("Meta Data")]
        public AlphavantageTechnicalAnalysisMetaData MetaData { get; set; }

        [JsonPropertyName("Error Message")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("Technical Analysis: SMA")]
        public Dictionary<DateTime, AlphavantageTechnicalAnalysis> TechnicalAnalysisSMA { get; set; }

        [JsonPropertyName("Technical Analysis: EMA")]
        public Dictionary<DateTime, AlphavantageTechnicalAnalysis> TechnicalAnalysisEMA { get; set; }
    }
}
