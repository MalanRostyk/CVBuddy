using Microsoft.AspNetCore.Identity;

namespace CVBuddy.Models
{
    public class User: IdentityUser
    {
        public IEnumerable<Project> ManyProjects { get; set; }        
    }
}
