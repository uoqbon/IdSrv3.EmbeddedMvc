using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Client;

namespace IdSrv3.EmbeddedMvc.Controllers
{
    public class CallApiController : Controller
    {
        // GET: CallApi/ClientCredentials
        public async Task<ActionResult> ClientCredentials()
        {
            var response = await GetTokenAsync();
            var result = await CallApi(response.AccessToken);

            ViewBag.Json = result;
            return View("ShowApiResult");
        }

        /// <summary>
        /// Request a token for the API from IdentityServer using the client credentials
        /// </summary>
        /// <returns></returns>
        private async Task<TokenResponse> GetTokenAsync()
        {
            var client = new OAuth2Client(
                new Uri("https://localhost:44305/identity/connect/token"),
                "mvc_service",
                "secret");

            return await client.RequestClientCredentialsAsync("sampleApi");
        }

        /// <summary>
        /// Calls the identity endpoint using the requested access token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);

            var json = await client.GetStringAsync("https://localhost:44306/identity");
            return JArray.Parse(json).ToString();
        }        
    }


}
