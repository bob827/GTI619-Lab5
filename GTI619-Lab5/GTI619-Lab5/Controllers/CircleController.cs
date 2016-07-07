using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Circle"), RequireHttps]
    public class CircleController : Controller
    {
        // GET: Circle
        public ActionResult Index()
        {
            return View();
        }
    }
}