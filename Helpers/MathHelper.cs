using System;

namespace BachelorThesis.Network.Helpers
{
    public static class MathHelper
    {
        public static double Sigmoid(double input)
        {
            return 1.0 / (1.0 + Math.Exp(-input));
        }

        public static double SigmoidPrime(double input)
        {
            return Sigmoid(input) * (1.0 - Sigmoid(input));
        }
    }
}