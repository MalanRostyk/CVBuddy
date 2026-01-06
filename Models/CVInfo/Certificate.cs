using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Certificate
    {
        [Key]
        public int CertId { get; set; }

        [Required(ErrorMessage = "An added certificate cannot be left empty")]
        [StringLength(90, MinimumLength = 2)]
        public string CertName { get; set; }

        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv? Cv { get; set; }
    }
}
