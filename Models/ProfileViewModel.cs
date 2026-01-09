using CVBuddy.Models.CVInfo;

namespace CVBuddy.Models
{
    public class ProfileViewModel
    {
        public User? ViewUser { get; set; } //måste vara nullable men kan aldrig bli null, alla profiler har en user
        public Cv? Cv { get; set; }
        public List<ProjectVM> Projects { get; set; } = new();//ändring
    }
}
