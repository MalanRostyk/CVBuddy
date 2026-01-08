using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class CertificateVM
    {
        public int CertId { get; set; }
        [DisplayName("Certificate name")]
        [Required(ErrorMessage = "An added certificate cannot be left empty.")]
        [StringLength(90, MinimumLength = 2)]
        public string CertName { get; set; }

    }
}
