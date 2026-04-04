using Shared.Responses;

namespace Client.Interfaces;

public interface IFeatureFlagService
{
    public AuthResponse Authenticate();
}