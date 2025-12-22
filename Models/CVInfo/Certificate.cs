using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Certificate
    {
        [Key]
        public string CertId { get; set; }
        public string CertName { get; set; }

        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv Cv { get; set; }
    }
}
