using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Logger;
using GTI619_Lab5.Models.Account;
using GTI619_Lab5.Utils;

namespace GTI619_Lab5.Controllers
{
    //[RequireHttps]
    public class AccountController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(AccountController));

        // GET: Account
        public ActionResult Index()
        {
            if (!SessionManager.IsUserLoggedIn()) return RedirectToAction("Login");
            
            return View();
        }

        public ActionResult Login()
        {
            if (SessionManager.IsUserLoggedIn()) return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            s_logger.Debug(string.Format("Login attempt with user {0}", model.Username));
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                return View(model);
            }

            // try login
            using (var context = new DatabaseEntities())
            {
                var user = context.Users.FirstOrDefault(x => x.Username.Equals(model.Username));

                if (user != null)
                {
                    if (user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.Password, user.PasswordSalt)))
                    {
                        SessionManager.SetLoggedInUser(user);

                        return RedirectToAction("Index");
                    }
                }
            }
            
            ModelState.AddModelError("", "Invalid username or password.");
            model.Password = string.Empty;
            return View(model);
        }

        public ActionResult Logout()
        {
            SessionManager.LogoutUser();
            return RedirectToAction("Login");
        }
    }
}