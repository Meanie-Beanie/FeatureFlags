using Client.Entities;
using Shared.Contracts;
using Shared.Responses;

namespace Client.Interfaces;
public interface IFeatureAccessService
{
    public Task<FeatureAccess> RequestAccessAsync(string apiKey);

    public bool HasFeature(string featureName);
}
