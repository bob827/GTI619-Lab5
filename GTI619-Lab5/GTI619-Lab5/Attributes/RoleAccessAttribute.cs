﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GTI619_Lab5.Utils;

namespace GTI619_Lab5.Attributes
{
    public class RoleAccessAttribute : AuthorizeAttribute
    {
        public string Role { get; private set; }

        public RoleAccessAttribute(string role)
        {
            Role = role.ToLower();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null) return false;

            if (!SessionManager.IsUserLoggedIn()) return false;

            var userId = SessionManager.GetLoggedInUserId();

            using (var context = new DatabaseEntities())
            {
                if (context.IsUserAdmin(userId)) return true;
                if (context.IsUserInRole(userId, Role)) return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result =
                new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Account", action = "Login" }));
        }
    }
}