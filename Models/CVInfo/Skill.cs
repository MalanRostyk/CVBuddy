using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public class Skill
    {
        [Key]
        public int Sid { get; set; }
        public string ASkill { get; set; }
        public string? Description{ get; set; }
        public string? Date{ get; set; }
    }
}
