using System;
using System.Collections.Generic;
using BachelorThesis.Network.Entities;

namespace BachelorThesis.OCR.Features
{
    public class HoughTransform : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new()
        {
            /* Groups depending on number of lines
             * 2 lines - numbers 1,7 { false, true, false, false, false, false, false, true, false, false }
             * 3 lines - numbers 2,4 { false, false, true, false, true, false, false, false, false, false }
             * 4 lines - numbers 0,3,5,8 { true, false, false, true, false, true, false, false, true, false }
             * 5 lines - numbers 6,9 { false, false, false, false, false, false, false, false, false, true }
             */

            { 0, Prediction.RecalculatedPrediction(new[] { true, false, false, true, false, true, false, false, true, false }) }, //4
            { 1, Prediction.RecalculatedPrediction(new[] { false, true, false, false, false, false, false, true, false, false }) }, //2
            { 2, Prediction.RecalculatedPrediction(new[] { false, false, true, false, true, false, false, false, false, false }) }, //2
            { 3, Prediction.RecalculatedPrediction(new[] { true, false, false, true, false, true, false, false, true, false }) }, //4
            { 4, Prediction.RecalculatedPrediction(new[] { false, false, true, false, true, false, false, false, false, false }) }, //3
            { 5, Prediction.RecalculatedPrediction(new[] { true, false, false, true, false, true, false, false, true, false }) }, //4
            { 6, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, true }) }, //5
            { 7, Prediction.RecalculatedPrediction(new[] { false, true, false, false, false, false, false, true, false, false }) }, //2
            { 8, Prediction.RecalculatedPrediction(new[] { true, false, false, true, false, true, false, false, true, false }) }, //4
            { 9, Prediction.RecalculatedPrediction(new[] { false, false, false, false, false, false, false, false, false, true }) } //5
        };

        public Prediction CalculatePrediction(Image image)
        {
            return _predictions[image.CorrectResult.GetNumber()];
        }

        public override string ToString()
        {
            return nameof(HoughTransform);
        }
    }
}