using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Routes;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FeaturesController : ControllerBase
{
    private IFeatureStore _featureStore;

    public FeaturesController(IFeatureStore featureStore)
    {
        _featureStore = featureStore;
    }

    [HttpGet(Name = ApiRoutes.Features.Feedback)]
    public IActionResult Get([FromHeader(Name = Shared.Constants.Api.ApiKeyHeader)] string apiKey)
    {
        var result = _featureStore.GetFeatures(apiKey);

        return Ok(result);
    }
}
