using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        public string? Country { get; set; } 
        public string? City { get; set; } 
        public string? Street { get; set; } 

        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public User? OneUser { get; set; }
    }
}