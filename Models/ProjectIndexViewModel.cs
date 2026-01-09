namespace CVBuddy.Models
{
    public class ProjectIndexViewModel
    {
        public List<Project> MyProjects { get; set; } = new();
        public List<Project> OtherProjects { get; set; } = new();
        public List<Project> PublicProjects { get; set; } = new();//ändring
        //testa alla variabler som filtrerar listorna i view lägga i controller, skapa fält för de här,
        //skapa uservm och projektvm tilltela med dto, anropa sedan fält från view via Model

    }
}