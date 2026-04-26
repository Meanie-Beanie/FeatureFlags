using Client.Entities;
using Client.Interfaces;
using Shared.Contracts;
using Shared.Responses;

namespace Client.Services;

public class FeatureAccessService : IFeatureAccessService
{
    private readonly IFeatureFlagService _featureFlagService;

    private List<string> EnabledFeatures = new();
    public bool IsUserAuthorized { get; private set; } = false;

    public FeatureAccessService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public bool HasFeature(string featureName)
    {
        if (!IsUserAuthorized)
            throw new InvalidOperationException("User is not authorized.");

        return EnabledFeatures.Contains(featureName);
    }

    public async Task<FeatureAccess> RequestAccessAsync(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("Api key cannot be null or empty." ,nameof(apiKey));

        var response = await _featureFlagService.GetFeatureFlags(apiKey);

        if (!response.IsAuthorized)
        {
            UpdateUserFeatures(response.IsAuthorized, new());
            return new() { IsAuthorized = false, ErrorMessage = "Invalid API key." };
        }

        // Convert Feature list into simplified string list.
        var listOfFeatures = response.Features.Select(x => x.Name).ToList();

        UpdateUserFeatures(response.IsAuthorized, listOfFeatures);

        var featureAccess = new FeatureAccess()
        {
            IsAuthorized = response.IsAuthorized,
            Features = listOfFeatures
        };

        return featureAccess;
    }

    private void UpdateUserFeatures(bool isAuthorized, List<string> enabledFeatures)
    {
        IsUserAuthorized = isAuthorized;
        EnabledFeatures = enabledFeatures;
    }
}
