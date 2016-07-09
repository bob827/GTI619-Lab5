using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Logger;

namespace GTI619_Lab5.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(HomeController));
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Login", "Account");
        }
    }
}