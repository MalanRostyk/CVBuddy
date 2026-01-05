

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Message
    {
        [Key]
        public int Mid { get; set; }
        [Required(ErrorMessage = "Must enter your name")]
        public string Sender { get; set; }
        [Required]
        public string MessageString { get; set; }

        public DateTime SendDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }


        [ForeignKey(nameof(RecieverId))]
        public string RecieverId { get; set; }
        public User Reciever { get; set; }
    }
}
