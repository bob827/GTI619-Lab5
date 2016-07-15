using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTI619_Lab5
{
    public partial class User
    {
        public bool HasToChangePassword()
        {
            if (MustChangePasswordAtNextLogon) return true;
            if (PasswordExpirationDate.HasValue && DateTime.Now >= PasswordExpirationDate.Value) return true;
            return false;
        }
    }
}