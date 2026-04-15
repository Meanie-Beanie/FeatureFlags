using Client.Interfaces;
using Shared.Responses;
using Shared.Routes;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Services;

public class FeatureFlagService : IFeatureFlagService
{
    private readonly HttpClient _client = new();

    public FeatureFlagService(HttpClient client)
    {
        _client = client;
    }

    public async Task<AuthResponse> GetFeatureFlags(string apiKey)
    {
        // We are using one HttpClient without DI (singleton) so we do not change the base properties of the httpclient
        // therefore we create HttpRequestMessage -class to wrap our specific needs and send it instead.
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Features.Base);
        requestMessage.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        var response = await _client.SendAsync(requestMessage);

        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content);

        return authResponse;
    }
}