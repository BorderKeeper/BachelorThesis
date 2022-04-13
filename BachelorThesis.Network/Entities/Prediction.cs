using System;
using System.Linq;

namespace BachelorThesis.Network.Entities
{
    public class Prediction
    {
        public double[] PredictionVector { get; set; }

        /// <summary>
        /// For holes it would be number of holes
        /// </summary>
        public int Aspect { get; set; }

        public int MaxAspect { get; set; }

        public Prediction(double[] predictionVector)
        {
            PredictionVector = predictionVector;
        }

        public Prediction()
        {
            PredictionVector = new double[10];
        }

        public static Prediction RecalculatedPrediction(bool[] validDigitsVector, int aspect, int maxAspect)
        {
            var prediction = new Prediction();

            prediction.Aspect = aspect;
            prediction.MaxAspect = maxAspect;

            var numberOfValidDigits = validDigitsVector.Count(isNotFalse => isNotFalse);

            for (int i = 0; i < 10; i++)
            {
                if (validDigitsVector[i])
                {
                    prediction.PredictionVector[i] = 1.0 / numberOfValidDigits;
                }
                else
                {
                    prediction.PredictionVector[i] = 0;
                }
            }

            return prediction;
        }

        public static Prediction EmptyPrediction => new Prediction();

        public static Prediction GetPredictionFromNumber(int number)
        {
            var prediction = EmptyPrediction;

            prediction.PredictionVector[number] = 1;

            return prediction;
        }

        public int GetNumber()
        {
            return Array.IndexOf(PredictionVector, PredictionVector.Max());
        }

        public override string ToString()
        {
            return "[" + string.Join(" ", PredictionVector.Select(v => $"{v:F}")) + "]";
        }
    }
}