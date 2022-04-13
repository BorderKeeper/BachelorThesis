using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class OcrResult
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public string Filename { get; set; }

        public string Feature { get; set; }

        public int Number { get; set; }

        public double Accuracy { get; set; }

        public OcrResult(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Filename = data.GetString(2);
            Feature = data.GetString(3);
            Number = data.GetInt32(4);
            Accuracy = data.GetDouble(5);
        }

        public override string ToString()
        {
            return $"{Accuracy:E2}";
        }
    }
}