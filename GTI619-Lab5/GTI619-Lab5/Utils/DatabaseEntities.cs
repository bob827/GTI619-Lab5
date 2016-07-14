using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI619_Lab5
{
    public partial class DatabaseEntities
    {
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
                .Any(role => role.RoleName.Equals("admin"));
        }
    }
}