using Owin;
using System.Web.Http;
using Thinktecture.IdentityServer.AccessTokenValidation;

namespace IdSrv3.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // we want to secure our API using IdentityServer. We need to:
            // 1. accept only tokens issued by IdentityServer
            // 2. accept only tokens that are issued for our API
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://localhost:44305/identity",
                RequiredScopes = new[] { "sampleApi" }
            });

            // web api configuration
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
        }
    }
}
