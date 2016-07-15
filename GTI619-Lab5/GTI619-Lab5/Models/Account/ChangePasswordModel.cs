using System.ComponentModel.DataAnnotations;

namespace GTI619_Lab5.Models.Account
{
    public class ChangePasswordModel
    {
        public int UserId { get; set; }

        [Required, Display(Name = "Old password")]
        public string OldPassword { get; set; }

        [Required, Display(Name = "New password")]
        public string NewPassword1 { get; set; }

        [Required, Display(Name = "Confirm new password")]
        public string NewPassword2 { get; set; }

        public ChangePasswordModel() { }

        public ChangePasswordModel(int userId)
        {
            UserId = userId;
        }
    }
}