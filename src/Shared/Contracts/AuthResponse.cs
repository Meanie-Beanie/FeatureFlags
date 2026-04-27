using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.Responses;

public sealed class AuthResponse
{
    [JsonRequired]
    public bool IsAuthorized { get; set; }
    [JsonRequired]
    public HttpStatusCode StatusCode { get; set; }
    [JsonRequired]
    public string? ErrorMessage { get; set; }

    [JsonRequired]
    public List<FeatureFlag> Features { get; set; } = new();
}
