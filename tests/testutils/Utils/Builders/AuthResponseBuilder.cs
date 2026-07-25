using Shared.Contracts;
using Shared.Responses;
using System.Net;
using System.Text.Json;

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
    public static string CreateJsonAuthResponse(bool IsAuthorized, HttpStatusCode statusCode, string? errorMessage, List<FeatureFlag> features)
    {
        return JsonSerializer.Serialize<AuthResponse>(new()
        {
            IsAuthorized = IsAuthorized,
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            Features = features ?? new List<FeatureFlag>()
        });
    }
}