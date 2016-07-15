using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GTI619_Lab5.Models.Admin
{
    public class EditUserRolesModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }

        public List<int> SelectedRoleIds { get; set; }

        public Dictionary<int, string> AvailableRoles { get; set; }

        [Required, Display(Name = "Admin password")]
        public string AdminPassword { get; set; }
    }
}