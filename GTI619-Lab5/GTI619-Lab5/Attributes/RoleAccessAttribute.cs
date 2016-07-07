using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GTI619_Lab5.Attributes
{
    public class RoleAccessAttribute : AuthorizeAttribute
    {
        public string Role { get; private set; }

        public RoleAccessAttribute(string role)
        {
            Role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null) return false;

            // aller chercher un token de connection ou quelque chose du genre
            // todo faire du code qui ressemble a ca:
            //var userId = httpContext.Session["USER_ID_CONNECTED"];
            //if (userIsAdmin(userId)) return true;
            //var roles = getUserRoles(userId);
            //if (roles.Contains(Role)) return true;
            //return false;

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result =
                new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
        }
    }
}