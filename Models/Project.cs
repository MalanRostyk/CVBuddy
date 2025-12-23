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
        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate{ get; set; }

        public DateTime? Enddate { get; set; }
        [NotMapped]
        public List<User> UsersInProject { get; set; } = new();//För att enklare hantera användare i projekt i koden

        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();


    }
}
