using CVBuddy.Models.CVInfo;
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
        public DbSet<Cv> Cvs { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasOne(p => p.OneCv)
                .WithOne(p => p.OneUser)
                .HasForeignKey<Cv>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CvProject>().HasKey(cp => new { cp.CvId, cp.Pid });//CVProject har komposit PK(CvId, Pid)

            builder.Entity<CvProject>()
                .HasOne(cv => cv.OneCv)
                .WithMany(cv => cv.CvProjects)
                .HasForeignKey(cv => cv.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CvProject>()
                .HasOne(p => p.OneProject)
                .WithMany(p => p.CvProjects)
                .HasForeignKey(p => p.Pid)
                .OnDelete(DeleteBehavior.Cascade);

                
        }
    }
}
