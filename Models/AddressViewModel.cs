using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class AddressViewModel
    {
        [StringLength(50, ErrorMessage ="Country cannot be longer than 50 characters")]
        public string? Country { get; set; }

        [StringLength(70, ErrorMessage = "City cannot be longer than 50 characters")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "Street cannot be longer than 50 characters")]
        public string? Street { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Country)
                && string.IsNullOrWhiteSpace(City)
                && string.IsNullOrWhiteSpace(Street);
        }
        public bool IsPartiallyFilled()
        {
            if (IsEmpty())
                return false;

            return string.IsNullOrWhiteSpace(Country)
                || string.IsNullOrWhiteSpace(City)
                || string.IsNullOrWhiteSpace(Street);
        }
    }
}
