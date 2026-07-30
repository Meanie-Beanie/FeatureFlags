using API.Entities;

namespace API.Interfaces;

public interface IFeatureStore
{
    UserFeatures GetFeatures(string apiKey);
}
