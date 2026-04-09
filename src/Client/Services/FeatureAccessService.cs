using Client.Interfaces;
using Shared.Responses;

namespace Client.Services;

public class FeatureAccessService : IFeatureAccessService
{
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureAccessService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public Task<AuthResponse> RequestAccessAsync(string apiKey)
    {
        return _featureFlagService.GetFeatureFlags(apiKey);
    }
}
