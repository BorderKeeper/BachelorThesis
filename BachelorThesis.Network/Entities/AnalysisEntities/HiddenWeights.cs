using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class HiddenWeights
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public int Index { get; set; }

        public int OutputLayer { get; set; }

        public double Weight { get; set; }

        public HiddenWeights(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Index = data.GetInt32(2);
            OutputLayer = data.GetInt32(3);
            Weight = data.GetDouble(4);
        }
    }
}