using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Routes;

// Routing file for API routes.

/*
We will have attribute routing for API routes so we will use things like:
"/{id}" as pure string as the Attribute routing will handle 'injecting' the routing parameter. It is also not capitalized as parameters are not.

Read more: https://learn.microsoft.com/en-us/aspnet/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2

** Important**

These routes are for our upcoming API project. Later on, we'll have to create a client version that can take these same paths but adjust them accordingly.
Reasoning is that httpclient will use connection strings as parameter but attribute binding requires differently built strings.
*/
public static class ApiRoutes
{

    // All gated features will be underneath api/features/{feature_name]. Idea is simple, these are specific features only available if the they have been unlocked for the user.
    public static class Features
    {
        public const string Base = "api/features";

        public const string Feedback = Base + "/feedback";
    }
}
