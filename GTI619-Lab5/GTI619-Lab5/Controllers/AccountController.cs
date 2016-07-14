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
                        
                        if (context.IsMaxLoginAttemptReachedForUserId(user.Id))
                        {
                            ModelState.AddModelError("", "Maximum number of unsuccessful login attempt reached.");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.TimeoutEndDate.HasValue && DateTime.Now < user.TimeoutEndDate)
                        {
                            ModelState.AddModelError("", "Account is timed out");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.IsLocked)
                        {
                            ModelState.AddModelError("", "Account is locked. Please contact an administrator to unlock the account.");
                            model.Password = string.Empty;
                            return View(model);
                        }

                        if (user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.Password, user.PasswordSalt)))
                        {
                            loginAttempt.IsSuccessful = true;

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
    }
}