﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GTI619_Lab5.Models.Admin
{
    public class ResetUserPasswordModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        [Required, Display(Name = "Admin password")]
        public string AdminPassword { get; set; }
    }
}