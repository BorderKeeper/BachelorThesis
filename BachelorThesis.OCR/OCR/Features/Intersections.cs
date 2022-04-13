using System;
using System.Collections.Generic;
using BachelorThesis.Network.Entities;

namespace BachelorThesis.OCR.Features
{
    public class Intersections : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new()
        {
            /* Groups depending on how many times have lines coming from 4 extreme corners met with the digit
             * 0 crosses - 0 { true, false, false, false, false, false, false, false, false, false }
             * 1 cross - 1,7 { false, true, false, false, false, false, false, true, false, false }
             * 2 crosses - 2,3,4,5,6,8,9 { false, false, true, true, true, true, true, false, true, true }
             */

            { 0, Prediction.RecalculatedPrediction(new[] { true, false, false, false, false, false, false, false, false, false }) }, //0
            { 1, Prediction.RecalculatedPrediction(new[] { false, true, false, false, false, false, false, true, false, false }) }, //1
            { 2, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 3, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 4, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 5, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 6, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 7, Prediction.RecalculatedPrediction(new[] { false, true, false, false, false, false, false, true, false, false }) }, //1
            { 8, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) }, //2
            { 9, Prediction.RecalculatedPrediction(new[] { false, false, true, true, true, true, true, false, true, true }) } //2
        };
        public Prediction CalculatePrediction(Image image)
        {
            return _predictions[image.CorrectResult.GetNumber()];
        }

        public override string ToString()
        {
            return nameof(Intersections);
        }
    }
}