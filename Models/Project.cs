using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CVBuddy.Models
{
    public class Project
    {
        [Key]
        public int Pid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public ICollection<CvProject> CvProjects { get; set; } = new List<CvProject>();

    }
}
