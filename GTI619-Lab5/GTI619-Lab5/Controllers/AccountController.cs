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
    [RequireHttps]
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
            var r = new Random();
            var model = new LoginModel
            {
                GridCardCol = r.Next( GridCardUtil.GridSize),
                GridCardRow = r.Next( GridCardUtil.GridSize)
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            var r = new Random();
            if (!ModelState.IsValid)
            {
                // il y a des erreurs dans le modele
                model.Password = string.Empty;
                model.GridCardValue = 0;
                model.GridCardCol = r.Next(GridCardUtil.GridSize);
                model.GridCardRow = r.Next(GridCardUtil.GridSize);
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

                    // validation si adresse ip a atteint la limite de tentative
                    if (context.IsMaxLoginAttemptReachedForIp(loginAttempt.ClientIpAddress))
                    {
                        s_logger.Warn(string.Format("Max login attempts was reached by IP {0}", loginAttempt.ClientIpAddress));
                        ModelState.AddModelError("", "Maximum number of unsuccessful login attempt reached.");
                        model.Password = string.Empty;
                        model.GridCardValue = 0;
                        model.GridCardCol = r.Next(GridCardUtil.GridSize);
                        model.GridCardRow = r.Next(GridCardUtil.GridSize);
                        return View(model);
                    }

                    // va chercher le user dans BD
                    var user = context.Users.FirstOrDefault(x => x.Username.Equals(model.Username));

                    if (user != null)
                    {
                        loginAttempt.UserId = user.Id;

                        // validation si le mot de passe est toujours valide
                        if (user.DefaultPasswordValidUntil.HasValue && DateTime.Now > user.DefaultPasswordValidUntil.Value)
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): default password has expired", user.Username, user.Id));
                            ModelState.AddModelError("", "Your password is no longer valid. Please contact an administrator to reset your password.");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }

                        // validation si le compte est verouille
                        if (user.IsLocked)
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): account is locked", user.Username, user.Id));
                            ModelState.AddModelError("", "Account is locked. Please contact an administrator to unlock the account.");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }

                        // validation si l'utilisateur a recu un timeout
                        if (user.TimeoutEndDate.HasValue && DateTime.Now < user.TimeoutEndDate)
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): accout is timedout", user.Username, user.Id));
                            ModelState.AddModelError("", "Account is timed out");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }

                        // validation si l'utilisateur a atteint la limite de tentative
                        if (context.IsMaxLoginAttemptReachedForUserId(user.Id))
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): the maximum unsuccessful attempt reached", user.Username, user.Id));
                            ModelState.AddModelError("", "Maximum number of unsuccessful login attempt reached.");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }

                        // validation si le mot de passe correspond a celui de la BD
                        if (!user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.Password, user.PasswordSalt)))
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): invalid password", user.Username, user.Id));
                            ModelState.AddModelError("", "Invalid username, password or grid card value.");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }
                        
                        // validation de la valeur de la grid gard
                        if (!GridCardUtil.IsValid(model.GridCardValue, user.GridCardSeed, model.GridCardCol, model.GridCardRow))
                        {
                            s_logger.Info(string.Format("Login refused for user {0}({1}): grid card value not valid", user.Username, user.Id));
                            ModelState.AddModelError("", "Invalid username, password or grid card value.");
                            model.Password = string.Empty;
                            model.GridCardValue = 0;
                            model.GridCardCol = r.Next(GridCardUtil.GridSize);
                            model.GridCardRow = r.Next(GridCardUtil.GridSize);
                            return View(model);
                        }

                        loginAttempt.IsSuccessful = true;

                        // validation si l'utilisateur doit changer de mot de passe
                        if (user.HasToChangePassword())
                        {
                            s_logger.Info(string.Format("Login valid {0}({1}): user has to change password", user.Username, user.Id));
                            TempData["userId"] = user.Id;
                            return RedirectToAction("ChangePassword");
                        }

                        // l'utilisateur est authentifie, on ajoute dans la session
                        SessionManager.SetLoggedInUser(user);

                        s_logger.Info(string.Format("Login valid {0}({1})", user.Username, user.Id));
                        return RedirectToAction("Index");
                    }
                }
                finally
                {
                    // sauvegarde dans la BD
                    context.SaveChanges();
                }
            }

            // erreur inconnu, on met un message par defaut
            ModelState.AddModelError("", "Invalid username, password or grid card value.");
            model.Password = string.Empty;
            model.GridCardValue = 0;
            model.GridCardCol = r.Next(GridCardUtil.GridSize);
            model.GridCardRow = r.Next(GridCardUtil.GridSize);
            return View(model);
        }

        public ActionResult Logout()
        {
            // deconnecte l'utilisateur
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
            if (!ModelState.IsValid)
            {
                return View(new ChangePasswordModel(model.UserId));
            }

            // valide que les 2 mots de passe sont les meme
            if (!model.NewPassword1.Equals(model.NewPassword2))
            {
                ModelState.AddModelError("", "The 2 password are not equal.");
                return View(new ChangePasswordModel(model.UserId));
            }

            // valide que l'ancien mot de passe et le nouveau sont different
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
                // va chercher historique de mot de passe dans la base de donnees
                var passwordHistory = context.PasswordHistories
                    .Where(x => x.UserId == model.UserId)
                    .OrderByDescending(x => x.DateChanged)
                    .Take(options.NumberOfPasswordToKeepInHistory)
                    .ToList();

                // valide le mot de passe selon les critere dans la base de donnees
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

                // valide ancien mot de passe
                if (user.PasswordHash.Equals(HashingUtil.SaltAndHash(model.OldPassword, user.PasswordSalt)))
                {
                    var history = new PasswordHistory
                    {
                        DateChanged = DateTime.Now,
                        PasswordSalt = user.PasswordSalt,
                        PasswordHash = user.PasswordHash,
                        UserId = model.UserId,
                        HashingVersion = user.HashingVersion
                    };
                    // ajout de l'ancien mot de passe dans l'history
                    context.PasswordHistories.Add(history);

                    // genere un nouveau salt pour le mot de passe
                    var newSalt = Guid.NewGuid().ToString();

                    user.PasswordHash = HashingUtil.SaltAndHash(model.NewPassword1, newSalt);
                    user.PasswordSalt = newSalt;
                    user.MustChangePasswordAtNextLogon = false;
                    user.PasswordExpirationDate = DateTime.Now.AddDays(options.PasswordExpirationDurationInDays);
                    user.DefaultPasswordValidUntil = null;
                    user.HashingVersion = HashingUtil.Version;

                    // sauvegarde dans la BD
                    context.SaveChanges();

                    s_logger.Info(string.Format("User {0}({1}) changed his password", user.Username, user.Id));
                    SessionManager.SetLoggedInUser(user);
                    TempData["message"] = "Password changed!";

                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError("", "The old password is not valid.");
            return View(new ChangePasswordModel(model.UserId));
        }

        public ActionResult MyGridCard()
        {
            if (!SessionManager.IsUserLoggedIn()) return RedirectToAction("Login");

            using (var context = new DatabaseEntities())
            {
                var user = context.Users.Find(SessionManager.GetLoggedInUserId());
                if(user == null) return RedirectToAction("Login");
                var gridCard = GridCardUtil.GenerateGrid(user.GridCardSeed);
                return View(new MyGridCardModel {GridCard = gridCard});
            }
        }
    }
}