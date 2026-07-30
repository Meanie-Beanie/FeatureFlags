using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Features;
using Shared.Routes;
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureStore _featureStore;
    private readonly IFeedbackService _feedbackService;

    public FeaturesController(IFeatureStore featureStore, IFeedbackService feedbackService)
    {
        _featureStore = featureStore;
        this._feedbackService = feedbackService;
    }

        // Note: we add a dash in front to ensure it is considered an absolute path. Otherwise it would inherit the  controller route in front of it.
    [HttpGet(Name = $"/{ApiRoutes.Features.Base}")]
    public IActionResult Get([FromHeader(Name = Shared.Constants.Api.ApiKeyHeader)] string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return Unauthorized("Provided Api key cannot be empty or null.");

        try
        {

            var result = _featureStore.GetFeatures(apiKey);

            return Ok(result);
        }

        catch (KeyNotFoundException e)
        {
            return Unauthorized("Provided Api key services were not found.");
        }

        catch (Exception e)
        {
            return BadRequest();
        }
    }

    // Note: we add a dash in front to ensure it is considered an absolute path. Otherwise it would inherit the  controller route in front of it.
    [HttpPost(Name = $"/{ApiRoutes.Features.Feedback}")]
    public IActionResult PostFeedback([FromHeader(Name = Shared.Constants.Api.ApiKeyHeader)] string apiKey, string message)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return Unauthorized("Provided Api key cannot be empty or null.");

        if (string.IsNullOrEmpty(message))
            return BadRequest("Message cannot be empty");

        try
        {

            if (_featureStore.HasFeature(apiKey, FeatureKeys.SendFeedback))
            {
                _feedbackService.Send(message);
                return Ok();
            }

            else
                return Unauthorized();
        }

        catch (KeyNotFoundException e)
        {
            return Unauthorized("Provided Api key services were not found.");
        }

        catch (Exception e)
        {
            return BadRequest();
        }
    }
}
