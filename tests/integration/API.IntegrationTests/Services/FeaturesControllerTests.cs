using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Shared;
using Shared.Contracts;
using Shared.Features;
using Shared.Responses;
using Shared.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.IntegrationTests.Services;
public class FeaturesControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly string _filepath = Path.Combine(Path.GetTempPath(), "FeaturesController-Json-Store.json");

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _httpClient;

    public FeaturesControllerTests(WebApplicationFactory<Program> factory)
    {

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IFeatureStore>();
                services.AddTransient<IFeatureStore>(_ => new JsonFeatureStore(_filepath));
            });
        });

        _httpClient = _factory.CreateClient();
    }

    /*
     * We could create this as helper.
    */ 
    private void CreateJsonFile(string apiKey, List<FeatureFlag> features)
    {
        UserFeatures userFeatures = new(apiKey, features);

        // Since it contains multiple listings for users features, we have to have it in list format or JSON will not be correct.
        var json = JsonSerializer.Serialize(new List<UserFeatures>() { userFeatures });

        File.WriteAllText(_filepath, json);
    }

    /*
     * TempPath actually creates has a file after the test runs, so we need to remove it.
    */
    public void Dispose()
    {
        if (File.Exists(_filepath))
            File.Delete(_filepath);
    }

    [Fact]
    public async Task Get_CorrectApiKeyAsHeader_ReturnsOkwithAuthResponse()
    {
        string apiKey = "123-test-key";
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Features.Base);
        request.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var features = await response.Content.ReadFromJsonAsync<UserFeatures>();
        Assert.NotNull(features);
        var feature = Assert.Single(features.Features);
        Assert.Equal(feature1.Name, feature.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task Get_InvalidApiKeyAsHeader_ReturnsUnauthorizedWithAuthResponse(string apiKey)
    {
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Features.Base);
        request.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        var raw = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var authResp = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResp);
        Assert.False(authResp.IsAuthorized);
        Assert.Empty(authResp.Features);
    }

    [Fact]
    public async Task Get_ApiKeyNotFound_ReturnsNotAuthorizedWithAuthResponse()
    {
        string apiKey = "123-test-key";
        string nonExistingKey = "I-am-not-real";
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Features.Base);
        request.Headers.Add(Constants.Api.ApiKeyHeader, nonExistingKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var authResp = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResp);
        Assert.False(authResp.IsAuthorized);
        Assert.Empty(authResp.Features);
    }

    [Fact]
    public async Task SendFeedback_ValidRequest_ReturnsOk()
    {
        string apiKey = "123-test-key";
        string message = "Test Message";
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Features.Feedback);
        request.Content = JsonContent.Create(message);
        request.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task SendFeedback_InvalidApiKey_ReturnsUnauthorizedWithAuthResponse(string apiKey)
    {
        string message = "test message";
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Features.Feedback);
        request.Content = JsonContent.Create(message);
        request.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var authResp = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResp);
        Assert.False(authResp.IsAuthorized);
        Assert.Empty(authResp.Features);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null)]
    public async Task SendFeedback_InvalidMessageAttached_ReturnsBadRequestWithAuthResponse(string message)
    {
        string apiKey = "123-test-key";
        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);

        var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Features.Feedback);
        request.Content = JsonContent.Create(message);
        request.Headers.Add(Constants.Api.ApiKeyHeader, apiKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var authResp = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResp);
        Assert.False(authResp.IsAuthorized);
        Assert.Empty(authResp.Features);
    }

    [Fact]
    public async Task SendFeedback_ApiKeyNotFound_ReturnsUnauthorizedWithAuthResponse()
    {
        string apiKey = "123-test-key";
        string nonExistingKey = "5567-not-existing";
        string message = "test message";

        FeatureFlag feature1 = new() { Name = FeatureKeys.SendFeedback };
        CreateJsonFile(apiKey, [feature1]);


        var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Features.Feedback);
        request.Content = JsonContent.Create(message);
        request.Headers.Add(Constants.Api.ApiKeyHeader, nonExistingKey);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var authResp = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResp);
        Assert.False(authResp.IsAuthorized);
        Assert.Empty(authResp.Features);
    }
}
