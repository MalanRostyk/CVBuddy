using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models
{
    public class MessageVM
    {
        public int Mid { get; set; }

        [Required(ErrorMessage = " A senders name is required")]
        [RegularExpression(@"^[A-ZÅÄÖa-zåäö0-9 .-]+$", ErrorMessage = " - Senders name can only contain " +
            "upper/lower case letters, numbers, dots and dashes. (Ex: Sender-me.1 or Sender Sendersson)")]
        [StringLength(50, MinimumLength = 2)]
        public string Sender { get; set; }
        
        [Required(ErrorMessage = " A message i required")]
        [StringLength(350, MinimumLength = 1)]//TextArea limiteras automatiskt till 350 tecken
        public string MessageString { get; set; }

        public DateTime SendDate { get; set; } = DateTime.Now;

        public bool IsRead { get; set; }
        
        public string RecieverId { get; set; }
    }
}
