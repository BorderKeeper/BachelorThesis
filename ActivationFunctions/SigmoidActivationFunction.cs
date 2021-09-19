using BachelorThesis.Network.Helpers;

namespace BachelorThesis.Network.ActivationFunctions
{
    public class SigmoidActivationFunction : IActivationFunction
    {
        public double Calculate(double input)
        {
            return MathHelper.Sigmoid(input);
        }
    }
}