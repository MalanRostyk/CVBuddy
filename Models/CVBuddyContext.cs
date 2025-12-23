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
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<PersonalCharacteristic> PersonalCharacteristics { get; set; }
        public DbSet<Interest> Interests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            //user > Cv
            builder.Entity<User>()
                .HasOne(p => p.OneCv)
                .WithOne(p => p.OneUser)
                .HasForeignKey<Cv>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Cv > Project (M-M via CvProject)
            builder.Entity<CvProject>().HasKey(cp => new { cp.CvId, cp.Pid });//CVProject har komposit PK(CvId, Pid)

            //One Cv har många CvProjects
            builder.Entity<CvProject>()
                .HasOne(cv => cv.OneCv)
                .WithMany(cv => cv.CvProjects)
                .HasForeignKey(cv => cv.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            //One Project har många CvProjects
            builder.Entity<CvProject>()
                .HasOne(p => p.OneProject)
                .WithMany(p => p.CvProjects)
                .HasForeignKey(p => p.Pid)
                .OnDelete(DeleteBehavior.Cascade);


            //Cv > Skill 1:M
            builder.Entity<Skill>()
                .HasOne(p => p.Cv)
                .WithMany(p => p.Skills)
                .HasForeignKey(p => p.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            //Cv > Experience  1:M
            builder.Entity<Experience>()
                .HasOne(p => p.Cv)
                .WithMany(p => p.Experiences)
                .HasForeignKey(p => p.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            //Cv > Education  1:1
            builder.Entity<Education>()
                .HasOne(p => p.Cv)
                .WithOne(p => p.Education)
                .OnDelete(DeleteBehavior.Restrict);

            //Cv > Certificate 1:M
            builder.Entity<Certificate>()
                .HasOne(p => p.Cv)
                .WithMany(p => p.Certificates)
                .HasForeignKey(p => p.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            //Cv > PersonalCharacteristic 1:M
            builder.Entity<PersonalCharacteristic>()
                .HasOne(p => p.Cv)
                .WithMany(p => p.PersonalCharacteristics)
                .HasForeignKey(p => p.CvId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Interest>()
                .HasOne(p => p.Cv)
                .WithMany(p => p.Interests)
                .HasForeignKey(p => p.CvId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
