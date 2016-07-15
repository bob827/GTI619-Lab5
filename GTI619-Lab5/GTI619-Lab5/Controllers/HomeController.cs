using System.Web.Mvc;
using GTI619_Lab5.Logger;
using GTI619_Lab5.Models.Home;
using GTI619_Lab5.Utils;

namespace GTI619_Lab5.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(HomeController));
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            if(!SessionManager.IsUserLoggedIn())return Content("");

            var model = new MenuModel();

            using (var context = new DatabaseEntities())
            {
                var userId = SessionManager.GetLoggedInUserId();

                if (context.IsUserAdmin(userId))
                {
                    model.IsAdmin = true;
                }
                else
                {
                    model.Roles = context.GetUserRoles(userId);
                }

                return PartialView(model);

            }
        }
    }
}