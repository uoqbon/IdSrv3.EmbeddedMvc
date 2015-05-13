using System.Web.Mvc;

namespace IdSrv3.EmbeddedMvc.Infrastructure
{
    /// <summary>
    /// Customized authorization attribute:
    /// The original MVC Authorize attribute will set the action's result 
    /// to 401 unauthorized when the user is authenticated, but not in one of the roles. 
    /// That 401 result triggers a redirect to authenticate with IdentityServer, which 
    /// authenticates and then redirects the user back, and then a redirect loop begins. 
    /// This behavior can be overcome by overriding the Authorize attribute's HandleUnauthorizedRequest 
    /// method as follows, and then use the customized authorization attribute instead of what comes with MVC.
    /// </summary>
    public class AuthAttribute : AuthorizeAttribute
    {       
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // 403 we know who you are, but you haven't been granted access
                filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                // 401 who are you? go login and then try again
                filterContext.Result = new HttpUnauthorizedResult(); ;
            }
        }
    }
}
