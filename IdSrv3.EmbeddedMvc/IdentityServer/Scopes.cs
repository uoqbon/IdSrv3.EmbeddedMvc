using System.Collections.Generic;

using Thinktecture.IdentityServer.Core.Models;

namespace IdSrv3.EmbeddedMvc.IdentityServer
{
    public static class Scopes
    {
        // This will add some role claims to our user
        // which we will be used later on for authorization

        public static IEnumerable<Scope> Get()
        {
            // This will defines a roles scope that includes the 
            // role claim and add that to the standard scopes

            var scopes = new List<Scope>
            {
                new Scope
                {
                    
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")
                    }
                },

                // register the WebAPI by extending the scopes.
                new Scope
                {
                    Enabled = true,
                    Name = "sampleApi",
                    Description = "Access to a sample API",
                    Type = ScopeType.Resource
                }
            };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}
