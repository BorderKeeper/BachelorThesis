using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Entities.AnalysisEntities;
using BachelorThesis.Network.Enums;
using BachelorThesis.Network.OCR;
using BachelorThesis.Network.Options;

namespace BachelorThesis.Network
{
    class Program
    {
        private const string SavePath = "D:\\BachelorThesis\\networks";

        private const string TrainingName = "FeatureNet-New";

        private const string ExperimentThreeNetwork = "PictureNetWithFeatureLayer-testWTF";
        private const string ExperimentTwoNetwork = "FeatureNet-Aspects";

        static void Main(string[] args)
        {
            //OCRFunctions();

            NeuralNetworkFunctions();

            //CompareOCRNetWithDeepNet();

            Console.ReadKey();
        }

        private static void CompareOCRNetWithDeepNet()
        {
            var featureNetwork = NeuralNetwork.Load(SavePath, ExperimentTwoNetwork);
            var deepNetwork = NeuralNetwork.Load(SavePath, ExperimentThreeNetwork);

            var data = MniskDataProvider.LoadTestPictures();

            foreach (DataRow picture in data)
            {
                var featureResult = new Prediction(featureNetwork.Predict(picture.Input));
                var deepResult = new Prediction(deepNetwork.Predict(picture.Input));

                var featureWeights = featureNetwork.Weights;
                var deepWeights = deepNetwork.Weights.TakeLast(2).ToArray();

                var featureIsRight = featureResult.GetNumber() == picture.Character;
                var deepIsRight = deepResult.GetNumber() == picture.Character;

                //Both networks agree let's compare their weights
                if (featureResult.GetNumber() == deepResult.GetNumber())
                {
                    Console.WriteLine($"[{picture.Character}] Agrees - Feature[{featureIsRight}]: {featureResult} Deep[{deepIsRight}]: {deepResult}");
                }
                else
                {
                    Console.WriteLine($"[{picture.Character}] Disagrees - Feature[{featureIsRight}]: {featureResult} Deep[{deepIsRight}]: {deepResult}");
                }
            }
        }

        private static void OCRFunctions()
        {
            //EXPERIMENT ONE
            OCRProgram.RunAndStoreOCR(MniskDataType.Test, true);
        }

        private static void NeuralNetworkFunctions()
        {
            //TestSimpleXor();

            //EXPERIMENT THREE
            //TrainOnPictures();
            //TestPictureNetwork();

            //EXPERIMENT TWO
            //TrainOnFeatures();
            TestFeatureNetwork();
        }

        private static void TestFeatureNetwork()
        {
            Console.WriteLine("Starting...");

            OCRProgram.RunAndStoreOCR(MniskDataType.Test, false);

            Console.WriteLine("OCR Finished...");

            var data = LocalStorageDataProvider.GetFeatures();

            var network = NeuralNetwork.Load(SavePath, TrainingName);

            DisplayFeatureData(data, network);
        }
        private static void DisplayFeatureData(List<OcrFeature> data, NeuralNetwork network)
        {
            var groupedByImage = data.GroupBy(e => e.Filename);

            var predicitons = new List<Tuple<int, Prediction>>();

            foreach (IGrouping<string, OcrFeature> imageGroup in groupedByImage)
            {
                var actual = imageGroup.First().Actual;

                var output = network.Predict(imageGroup.Select(e => (double)(e.Prediction.Aspect / e.Prediction.MaxAspect)).ToArray());
                var prediction = new Prediction(output);

                predicitons.Add(new Tuple<int, Prediction>(actual, prediction));

                Console.WriteLine($"[{actual}]: {prediction.GetNumber()}");
            }

            var groupedByDigit = predicitons.GroupBy(e => e.Item1);

            Console.WriteLine("Actual");
            Console.WriteLine("  0 1 2 3 4 5 6 7 8 9");

            foreach (IGrouping<int, Tuple<int, Prediction>> tuples in groupedByDigit)
            {
                var n0 = tuples.Count(e => e.Item2.GetNumber() == 0);
                var n1 = tuples.Count(e => e.Item2.GetNumber() == 1);
                var n2 = tuples.Count(e => e.Item2.GetNumber() == 2);
                var n3 = tuples.Count(e => e.Item2.GetNumber() == 3);
                var n4 = tuples.Count(e => e.Item2.GetNumber() == 4);
                var n5 = tuples.Count(e => e.Item2.GetNumber() == 5);
                var n6 = tuples.Count(e => e.Item2.GetNumber() == 6);
                var n7 = tuples.Count(e => e.Item2.GetNumber() == 7);
                var n8 = tuples.Count(e => e.Item2.GetNumber() == 8);
                var n9 = tuples.Count(e => e.Item2.GetNumber() == 9);

                Console.WriteLine($"{tuples.Key} {n0} {n1} {n2} {n3} {n4} {n5} {n6} {n7} {n8} {n9}");
            }
        }


