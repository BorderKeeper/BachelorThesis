using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BachelorThesis.Network.ActivationFunctions;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Enums;
using BachelorThesis.Network.Helpers;
using BachelorThesis.Network.Options;
using Newtonsoft.Json;

namespace BachelorThesis.Network
{
    public class NeuralNetwork
    {
        public Neuron[][] Neurons;

        public double[][,] Weights;

        public List<double> Costs;

        private NeuralNetworkOptions _options;

        private readonly Random _random = new Random(12345);

        private int NumberOfLayers => Neurons.GetLength(0);

        public NeuralNetwork(List<int> layers, NeuralNetworkOptions options)
        {
            _options = options;
            _options.OnIterationCompleted += (sender, args) => { };

            Costs = new List<double>();

            SetupNetwork(layers);
        }

        private NeuralNetwork()
        {
        }

        public double[] Predict(double[] input)
        {
            FillInputLayer(input);

            for (int layer = 1; layer < NumberOfLayers; layer++)
            {
                for (int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    var previousLayer = layer - 1;
                    double sum = 0;

                    for (int inputNeuron = 0; inputNeuron < Neurons[layer - 1].Length; inputNeuron++)
                    {
                        sum += Neurons[previousLayer][inputNeuron].Output * Weights[previousLayer][inputNeuron, neuron];
                    }

                    Neurons[layer][neuron].Input = sum;
                    Neurons[layer][neuron].Output = MathHelper.Sigmoid(sum + Neurons[layer][neuron].Bias);
                }
            }

            return Neurons.Last().Select(neuron => neuron.Output).ToArray();
        }

        public void Train(double[][] trainingData, double[][] desiredOutput)
        {
            SetupWeights();
            ResetBiases();

            var trainingDataPerIteration = trainingData.GetLength(0);

            var iterations = _options.Iterations;

            while (iterations > 0)
            {
                for (int trainingIndex = 0; trainingIndex < trainingDataPerIteration; trainingIndex++)
                {
                    _ = Predict(trainingData[trainingIndex]);

                    CalculateOutputErrorsFromCost(desiredOutput, trainingIndex);

                    BackPropagateError();

                    ReadjustBias();

                    ReadjustWeights();
                }

                CallOnIterationCompleted(iterations);

                iterations--;
            }
        }

        public void Save(string path)
        {
            File.WriteAllText(Path.Combine(path, "neurons.json"), JsonConvert.SerializeObject(Neurons));
            File.WriteAllText(Path.Combine(path, "weights.json"), JsonConvert.SerializeObject(Weights));
            File.WriteAllText(Path.Combine(path, "options.json"), JsonConvert.SerializeObject(_options));
        }

        public static NeuralNetwork Load(string path)
        {
            var neuralNetwork = new NeuralNetwork
            {
                Neurons = JsonConvert.DeserializeObject<Neuron[][]>(File.ReadAllText(Path.Combine(path, "neurons.json"))),
                Weights = JsonConvert.DeserializeObject<double[][,]>(File.ReadAllText(Path.Combine(path, "weights.json"))),
                _options = JsonConvert.DeserializeObject<NeuralNetworkOptions>(File.ReadAllText(Path.Combine(path, "options.json")))
            };

            return neuralNetwork;
        }

        private void FillInputLayer(double[] input)
        {
            for (int neuron = 0; neuron < Neurons[0].Length; neuron++)
            {
                Neurons[0][neuron].Output = input[neuron];
            }
        }

        private void ReadjustWeights()
        {
            var layersWithoutOutput = NumberOfLayers - 1;
            for (int layer = 0; layer < layersWithoutOutput; layer++)
            {
                for (int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    for (int nextNeuron = 0; nextNeuron < Neurons[layer + 1].Length; nextNeuron++)
                    {
                        Weights[layer][neuron, nextNeuron] -= _options.Alpha * Neurons[layer][neuron].Output * Neurons[layer + 1][nextNeuron].Error;

                        if (_options.UseL2Regularization)
                        {
                            Weights[layer][neuron, nextNeuron] -= _options.Lambda * Weights[layer][neuron, nextNeuron];
                        }
                    }
                }
            }
        }

