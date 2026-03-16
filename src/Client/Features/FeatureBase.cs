using Client.Interfaces;
using Client.Services;

namespace Client.Features;

public abstract class FeatureBase : IFeature
{
    private readonly IFeatureAccessService _featureAccessService;
    private readonly string _featureName;

    protected FeatureBase(IFeatureAccessService featureAccessService, string featureName)
    {
        _featureAccessService = featureAccessService;
        _featureName = featureName;
    }

    public virtual bool CanUse
        => _featureAccessService.HasFeature(_featureName) && _featureAccessService.IsUserAuthorized;

    public abstract Task<bool> ExecuteAsync(CancellationToken cancellationToken = default);
}