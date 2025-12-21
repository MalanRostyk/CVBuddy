using CVBuddy.Models.CVInfo;
using Microsoft.AspNetCore.Identity;


namespace CVBuddy.Models
{
    public class User: IdentityUser
    {
        public Cv OneCv { get; set; }
    }
}
