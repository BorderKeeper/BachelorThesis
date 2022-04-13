using System.Collections.Generic;
using System.IO;
using BachelorThesis.Network.Entities.AnalysisEntities;
using Newtonsoft.Json;

namespace BachelorThesis.Network
{
    public static class LocalStorageDataProvider
    {
        private const string FeaturesSavePath = "D:\\BachelorThesis\\features";
        private const string FeaturesFilename = "features.json";

        public static List<OcrFeature> GetFeatures()
        {
            var filePath = Path.Combine(FeaturesSavePath, FeaturesFilename);

            var rawFeatures = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<List<OcrFeature>>(rawFeatures);
        }

        public static void StoreFeatures(List<OcrFeature> features)
        {
            var filePath = Path.Combine(FeaturesSavePath, FeaturesFilename);

            var rawFeatures = JsonConvert.SerializeObject(features);

            File.WriteAllText(filePath, rawFeatures);
        }
    }
}