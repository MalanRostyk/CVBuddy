using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
