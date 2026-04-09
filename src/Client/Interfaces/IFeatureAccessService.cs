using Shared.Responses;

namespace Client.Interfaces;
public interface IFeatureAccessService
{
    public Task<AuthResponse> RequestAccessAsync(string apiKey);
}
