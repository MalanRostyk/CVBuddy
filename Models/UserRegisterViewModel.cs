using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class UserRegisterViewModel
    {
        [Required(ErrorMessage = "Put name")]
        [StringLength(100, ErrorMessage ="Too long Name")]
        public string Name { get; set; }


        [Required(ErrorMessage="Put password")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm password")]
        [DataType(DataType.Password)]
        [Display(Name ="Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}
