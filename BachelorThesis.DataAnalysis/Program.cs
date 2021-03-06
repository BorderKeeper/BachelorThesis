using System;
using System.Collections.Generic;
using System.Linq;
using BachelorThesis.Network;
using BachelorThesis.Network.Entities.AnalysisEntities;

namespace BachelorThesis.DataAnalysis
{
    class Program
    {
        public const int Run = 0;
        public const double SimilarityThreshold = 0.1;

        private static SqlDataProvider _dal;

        private static List<HiddenLayer> _hiddenLayer;
        private static List<HiddenLayerValues> _hiddenLayerValues;
        private static List<HiddenWeights> _weights;
        private static List<Network.Entities.AnalysisEntities.Network> _networks;
        private static List<OcrFeature> _features;
        private static List<OcrResult> _results;

        static void Main(string[] args)
        {
            _dal = new SqlDataProvider();

            /*_hiddenLayer = GetHiddenLayer();

            _hiddenLayerValues = GetHiddenLayerValues();

            _weights = GetHiddenLayerWeights();

            _networks = GetNetworks();*/

            _features = GetFeatures();

            _results = GetResults();

            Accuracy("HoleDetector");
            Accuracy("HoughTransform");
            Accuracy("RightAndLeftEntries");
            Accuracy("CrossInTheCenter");
            Accuracy("Intersections");

            /*foreach (Entities.Network network in _networks.Where(n => n.Run == Run))
            {
                var neurons = _hiddenLayer.Where(n => n.Run == network.Run);

                foreach (HiddenLayer neuron in neurons)
                {
                    var output = _hiddenLayerValues.Single(n => n.Filename == network.Filename && n.Run == network.Run && n.Index == neuron.Index);

                    var neuronWeights = _weights.Where(w => w.Run == network.Run && w.Index == neuron.Index);

                    MatchNeuronToFeatures(neuron, output, neuronWeights, network.Filename);
                }
            }*/
        }

        private static void Accuracy(string feature)
        {
            var numberOfWrongGuesses = 0;
            int guesses = 0;

            foreach (OcrFeature ocrFeature in _features.Where(f => f.Filename == "img_434.jpg"))
            {
                var actual = ocrFeature.Actual;

                var vector = _results.Where(r => r.Run == Run && r.Filename == ocrFeature.Filename && r.Feature == feature && r.Filename == "img_434.jpg");
                var guessedDigit = vector.First(d => d.Accuracy == vector.Max(d => d.Accuracy));

                if (actual != guessedDigit.Number) numberOfWrongGuesses++;
                guesses++;
            }

            var accuracy = (double) numberOfWrongGuesses / (double) guesses;
            Console.WriteLine($"{feature} guessed {numberOfWrongGuesses} times wrong out of {guesses}, which is an accuracy of {accuracy:P}");
        }

        private static void MatchNeuronToFeatures(HiddenLayer neuron, HiddenLayerValues output, IEnumerable<HiddenWeights> neuronWeights, string fileName)
        {
            var neuronWeightedOutputVector = neuronWeights.Select(w => w.Weight * output.Output);

            var currentFeatures = _features.Where(f => f.Run == Run && f.Filename == fileName);

            foreach (OcrFeature currentFeature in currentFeatures)
            {
                var outputVector = _results.Where(r => r.Run == Run && r.Filename == fileName && r.Feature == currentFeature.Feature);

                if (MatchingNeuronAndOcrOutputs(neuronWeightedOutputVector.ToArray(), outputVector.ToArray()) > 5)
                {
                    if (outputVector.Any(v => v.Accuracy != 0))
                    {
                        Console.WriteLine("Matched");
                    }
                }
            }
        }

        private static int MatchingNeuronAndOcrOutputs(double[] neuronWeightedOutputVector, OcrResult[] outputVector)
        {
            var matches = 0;

            for (int digit = 0; digit < 10; digit++)
            {
                var neuronDigitAccuracy = neuronWeightedOutputVector[digit];
                var ocrDigitAccuracy = outputVector[digit].Accuracy;

                if (Math.Abs(neuronDigitAccuracy - ocrDigitAccuracy) <= SimilarityThreshold)
                {
                    matches++;
                }
            }

            return matches;
        }

        private static List<HiddenLayer> GetHiddenLayer()
        {
            var hiddenLayer = new List<HiddenLayer>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_layer");

            while (reader.Read())
            {
                hiddenLayer.Add(new HiddenLayer(reader));
            }

            return hiddenLayer;
        }

        private static List<HiddenLayerValues> GetHiddenLayerValues()
        {
            var hiddenLayerValues = new List<HiddenLayerValues>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_layer_values");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new HiddenLayerValues(reader));
            }

            return hiddenLayerValues;
        }

        private static List<HiddenWeights> GetHiddenLayerWeights()
        {
            var hiddenLayerValues = new List<HiddenWeights>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_weights");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new HiddenWeights(reader));
            }

            return hiddenLayerValues;
        }

        private static List<Network.Entities.AnalysisEntities.Network> GetNetworks()
        {
            var hiddenLayerValues = new List<Network.Entities.AnalysisEntities.Network>();

            using var reader = _dal.Read("SELECT * FROM network");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new Network.Entities.AnalysisEntities.Network(reader));
            }

            return hiddenLayerValues;
        }

        private static List<OcrFeature> GetFeatures()
        {
            var hiddenLayerValues = new List<OcrFeature>();

            using var reader = _dal.Read("SELECT * FROM ocr_features");

            while (reader.Read())
            {
                //hiddenLayerValues.Add(new OcrFeature(reader));
            }

            return hiddenLayerValues;
        }

        private static List<OcrResult> GetResults()
        {
            var hiddenLayerValues = new List<OcrResult>();

            using var reader = _dal.Read("SELECT * FROM ocr_results");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new OcrResult(reader));
            }

            return hiddenLayerValues;
        }
    }
}
