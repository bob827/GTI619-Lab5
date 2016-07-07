using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTI619_Lab5.Attributes;

namespace GTI619_Lab5.Controllers
{
    [RoleAccess("Square"), RequireHttps]
    public class SquareController : Controller
    {
        // GET: Square
        public ActionResult Index()
        {
            return View();
        }
    }
}