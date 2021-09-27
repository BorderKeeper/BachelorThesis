using System.Data;

namespace BachelorThesis.DataAnalysis.Entities
{
    public class OcrFeature
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public string Filename { get; set; }

        public string Feature { get; set; }

        public int Actual { get; set; }

        public OcrFeature(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Filename = data.GetString(2);
            Feature = data.GetString(3);
            Actual = data.GetInt32(4);
        }
    }
}