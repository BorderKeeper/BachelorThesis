using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class HiddenLayerValues
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public int Index { get; set; }

        public string Filename { get; set; }

        public double Output { get; set; }

        public HiddenLayerValues(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Index = data.GetInt32(2);
            Filename = data.GetString(3);
            //Bias = data.GetDouble(4);
            Output = data.GetDouble(5);
        }
    }
}