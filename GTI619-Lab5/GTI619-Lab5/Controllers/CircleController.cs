using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;
using GTI619_Lab5.Logger;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Circle"), RequireHttps]
    public class CircleController : Controller
    {
        private static readonly ILogger s_logger = LogManager.GetLogger(typeof(CircleController));

        // GET: Circle
        public ActionResult Index()
        {
            return View();
        }
    }
}