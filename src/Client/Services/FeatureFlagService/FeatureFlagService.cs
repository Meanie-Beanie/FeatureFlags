using Client.Interfaces;
using Shared.Responses;

namespace Client.Services.FeatureFlagService;

public class FeatureFlagService : IFeatureFlagService
{
    public AuthResponse Authenticate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}