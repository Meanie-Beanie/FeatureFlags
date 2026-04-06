using Client.Interfaces;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.FeatureAccess;

public class FeatureAccessService : IFeatureAccessService
{
    private readonly IFeatureFlagService _featureFlagService;

    public FeatureAccessService(IFeatureFlagService featureFlagService)
    {
        _featureFlagService = featureFlagService;
    }

    public async Task<AuthResponse> RequestAccessAsync()
    {
        return new AuthResponse();
    }
}
