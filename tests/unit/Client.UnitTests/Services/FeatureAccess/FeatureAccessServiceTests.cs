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
using Client.Entities;


namespace Client.UnitTests.Services.FeatureAccess;

public class FeatureAccessServiceTests
{
    [Fact]
    public async Task RequestAccess_ValidRequest_ReturnsAuthResultFromFeatureFlagService()
    {
        // Assign;
        var expected = new AuthResponse();
        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(expected);

        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act
        var result = await featureAccessService
            .RequestAccessAsync(It.IsAny<string>());

        // Assert
        Assert.Same(expected, result);
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(It.IsAny<string>()), Times.Once());
    }
}