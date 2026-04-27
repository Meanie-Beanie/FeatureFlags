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
}
