namespace CVBuddy.Models
{
    public class HomeIndexViewModel
    {
        public List<User> UserList {  get; set; } = new List<User>();

        public List<Project> ProjectList { get; set; } = new List<Project>();

        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
