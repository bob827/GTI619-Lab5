using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Models.Admin
{
    public class CreateUserModel
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; }

        [Required, Display(Name = "Email"), EmailAddress]
        public string Email { get; set; }

        [Required, Display(Name = "Admin password")]
        public string AdminPassword { get; set; }
    }
}