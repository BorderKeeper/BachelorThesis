using System.Linq;

namespace BachelorThesis.OCR
{
    public class Image
    {
        public double[][] PixelData { get; set; }

        public Image(double[][] pixelData)
        {
            PixelData = pixelData;
        }

        public static bool IsPixelWhite(double color)
        {
            return color < 0.1;
        }

        public int Height => PixelData.Length;

        public int Width => PixelData[0].Length;
    }
}