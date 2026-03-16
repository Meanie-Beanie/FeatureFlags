using Client.Interfaces;
using Client.Services;
using Client.UnitTests.TestUtils;
using Moq;
using RichardSzalay.MockHttp;
using Shared.Contracts;
using Shared.Responses;
using Shared.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Client.IntegrationTests.Services;


public class FeatureAccessServiceTests
{
    [Fact]
    public async Task RequestAvailableServicesAsync_RequestingFeaturesFromApi_ReturnsAuthResponseWithFeatures()
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
                .Respond("application/json", authResponseJson);

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        FeatureFlagService featureFlagService = new(client);
        FeatureAccessService SUT = new FeatureAccessService(featureFlagService);

        var result = await SUT
            .RequestAvailableServicesAsync(testApiKey);
        Assert.NotNull(result.Features);
        Assert.True(result.IsAuthorized);
        Assert.Equal(features.Count, result.Features.Count);
        Assert.True(features.All(x => result.Features.Contains(x.Name)));
        mockHttp.VerifyNoOutstandingExpectation();
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public async Task RequestAccess_EmptyApiKeyProvided_ThrowsArgumentException(string apiKey)
    {
        var baseUrl = new Uri("https://www.test.com");
        var mockHttp = new Mock<HttpClient>();

        FeatureFlagService featureFlagService = new(mockHttp.Object);
        FeatureAccessService SUT = new FeatureAccessService(featureFlagService);

        await Assert.ThrowsAsync<ArgumentException>(() => SUT.RequestAvailableServicesAsync(apiKey));
    }

    [Fact]
    public async Task RequestAccess_NoServicesAssociatedWithApiKey_ReturnsNoAccessWithErrorMessage()
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;
        var testApiKey = "test-Api-Key";
        var errorMessage = "Invalid API key.";

        var authResponse = AuthResponseBuilder.CreateJsonAuthResponse(false, HttpStatusCode.Unauthorized, errorMessage, features: new());

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.Expect(HttpMethod.Get, featureUrl)
                .WithHeaders(Constants.Api.ApiKeyHeader, testApiKey)
                .Respond("application/json", authResponse); // Respond with JSON

        var client = mockHttp.ToHttpClient();
        client.BaseAddress = baseUrl;

        var featureFlagService = new FeatureFlagService(client);
        var SUT = new FeatureAccessService(featureFlagService);

        var result = await SUT
            .RequestAvailableServicesAsync(testApiKey);

        Assert.False(result.IsAuthorized);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.True(result.Features is null || result.Features.Count == 0);
    }
}
