using Client.Interfaces;
using Shared.Responses;

namespace Client.Services;

public class FeatureFlagService : IFeatureFlagService
{
    public Task<AuthResponse> GetFeatureFlags(string apiKey)
    {
        throw new NotImplementedException();
    }
}