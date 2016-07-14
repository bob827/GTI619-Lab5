using System;
using System.Web;

namespace GTI619_Lab5.Utils
{
    public class SessionManager
    {
        private const string USER_ID_KEY = "LOGGED_IN_USER_ID";
        private const string USERNAME_KEY = "LOGGED_IN_USERNAME";

        public static void SetLoggedInUser(User user)
        {
            HttpContext.Current.Session.Add(USER_ID_KEY, user.Id);
            HttpContext.Current.Session.Add(USERNAME_KEY, user.Username);
        }

        public static int GetLoggedInUserId()
        {
            if (IsUserLoggedIn())
                return (int) HttpContext.Current.Session[USER_ID_KEY];

            throw new NullReferenceException();
        }

        public static string GetLoggedInUsername()
        {
            if (IsUserLoggedIn())
                return HttpContext.Current.Session[USERNAME_KEY].ToString();

            throw new NullReferenceException();
        }

        public static bool IsUserLoggedIn()
        {
            return HttpContext.Current.Session[USER_ID_KEY] != null;
        }

        public static void LogoutUser()
        {
            if (IsUserLoggedIn())
            {
                HttpContext.Current.Session.Remove(USER_ID_KEY);
                HttpContext.Current.Session.Remove(USERNAME_KEY);
            }
        }
    }
}