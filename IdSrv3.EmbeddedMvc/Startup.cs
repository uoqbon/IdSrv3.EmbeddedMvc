using IdSrv3.EmbeddedMvc.IdentityServer;
using IdSrv3.EmbeddedMvc.Infrastructure;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web.Helpers;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Configuration;

namespace IdSrv3.EmbeddedMvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // IdentityServer is configured in the startup class. 
            // Here we provide information about the clients, users, scopes, 
            // the signing certificate and some other configuration options. 
            // In production you should load the signing certificate from the 
            // Windows certificate store or some other secured source.

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),

                    Factory = InMemoryFactory.Create(
                        users: Users.Get(),
                        clients: Clients.Get(),
                        //scopes: StandardScopes.All
                        scopes: Scopes.Get() 
                        ),

                    // Add Google authentication middleware
                    AuthenticationOptions = new Thinktecture.IdentityServer.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders
                    }
                });
            });

            // Configures the cookie middleware

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            // Point the OpenID Connect middleware to the 
            // embedded version of IdentityServer

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://localhost:44305/identity",

                ClientId = "mvc",
                RedirectUri = "https://localhost:44305/",
                ResponseType = "id_token",

                // By default the OIDC middleware asks for two scopes: openid and profile
                // this is why IdentityServer includes the subject and name claims.
                // We will add a request to the roles scope
                Scope = "openid profile roles",                

                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false,

                // The process of turning raw incoming claims into application specific 
                // claims is called claims transformation. During this process you take 
                // the incoming claims, decide which claims you want to keep and maybe need to contact 
                // additional data stores to retrieve more claims that are required by the application.

                // The OIDC middleware has a notification that you can use
                // to do claims transformation - the resulting claims will be stored in the cookie

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        // we want to keep first name, last name, subject and roles
                        var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        var roles = id.FindAll(Constants.ClaimTypes.Role);

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        nid.AddClaim(givenName);
                        nid.AddClaim(familyName);
                        nid.AddClaim(sub);
                        nid.AddClaims(roles);

                        // add some other app specific claim
                        nid.AddClaim(new Claim("app_specific", "some data"));
                                                
                        // Send the initial identity token back that the client received 
                        // during the authentication process
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);
                    },

                    // Attach the id_token when the user logs out and we make the roundtrip to IdentityServer
                    RedirectToIdentityProvider = async n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token").Value;
                            n.ProtocolMessage.IdTokenHint = idTokenHint;
                        }
                    }
                }                
            });

            // Wire up the authorization manager into the OWIN pipeline
            app.UseResourceAuthorization(new AuthorizationManager());

            // Long claim names come from Microsoft's JWT handler 
            // trying to map some claim types to .NET's ClaimTypes class types.
            // We can turn off this behavior with the following line of code.

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();            
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "831748436374-m1fkk9fkvhvkdeh2beolp523dt8r8qk1.apps.googleusercontent.com",
                ClientSecret = "Pvz6WURh3xAwzsIGTmXXLZXI",
                
            });
        }
    }
}
