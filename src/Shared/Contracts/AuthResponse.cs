using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Responses;

public sealed class AuthResponse
{
    public bool IsAuthorized { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string? ErrorMessage { get; set; }

    public List<FeatureFlag> Features { get; set; } = new();
}
