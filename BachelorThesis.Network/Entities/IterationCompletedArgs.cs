namespace BachelorThesis.Network.Entities
{
    public class IterationCompletedArgs
    {
        public int Iteration { get; set; }

        /// <summary>
        /// The closer to zero the better
        /// </summary>
        public double AverageError { get; set; }

        /// <summary>
        /// Gets number of items predicted right vs wrong
        /// </summary>
        public int NoOfPredicted { get; set; }
    }
}