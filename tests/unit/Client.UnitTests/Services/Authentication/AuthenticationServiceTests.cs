using Client.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.UnitTests.Services.Authentication;
public class AuthenticationServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_SuccessfullyAuthenticated_ReturnsOk200()
    {
        // Assign
        var httpClientMock = new Mock<HttpClient>();
        var authenticationService = new AuthenticationService(httpClientMock);

        // Act
        var authResult = await authenticationService
            .AuthenticateAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, authResult);
    }
}
