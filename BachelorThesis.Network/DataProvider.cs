using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace BachelorThesis.Network
{
    public static class DataProvider
    {
        public const string MniskPath = "D:\\BachelorThesis\\MNISK\\Extracted";

        public static Dictionary<double[], double[]> LoadTrainingPictures()
        {
            var testData = new DirectoryInfo(Path.Combine(MniskPath, "testSample"));

            var data = new Dictionary<double[], double[]>();
            foreach (var numberFile in testData.EnumerateFiles())
            {
                Bitmap img = new Bitmap(numberFile.FullName);

                var imageList = new double[img.Width * img.Height];

                for (int y = 0; y < img.Width; y++)
                {
                    for (int x = 0; x < img.Height; x++)
                    {
                        var brightness = img.GetPixel(x, y).GetBrightness();

                        //Increased aliasing to help the neural net
                        if (brightness > 0.1)
                        {
                            imageList[y * 28 + x] = img.GetPixel(x, y).GetBrightness();
                        }
                        else
                        {
                            imageList[y * 28 + x] = 0;
                        }
                    }
                }

                var nullExpectedResult = new double[] { 0 };

                data.Add(nullExpectedResult, imageList);
            }

            return data;
        }

        public static Dictionary<double[], double[]> LoadTestPictures()
        {
            var numberFolders = new DirectoryInfo(Path.Combine(MniskPath, "trainingSample", "trainingSample"));

            var data = new Dictionary<double[], double[]>();
            foreach (var numberDirectory in numberFolders.EnumerateDirectories())
            {
                var character = int.Parse(numberDirectory.Name);

                foreach (var numberFile in numberDirectory.EnumerateFiles())
                {
                    Bitmap img = new Bitmap(numberFile.FullName);

                    var imageList = new double[img.Width * img.Height];

                    for (int y = 0; y < img.Width; y++)
                    {
                        for (int x = 0; x < img.Height; x++)
                        {
                            var brightness = img.GetPixel(x, y).GetBrightness();

                            //Increased aliasing to help the neural net
                            if (brightness > 0.1)
                            {
                                imageList[y * 28 + x] = img.GetPixel(x, y).GetBrightness();
                            }
                            else
                            {
                                imageList[y * 28 + x] = 0;
                            }
                        }
                    }

                    var nullExpectedResult = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    nullExpectedResult[character] = 1;

                    data.Add(nullExpectedResult, imageList);
                }
            }

            return data;
        }
    }
}