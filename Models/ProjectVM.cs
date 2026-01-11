namespace CVBuddy.Models
{
    public class ProjectVM
    {
        public Project Project { get; set; } = null!;

        public List<User> UsersInProject { get; set; } = new();

        public List<User> ActiveUsers { get; set; } = new();

        public ProjectUser? Relation { get; set; }

        public ProjectUser? Owner { get; set; }

        public bool IsUserInProject { get; set; }
    }
}
