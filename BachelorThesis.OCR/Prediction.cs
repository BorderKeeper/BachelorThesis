using System;
using System.Linq;

namespace BachelorThesis.OCR
{
    public class Prediction
    {
        public double[] PredictionVector { get; set; }

        public Prediction(double[] predictionVector)
        {
            PredictionVector = predictionVector;
        }

        public Prediction()
        {
            PredictionVector = new double[10];
        }

        public static Prediction RecalculatedPrediction(bool[] validDigitsVector)
        {
            var prediction = new Prediction();

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

        public static Prediction EmptyPrediction => new();

        public double GetNumber()
        {
            return Array.IndexOf(PredictionVector, PredictionVector.Max());
        }

        public override string ToString()
        {
            return string.Join(" ", PredictionVector.Select(v => $"{v:F}"));
        }
    }
}