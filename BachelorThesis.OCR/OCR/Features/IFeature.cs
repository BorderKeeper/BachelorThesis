using BachelorThesis.Network.Entities;

namespace BachelorThesis.OCR.Features
{
    public interface IFeature
    {
        Prediction CalculatePrediction(Image image);

        string ToString();
    }
}