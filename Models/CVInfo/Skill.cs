using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class Skill
    {
        [Key]
        public int Sid { get; set; }
        public string ASkill { get; set; }
        public string? Description{ get; set; }
        public string? Date{ get; set; }
    
        public int CvId { get; set; }
        [ForeignKey(nameof(CvId))]
        public Cv Cv { get; set; }
    }
}
