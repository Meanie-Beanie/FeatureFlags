using Client.Interfaces;
using Shared.Responses;
using Shared.Routes;
using System.Net;
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
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("Api key is required.", nameof(apiKey));

        // We are using one HttpClient without DI (singleton) so we do not change the base properties of the httpclient
        // therefore we create HttpRequestMessage -class to wrap our specific needs and send it instead.
        using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Features.Base);
        requestMessage.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        using var response = await _client.SendAsync(requestMessage);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return new() { IsAuthorized = false, StatusCode = response.StatusCode, ErrorMessage = "User is unauthorized."};

        var content = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException("Api response content body is empty.");

        try
        {
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(content);

            if (authResponse == null)
                throw new InvalidOperationException("Api response body is null.");

            // We'll throw an error if code is not 2xx. Why not sooner?
            // We want to handle unauthorized -situation but if it is Bad Request or other error, throw.
            response.EnsureSuccessStatusCode();

            return authResponse;
        }

        catch (JsonException  ex)
        {
                return new() {
                    IsAuthorized = false,
                    StatusCode = response.StatusCode,
                    ErrorMessage = ex.Message
                };
        }
    }
}