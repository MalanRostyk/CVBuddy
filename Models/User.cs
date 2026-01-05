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
<<<<<<< HEAD
=======

        [Required]
        public string FirstName{ get; set; }
        [Required]
        public string LastName{ get; set; }
        public Address OneAddress { get; set; }
        public bool IsDeactivated { get; set; } = false;
        public bool HasPrivateProfile { get; set; } = false;

        public List<Message> MessageList { get; set; }
        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
>>>>>>> den-senaste-v2-05
    }
}
