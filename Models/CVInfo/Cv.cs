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

        //Nullable kommentarer för att annars kan inte ett cv skapas utan respektive egenskap, därmed db får falsk data om inte kan lämnas tomt
        public List<Skill> Skills { get; set; } = new(); //Objekt i List borde vara nullable


        public Education Education { get; set; } //Borde vara nullable

        public List<Experience> Experiences { get; set; } = new(); //Objekt i List borde vara nullable

        public List<Certificate> Certificates{ get; set; } = new();//Objekt i List borde vara nullable

        public List<PersonalCharacteristic> PersonalCharacteristics{ get; set; } = new();//Objekt i List borde vara nullable



        public List<Interest> Interests { get; set; } = new();
        public string? ImageFilePath { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; } //Borde vara nullable
        public int ReadCount { get; set; }
        public string? UserId { get; set; } //Vart null

        //[NotMapped]
        [ForeignKey(nameof(UserId))]
        public User? OneUser { get; set; } //tilldelas?
        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
    }
}
