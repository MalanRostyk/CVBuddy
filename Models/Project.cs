using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Project
    {
        [Key]
        public int Pid { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? Enddate { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
        [NotMapped]
        public List<User> UsersInProject { get; set; } = new();
        [NotMapped]
        public string? UserId { get; set; }
    }
}
