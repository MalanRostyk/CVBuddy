using CVBuddy.Models.CVInfo;

namespace CVBuddy.Models
{
    public class ProfileViewModel
    {
        public User? ViewUser { get; set; } 

        public Cv? Cv { get; set; }

        public List<ProjectVM> Projects { get; set; } = new();
    }
}
