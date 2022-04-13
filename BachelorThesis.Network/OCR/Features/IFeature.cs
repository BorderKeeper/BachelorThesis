using BachelorThesis.Network.Entities;

namespace BachelorThesis.Network.OCR.Features
{
    public interface IFeature
    {
        Prediction CalculatePrediction(Image image);

        string ToString();
    }
}