using Client.Entities;
using Client.Features;
using Client.Interfaces;
using Client.Services;
using Client.UnitTests.TestUtils;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Client.UnitTests.Features;
public class FeedbackFeatureTests
{
    [Fact]
    public async Task ExecuteAsync_FeedbackIsSentSuccessfully_ReturnsTrue()
    {
        string feedback = "test feedback";
        string apiKey = "test-api-key";
        HttpResponseMessage response = new(HttpStatusCode.OK);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();
        var featureAccessMock = new Mock<IFeatureAccessService>();

        featureAccessMock.Setup(x => x.ApiKey).Returns(apiKey);
        uiMock.Setup(x => x.GetInput()).Returns(feedback);
        httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);
        var result = await sut.ExecuteAsync();

        Assert.True(result);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    public async Task ExecuteAsync_FeedbackCannotBeSent_ReturnsFalse(HttpStatusCode statusCode)
    {
        string feedback = "test feedback";
        string apiKey = "test-api-key";
        HttpResponseMessage response = new(statusCode);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();
        var featureAccessMock = new Mock<IFeatureAccessService>();

        featureAccessMock.Setup(x => x.ApiKey).Returns(apiKey);
        uiMock.Setup(x => x.GetInput()).Returns(feedback);
        httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);
        var result = await sut.ExecuteAsync();

        Assert.False(result);
    }

    [Fact]
    public async Task ExecuteAsync_FeedbackIsSent_ReturnsTrue()
    {
        string feedback = "test feedback";
        string apiKey = "test-api-key";
        HttpResponseMessage response = new(HttpStatusCode.OK);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();
        var featureAccessMock = new Mock<IFeatureAccessService>();

        featureAccessMock.Setup(x => x.ApiKey).Returns(apiKey);
        uiMock.Setup(x => x.GetInput()).Returns(feedback);
        httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);
        var result = await sut.ExecuteAsync();

        Assert.True(result);
    }

    [Theory]
    [InlineData(null)] // Null
    [InlineData("")] // Empty
    [InlineData("              ")] //Whitespaces
    public async Task ExecuteAsync_FeedbackIsEmpty_InformsUserAndReturnsFalse(string feedback)
    {
        // This notifies the user, hard-coded atm.
        string notificationMessage = "Feedback cannot be empty.";
        string apiKey = "test-api-key";
        HttpResponseMessage response = new(HttpStatusCode.OK);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();
        var featureAccessMock = new Mock<IFeatureAccessService>();

        featureAccessMock.Setup(x => x.ApiKey).Returns(apiKey);
        uiMock.Setup(x => x.GetInput()).Returns(feedback);
        httpClientMock.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);
        var result = await sut.ExecuteAsync();

        Assert.False(result);
        uiMock.Verify(x => x.ShowMessage(notificationMessage), Times.Once);
    }

    [Fact]
    public void CanUse_FeatureIsEnabled_ReturnsTrue()
    {
        var featureAccessMock = new Mock<IFeatureAccessService>();
        featureAccessMock.Setup(x => x.IsUserAuthorized).Returns(true);
        featureAccessMock.Setup(x => x.HasFeature(It.IsAny<string>())).Returns(true);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);

        Assert.True(sut.CanUse);
    }

    // Both properties need to be true for the feature to be activated, so we test the cases where one of them or both are not.
    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void CanUse_FeatureIsDisabled_ReturnsFalse(bool isAuthorized, bool HasFeature)
    {
        var featureAccessMock = new Mock<IFeatureAccessService>();
        featureAccessMock.Setup(x => x.IsUserAuthorized).Returns(isAuthorized);
        featureAccessMock.Setup(x => x.HasFeature(It.IsAny<string>())).Returns(HasFeature);

        var httpClientMock = new Mock<HttpClient>();
        var uiMock = new Mock<IUserInterface>();

        var sut = new FeedbackFeature(featureAccessMock.Object, uiMock.Object, httpClientMock.Object);

        Assert.False(sut.CanUse);
    }
}
