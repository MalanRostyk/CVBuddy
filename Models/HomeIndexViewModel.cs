namespace CVBuddy.Models
{
    public class HomeIndexViewModel
    {
        public List<User> UserList { get; set; } = new List<User>();

        public List<ProjectVM> ProjectList { get; set; } = new();//ändring 

    }
}