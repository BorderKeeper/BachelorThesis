using BachelorThesis.Network.Entities;

namespace BachelorThesis.Network.OCR
{
    public class Image
    {
        public Prediction CorrectResult { get; set; }

        public double[][] PixelData { get; set; }

        public string Filename { get; set; }

        public Image(double[][] pixelData, Prediction correctResult, string filename)
        {
            PixelData = pixelData;
            CorrectResult = correctResult;
            Filename = filename;
        }

        public static bool IsPixelWhite(double color)
        {
            return color < 0.1;
        }

        public int Height => PixelData.Length;

        public int Width => PixelData[0].Length;
    }
}