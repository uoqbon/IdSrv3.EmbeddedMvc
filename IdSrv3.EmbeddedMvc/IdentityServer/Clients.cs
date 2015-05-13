using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Models;

namespace IdSrv3.EmbeddedMvc.IdentityServer
{
    public static class Clients
    {
        // IdentityServer needs some information about the clients 
        // it is going to support, this can be simply supplied using a Client object

        public static IEnumerable<Client> Get()
        {
            return new[]
        {
            new Client 
            {
                Enabled = true,
                ClientName = "MVC Client",
                ClientId = "mvc",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    "https://localhost:44305/"
                },
                // Register a valid URL to return to after the logout procedure is complete.
                // The client has to prove its identity to the logout endpoint to make 
                // sure we redirect to the right URL (see Startup.cs)
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost:44305/"
                }
            },

            // To call the WebAPI, we can either use 
            // client credentials or delegate users identity            
            // register a new client for the MVC app
            // IdentityServer only allows one flow per client
            // we need to create a new client for the service to 
            // service communication
            new Client
            {
                Enabled = true,
                ClientName = "MVC Client (service communication)",
                ClientId = "mvc_service",
                ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret("secret".Sha256())
                },

                Flow = Flows.ClientCredentials
            }
        };
        }
    }
}
