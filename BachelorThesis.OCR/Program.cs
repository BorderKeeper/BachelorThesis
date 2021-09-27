﻿using System;
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

        private static SqlDataProvider _dal;

        static void Main(string[] args)
        {
            int run = 0;

            _dal = new SqlDataProvider();

            var data = DataProvider.LoadTestPictures();

            var images = data.Select(e => new Image(ConvertImageStreamToMatrix(e.Input, 28), new Prediction(e.Output), e.ImageName));

            var features = FeatureFactory.GetFeatures();

            foreach (Image image in images)
            {
                Prediction outputPrediction = new Prediction();

                var featureVectors = new List<Prediction>();

                foreach (IFeature feature in features)
                {
                    var prediction = feature.CalculatePrediction(image);

                    _dal.Update($"INSERT INTO ocr_features (run, filename, feature, actual) VALUES ({run}, \"{image.Filename}\", \"{feature.GetType().Name}\", {image.CorrectResult.GetNumber()})");

                    for (int i = 0; i < 10; i++)
                    {
                        _dal.Update($"INSERT INTO ocr_results (run, filename, feature, number, accuracy) VALUES ({run}, \"{image.Filename}\", \"{feature.GetType().Name}\", {i}, {prediction.PredictionVector[i]})");
                    }

                    featureVectors.Add(prediction);
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
