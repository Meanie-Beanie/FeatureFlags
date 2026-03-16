using Shared.Responses;

namespace Client.Interfaces;

public interface IFeatureFlagService
{
    public Task<AuthResponse> GetFeatureFlags(string apiKey);
}