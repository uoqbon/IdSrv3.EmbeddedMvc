using System.Linq;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace IdSrv3.EmbeddedMvc.Infrastructure
{
    public class AuthorizationManager : ResourceAuthorizationManager
    {
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            switch (context.Resource.First().Value)
            {
                case "ContactDetails":
                    return AuthorizeContactDetails(context);
                default:
                    return Nok();
            }
        }

        private Task<bool> AuthorizeContactDetails(ResourceAuthorizationContext context)
        {
            switch (context.Action.First().Value)
            {
                case "Read":
                    return Eval(true);
                    //return Eval(context.Principal.HasClaim("role", "Geek")); // Roles not applicable to Google
                case "Write":
                    return Eval(context.Principal.HasClaim("role", "Operator"));
                default:
                    return Nok();
            }
        }
    }
}
