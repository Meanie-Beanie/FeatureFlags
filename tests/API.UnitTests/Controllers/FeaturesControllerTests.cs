using API.Controllers;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
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

        FeaturesController controller = new FeaturesController();

        var result = controller.Get(apiKey);
    }
}
