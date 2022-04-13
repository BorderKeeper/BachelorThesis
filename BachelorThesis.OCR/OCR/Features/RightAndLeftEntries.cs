using System;
using System.Collections.Generic;
using BachelorThesis.Network.Entities;

namespace BachelorThesis.OCR.Features
{
    public class RightAndLeftEntries : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new()
        {
            /* Groups depending on number of entries
             * NA - 0,1,4,8 { false, false, false, false, false, false, false, false, false, false }
             * TL BR - 2
             * TL BL - 3
             * TR BL - 5
             * TR - 6
             * BL - 7,9
             */

            { 0, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }) }, //NA
            { 1, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }) }, //NA
            { 2, Prediction.RecalculatedPrediction(new[] { false, false, true, false, false, false, false, false, false, false }) }, //TL BR
            { 3, Prediction.RecalculatedPrediction(new[] { false, false, false, true, false, false, false, false, false, false }) }, //TL BL
            { 4, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }) }, //NA
            { 5, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, true, false, false, false, false }) }, //TR BL
            { 6, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, true, false, false, false }) }, //TR
            { 7, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, true, false, true }) }, //BL
            { 8, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, false }) }, //NA
            { 9, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, true, false, true }) } //BL
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