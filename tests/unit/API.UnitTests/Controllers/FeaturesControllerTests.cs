using API.Controllers;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Contracts;
using Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.UnitTests.Services;


// Note: File name is not changed each test, can cause issues when running async etc..

public class FeaturesControllerTests
{
    [Fact]
    public void Get_CorrectApiKeyAsHeader_ReturnsOk()
    {
        string apiKey = "123-test-key";

        FeatureFlag feature1 = new() { Name = "feature1" };
        FeatureFlag feature2 = new() { Name = "feature2" };
        UserFeatures userFeatures = new UserFeatures(apiKey, [feature1, feature2]);

        var mockFeedbackService = new Mock<IFeedbackService>();
        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.GetFeatures(apiKey)).Returns(userFeatures);

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.Get(apiKey);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var okResult = Assert.IsType<OkObjectResult>(result);
        var body = Assert.IsType<AuthResponse>(okResult.Value);
        Assert.Contains(feature1, body.Features);
        Assert.Contains(feature2, body.Features);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData(null)]
    public void Get_InvalidApiKeyProvided_ReturnsUnauthorizedResponse(string apiKey)
    {
        FeatureFlag feature1 = new() { Name = "feature1" };
        FeatureFlag feature2 = new() { Name = "feature2" };
        UserFeatures userFeatures = new UserFeatures(apiKey, [feature1, feature2]);

        var mockFeedbackService = new Mock<IFeedbackService>();
        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.GetFeatures(apiKey)).Returns(userFeatures);

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.Get(apiKey);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var badRequestResult = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void Get_ApiKeyProvidedIsNotFound_ReturnsUnauthorized()
    {
        string apiKey = "123-test-key";

        var mockFeedbackService = new Mock<IFeedbackService>();
        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.GetFeatures(apiKey)).Throws(new KeyNotFoundException());

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.Get(apiKey);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var badRequestResult = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    // We throw just exception, since we know every exception inherits from it so they are covered.
    [Fact]
    public void Get_ExceptionThrownInFeatureStore_ReturnsBadRequest()
    {
        string apiKey = "123-test-key";

        var mockFeedbackService = new Mock<IFeedbackService>();
        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.GetFeatures(apiKey)).Throws(new Exception());

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.Get(apiKey);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void PostFeedback_FeedbackIsSent_ReturnsOk()
    {
        string apiKey = "123-test-key";
        string message = "Test";

        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.HasFeature(apiKey, It.IsAny<string>())).Returns(true);

        var mockFeedbackService = new Mock<IFeedbackService>();
        mockFeedbackService.Setup(x => x.Send(message));

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.PostFeedback(apiKey, message);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void PostFeedback_ExceptionIsThrown_ReturnsBadRequest()
    {
        string apiKey = "123-test-key";
        string message = "Test";

        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.HasFeature(apiKey, It.IsAny<string>())).Returns(true); 

        var mockFeedbackService = new Mock<IFeedbackService>();
        mockFeedbackService.Setup(x => x.Send(message)).Throws(new Exception());

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.PostFeedback(apiKey, message);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var okResult = Assert.IsType<BadRequestResult>(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void PostFeedback_ApiKeyIsNotValid_ReturnsUnauthorized(string apiKey)
    {
        string message = "Test";

        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.HasFeature(apiKey, It.IsAny<string>())).Returns(false);

        var mockFeedbackService = new Mock<IFeedbackService>();
        mockFeedbackService.Setup(x => x.Send(message));

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.PostFeedback(apiKey, message);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void PostFeedback_ApiKeyIsNotFound_ReturnsUnauthorized()
    {
        string apiKey = "123-test-key";
        string message = "Test";

        var mockFeatureStore = new Mock<IFeatureStore>();
        mockFeatureStore.Setup(x => x.HasFeature(apiKey, It.IsAny<string>())).Returns(false);

        var mockFeedbackService = new Mock<IFeedbackService>();
        mockFeedbackService.Setup(x => x.Send(message));

        FeaturesController controller = new FeaturesController(mockFeatureStore.Object, mockFeedbackService.Object);

        var result = controller.PostFeedback(apiKey, message);

        // We need the OkObjectResult type to access values, we'll assert it at the same time.
        var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
    }
}

