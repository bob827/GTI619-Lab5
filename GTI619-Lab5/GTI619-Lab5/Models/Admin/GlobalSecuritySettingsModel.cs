using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Models.Admin
{
    public class GlobalSecuritySettingsModel
    {
        [Required, Display(Name = "Admin password")]
        public string AdminPassword { get; set; }

        [Required, Display(Name = "Minimum password length"), Range(6, int.MaxValue, ErrorMessage = "Password length should be of at least 6 characters")]
        public int MinPasswordLength { get; set; }

        [Required, Display(Name = "Maximum login attempt limit"), Range(1, 10)]
        public int MaxLoginAttempt { get; set; }

        [Required, Display(Name = "Uppercase letter")]
        public bool PasswordShouldHaveUpperCase { get; set; }

        [Required, Display(Name = "Lowercase letter")]
        public bool PasswordShouldHaveLowerCase { get; set; }

        [Required, Display(Name = "Special characters")]
        public bool PasswordShouldHaveSpecialChars { get; set; }

        [Required, Display(Name = "Number")]
        public bool PasswordShouldHaveNumbers { get; set; }
    }
}