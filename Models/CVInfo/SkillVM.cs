using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class SkillVM
    {
        public int Sid { get; set; }

        [DisplayName("Skill name")]
        [Required(ErrorMessage = "You need to enter what the skill is.")] //Få ha alla tecken vid fall att "ASP.NET" eller "Fork-lift license"
        [StringLength(90, MinimumLength = 3)]
        public string ASkill { get; set; }

        [StringLength(90)]
        public string? Description { get; set; }//TextArea ingen tecken validering

        [Range(typeof(DateTime), "1900-01-01", "2049-12-31", ErrorMessage = "Date can only be after year 1900 and before year 2050.")]
        public DateTime? Date { get; set; }

    }
}
