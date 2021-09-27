using System;
using System.Collections.Generic;
using System.Linq;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Options;

namespace BachelorThesis.Network
{
    class Program
    {
        private const string SavePath = "D:\\BachelorThesis\\networks";

        private static SqlDataProvider _dal;

        static void Main(string[] args)
        {
            //TestSimpleXor();

            //TrainOnPictures();

            _dal = new SqlDataProvider();

            TestNetwork();

            Console.ReadKey();
        }

        private static void TestNetwork()
        {
            var data = DataProvider.LoadTestPictures();

            var network = NeuralNetwork.Load(SavePath);

            DisplayData(data, network);
        }

        private static void TrainOnPictures()
        {
            var layers = new List<int> {784, 300, 30, 10};

            var options = new NeuralNetworkOptions
            {
                Iterations = 10000,
                Alpha = 3.5,
                UseL2Regularization = true,
                Lambda = 0.0003
            };

            options.OnIterationCompleted += (sender, args) =>
            {
                Console.WriteLine($"{args.Iteration}: Cost: {args.AverageError:R3}");
            };

            var network = new NeuralNetwork(layers, options);

            var data = DataProvider.LoadTrainingPictures();

            //DrawPicture(trainingData, expectedResult, 464);

            var randomizedData = RandomizeData(data);

            network.Train(randomizedData.Select(e => e.Value).ToArray(), randomizedData.Select(e => e.Key).ToArray());

            network.Save(SavePath);
        }

        private static void DisplayData(IEnumerable<DataRow> data, NeuralNetwork network)
        {
            int run = 0;

            for (int n = 0; n < network.Neurons[^2].Length; n++)
            {
                var neuron = network.Neurons[^2][n];

                _dal.Update($"INSERT INTO last_hidden_layer (run, `index`, bias) VALUES ({run}, {n}, {neuron.Bias})");

                for (int ln = 0; ln < network.Neurons.Last().Length; ln++)
                {
                    var weight = network.Weights[^2][n,ln];

                    _dal.Update($"INSERT INTO last_hidden_weights (run, `index`, output_layer, weight) values ({run}, {n}, {ln}, {weight})");
                }
            }

            int trainingDataIndex = 0;
            foreach (var row in data)
            {
                var output = network.Predict(row.Input);

                var outputWithPercentage = new Dictionary<int, double>();
                for(int i = 0; i < output.Length; i++)
                {
                    outputWithPercentage.Add(i, output[i]);
                }

                var top3 = outputWithPercentage.OrderByDescending(e => e.Value).Take(3);

                _dal.Update($"INSERT INTO network (run, filename, guessed, accuracy, actual) VALUES ({run}, \"{row.ImageName}\", {top3.First().Key}, {top3.First().Value}, {row.Character})");

                for (int n = 0; n < network.Neurons[^2].Length; n++)
                {
                    var neuron = network.Neurons[^2][n];

                    _dal.Update($"INSERT INTO last_hidden_layer_values (run, `index`, filename, bias, output) VALUE ({run}, {n}, \"{row.ImageName}\", {neuron.Bias}, {neuron.Output})");
                }

                Console.WriteLine($"actual:{string.Join(", ", top3.Select(x => $"|{x.Key}|: {x.Value:P}"))}, expected: {row.Character}");
                trainingDataIndex++;
            }
        }

        private static Dictionary<double[], double[]> RandomizeData(Dictionary<double[], double[]> data)
        {
            Random random = new Random();
            var randomizedData = data.OrderBy(x => random.Next()).ToDictionary(item => item.Key, item => item.Value);
            return randomizedData;
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
