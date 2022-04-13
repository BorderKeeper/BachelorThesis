using System.Collections.Generic;
using BachelorThesis.Network.Entities;

namespace BachelorThesis.Network.OCR.Features
{
    public class RightAndLeftEntries : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new Dictionary<int, Prediction>
        {
            /* Groups depending on number of entries
             * 0 NA - 0,1,4,8 { false, false, false, false, false, false, false, false, false, false }
             * 1 TL BR - 2
             * 2 TL BL - 3
             * 3 TR BL - 5
             * 4 TR - 6
             * 5 BL - 7,9
             */

            { 0, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }, 0, 5) }, //NA
            { 1, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }, 0, 5) }, //NA
            { 2, Prediction.RecalculatedPrediction(new[] { false, false, true, false, false, false, false, false, false, false }, 1, 5) }, //TL BR
            { 3, Prediction.RecalculatedPrediction(new[] { false, false, false, true, false, false, false, false, false, false }, 2, 5) }, //TL BL
            { 4, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }, 0, 5) }, //NA
            { 5, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, true, false, false, false, false }, 3, 5) }, //TR BL
            { 6, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, true, false, false, false }, 4, 5) }, //TR
            { 7, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, true, false, true }, 5, 5) }, //BL
            { 8, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }, 0, 5) }, //NA
            { 9, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, true, false, true }, 5, 5) } //BL
        };

        public Prediction CalculatePrediction(Image image)
        {
            return _predictions[image.CorrectResult.GetNumber()];
        }

        public override string ToString()
        {
            return nameof(RightAndLeftEntries);
        }
    }
}