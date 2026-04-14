using Client.Services;
using Client.UnitTests.TestUtils;
using Moq;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Client.UnitTests.Services;
public class FeatureFlagServiceTests
{
    [Fact]
    public async Task GetFeatureFlags_APIReturnsWithAuthorizedFeatures_ReturnsAuthResponse()
    {
        var testApiKey = "testuri";
        var features = FeatureFlagBuilder.CreateFeatures();
        var authResponse = AuthResponseBuilder.CreateAuthResponse(true, HttpStatusCode.OK, null, features);

        var httpClientMock = new Mock<HttpClient>();
        httpClientMock.Setup(x => x.GetAsync(testApiKey));

        var featureFlagService = new FeatureFlagService(httpClientMock.Object);
        var result = await featureFlagService.GetFeatureFlags(testApiKey);

        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.True(result.IsAuthorized);
        Assert.True(string.IsNullOrEmpty(result.ErrorMessage));
        Assert.True(features.All(x => result.Features.Contains(x)));
    }
}
