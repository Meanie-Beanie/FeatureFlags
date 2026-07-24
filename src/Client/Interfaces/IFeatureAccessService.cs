using Client.Entities;
using Shared.Contracts;
using Shared.Responses;

namespace Client.Interfaces;
public interface IFeatureAccessService
{
    public bool IsUserAuthorized { get; }

    public string? ApiKey { get; }

    public Task<FeatureAccess> RequestAvailableServicesAsync(string apiKey);

    public bool HasFeature(string featureName);

    public IReadOnlyList<string> EnabledFeatures { get; }
}
