using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Client;
using Client.Interfaces;
using Moq;

namespace Client.UnitTests;

public class AppTests
{
    // Return codes from the app is 0 for success.
    [Fact(Skip = "Too broad test, refactor when more status codes are added or app is nearing the completion.")]
    public async Task Run_AppRunsAtStart_ReturnZero()
    {
        //// Assign
        //App app = new App();

        //// Act
        //var returnCode = await app.RunAsync();

        //// Assert
        //// If program gets here without throwing errors, we know that it has started.
        //Assert.Equal(0, returnCode);
    }


}
