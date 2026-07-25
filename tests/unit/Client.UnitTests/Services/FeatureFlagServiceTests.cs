using Client.Services;
using Client.UnitTests.TestUtils;
using Moq;
using RichardSzalay.MockHttp;
using Shared.Responses;
using Shared.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.UnitTests.Services;
public class FeatureFlagServiceTests
{
    [Fact]
    public async Task GetFeatureFlags_ApiReturnsWithAuthorizedFeatures_ReturnsAuthResponse()
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;
        var testApiKey = "test-Api-Key";

        var features = FeatureFlagBuilder.CreateFeatures();
        var authResponseJson = AuthResponseBuilder.CreateJsonAuthResponse(IsAuthorized: true, statusCode: HttpStatusCode.OK,
            errorMessage: null, features: features);

        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, featureUrl)
                .WithHeaders(Constants.Api.ApiKeyHeader, testApiKey)
                .Respond("application/json", authResponseJson); // Respond with JSON

        // Inject the handler or client into your application code
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        var sut = new FeatureFlagService(client);
        var result = await sut.GetFeatureFlags(testApiKey);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.IsAuthorized);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.Equal(features.Count, result.Features.Count);
        Assert.Equal(features.Select(x => x.Name), result.Features.Select(x => x.Name));
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Theory]
    [InlineData(null)] // Null
    [InlineData("")] // Empty
    [InlineData("              ")] //Whitespaces
    public async Task GetFeatureFlags_EmptyApiKeyProvided_ThrowsArgumentException(string apiKey)
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;

        var features = FeatureFlagBuilder.CreateFeatures();
        var authResponseJson = AuthResponseBuilder.CreateJsonAuthResponse(IsAuthorized: true, statusCode: HttpStatusCode.OK,
            errorMessage: null, features: features);

        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, featureUrl)
                .WithHeaders(Constants.Api.ApiKeyHeader, apiKey)
                .Respond("application/json", authResponseJson); // Respond with JSON

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        var sut = new FeatureFlagService(client);

        await Assert.ThrowsAsync<ArgumentException>(() => sut.GetFeatureFlags(apiKey));
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Theory]
    [InlineData("null")]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("not json")]
    [InlineData("{\"isAuthorized\":true}")]
    [InlineData("[]")]
    public async Task GetFeatureFlags_InvalidResponseBody_ThrowsInvalidOperationException(string jsonBody)
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;
        var testApiKey = "test-Api-Key";

        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, featureUrl)
                .WithHeaders(Constants.Api.ApiKeyHeader, testApiKey)
                .Respond(HttpStatusCode.OK, "application/json", jsonBody);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        var sut = new FeatureFlagService(client);
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetFeatureFlags(testApiKey));
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task GetFeatureFlags_401UnauthorizedApiResponse_ReturnsAuthResponse()
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;
        var testApiKey = "test-Api-Key";
        var errorMessage = "User is unauthorized.";

        var features = FeatureFlagBuilder.CreateFeatures();
        var authResponseJson = AuthResponseBuilder.CreateJsonAuthResponse(IsAuthorized: false, HttpStatusCode.Unauthorized,
            errorMessage: errorMessage, features: new());

        var mockHttp = new MockHttpMessageHandler();

        mockHttp.Expect(HttpMethod.Get, featureUrl)
                .WithHeaders(Constants.Api.ApiKeyHeader, testApiKey)
                .Respond(HttpStatusCode.Unauthorized, "application/json", authResponseJson);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        var sut = new FeatureFlagService(client);
        var result = await sut.GetFeatureFlags(testApiKey);

        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
        Assert.False(result.IsAuthorized);
        Assert.False(string.IsNullOrEmpty(result.ErrorMessage)); // Has to have some kind of error message
        Assert.True(result.Features.Count == 0);
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
