using Client.Entities;
using Client.Interfaces;
using Client.Services;
using Client.UnitTests.TestUtils;
using Moq;
using Shared.Contracts;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Client.UnitTests.Services;

public class FeatureAccessServiceTests : AuthResponseBuilder
{
    [Fact]
    public async Task RequestAccess_ValidRequest_ReturnsFeatureAccess()
    {
        // Arrange
        List<FeatureFlag> features = FeatureFlagBuilder.CreateFeatures();
        AuthResponse authResponse = CreateAuthResponse(true, HttpStatusCode.OK, null, features);

        string apiKeyFake = "test-key";

        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(authResponse);

        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act
        var result = await featureAccessService
            .RequestAccessAsync(apiKeyFake);

        // Assert
        Assert.NotNull(result.Features);
        Assert.True(result.IsAuthorized);
        Assert.Equal(features.Count, result.Features.Count);
        Assert.True(features.All(x => result.Features.Contains(x.Name)));
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(apiKeyFake), Times.Once());
    }

    [Fact]
    public async Task RequestAccess_InvalidAPIKeyGiven_ReturnsNoAccessWithErrorMessage()
    {
        // Arrange
        AuthResponse authResponse = CreateAuthResponse(false, HttpStatusCode.Unauthorized, "Invalid API key.", features: new());
        string apiKeyFake = "test-key";

        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(authResponse);

        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act
        var result = await featureAccessService
            .RequestAccessAsync(apiKeyFake);

        // Assert
        Assert.False(result.IsAuthorized);
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(apiKeyFake), Times.Once());
        Assert.False(string.IsNullOrEmpty(result.ErrorMessage)); // Not finalized
    }

    [Fact]
    public void HasAccess_UserIsNotAuthorized_ThrowsInvalidOperationError()
    {
        // Arrange
        string testFeatureName = "Test1-Feature";
        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => featureAccessService.HasFeature(testFeatureName));
    }

    [Fact]
    public async Task HasAccess_EnabledFeatureIsFound_ReturnsTrue()
    {
        // Arrange
        string testFeatureName = "Test1-Feature";
        List<FeatureFlag> features = new() { new() { Name = testFeatureName } };
        AuthResponse authResponse = CreateAuthResponse(true, HttpStatusCode.OK, null, features);

        string apiKeyFake = "test-key";

        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(authResponse);

        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);


        // Act
        await featureAccessService.RequestAccessAsync(apiKeyFake); //Authorizes the user and sets up the enabled features
        var result = featureAccessService.HasFeature(testFeatureName);

        // Assert
        Assert.True(result);
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(apiKeyFake), Times.Once());
    }

    [Fact]
    public async Task HasAccess_NoEnabledFeatureFound_ReturnsFalse()
    {
        // Arrange
        string NonExistingFeatureName = "Non-existing-Feature";
        List<FeatureFlag> features = new(); // Empty featurelist
        AuthResponse authResponse = AuthResponseBuilder.CreateAuthResponse(true, HttpStatusCode.OK, null, features);

        string apiKeyFake = "test-key";

        var featureFlagServiceMock = new Mock<IFeatureFlagService>();
        featureFlagServiceMock.Setup(x => x.GetFeatureFlags(It.IsAny<string>()))
                .ReturnsAsync(authResponse);

        var featureAccessService = new FeatureAccessService(featureFlagServiceMock.Object);


        // Act
        await featureAccessService.RequestAccessAsync(apiKeyFake); //Authorizes the user and sets up the enabled features
        var result = featureAccessService.HasFeature(NonExistingFeatureName);

        // Assert
        Assert.False(result);
        featureFlagServiceMock.Verify(x => x.GetFeatureFlags(apiKeyFake), Times.Once());
    }
}