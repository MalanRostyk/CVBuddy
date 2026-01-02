using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;


namespace CVBuddy.Models
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Cv OneCv { get; set; }
        public Address OneAddress { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
