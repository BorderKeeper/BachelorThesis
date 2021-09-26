using System.Collections.Generic;

namespace BachelorThesis.OCR.Features
{
    public static class FeatureFactory
    {
        public static IEnumerable<IFeature> GetFeatures()
        {
            return new List<IFeature>
            {
                new HoleDetector(),
                new HoughTransform(),
                new RightAndLeftEntries(),
                new CrossInTheCenter(),
                new Intersections()
            };
        }
    }
}