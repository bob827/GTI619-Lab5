using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI619_Lab5
{
    public partial class DatabaseEntities
    {
        private const string ADMIN_ROLENAME = "admin";
        public List<string> GetUserRoles(int userId)
        {
            return Roles
                .Where(role => role.Users.Any(user => user.Id == userId))
                .Select(x => x.RoleName.ToLower())
                .ToList();
        }

        public bool IsUserAdmin(int userId)
        {
            return Roles
                .Where(role => role.Users.Any(user => user.Id == userId))
                .Any(role => role.RoleName.ToLower().Equals(ADMIN_ROLENAME));
        }

        public bool IsUserInRole(int userId, string roleName)
        {
            return Roles
                .Where(role => role.Users.Any(user => user.Id == userId))
                .Any(role => role.RoleName.ToLower().Equals(roleName.ToLower()));
        }

        public bool IsMaxLoginAttemptReachedForIp(string ip)
        {
            var options = AdminOptions.Single();
            var lastMinute = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            // compte le nombre de tentative de connexion avec un adresse ip qui n'ont pas fonctionner dans la derniere minute
            return LoginAttempts.Count(x => x.ClientIpAddress.Equals(ip)&& x.Date >= lastMinute && !x.IsSuccessful) >= options.MaxLoginAttempt;
        }

        public bool IsMaxLoginAttemptReachedForUserId(int userId)
        {
            var options = AdminOptions.Single();
            var lastMinute = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            // compte le nombre de tentative de connexion avec un user id qui n'ont pas fonctionner dans la derniere minute
            var result = LoginAttempts.Count(x => x.UserId == userId && x.Date >= lastMinute && !x.IsSuccessful) >= options.MaxLoginAttempt;
            if (result)
            {
                var user = Users.Find(userId);
                user.TimeoutEndDate = DateTime.Now.Add(TimeSpan.FromMinutes(options.TimeoutAfterMaxLoginReachedInMinutes));

                // limit was reached
                // validate if we need to lock the account

                lastMinute = lastMinute.Subtract(TimeSpan.FromMinutes(1));

                // compte le nombre de tentative de connexion avec un user id qui n'ont pas fonctionner dans les 2 dernieres minutes
                if (LoginAttempts.Count(x => x.UserId == userId && x.Date >= lastMinute && !x.IsSuccessful) >= options.MaxLoginAttempt*2)
                {
                    user.TimeoutEndDate = null;
                    user.IsLocked = true;
                    SaveChanges();
                }
            }

            return result;
        }
    }
}