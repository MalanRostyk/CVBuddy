using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class InterestVM
    {

        [Required]
        [StringLength(90, MinimumLength = 3)]
        public string InterestName { get; set; }

        //public int CvId { get; set; }
    }
}
