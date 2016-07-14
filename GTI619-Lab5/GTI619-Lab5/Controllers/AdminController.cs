using System.Linq;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Logger;
using GTI619_Lab5.Models.Admin;
using GTI619_Lab5.Utils;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Admin")/*, RequireHttps*/]
    public class AdminController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(AdminController));

        // GET: Admin
        public ActionResult Index()
        {
            using (var context = new DatabaseEntities())
            {
                var userId = SessionManager.GetLoggedInUserId();
                var users = context.Users.Where(user => user.Id != userId).ToList();
                var model = new IndexModel
                {
                    UserDictionary = users.ToDictionary(k => k.Id, v => v.Username)
                };
                return View(model);
            }
        }

        public ActionResult ResetUserPassword(int id)
        {
            s_logger.Info(string.Format("Reseting password of user {0}", id));
            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(id);
                if (user == null) return RedirectToAction("Index");

                var model = new ResetUserPasswordModel
                {
                    UserId = id,
                    Username = user.Username
                };
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResetUserPassword(int id, ResetUserPasswordModel model)
        {
            s_logger.Info(string.Format("Reseting password of user {0}", id));
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }

            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        // todo reset the password of the user
                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }

        public ActionResult GlobalSecuritySettings()
        {
            using (var context = new DatabaseEntities())
            {
                var options = context.AdminOptions.Single();

                var model = new GlobalSecuritySettingsModel
                {
                    MinPasswordLength = options.MinPasswordLength,
                    PasswordShouldHaveUpperCase = options.IsUpperCaseCharacterRequired,
                    PasswordShouldHaveLowerCase = options.IsLowerCaseCharacterRequired,
                    PasswordShouldHaveNumbers = options.IsNumberRequired,
                    PasswordShouldHaveSpecialChars = options.IsSpecialCharacterRequired,
                    MaxLoginAttempt = options.MaxLoginAttempt,
                    TimeoutAfterMaxLoginReachedInMinutes = options.TimeoutAfterMaxLoginReachedInMinutes,
                    PasswordExpirationDurationInDays = options.PasswordExpirationDurationInDays,
                    NumberOfPasswordToKeepInHistory = options.NumberOfPasswordToKeepInHistory
                };
                return View(model);
            }
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


            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        var options = context.AdminOptions.Single();
                        
                        options.MinPasswordLength = model.MinPasswordLength;
                        options.IsUpperCaseCharacterRequired = model.PasswordShouldHaveUpperCase;
                        options.IsLowerCaseCharacterRequired = model.PasswordShouldHaveLowerCase;
                        options.IsNumberRequired = model.PasswordShouldHaveNumbers;
                        options.IsSpecialCharacterRequired = model.PasswordShouldHaveSpecialChars;
                        options.MaxLoginAttempt = model.MaxLoginAttempt;
                        options.TimeoutAfterMaxLoginReachedInMinutes = model.TimeoutAfterMaxLoginReachedInMinutes;
                        options.PasswordExpirationDurationInDays = model.PasswordExpirationDurationInDays;
                        options.NumberOfPasswordToKeepInHistory = model.NumberOfPasswordToKeepInHistory;

                        context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
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

            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        // todo create the new user
                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }
    }
}