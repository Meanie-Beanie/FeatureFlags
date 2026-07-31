using Client.Interfaces;
using Client.Services;
using Shared;
using Shared.Contracts;
using Shared.Features;
using Shared.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Client.Features;
public sealed class FeedbackFeature : FeatureBase
{
    private IFeatureAccessService _featureAccessService;
    private HttpClient _httpClient;
    private IUserInterface _UI;

    public FeedbackFeature(IFeatureAccessService featureAccessService, IUserInterface userInterface, HttpClient httpClient) : base(featureAccessService, FeatureKeys.SendFeedback)
    {
        _featureAccessService = featureAccessService;
        _UI = userInterface;
        _httpClient = httpClient;
    }

    public override async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _UI.ShowMessage("Give your feedback:");
        var feedback = _UI.GetInput();

        if (string.IsNullOrWhiteSpace(feedback))
        {
            _UI.ShowMessage("Feedback cannot be empty.");
            return false;
        }

        try
        {
            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Features.Feedback)
            {
                Content = JsonContent.Create(feedback)
            };
            requestMessage.Headers.Add(Constants.Api.ApiKeyHeader, _featureAccessService.ApiKey);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _UI.ShowMessage($"Sending feedback failed ({(int)response.StatusCode}).");
                return false;
            }

            _UI.ShowMessage("Feedback succesfully sent.");
            return true;
        }

        catch (HttpRequestException)
        {
            _UI.ShowMessage("Unable to send feedback");
            return false;
        }
    }
}
