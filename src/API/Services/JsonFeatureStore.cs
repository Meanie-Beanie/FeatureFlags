
using API.Entities;
using Shared.Contracts;

namespace API.UnitTests.Services;

public class JsonFeatureStore
{
    private string _filepath;

    public JsonFeatureStore(string filepath)
    {
        _filepath = filepath;
    }

    public UserFeatures GetFeatures(string apiKey)
    {
        return new UserFeatures(apiKey, [new FeatureFlag() { Name = "" }, new FeatureFlag() { Name= ""}]);
    }
}