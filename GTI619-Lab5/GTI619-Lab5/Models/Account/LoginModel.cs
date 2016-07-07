using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Models.Account
{
    public class LoginModel
    {
        [Required, Display(Name = "Username")]
        public string Username { get; set; }

        [Required, Display(Name = "Password")]
        public string Password { get; set; }
    }
}