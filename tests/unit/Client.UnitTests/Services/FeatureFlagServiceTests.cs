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
    public async Task GetFeatureFlags_APIReturnsWithAuthorizedFeatures_ReturnsAuthResponse()
    {
        var baseUrl = new Uri("https://www.test.com");
        var featureUrl = baseUrl + ApiRoutes.Features.Base;
        var testApiKey = "test-Api-Key";

        var features = FeatureFlagBuilder.CreateFeatures();
        var authResponseJson = AuthResponseBuilder.CreateJsonAuthResponse(true, HttpStatusCode.OK, null, features);

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
    }
}
