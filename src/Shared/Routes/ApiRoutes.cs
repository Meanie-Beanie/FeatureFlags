using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Routes;

/*
Routing file for API routes.

Note:

We will have attribute routing for API routes so we will use things like:
"/{id}" as pure string as the Attribute routing will handle 'injecting' the routing parameter. It is also not capitalized as parameters are not.

Read more: https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
*/
public static class ApiRoutes
{
    public static class Features
    {
        public const string Base = "api/features";
        // public static string GetFeature = Base + "/{Id]";

    }
}
