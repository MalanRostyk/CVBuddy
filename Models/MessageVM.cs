using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class MessageVM
    {
        public int Mid { get; set; }

        [Required(ErrorMessage = "Must enter your name")]
        [StringLength(50, MinimumLength = 2)]
        public string Sender { get; set; }
        [Required]
        [StringLength(350, MinimumLength = 1)]
        public string MessageString { get; set; }

        public DateTime SendDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }
        public string RecieverId { get; set; }
    }
}
