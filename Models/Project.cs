using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Project
    {
        [Key]
        public int Pid { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MaxLength(200)] //Ska matcha maxlength="200" i view på inputfältet
        public string? Description { get; set; }

        [Required]
        //[DataType(DataType.Date)] //Kom tillbaka hit, den sätter i view startande datumet till 0001-01-01
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        public DateTime? Enddate { get; set; }  //Ändra till stort D, glöm ej att även ändra i view också
        [NotMapped]
        public List<User> UsersInProject { get; set; } = new();//ONödig, kan ej användas efter att det serialiserats, när det kommer från db så är den null oavsett

        public DateTime PublisDate { get; set; } = DateTime.Now;

        [NotMapped]
        public string? UserId{ get; set; }

        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
