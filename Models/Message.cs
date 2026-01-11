using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Message
    {
        [Key]
        public int Mid { get; set; }

        public string Sender { get; set; }

        public string MessageString { get; set; }

        public DateTime SendDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }

        public string RecieverId { get; set; }
        [ForeignKey(nameof(RecieverId))]
        public User Reciever { get; set; }
    }
}
