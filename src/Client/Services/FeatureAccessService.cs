using Client.Entities;
using Client.Interfaces;
using Shared.Contracts;
using Shared.Responses;

namespace Client.Services;

public sealed class FeatureAccessService : IFeatureAccessService
{

    public string? ApiKey { get; private set; }

    private readonly IFeatureFlagService _featureFlagService;
    private List<string> _enabledFeatures = new();

    // Due to it being unmutable collection, we'll just expose our internal storage through it.
    public IReadOnlyList<string> EnabledFeatures => _enabledFeatures;

    public bool IsUserAuthorized { get; private set; } = false;

    public FeatureAccessService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public bool HasFeature(string featureName)
    {
        if (!IsUserAuthorized)
            throw new InvalidOperationException("User is not authorized.");

        if (string.IsNullOrWhiteSpace(featureName))
            throw new ArgumentNullException("Feature name cannot be null or empty.", nameof(featureName));

        return _enabledFeatures.Contains(featureName);
    }

    public async Task<FeatureAccess> RequestAvailableServicesAsync(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("Api key cannot be null or empty." ,nameof(apiKey));

        var response = await _featureFlagService.GetFeatureFlags(apiKey);

        if (!response.IsAuthorized)
        {
            UpdateUserFeatures(response.IsAuthorized, new());
            return new() { IsAuthorized = false, ErrorMessage = "Invalid API key." };
        }

        // Store API key in case we need to do a refetch.
        ApiKey = apiKey;

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

    //public Task<FeatureAccess> RefetchAsync()
    //{
    //    if (ApiKey is null)
    //        throw new InvalidOperationException("User has not been authorized yet.");
    //    return RequestAvailableServicesAsync(ApiKey);
    //}

    //private void UpdateApiKey(string apiKey)
    //{
    //    if (string.IsNullOrWhiteSpace(apiKey))
    //        throw new ArgumentException($"Api Key cannot be null.", nameof(apiKey));

    //    apiKey = apiKey.ToLower();

    //    RefetchAsync();
    //}

    private void UpdateUserFeatures(bool isAuthorized, List<string> enabledFeatures)
    {
        IsUserAuthorized = isAuthorized;
        _enabledFeatures = enabledFeatures;
    }
}
