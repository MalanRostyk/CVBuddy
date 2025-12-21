using System.ComponentModel.DataAnnotations;

namespace CVBuddy.Models.CVInfo
{
    public abstract class Skill : Education
    {
        public List<string?> SkillList { get; set; } = new();
        public string? Description{ get; set; }
    }
}
