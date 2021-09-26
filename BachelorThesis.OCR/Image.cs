namespace BachelorThesis.OCR
{
    public class Image
    {
        public Prediction CorrectResult { get; set; }

        public double[][] PixelData { get; set; }

        public Image(double[][] pixelData, Prediction correctResult)
        {
            PixelData = pixelData;
            CorrectResult = correctResult;
        }

        public static bool IsPixelWhite(double color)
        {
            return color < 0.1;
        }

        public int Height => PixelData.Length;

        public int Width => PixelData[0].Length;
    }
}