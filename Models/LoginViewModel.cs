using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="UserName can not be empty")]
        [StringLength(72)]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Password can not be empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}
