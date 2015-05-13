using IdSrv3.EmbeddedMvc.Infrastructure;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace IdSrv3.EmbeddedMvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //[Auth(Roles = "Geek")] // Roles not applicable with Google
        [Auth]
        public ActionResult About()
        {
            return View((User as ClaimsPrincipal).Claims);
        }

        /// <summary>
        /// We are using a new authorization infrastructure that separates
        /// the authorization logic into an authorization manager that knows 
        /// about actions, resources and who is allowed to do which operation 
        /// in this application. (See Startup.cs)
        /// We annotate the Contact action on the Home controller 
        /// with an attribute that expresses that executing that 
        /// action is going to Read the ContactDetails resource.
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize("Read", "ContactDetails")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// The HandleForbidden filter will redirect to a specified view 
        /// whenever a 403 got emitted - by default it will look for a view called 
        /// 'Forbidden'
        /// </summary>
        /// <returns></returns>
        [ResourceAuthorize("Write", "ContactDetails")]        
        [HandleForbidden]
        public ActionResult UpdateContact()
        {
            ViewBag.Message = "Upate your contact details!";

            return View();
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}