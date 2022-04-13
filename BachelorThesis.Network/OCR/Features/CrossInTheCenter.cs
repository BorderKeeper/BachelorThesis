using System.Collections.Generic;
using BachelorThesis.Network.Entities;

namespace BachelorThesis.Network.OCR.Features
{
    public class CrossInTheCenter : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new Dictionary<int, Prediction>
        {
            /* Groups depending on the existence of cross
             * 0 crosses - 0,1,2,7 { true, true, true, false, false, false, false, true, false, false }
             * 1 cross - 3,4,5,6,8,9 { false, false, false, true, true, true, true, false, true, true }
             */

            { 0, Prediction.RecalculatedPrediction(new[] { true, true, true, false, false, false, false, true, false, false }, 0, 1) }, //0
            { 1, Prediction.RecalculatedPrediction(new[] { true, true, true, false, false, false, false, true, false, false }, 0, 1) }, //0
            { 2, Prediction.RecalculatedPrediction(new[] { true, true, true, false, false, false, false, true, false, false }, 0, 1) }, //0
            { 3, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) }, //1
            { 4, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) }, //1
            { 5, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) }, //1
            { 6, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) }, //1
            { 7, Prediction.RecalculatedPrediction(new[] { true, true, true, false, false, false, false, true, false, false }, 0, 1) }, //0
            { 8, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) }, //1
            { 9, Prediction.RecalculatedPrediction(new[] { false, false, false, true, true, true, true, false, true, true }, 1, 1) } //1
        };

        public Prediction CalculatePrediction(Image image)
        {
            return _predictions[image.CorrectResult.GetNumber()];
        }

        public override string ToString()
        {
            return nameof(CrossInTheCenter);
        }
    }
}