        private void ReadjustBias()
        {
            for (int layer = 1; layer < NumberOfLayers; layer++)
            {
                for (int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    Neurons[layer][neuron].Bias -= _options.Alpha * Neurons[layer][neuron].Error;
                }
            }
        }

        private void CalculateOutputErrorsFromCost(double[][] desiredOutput, int trainingIndex)
        {
            var outputLayerSize = Neurons.Last().Length;
            var cost = new double[outputLayerSize];

            for (int outputNeuronIndex = 0; outputNeuronIndex < outputLayerSize; outputNeuronIndex++)
            {
                var outputNeuron = Neurons[NumberOfLayers - 1][outputNeuronIndex];

                cost[outputNeuronIndex] = outputNeuron.Output - desiredOutput[trainingIndex][outputNeuronIndex];

                Costs.Add(cost.Average());

                Neurons[NumberOfLayers - 1][outputNeuronIndex].Error = cost[outputNeuronIndex] * MathHelper.SigmoidPrime(outputNeuron.Input + outputNeuron.Bias);
            }
        }

        private void BackPropagateError()
        {
            //Skipping input and output, going from output to input
            var layerWithoutOutput = NumberOfLayers - 1;

            for (int layer = layerWithoutOutput - 1; layer > 0; layer--)
            {
                for(int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    double sum = 0;

                    for (int nextNeuron = 0; nextNeuron < Neurons[layer + 1].Length; nextNeuron++)
                    {
                        sum += Weights[layer][neuron, nextNeuron] * Neurons[layer + 1][nextNeuron].Error;
                    }

                    Neurons[layer][neuron].Error = sum * MathHelper.SigmoidPrime(Neurons[layer][neuron].Input + Neurons[layer][neuron].Bias);
                }
            }
        }

        private void SetupNetwork(List<int> layers)
        {
            Neurons = new Neuron[layers.Count][];

            for (int layer = 0; layer < layers.Count; layer++)
            {
                var neuronsInLayer = layers[layer];

                Neurons[layer] = new Neuron[neuronsInLayer];

                for (int neuron = 0; neuron < neuronsInLayer; neuron++)
                {
                    Neurons[layer][neuron] = new Neuron();
                }
            }

            SetupWeights();
            ResetBiases();
        }

        private IActivationFunction GetActivationFunction()
        {
            switch (_options.ActivationFunctionType)
            {
                case ActivationFunctionType.Sigmoid:
                    return new SigmoidActivationFunction();
            }

            return new SigmoidActivationFunction();
        }

        private void SetupWeights()
        {
            var layersWithoutOutput = NumberOfLayers - 1;

            Weights = new double[layersWithoutOutput][,];
            for (int layer = 0; layer < layersWithoutOutput; layer++)
            {
                Weights[layer] = new double[Neurons[layer].Length, Neurons[layer+1].Length];

                for (int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    for (int nextNeuron = 0; nextNeuron < Neurons[layer + 1].Length; nextNeuron++)
                    {
                        Weights[layer][neuron,nextNeuron] = WeightFunction(layer+1);
                    }
                }
            }
        }

        private void ResetBiases()
        {
            for (int layer = 0; layer < NumberOfLayers; layer++)
            {
                for (int neuron = 0; neuron < Neurons[layer].Length; neuron++)
                {
                    Neurons[layer][neuron].Bias = 0;
                }
            }
        }

        private double WeightFunction(int layer)
        {
            var previousLayerLength = layer > 0 ? Neurons[layer - 1].Length : 0;
            var thisLayerLength = Neurons[layer].Length;

            var nonRandomizedWeight = Math.Sqrt(6) / Math.Sqrt(thisLayerLength + previousLayerLength);

            return _random.NextDouble() * 2 * nonRandomizedWeight - nonRandomizedWeight;
        }

        private void CallOnIterationCompleted(int iterations)
        {
            _options.OnIterationCompleted.Invoke(this, new IterationCompletedArgs
            {
                Iteration = iterations,
                AverageError = Costs.Average()
            });

            Costs.Clear();
        }
    }
}