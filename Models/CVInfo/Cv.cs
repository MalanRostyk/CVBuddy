using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CVBuddy.Models.CVInfo
{
    public class Cv
    {
        [Key]
        public int Cid { get; set; }


        public List<Skill> Skills { get; set; } = new();
        

        public Education Education { get; set; }

        public List<Experience> Experiences { get; set; } = new();

        public List<Certificate> Certificates{ get; set; } = new();
       
        public List<PersonalCharacteristic> PersonalCharacteristics{ get; set; } = new();



        public List<Interest> Interests { get; set; } = new();
        public string? ImageFilePath { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; } 
        public int ReadCount { get; set; }
        public string? UserId { get; set; } //Vart null

        //[NotMapped]
        [ForeignKey(nameof(UserId))]
        public User? OneUser { get; set; } //tilldelas?
        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
    }
}
