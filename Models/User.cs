using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace CVBuddy.Models
{
    public class User: IdentityUser
    {
        public Cv OneCv { get; set; }

        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

        [Required]
        public string FirstName{ get; set; }
        [Required]
        public string LastName{ get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
