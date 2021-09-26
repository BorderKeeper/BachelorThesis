using System;
using System.Collections.Generic;
using System.Linq;

namespace BachelorThesis.OCR
{
    public class HoleDetector
    {
        private readonly Dictionary<int, Prediction> Predictions = new()
        {
            { 0, Prediction.RecalculatedPrediction(new [] { false, true, true, true, false, true, false, true, false, false })},
            { 1, Prediction.RecalculatedPrediction(new [] { true, false, false, false, true, false, true, false, false, true })},
            { 2, Prediction.RecalculatedPrediction(new [] { false, false, false, false, false, false, false, false, true, false })}
        };

        private int[][] GroupFlag;

        public Image Image;

        public Prediction GetPredictionFromImage(Image image)
        {
            Image = image;

            int groupCounter = 0;

            FillGroupFlag(image);

            for (int rowIndex = 0; rowIndex < image.Height; rowIndex++)
            {
                var row = image.PixelData[rowIndex];

                for (int pixelIndex = 0; pixelIndex < image.Width; pixelIndex++)
                {
                    var pixelGroup = GroupFlag[rowIndex][pixelIndex];

                    if (pixelGroup == -1 && Image.IsPixelWhite(row[pixelIndex]))
                    {
                        MarkAndTraverse(groupCounter, rowIndex, pixelIndex);

                        groupCounter++;
                    }
                }
            }

            Console.WriteLine("--------------------------------------------");
            foreach (int[] row in GroupFlag)
            {
                Console.WriteLine();
                foreach (int i in row)
                {
                    if (i == -1) Console.Write("X ");
                    else Console.Write($"{i} ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");

            var smallHoles = GroupFlag.SelectMany(e => e).Where(e => e != -1).GroupBy(e => e).Select(e => e.Count() < 5).Count(e => e);

            var numberOfHoles = groupCounter - smallHoles - 1; //One hole is the background

            if (numberOfHoles > 2)
            {
                return Prediction.EmptyPrediction;
            }

            return Predictions[numberOfHoles];
        }

        private void MarkAndTraverse(int groupCounter, int rowIndex, int pixelIndex)
        {
            if (rowIndex >= Image.Height || pixelIndex >= Image.Width) return;
            if (rowIndex < 0 || pixelIndex < 0) return;
            if (!Image.IsPixelWhite(Image.PixelData[rowIndex][pixelIndex])) return;
            if (GroupFlag[rowIndex][pixelIndex] != -1) return;

            GroupFlag[rowIndex][pixelIndex] = groupCounter;
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
            GroupFlag = new int[image.PixelData.Length][];

            for (int rowIndex = 0; rowIndex < image.PixelData.Length; rowIndex++)
            {
                GroupFlag[rowIndex] = new int[image.PixelData[rowIndex].Length];
                Array.Fill(GroupFlag[rowIndex], -1);
            }
        }

        private int WhatGroupIsPixelConnectedTo(Image image, int row, int pixel)
        {
            if (row != 0 && Image.IsPixelWhite(image.PixelData[row - 1][pixel]))
            {
                return GroupFlag[row - 1][pixel];
            }

            if (pixel != 0 && Image.IsPixelWhite(image.PixelData[row][pixel - 1]))
            {
                return GroupFlag[row][pixel - 1];
            }

            if (row != (image.PixelData.Length - 1) && Image.IsPixelWhite(image.PixelData[row + 1][pixel]))
            {
                return GroupFlag[row + 1][pixel];
            }

            if (pixel != (image.PixelData[row].Length - 1) && Image.IsPixelWhite(image.PixelData[row][pixel + 1]))
            {
                return GroupFlag[row][pixel + 1];
            }

            return -1;
        }
    }
}