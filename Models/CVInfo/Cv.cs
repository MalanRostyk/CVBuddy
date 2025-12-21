using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CVBuddy.Models.CVInfo
{
    public sealed class Cv : Skill
    {
        [Key]
        public int Cid { get; set; }    
        public List<string> Certificates{ get; set; }
        public List<string> PersonalCharacteristics{ get; set; }
        public string Interests{ get; set; }
        public string ImageFilePath { get; set; }
        public int ReadCount { get; set; } //krav
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User OneUser { get; set; }
        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
    }
}
