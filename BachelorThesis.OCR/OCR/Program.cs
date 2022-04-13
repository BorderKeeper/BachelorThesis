using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BachelorThesis.Network;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Entities.AnalysisEntities;
using BachelorThesis.OCR.Features;

namespace BachelorThesis.OCR
{
    class OCRProgram
    {
        /*
         * Features:
         * Hough Transform - Detect Lines and circles in image [Feature output: N/A]
         * Euler Number - Topology detection [Feature output: N/A]
         * Holes - Number of holes in the picture [Feature output: PredictionVector]
         */
        public static void RunAndStoreOCR()
        {
            int run = 0;

            var data = MniskDataProvider.LoadTestPictures();

            var images = data.Select(e => new Image(ConvertImageStreamToMatrix(e.Input, 28), new Prediction(e.Output), e.ImageName));

            var features = FeatureFactory.GetFeatures();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var featureResults = new List<OcrFeature>();

            foreach (Image image in images)
            {
                foreach (IFeature feature in features)
                {
                    var prediction = feature.CalculatePrediction(image);

                    featureResults.Add(new OcrFeature
                    {
                        Actual = image.CorrectResult.GetNumber(),
                        Feature = feature.ToString(),
                        Filename = image.Filename,
                        PredictionVector = prediction,
                        Run = run
                    });

                    //Console.WriteLine($"[{feature}] Predicted: {prediction.GetNumber()}, Actual: {image.CorrectResult.GetNumber()}");
                }
            }

            LocalStorageDataProvider.StoreFeatures(featureResults);

            Console.WriteLine($"Stopwatch elapsed: {stopwatch.ElapsedMilliseconds}");
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
