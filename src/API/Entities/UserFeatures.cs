using Shared.Contracts;

namespace API.Entities;

public class UserFeatures
{
    public string ApiKey { get; init; }
    public List<FeatureFlag> Features { get; set; }

    public UserFeatures(string apiKey, List<FeatureFlag> features)
    {
        ApiKey = apiKey;
        Features = features;
    }
}
