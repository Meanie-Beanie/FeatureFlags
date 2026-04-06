
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
    public async Task RequestAccess_ValidRequest_ReturnsAuthResponse()
    {
        // Assign
        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        var authenticationService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act
        var authResult = await authenticationService
            .RequestAccessAsync();

        // Assert
        Assert.IsType<AuthResponse>(authResult);
    }
}
