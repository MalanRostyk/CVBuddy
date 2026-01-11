using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;

namespace CVBuddy.Models
{
    public class User: IdentityUser
    {
        public Cv OneCv { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName{ get; set; } = string.Empty;

        public Address? OneAddress { get; set; }

        public bool IsDeactivated { get; set; } = false;

        public bool HasPrivateProfile { get; set; } = false;

        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        public List<Message> MessageList { get; set; }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
