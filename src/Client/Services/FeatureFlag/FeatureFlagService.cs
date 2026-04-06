using Client.Interfaces;
using Shared.Responses;

namespace Client.Services.FeatureFlag;

public class FeatureFlagService : IFeatureFlagService
{
    public Task<AuthResponse> GetFeatureFlags(string apiKey)
    {
        throw new NotImplementedException();
    }
}