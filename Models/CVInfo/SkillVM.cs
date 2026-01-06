using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class SkillVM
    {
        [Required(ErrorMessage = "Det funkade!!!!")]
        public string ASkill { get; set; }
        public string? Description { get; set; }
    }
}
