using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Models.Account;

namespace GTI619_Lab5.Controllers
{
    [RequireHttps]
    public class AccountController : Controller
    {
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