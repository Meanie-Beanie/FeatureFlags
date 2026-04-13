using Client.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UnitTests.Services;
public class FeatureFlagServiceTests
{
    [Fact]
    public void GetFeatureFlags_GetsValidAPIResponse_ReturnsAuthResponse()
    {
        var testApiKey = "testuri";

        var httpClientMock = new Mock<HttpClient>();
        httpClientMock.Setup(x => x.GetAsync(testApiKey));

        var featureFlagService = new FeatureFlagService(httpClientMock.Object);
        var result = featureFlagService.GetFeatureFlags(testApiKey);
    }
}
