using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Models
{
    public class CVBuddyContext: IdentityDbContext<User>
    {
        public CVBuddyContext(DbContextOptions<CVBuddyContext> options):base(options)
        {
            
        }
        public DbSet<User> Users{ get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //    builder.Entity<User>().HasData(new User { Id = "1", UserName = "zbr2k", PasswordHash = "Asdfgh10" });
        //}
    }
}
