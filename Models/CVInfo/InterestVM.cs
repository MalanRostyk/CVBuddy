using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class InterestVM
    {
        public int InterestId { get; set; }

        [DisplayName("Interest name")]
        [Required]
        [StringLength(90, MinimumLength = 3)]
        public string InterestName { get; set; }
    }
}
