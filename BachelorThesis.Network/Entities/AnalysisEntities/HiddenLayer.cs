using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class HiddenLayer
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public int Index { get; set; }

        public double Bias { get; set; }

        public HiddenLayer(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Index = data.GetInt32(2);
            Bias = data.GetDouble(3);
        }
    }
}