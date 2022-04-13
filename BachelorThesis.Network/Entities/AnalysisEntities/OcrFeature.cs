using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class OcrFeature
    {
        public int Run { get; set; }

        public string Filename { get; set; }

        public string Feature { get; set; }

        public int Actual { get; set; }

        public Prediction Prediction { get; set; }
    }
}