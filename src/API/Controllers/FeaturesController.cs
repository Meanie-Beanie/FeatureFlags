using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Features;
using Shared.Responses;
using Shared.Routes;
using System.Net;
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
    [HttpGet($"/{ApiRoutes.Features.Base}", Name = ApiRoutes.Features.Base)]
    public IActionResult Get([FromHeader(Name = Shared.Constants.Api.ApiKeyHeader)] string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return Unauthorized(new AuthResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ErrorMessage = "Api key cannot be null.",
                IsAuthorized = false
            });

        try
        {
            var result = _featureStore.GetFeatures(apiKey);

            return Ok(new AuthResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsAuthorized = true,
                Features = result.Features
            });
        }

        catch (KeyNotFoundException e)
        {
            return Unauthorized(new AuthResponse { 
                StatusCode = HttpStatusCode.Unauthorized,
                ErrorMessage = "Provided Api key services were not found.",
                IsAuthorized = false
            });
        }

        catch (Exception)
        {
            return BadRequest(new AuthResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessage = "Something went wrong.",
                IsAuthorized = false
            });
        }
    }

    // Note: we add a dash in front to ensure it is considered an absolute path. Otherwise it would inherit the  controller route in front of it.
    [HttpPost($"/{ApiRoutes.Features.Feedback}", Name = ApiRoutes.Features.Feedback)]
    public IActionResult PostFeedback([FromHeader(Name = Shared.Constants.Api.ApiKeyHeader)] string? apiKey, [FromBody]string? message)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Unauthorized(new AuthResponse { 
                StatusCode = HttpStatusCode.Unauthorized,
                ErrorMessage = "Provided Api key cannot be empty or null.",
                IsAuthorized = false
            });
        }


        if (string.IsNullOrWhiteSpace(message))
        {
            return BadRequest(new AuthResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorMessage = "Feedback message cannot be null or empty.",
                IsAuthorized = false
            });
        }


        try
        {

            if (_featureStore.HasFeature(apiKey, FeatureKeys.SendFeedback))
            {
                _feedbackService.Send(message);
                return Ok();
            }

            else
            {
                return Unauthorized(new AuthResponse
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Api key has no right to send feedback.",
                    IsAuthorized = false
                });
            }
        }

        catch (KeyNotFoundException e)
        {
            return Unauthorized(new AuthResponse
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ErrorMessage = "Provided Api key services were not found.",
                IsAuthorized = false
            });
        }

        catch (Exception)
        {
            return BadRequest();
        }
    }
}
