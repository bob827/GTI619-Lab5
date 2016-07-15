using System.ComponentModel.DataAnnotations;

namespace GTI619_Lab5.Models.Admin
{
    public class DeleteUserModel
    {
        public int UserId { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required, Display(Name = "Admin password")]
        public string AdminPassword { get; set; }
    }
}