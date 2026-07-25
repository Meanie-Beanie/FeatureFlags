using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UnitTests.TestUtils;
public class FeatureFlagBuilder
{
    public static List<FeatureFlag> CreateFeatures()
    {
        List<FeatureFlag> features = new()
        {
            new FeatureFlag { Name = "Device1" },
            new FeatureFlag { Name = "Device2" }
        };

        return features;
    }
}
