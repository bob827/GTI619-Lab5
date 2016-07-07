using System;
using System.ComponentModel.DataAnnotations;

namespace GTI619_Lab5.Models.Admin
{
    public class ResetUserPasswordModel
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }

        [Required, Display(Name = "Password")]
        public string AdminPassword { get; set; }
    }
}