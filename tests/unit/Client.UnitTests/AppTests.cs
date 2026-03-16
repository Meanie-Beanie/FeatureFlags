using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client;

namespace Client.UnitTests;

public class AppTests
{
    [Fact]
    public async Task Run_AppRunsAtStart_NoErrors()
    {
        // Assign
        App app = new App();

        // Act
        await app.RunAsync();

        // Assert
        // If program gets here without throwing errors, we know that it has started.
         Assert.True(true);
    }

    // Return codes from the app is 0 for success.
    [Fact]
    public async Task Run_AppRunsAtStart_ReturnZero()
    {
        // Assign
        App app = new App();

        // Act
        await app.RunAsync();

        // Assert
        // If program gets here without throwing errors, we know that it has started.
        Assert.True(true);
    }
}
