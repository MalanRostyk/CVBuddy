using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Required(ErrorMessage = "Please put your street")]
        [StringLength(100, ErrorMessage = "Street can not be longer than 100 characters")]
        public string Street { get; set; }
        [Required(ErrorMessage = "Please put your city")]
        [StringLength(70, ErrorMessage = "City can not be longer than 70 characters")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please put your country")]
        [StringLength(50, ErrorMessage = "ountry can not be longer than 50 characters")]
        public string Country { get; set; }
        public string UserId { get; set; }
        public User OneUser { get; set; }
    }
}