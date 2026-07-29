using API.Entities;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.UnitTests.Services;


// Note: File name is not changed each test, can cause issues when running async etc..

public class JsonFeatureFlagStoreTests : IDisposable
{
    private readonly string _filepath = Path.Combine(Path.GetTempPath(), "test-feature-flag.json");
    /*
     * We create the 
    */
    private JsonFeatureStore CreateJsonFeatureStore(string apiKey, List<FeatureFlag> features)
    {
        UserFeatures userFeatures = new(apiKey, features);
        var json = JsonSerializer.Serialize(userFeatures);

        File.WriteAllText(_filepath, json);

        return new JsonFeatureStore(_filepath);
    }

    /*
     * TempPath actually creates has a file after the test runs, so we need to remove it.
    */ 
    public void Dispose()
    {
        if (File.Exists(_filepath))
            File.Delete(_filepath);
    }

    [Fact]
    public void GetFeatures_GetsFeaturesForTheApiKey_ReturnsFeatures()
    {
        string apiKey = "123-test-key";
        FeatureFlag feature1 = new() { Name = "feature1" };
        FeatureFlag feature2 = new() { Name = "feature2" };

        var SUT = CreateJsonFeatureStore(apiKey, [feature1, feature2]);
        var result = SUT.GetFeatures(apiKey);

        Assert.Equal(apiKey, result.ApiKey);
        Assert.Distinct(result.Features);
        Assert.True(result.Features.Count > 0);
    }
}
