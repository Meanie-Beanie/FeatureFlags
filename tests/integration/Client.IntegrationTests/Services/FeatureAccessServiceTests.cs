using Client.Interfaces;
using Client.Services;
using Client.UnitTests.TestUtils;
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
    public async Task RequestAvailableServicesAsync_RequestingFeaturesFromApi_ReturnsFeatures()
    {
        // Arrange
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

        FeatureFlagService featureFlagService = new(client);
        FeatureAccessService SUT = new FeatureAccessService(featureFlagService);

        // Act
        var result = await SUT
            .RequestAvailableServicesAsync(testApiKey);

        //// Assert
        Assert.NotNull(result.Features);
        Assert.True(result.IsAuthorized);
        Assert.Equal(features.Count, result.Features.Count);
        Assert.True(features.All(x => result.Features.Contains(x.Name)));
        mockHttp.VerifyNoOutstandingExpectation();
    }
}
