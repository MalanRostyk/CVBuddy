using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Interest
    {
        [Key]
        public int InterestId { get; set; }

        [Required(ErrorMessage = "An added interest cannot be left empty.")] 
        [StringLength(90)]
        public string InterestName { get; set; }

        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv? Cv { get; set; }
    }
}
