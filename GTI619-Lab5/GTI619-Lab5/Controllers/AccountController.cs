using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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

            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public ActionResult Login()
        {
            if (SessionManager.IsUserLoggedIn()) return RedirectToAction("Index");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
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
                try
                {
                    var loginAttempt = new LoginAttempt
                    {
                        Date = DateTime.Now,
                        ClientUserAgent = Request.UserAgent,
                        ClientIpAddress = Request.UserHostAddress
                    };
                    context.LoginAttempts.Add(loginAttempt);

                    if (context.IsMaxLoginAttemptReachedForIp(loginAttempt.ClientIpAddress))
                    {
                        ModelState.AddModelError("", "Maximum number of unsuccessful login attempt reached.");
                        model.Password = string.Empty;
                        return View(model);
                    }

                    var user = context.Users.FirstOrDefault(x => x.Username.Equals(model.Username));

                    if (user != null)
                    {
                        loginAttempt.UserId = user.Id;

                        if (user.DefaultPasswordValidUntil.HasValue && DateTime.Now > user.DefaultPasswordValidUntil.Value)
                        {
                            ModelState.AddModelError("", "Your password is no longer valid. Please contact an administrator to reset your password.");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.IsLocked)
                        {
                            ModelState.AddModelError("", "Account is locked. Please contact an administrator to unlock the account.");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.TimeoutEndDate.HasValue && DateTime.Now < user.TimeoutEndDate)
                        {
                            ModelState.AddModelError("", "Account is timed out");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (context.IsMaxLoginAttemptReachedForUserId(user.Id))
                        {
                            ModelState.AddModelError("", "Maximum number of unsuccessful login attempt reached.");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.Password, user.PasswordSalt)))
                        {
                            loginAttempt.IsSuccessful = true;

                            if (user.HasToChangePassword())
                            {
                                TempData["userId"] = user.Id;
                                return RedirectToAction("ChangePassword");
                            }

                            SessionManager.SetLoggedInUser(user);

                            return RedirectToAction("Index");
                        }
                    }
                }
                finally
                {
                    context.SaveChanges();
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

        public ActionResult ChangePassword()
        {
            var userId = (int?)TempData["userId"];
            if (!userId.HasValue)
            {
                if(!SessionManager.IsUserLoggedIn()) return RedirectToAction("Login");
                userId = SessionManager.GetLoggedInUserId();
            }
            return View(new ChangePasswordModel(userId.Value));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            s_logger.Debug(string.Format("Change password (userid={0})", model.UserId));
            
            if (!ModelState.IsValid)
            {
                return View(new ChangePasswordModel(model.UserId));
            }

            if (!model.NewPassword1.Equals(model.NewPassword2))
            {
                ModelState.AddModelError("", "The 2 password are not equal.");
                return View(new ChangePasswordModel(model.UserId));
            }

            if (model.OldPassword.Equals(model.NewPassword1))
            {
                ModelState.AddModelError("", "The old and new password cannot be the same.");
                return View(new ChangePasswordModel(model.UserId));
            }

            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(model.UserId);

                if (user == null)
                {
                    // user was not found, we disconnect the user that is logged in
                    SessionManager.LogoutUser();
                    return RedirectToAction("Login");
                }

                if (SessionManager.IsUserLoggedIn())
                {
                    // user tried to change the password of another user
                    if (SessionManager.GetLoggedInUserId() != user.Id)
                    {
                        ModelState.AddModelError("", "An error occured, please try again.");
                        return View(new ChangePasswordModel(SessionManager.GetLoggedInUserId()));
                    }
                }
                else
                {
                    // user is not logged in and he does not have to change password
                    // he should not be here
                    if (!user.HasToChangePassword())
                    {
                        return RedirectToAction("Login");
                    }
                }

                var options = context.AdminOptions.Single();
                var passwordHistory = context.PasswordHistories
                    .Where(x => x.UserId == model.UserId)
                    .OrderByDescending(x => x.DateChanged)
                    .Take(options.NumberOfPasswordToKeepInHistory)
                    .ToList();

                if (!PasswordValidator.Validate(model.NewPassword1, options, passwordHistory))
                {
                    ModelState.AddModelError("", string.Format("Password must be {0} characters long.", options.MinPasswordLength));
                    if(options.IsLowerCaseCharacterRequired)
                        ModelState.AddModelError("", "Password must contain at least one lower case character");
                    if (options.IsUpperCaseCharacterRequired)
                        ModelState.AddModelError("", "Password must contain at least one upper case character");
                    if (options.IsNumberRequired)
                        ModelState.AddModelError("", "Password must contain at least one number");
                    if (options.IsSpecialCharacterRequired)
                        ModelState.AddModelError("", "Password must contain at least one special character");
                    ModelState.AddModelError("", string.Format("Password cannot be the same as one of the {0} last.", options.NumberOfPasswordToKeepInHistory));

                    return View(new ChangePasswordModel(model.UserId));
                }

                if (user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.OldPassword, user.PasswordSalt)))
                {
                    var history = new PasswordHistory
                    {
                        DateChanged = DateTime.Now,
                        PasswordSalt = user.PasswordSalt,
                        PasswordHash = user.PasswordHash,
                        UserId = model.UserId
                    };
                    context.PasswordHistories.Add(history);

                    var newSalt = Guid.NewGuid().ToString();

                    user.PasswordHash = HashingUtil.SaltAndHash(model.NewPassword1, newSalt);
                    user.PasswordSalt = newSalt;
                    user.MustChangePasswordAtNextLogon = false;
                    user.PasswordExpirationDate = DateTime.Now.AddDays(options.PasswordExpirationDurationInDays);
                    user.DefaultPasswordValidUntil = null;

                    context.SaveChanges();
                    
                    SessionManager.SetLoggedInUser(user);
                    TempData["message"] = "Password changed!";

                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("", "The old password is not valid.");
            return View(new ChangePasswordModel(model.UserId));
        }
    }
}