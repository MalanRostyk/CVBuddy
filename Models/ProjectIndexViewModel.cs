namespace CVBuddy.Models
{
    public class ProjectIndexViewModel
    {
        public List<ProjectVM> MyProjects { get; set; } = new();//ändring 
        public List<ProjectVM> OtherProjects { get; set; } = new();//ändring
        public List<ProjectVM> PublicProjects { get; set; } = new();//ändring

    }
}