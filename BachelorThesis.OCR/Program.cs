using System;
using System.Collections.Generic;
using System.Linq;
using BachelorThesis.Network;
using BachelorThesis.OCR.Features;

namespace BachelorThesis.OCR
{
    class Program
    {
        /*
         * Features:
         * Hough Transform - Detect Lines and circles in image [Feature output: N/A]
         * Euler Number - Topology detection [Feature output: N/A]
         * Holes - Number of holes in the picture [Feature output: PredictionVector]
         */

        private static Dictionary<double[], double[][]> ConvertedData { get; set; }

        static void Main(string[] args)
        {
            var data = DataProvider.LoadTestPictures();

            ConvertedData = data.Select(image => new KeyValuePair<double[], double[][]>(image.Key, ConvertImageStreamToMatrix(image.Value, 28)))
                .ToDictionary(i => i.Key, i => i.Value);

            var images = ConvertedData.Select(e => new Image(e.Value, new Prediction(e.Key)));

            var features = FeatureFactory.GetFeatures();

            foreach (Image image in images)
            {
                Prediction outputPrediction = new Prediction();

                var featureVectors = new List<Prediction>();

                foreach (IFeature feature in features)
                {
                    featureVectors.Add(feature.CalculatePrediction(image));
                }

                for (int i = 0; i < 9; i++)
                {
                    outputPrediction.PredictionVector[i] = featureVectors.Select(p => p.PredictionVector).Average(v => v[i]);
                }

                Console.WriteLine($"[{outputPrediction}] Predicted: {outputPrediction.GetNumber()}, Actual: {image.CorrectResult.GetNumber()}"); 
            }
        }

        private static double[][] ConvertImageStreamToMatrix(double[] imageStream, int lineCount)
        {
            List<double[]> image = new List<double[]>();

            double[] buffer = new double[lineCount];

            for (int i = 0; i < imageStream.Length; i++)
            {
                buffer[i % lineCount] = imageStream[i];

                if (i % lineCount == 0 && i != 0)
                {
                    image.Add(buffer);

                    buffer = new double[lineCount];
                }
            }

            return image.ToArray();
        }
    }
}
