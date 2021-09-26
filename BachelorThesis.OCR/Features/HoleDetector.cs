using System;
using System.Collections.Generic;
using System.Linq;

namespace BachelorThesis.OCR.Features
{
    public class HoleDetector : IFeature
    {
        private readonly Dictionary<int, Prediction> _predictions = new()
        {
            { 0, Prediction.RecalculatedPrediction(new [] { false, true, true, true, false, true, false, true, false, false })},
            { 1, Prediction.RecalculatedPrediction(new [] { true, false, false, false, true, false, true, false, false, true })},
            { 2, Prediction.RecalculatedPrediction(new [] { false, false, false, false, false, false, false, false, true, false })}
        };

        private int[][] _groupFlag;

        public Image Image;

        public Prediction CalculatePrediction(Image image)
        {
            Image = image;

            int groupCounter = 0;

            FillGroupFlag(image);

            for (int rowIndex = 0; rowIndex < image.Height; rowIndex++)
            {
                var row = image.PixelData[rowIndex];

                for (int pixelIndex = 0; pixelIndex < image.Width; pixelIndex++)
                {
                    var pixelGroup = _groupFlag[rowIndex][pixelIndex];

                    if (pixelGroup == -1 && Image.IsPixelWhite(row[pixelIndex]))
                    {
                        MarkAndTraverse(groupCounter, rowIndex, pixelIndex);

                        groupCounter++;
                    }
                }
            }

            var smallHoles = _groupFlag.SelectMany(e => e).Where(e => e != -1).GroupBy(e => e).Select(e => e.Count() < 5).Count(e => e);

            var numberOfHoles = groupCounter - smallHoles - 1; //One hole is the background

            if (numberOfHoles > 2)
            {
                return Prediction.EmptyPrediction;
            }

            return _predictions[numberOfHoles];
        }

        private void MarkAndTraverse(int groupCounter, int rowIndex, int pixelIndex)
        {
            if (rowIndex >= Image.Height || pixelIndex >= Image.Width) return;
            if (rowIndex < 0 || pixelIndex < 0) return;
            if (!Image.IsPixelWhite(Image.PixelData[rowIndex][pixelIndex])) return;
            if (_groupFlag[rowIndex][pixelIndex] != -1) return;

            _groupFlag[rowIndex][pixelIndex] = groupCounter;
            MarkAndTraverse(groupCounter, rowIndex+1, pixelIndex);
            MarkAndTraverse(groupCounter, rowIndex-1, pixelIndex);
            MarkAndTraverse(groupCounter, rowIndex, pixelIndex+1);
            MarkAndTraverse(groupCounter, rowIndex, pixelIndex-1);
            MarkAndTraverse(groupCounter, rowIndex+1, pixelIndex+1);
            MarkAndTraverse(groupCounter, rowIndex-1, pixelIndex-1);
            MarkAndTraverse(groupCounter, rowIndex+1, pixelIndex-1);
            MarkAndTraverse(groupCounter, rowIndex-1, pixelIndex+1);
        }
        
        private void FillGroupFlag(Image image)
        {
            _groupFlag = new int[image.PixelData.Length][];

            for (int rowIndex = 0; rowIndex < image.PixelData.Length; rowIndex++)
            {
                _groupFlag[rowIndex] = new int[image.PixelData[rowIndex].Length];
                Array.Fill(_groupFlag[rowIndex], -1);
            }
        }

        private int WhatGroupIsPixelConnectedTo(Image image, int row, int pixel)
        {
            if (row != 0 && Image.IsPixelWhite(image.PixelData[row - 1][pixel]))
            {
                return _groupFlag[row - 1][pixel];
            }

            if (pixel != 0 && Image.IsPixelWhite(image.PixelData[row][pixel - 1]))
            {
                return _groupFlag[row][pixel - 1];
            }

            if (row != (image.PixelData.Length - 1) && Image.IsPixelWhite(image.PixelData[row + 1][pixel]))
            {
                return _groupFlag[row + 1][pixel];
            }

            if (pixel != (image.PixelData[row].Length - 1) && Image.IsPixelWhite(image.PixelData[row][pixel + 1]))
            {
                return _groupFlag[row][pixel + 1];
            }

            return -1;
        }
    }
}