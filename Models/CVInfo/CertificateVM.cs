using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class CertificateVM
    {

        [Required(ErrorMessage = "An added certificate cannot be left empty.")]
        [StringLength(90)]
        public string CertName { get; set; }

    }
}
