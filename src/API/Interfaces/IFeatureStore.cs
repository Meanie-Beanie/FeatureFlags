using API.Entities;
using Shared.Contracts;

namespace API.Interfaces;

public interface IFeatureStore
{

    bool HasFeature(string apiKey, string featureName);

    UserFeatures GetFeatures(string apiKey);
}