        private static void TestPictureNetwork()
        {
            var data = MniskDataProvider.LoadTestPictures();

            var network = NeuralNetwork.Load(SavePath, TrainingName);

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            int wrongAnswers = 0;
            foreach (DataRow row in data)
            {
                var result = new Prediction(network.Predict(row.Input));
                var expected = new Prediction(row.Output);

                if (result.GetNumber() == expected.GetNumber())
                {
                    Console.WriteLine($"[{expected.GetNumber()}] {result.GetNumber()} was correct!");
                }
                else
                {
                    Console.WriteLine($"[{expected.GetNumber()}] {result.GetNumber()} was wrong!");
                    wrongAnswers++;
                }
            }

            Console.WriteLine($"Stopwatch elapsed: {stopwatch.ElapsedMilliseconds} Out of {data.Count()}, {wrongAnswers} was incorrect.");
        }

        private static void TrainOnFeatures()
        {
            Console.WriteLine("Starting...");

            OCRProgram.RunAndStoreOCR(MniskDataType.Training, false);

            Console.WriteLine("OCR Finished...");

            var layers = new List<int> { 5, 10 };

            var options = new NeuralNetworkOptions
            {
                Iterations = 1000,
                Alpha = 3.5,
                UseL2Regularization = true,
                Lambda = 0.0003
            };

            var data = LocalStorageDataProvider.GetFeatures();

            options.OnIterationCompleted += (sender, args) =>
            {
                Console.WriteLine($"{args.Iteration}: Cost: {args.AverageError:R3}, Right Guesses: {args.NoOfPredicted}/{data.Count}");
            };

            var network = new NeuralNetwork(layers, options);

            

            var randomizedData = RandomizeImageData(data);

            var groupedByImage = randomizedData.GroupBy(e => e.Filename);

            //number of images * features
            var input = new double[groupedByImage.Count()][];
            var output = new double[groupedByImage.Count()][];

            //over each image
            for (int image = 0; image < groupedByImage.Count(); image++)
            {
                input[image] = new double[5];

                //over each feature
                for (int feature = 0; feature < 5; feature++)
                {
                    var ocrFeature = groupedByImage.ToArray()[image].ToArray()[feature];

                    input[image][feature] = ocrFeature.Prediction.Aspect / ocrFeature.Prediction.MaxAspect;
                    output[image] = Prediction.GetPredictionFromNumber(ocrFeature.Actual).PredictionVector;
                }
            }

            network.Train(input, output);

            network.Save(SavePath, TrainingName);

            Console.WriteLine("Network trained and saved.");
        }

        private static void TrainOnPictures()
        {
            var layers = new List<int> {784, 300, 5, 10};

            var options = new NeuralNetworkOptions
            {
                Iterations = 100,
                Alpha = 3.5,
                UseL2Regularization = true,
                Lambda = 0.0003
            };

            var network = new NeuralNetwork(layers, options);

            var data = MniskDataProvider.LoadTrainingPictures().ToList();

            options.OnIterationCompleted += (sender, args) =>
            {
                Console.WriteLine($"{args.Iteration}: Cost: {args.AverageError:R3}, Right Guesses: {args.NoOfPredicted}/{data.Count}");
            };

            //DrawPicture(trainingData, expectedResult, 464);

            var randomizedData = RandomizeImageData(data);

            network.Train(randomizedData.Select(e => e.Input).ToArray(), randomizedData.Select(e => e.Output).ToArray());

            network.Save(SavePath, TrainingName);
        }

        private static List<T> RandomizeImageData<T>(List<T> data)
        {
            Random random = new Random();
            var randomizedData = data.OrderBy(x => random.Next());
            return randomizedData.ToList();
        }

        private static void DrawPicture(List<double[]> trainingData, List<double[]> outputResult, int index)
        {
            Console.WriteLine(string.Join(", ", outputResult[index]));
            Console.WriteLine("----------------------------------------");

            for (int i = 0; i < trainingData[index].Length; i++)
            {
                if (trainingData[index][i] != 0)
                {
                    Console.Write(" X ");
                }
                else
                {
                    Console.Write("   ");
                }

                if (i % 28 == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("----------------------------------------");
        }

        private static void TestSimpleXor()
        {
            var xorInput = new double[][]
            {
                new double[]{ 1, 0 },
                new double[]{ 0, 1 },
                new double[]{ 1, 1 },
                new double[]{ 0, 0 },
                new double[]{ 1, 1 },
                new double[]{ 0, 0 },
                new double[]{ 1, 0 },
                new double[]{ 0, 1 },
            };

            var result = new double[] { 1, 1, 0, 0, 0, 0, 1, 1 };

            var network = new NeuralNetwork(new List<int> { 2, 2, 1 }, new NeuralNetworkOptions
            {
                Iterations = 1000,
                Alpha = 3.5,
                UseL2Regularization = true,
                Lambda = 0.0003
            });

            network.Train(xorInput, result.Select(expectedResult => new[] { expectedResult }).ToArray());

            foreach (var input in xorInput)
            {
                Console.WriteLine($"input {input[0]},{input[1]} => {network.Predict(input)[0]}");
            }
        }
    }
}
