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

namespace GTI619_Lab5.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(AccountController));

        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
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
            // todo on invalid login add to counter in db
            ModelState.AddModelError("", "Invalid username or password.");
            model.Password = string.Empty;
            return View(model);

            //if(userIsAdmin(userId) 
            //    return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }
    }
}