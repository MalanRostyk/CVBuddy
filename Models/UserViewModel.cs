using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class UserViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "User name is Required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is Required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is Required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        public AddressViewModel? AddressVm { get; set; }
    }
}
