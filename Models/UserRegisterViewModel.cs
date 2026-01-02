using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Put name")]
        [StringLength(100, ErrorMessage ="Too long Name")]
        public string UserName { get; set; }


        [Required(ErrorMessage="Put password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password")]//ska matcha password
        public string ConfirmPassword { get; set; }
    }
}
