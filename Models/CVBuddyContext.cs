using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CVBuddy.Models
{
    public class CVBuddyContext: IdentityDbContext<User>
    {
        public CVBuddyContext(DbContextOptions<CVBuddyContext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Project>()
                .HasOne(p => p.OneUser)
                .WithMany(u => u.ManyProjects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
