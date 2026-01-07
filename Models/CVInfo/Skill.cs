using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Skill
    {
        [Key]
        public int Sid { get; set; }

        [Required(ErrorMessage = "You need to enter what the skill is.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [StringLength(90, MinimumLength = 2)]
        public string ASkill { get; set; }

        [StringLength(120)]
        public string? Description{ get; set; }//TextArea ingen tecken validering
        public DateTime? Date{ get; set; }
    
        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv? Cv { get; set; }
    }
}
