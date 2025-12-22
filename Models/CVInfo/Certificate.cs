using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class Certificate
    {
        [Key]
        public string CertId { get; set; }
        public string CertName { get; set; }
    }
}
