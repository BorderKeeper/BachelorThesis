using System.Data;

namespace BachelorThesis.Network.Entities.AnalysisEntities
{
    public class Network
    {
        public int Id { get; set; }

        public int Run { get; set; }

        public string Filename { get; set; }

        public int Guessed { get; set; }

        public double Accuracy { get; set; }

        public int Actual { get; set; }

        public Network(IDataRecord data)
        {
            Id = data.GetInt32(0);
            Run = data.GetInt32(1);
            Filename = data.GetString(2);
            Guessed = data.GetInt32(3);
            Accuracy = data.GetDouble(4);
            Actual = data.GetInt32(5);
        }
    }
}