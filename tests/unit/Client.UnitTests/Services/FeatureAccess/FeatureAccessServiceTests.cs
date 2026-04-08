
using Moq;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Client.Services;
using Client.Interfaces;
using Client.Services.FeatureAccess;


namespace Client.UnitTests.Services.FeatureAccess;

public class FeatureAccessServiceTests
{
    [Fact]
    public async Task RequestAccess_ValidRequest_ReturnsAuthResultFromFeatureFlagService()
    {
        // Assign
        var expected = new AuthResponse();
        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(expected);

        var authenticationService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act
        var authResult = await authenticationService
            .RequestAccessAsync();

        // Assert
        Assert.Same(expected, authResult);
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(It.IsAny<string>()), Times.Once());
    }
}
