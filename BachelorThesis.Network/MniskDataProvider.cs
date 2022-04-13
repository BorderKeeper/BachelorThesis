using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BachelorThesis.Network.Entities;
using MySql.Data.MySqlClient;

namespace BachelorThesis.Network
{
    public static class MniskDataProvider
    {
        private const string MniskPath = "D:\\BachelorThesis\\MNISK";

        private const double BrightnessCutoff = 0.1;

        public static IEnumerable<DataRow> LoadTrainingPictures()
        {
            return LoadPictures("trainingData");
        }

        public static IEnumerable<DataRow> LoadTestPictures()
        {
            return LoadPictures("testData");
        }
        private static IEnumerable<DataRow> LoadPictures(string folder)
        {
            //var dal = new SqlDataProvider();

            var numberFolders = new DirectoryInfo(Path.Combine(MniskPath, "testData"));

            var data = new List<DataRow>();
            foreach (var numberDirectory in numberFolders.EnumerateDirectories())
            {
                var character = int.Parse(numberDirectory.Name);

                foreach (var numberFile in numberDirectory.EnumerateFiles())
                {
                    Bitmap img = new Bitmap(numberFile.FullName);

                    var imageList = new double[img.Width * img.Height];

                    //dal.Update($"INSERT INTO images (filename, digit) VALUES (\"{numberFile.Name}\", {character})");

                    for (int y = 0; y < img.Width; y++)
                    {
                        for (int x = 0; x < img.Height; x++)
                        {
                            var brightness = img.GetPixel(x, y).GetBrightness();

                            //Increased aliasing to help the neural net
                            if (brightness > BrightnessCutoff)
                            {
                                imageList[y * 28 + x] = img.GetPixel(x, y).GetBrightness();
                            }
                            else
                            {
                                imageList[y * 28 + x] = 0;
                            }
                        }
                    }

                    var nullExpectedResult = new double[10];
                    nullExpectedResult[character] = 1;

                    data.Add(new DataRow { Character = character, ImageName = numberFile.Name, Output = nullExpectedResult, Input = imageList });
                }
            }

            return data;
        }
    }
}