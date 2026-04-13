using Client.Interfaces;
using Shared.Responses;

namespace Client.Services;

public class FeatureFlagService : IFeatureFlagService
{

    private readonly HttpClient _client = new();

    public FeatureFlagService(HttpClient client)
    {
        _client = client;
    }

    public Task<AuthResponse> GetFeatureFlags(string apiKey)
    {
        throw new NotImplementedException();
    }
}