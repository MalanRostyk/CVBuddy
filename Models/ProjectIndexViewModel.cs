namespace CVBuddy.Models
{
    public class ProjectIndexViewModel
    {
        public List<Project> MyProjects { get; set; } = new();
        public List<Project> OtherProjects { get; set; } = new();
    }
}