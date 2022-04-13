using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Entities.AnalysisEntities;
using BachelorThesis.Network.Enums;
using BachelorThesis.Network.OCR.Features;

namespace BachelorThesis.Network.OCR
{
    class OCRProgram
    {
        private const string ExperimentOneSavePath = "D:\\BachelorThesis\\experimentOne";

        /*
         * Features:
         * Hough Transform - Detect Lines and circles in image [Feature output: N/A]
         * Euler Number - Topology detection [Feature output: N/A]
         * Holes - Number of holes in the picture [Feature output: Prediction]
         */
        public static void RunAndStoreOCR(MniskDataType dataType, bool createCsvFile)
        {
            int run = 0;

            var data = dataType == MniskDataType.Test ? MniskDataProvider.LoadTestPictures() : MniskDataProvider.LoadTrainingPictures();

            var images = data.Select(e => new Image(ConvertImageStreamToMatrix(e.Input, 28), new Prediction(e.Output), e.ImageName));

            var features = FeatureFactory.GetFeatures();

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            var featureResults = new List<OcrFeature>();

            StringBuilder csv = new StringBuilder();

            csv.AppendLine("Filename,Correct Result,0,1,2,3,4,5,6,7,8,9");

            foreach (Image image in images)
            {
                var predictionsOnImage = new List<OcrFeature>();

                foreach (IFeature feature in features)
                {
                    var prediction = feature.CalculatePrediction(image);

                    var ocrFeature = new OcrFeature
                    {
                        Actual = image.CorrectResult.GetNumber(),
                        Feature = feature.ToString(),
                        Filename = image.Filename,
                        Prediction = prediction,
                        Run = run
                    };

                    predictionsOnImage.Add(ocrFeature);
                    featureResults.Add(ocrFeature);
                }

                var summedPrediction = new Prediction();

                for (int i = 0; i <= 9; i++)
                {
                    summedPrediction.PredictionVector[i] = predictionsOnImage.Sum(e => e.Prediction.PredictionVector[i]);
                }

                summedPrediction.PredictionVector = summedPrediction.PredictionVector.Select(p => p / 5).ToArray();

                var textSum = string.Join(",", summedPrediction.PredictionVector);
                csv.AppendLine($"{image.Filename},{image.CorrectResult.GetNumber()},{textSum}");
            }

            if(createCsvFile)
                File.WriteAllText(Path.Combine(ExperimentOneSavePath, "experimentOne.csv"), csv.ToString());

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
