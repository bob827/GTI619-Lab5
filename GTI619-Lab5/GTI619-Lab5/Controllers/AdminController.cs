using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Logger;
using GTI619_Lab5.Models.Admin;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Admin"), RequireHttps]
    public class AdminController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(AdminController));

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
            s_logger.Info(string.Format("Reseting password of user {0}", id));
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
            s_logger.Info(string.Format("Reseting password of user {0}", id));
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }
            
            // ValidatePassword()
            ModelState.AddModelError("", "Admin Password is not valid.");

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
            s_logger.Info("Changing global security settings");
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }
            return View();
        }

        public ActionResult CreateUser()
        {
            var model = new CreateUserModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateUser(CreateUserModel model)
        {
            s_logger.Info("Creating a new user");
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }
            // ValidatePassword()
            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }
    }
}