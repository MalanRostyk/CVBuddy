using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required(ErrorMessage = "Please put your country")]
        [StringLength(50, ErrorMessage = "ountry can not be longer than 50 characters")]
        public string Country { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Please put your city")]
        [StringLength(70, ErrorMessage = "City can not be longer than 70 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please put your street")]
        [StringLength(100, ErrorMessage = "Street can not be longer than 100 characters")]
        public string Street { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public User? OneUser { get; set; }
    }
}