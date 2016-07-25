using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Logger;
using GTI619_Lab5.Models.Admin;
using GTI619_Lab5.Utils;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Admin"), RequireHttps]
    public class AdminController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(AdminController));

        // GET: Admin
        public ActionResult Index()
        {
            if (TempData["message"] != null)
                ViewBag.Message = TempData["message"];

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

        public ActionResult EditUserRoles(int id)
        {
            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(id);
                if (user == null) return RedirectToAction("Index");

                var model = new EditUserRolesModel
                {
                    UserId = id,
                    Username = user.Username,
                    AvailableRoles = context.Roles.ToDictionary(k => k.Id, v => v.RoleName),
                    SelectedRoleIds = user.Roles.Select(x => x.Id).ToList()
                };
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditUserRoles(int id, EditUserRolesModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }

            if (id != model.UserId)
            {
                return RedirectToAction("EditUserRoles", new {id});
            }

            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        var user = context.Users.Find(model.UserId);
                        var selectedRoles = context.Roles.Where(r => model.SelectedRoleIds.Contains(r.Id)).ToList();

                        var userRoles = user.Roles.ToList();

                        foreach (var role in userRoles)
                        {
                            if (!selectedRoles.Contains(role))
                                user.Roles.Remove(role);
                        }

                        foreach (var selectedRole in selectedRoles)
                        {
                            if(!user.Roles.Any(x=>x.Id.Equals(selectedRole.Id)))
                                user.Roles.Add(selectedRole);
                        }

                        context.SaveChanges();
                        s_logger.Info(string.Format("Roles of user {0}({1}) were changed by user {2}({3})",
                            user.Username, user.Id, adminUser.Username, adminUser.Id));

                        // il faudrait probablement envoyer le password au user par courriel
                        TempData["message"] = "Roles were changed.";

                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }

        public ActionResult ResetUserPassword(int id)
        {
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
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }

            if (id != model.UserId)
            {
                return RedirectToAction("ResetUserPassword", new { id });
            }

            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        var options = context.AdminOptions.Single();
                        var user = context.Users.Find(model.UserId);

                        // ajouter le password dans le history
                        var history = new PasswordHistory
                        {
                            DateChanged = DateTime.Now,
                            PasswordSalt = user.PasswordSalt,
                            PasswordHash = user.PasswordHash,
                            UserId = model.UserId,
                            HashingVersion = user.HashingVersion
                        };
                        context.PasswordHistories.Add(history);

                        var password = Membership.GeneratePassword(options.MinPasswordLength, options.IsSpecialCharacterRequired ? 1 : 0);
                        var newSalt = Guid.NewGuid().ToString();
                        user.PasswordHash = HashingUtil.SaltAndHash(password, newSalt);
                        user.PasswordSalt = newSalt;
                        user.MustChangePasswordAtNextLogon = true;
                        user.DefaultPasswordValidUntil = DateTime.Now.AddHours(1);
                        user.IsLocked = false;
                        user.TimeoutEndDate = null;
                        user.HashingVersion = HashingUtil.Version;

                        s_logger.Info(string.Format("Password of user {0}({1}) was reset by user {2}({3})",
                            user.Username, user.Id, adminUser.Username, adminUser.Id));

                        // il faudrait probablement envoyer le password au user par courriel
                        TempData["message"] = string.Format("User password was changed to: {0}", password);

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

                        s_logger.Info(string.Format("Security options were changed by user {0}({1})",
                            adminUser.Username, adminUser.Id));

                        TempData["message"] = "Options changed!";

                        return RedirectToAction("Index", "Account");
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

        [HttpPost, ValidateAntiForgeryToken]
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
                        var options = context.AdminOptions.Single();
                        var password = Membership.GeneratePassword(options.MinPasswordLength, options.IsSpecialCharacterRequired ? 1 : 0);
                        var salt = Guid.NewGuid().ToString();
                        var user = new User
                        {
                            Username = model.Username,
                            Email = model.Email,
                            PasswordHash = HashingUtil.SaltAndHash(password, salt),
                            PasswordSalt = salt,
                            MustChangePasswordAtNextLogon = true,
                            DefaultPasswordValidUntil = DateTime.Now.AddHours(1),
                            HashingVersion = HashingUtil.Version,
                            GridCardSeed = GridCardUtil.GenerateSeed()
                        };
                        context.Users.Add(user);
                        context.SaveChanges();

                        s_logger.Info(string.Format("User {0}({1}) was created by user {2}({3})",
                            user.Username, user.Id, adminUser.Username, adminUser.Id));

                        // il faudrait probablement envoyer le password au user par courriel
                        TempData["message"] = string.Format("User was created with the password: {0}", password);

                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }
        public ActionResult DeleteUser(int id)
        {
            if (SessionManager.GetLoggedInUserId() == id)
            {
                return RedirectToAction("Index");
            }

            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(id);
                if (user == null) return RedirectToAction("Index");

                var model = new DeleteUserModel
                {
                    UserId = id,
                    Username = user.Username
                };
                return View(model);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int id, DeleteUserModel model)
        {
            if (SessionManager.GetLoggedInUserId() == id)
            {
                return RedirectToAction("Index");
            }

            s_logger.Info(string.Format("Deleting user {0}", id));
            if (!ModelState.IsValid)
            {
                model.AdminPassword = string.Empty;
                return View(model);
            }

            if (id != model.UserId)
            {
                return RedirectToAction("DeleteUser", new { id });
            }

            using (var context = new DatabaseEntities())
            {
                var adminUser = context.Users.Find(SessionManager.GetLoggedInUserId());

                if (adminUser != null)
                {
                    if (adminUser.PasswordHash.Equals(HashingUtil.SaltAndHash(model.AdminPassword, adminUser.PasswordSalt)))
                    {
                        var user = context.Users.Find(model.UserId);

                        foreach (var passHistory in context.PasswordHistories.Where(x=>x.UserId == user.Id))
                        {
                            context.Entry(passHistory).State = EntityState.Deleted;
                        }
                        foreach (var loginAttempt in context.LoginAttempts.Where(x => x.UserId == user.Id))
                        {
                            loginAttempt.UserId = null;
                        }
                        foreach (var role in user.Roles)
                        {
                            role.Users.Remove(user);
                        }
                        context.Entry(user).State = EntityState.Deleted;

                        context.SaveChanges();

                        s_logger.Info(string.Format("User {0}({1}) was deleted by user {2}({3})",
                            user.Username, user.Id, adminUser.Username, adminUser.Id));

                        TempData["message"] = string.Format("User {0} was deleted.", user.Username);

                        return RedirectToAction("Index");
                    }
                }
            }

            ModelState.AddModelError("", "Admin Password is not valid.");

            model.AdminPassword = string.Empty;
            return View(model);
        }

        public ActionResult ShowGridCard(int id)
        {
            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(id);
                if (user == null) return RedirectToAction("Index");
                var gridCard = GridCardUtil.GenerateGrid(user.GridCardSeed);
                return View(new ShowGridCardModel {GridCard = gridCard, Username = user.Username});
            }
        }
    }
}