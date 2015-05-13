using System.Collections.Generic;
using System.Security.Claims;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Services.InMemory;

namespace IdSrv3.EmbeddedMvc.IdentityServer
{
    public static class Users
    {
        // This will add some users to IdentityServer. You can retrieve 
        // user information from any data store and IdentityServer provides
        // out of the box support for ASP.NET Identity and MembershipReboot

        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
        {
            new InMemoryUser
            {
                Username = "bob",
                Password = "secret",
                Subject = "1",

                Claims = new[]
                {
                    new Claim(Constants.ClaimTypes.GivenName, "Bob"),
                    new Claim(Constants.ClaimTypes.FamilyName, "Smith"),
                    new Claim(Constants.ClaimTypes.Role, "Geek"),
                    new Claim(Constants.ClaimTypes.Role, "Foo")
                }
            }
        };
        }
    }
}
