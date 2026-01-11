using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models.CVInfo
{
    public class CvVM
    {
        public int Cid { get; set; }

        public List<Skill> Skills { get; set; } = new(); 

        public Education? Education { get; set; } 

        public List<Experience> Experiences { get; set; } = new(); 

        public List<Certificate> Certificates { get; set; } = new();

        public List<PersonalCharacteristic> PersonalCharacteristics { get; set; } = new();

        public DateTime PublishDate { get; set; } = DateTime.Now;

        public List<Interest> Interests { get; set; } = new();

        public string? ImageFilePath { get; set; } = "pic.png";

        [NotMapped]
        [Required(ErrorMessage = "Upload an image please.")]
        [ExtensionValidation("jpg,png,jfif,webp")]
        public IFormFile? ImageFile { get; set; }

        public int ReadCount { get; set; }

        public string? UserId { get; set; }

        public User? OneUser { get; set; } 

        [NotMapped]
        public List<Project> UsersProjects { get; set; } = new();
    }
}
