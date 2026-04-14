using Shared.Contracts;
using Shared.Responses;
using System.Net;

namespace Client.UnitTests.TestUtils;

public class AuthResponseBuilder
{
    public static AuthResponse CreateAuthResponse(bool IsAuthorized, HttpStatusCode statusCode, string? errorMessage, List<FeatureFlag> features)
    {
        return new()
        {
            IsAuthorized = IsAuthorized,
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            Features = features ?? new List<FeatureFlag>()
        };
    }
}