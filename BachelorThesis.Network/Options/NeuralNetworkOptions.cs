using System;
using BachelorThesis.Network.Entities;
using BachelorThesis.Network.Enums;
using Newtonsoft.Json;

namespace BachelorThesis.Network.Options
{
    public class NeuralNetworkOptions
    {
        public int Iterations { get; set; } = 5000;

        public bool UseL2Regularization { get; set; } = true;

        public double Lambda { get; set; } = 0.00003;

        public double Alpha { get; set; } = 5.5;

        public ActivationFunctionType ActivationFunctionType { get; set; } = ActivationFunctionType.Sigmoid;

        [JsonIgnore]
        public EventHandler<IterationCompletedArgs> OnIterationCompleted;
    }
}