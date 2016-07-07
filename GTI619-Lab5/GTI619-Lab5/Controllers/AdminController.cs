using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Models.Admin;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Admin"), RequireHttps]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var model = new IndexModel
            {
                UserDictionary = new Dictionary<Guid, string> { { Guid.NewGuid(), "Username 1"}}
            };
            return View(model);
        }

        public ActionResult ResetUserPassword(Guid id)
        {
            var model = new ResetUserPasswordModel
            {
                UserId = id,
                Username = "load this from the db"
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetUserPassword(Guid id, ResetUserPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }
            
            // ValidatePassword()
            ModelState.AddModelError("", "Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
            //PasswordGenerator.GenerateNewFor(id);
            return RedirectToAction("Index");
        }

        public ActionResult GlobalSecuritySettings()
        {
            var model = new GlobalSecuritySettingsModel
            {
                MaxLoginAttempt = 5,
                MinPasswordLength = 12,
                PasswordShouldHaveLowerCase = true,
                PasswordShouldHaveNumbers = true,
                PasswordShouldHaveSpecialChars = true,
                PasswordShouldHaveUpperCase = true
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult GlobalSecuritySettings(GlobalSecuritySettingsModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }
            return View();
        }
    }
}