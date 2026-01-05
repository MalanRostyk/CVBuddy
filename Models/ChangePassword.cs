using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class ChangePassword
    {
<<<<<<< HEAD
        [Required(ErrorMessage ="Please put your old password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage ="Please put your new password")]
=======
        [Required(ErrorMessage = "Please put your old password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please put your new password")]
>>>>>>> den-senaste-v2-05
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,}$", ErrorMessage = "Password should contain at least: 8 characters, 1 uppercase letter, 1 lowercase letter, 1 number")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> den-senaste-v2-05
