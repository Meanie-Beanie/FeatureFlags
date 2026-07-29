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

        // Since it contains multiple listings for users features, we have to have it in list format or JSON will not be correct.
        var json = JsonSerializer.Serialize(new List<UserFeatures>(){ userFeatures});

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
    public void GetFeatures_CorrectApiKeyProvided_ReturnsUserFeatures()
    {
        string apiKey = "123-test-key";
        FeatureFlag feature1 = new() { Name = "feature1" };
        FeatureFlag feature2 = new() { Name = "feature2" };

        var SUT = CreateJsonFeatureStore(apiKey, [feature1, feature2]);
        var result = SUT.GetFeatures(apiKey);

        Assert.Equal(apiKey, result.ApiKey);
        Assert.True(result.Features.FirstOrDefault(x => x.Name == feature1.Name) != null); // Lazy way to ensure both are present.
        Assert.True(result.Features.FirstOrDefault(x => x.Name == feature2.Name) != null); // Lazy way to ensure both are present.
        Assert.True(result.Features.Count > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("{}")]
    public void GetFeatures_EmptyJsonFileProvided_ThrowsException(string jsonFile)
    {
        string apiKey = "123-test-key";

        File.WriteAllText(_filepath, jsonFile);

        var SUT = new JsonFeatureStore(_filepath);

        Assert.Throws<InvalidOperationException>(() => SUT.GetFeatures(apiKey));
    }

    [Fact]
    public void GetFeatures_CorrectApiKeyProvidedButUserHasNoFeatures_ReturnsUserFeaturesWithNoFeatures()
    {
        string apiKey = "123-test-key";

        var SUT = CreateJsonFeatureStore(apiKey, new());
        var result = SUT.GetFeatures(apiKey);

        Assert.Equal(apiKey, result.ApiKey);
        Assert.True(result.Features.Count == 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void GetFeatures_InvalidApiKeyProvided_ThrowArgumentException(string apiKey)
    {
        var SUT = CreateJsonFeatureStore(apiKey, new());
        Assert.Throws<ArgumentException>(() => SUT.GetFeatures(apiKey));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void GetFeatures_InvalidPathProvided_ThrowArgumentException(string path)
    {
        Assert.Throws<ArgumentException>(() => new JsonFeatureStore(path));
    }
}
