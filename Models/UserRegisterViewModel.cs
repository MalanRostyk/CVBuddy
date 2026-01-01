using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class UserRegisterViewModel
    {


        [Required(ErrorMessage = "Must enter first name")]
        [StringLength(100, ErrorMessage = "Too long first Name, max 72 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Must enter last name")]
        [StringLength(100, ErrorMessage = "Too long last Name, max 72 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Enter email")]
        [StringLength(100, ErrorMessage = "Too long last Name, max 100 characters")]
        public string Email { get; set; }
              
        
        [Required(ErrorMessage = "Must enter username")]
        [StringLength(36, ErrorMessage = "Too long username, max 36 characters")]
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
