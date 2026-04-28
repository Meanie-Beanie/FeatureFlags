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

namespace Client.UnitTests.Features;
public class FeedbackFeatureTests
{
    [Fact]
    public void CanUse_FeatureIsEnabled_ReturnsTrue()
    {
        var featureAccessMock = new Mock<IFeatureAccessService>();
        featureAccessMock.Setup(x => x.IsUserAuthorized).Returns(true);
        featureAccessMock.Setup(x => x.HasFeature(It.IsAny<string>())).Returns(true);

        var sut = new FeedbackFeature(featureAccessMock.Object);

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

        var sut = new FeedbackFeature(featureAccessMock.Object);

        Assert.False(sut.CanUse);
    }
}
