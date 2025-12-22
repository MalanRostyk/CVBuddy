using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CVBuddy.Models.CVInfo
{
    public class Cv
    {
        [Key]
        public int Cid { get; set; }

        public int SkillsId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(SkillsId))]
        public List<Skill> Skills { get; set; } = new();
        
        public int EduId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(EduId))]
        public Education Education { get; set; }

        public List<int> ExpIds { get; set; } = new();

        [NotMapped]
        [ForeignKey(nameof(ExpIds))]
        public List<Experience> Experiences { get; set; } = new();

        public List<int> CertIds { get; set; } = new();

        [NotMapped]
        [ForeignKey(nameof(CertIds))]
        public List<Certificate> Certificates{ get; set; } = new();
        public List<int> PCIds { get; set; } = new();

        [NotMapped]
        [ForeignKey(nameof(PCIds))]
        public List<PersonalCharacteristic> PersonalCharacteristics{ get; set; } = new();
        public string? Interests{ get; set; }
        public string? ImageFilePath { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; } 
        public int ReadCount { get; set; }
        public string? UserId { get; set; }

        [NotMapped]
        [ForeignKey(nameof(UserId))]
        public User? OneUser { get; set; }
        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
    }
}
