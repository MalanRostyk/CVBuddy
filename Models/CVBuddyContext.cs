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
        public DbSet<User> Users { get; set; } //override för att kunna läggat till User entiteten
        public DbSet<Project> Projects { get; set; }
        public DbSet<Cv> Cvs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<PersonalCharacteristic> PersonalCharacteristics { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<CvProject> CvProjects { get; set; }
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);


            //User > Project (M-M via ProjectUser)
            builder.Entity<ProjectUser>().HasKey(pu => new { pu.UserId, pu.ProjId });
            //One Project har många ProjectUsers
            builder.Entity<ProjectUser>()
                .HasOne(p => p.Project)
                .WithMany(pu => pu.ProjectUsers)
                .HasForeignKey(p => p.ProjId)
                .OnDelete(DeleteBehavior.Cascade);
            //One User har många ProjectUsers
            builder.Entity<ProjectUser>()
                .HasOne(u => u.User)
                .WithMany(pu => pu.ProjectUsers)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            //user > Cv
            builder.Entity<User>()
                .HasOne(p => p.OneCv)
                .WithOne(p => p.OneUser)
                .HasForeignKey<Cv>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            //Cv > Project (M-M via CvProject)
            builder.Entity<CvProject>().HasKey(cp => new { cp.CvId, cp.ProjId });//CVProject har komposit PK(CvId, Pid)

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
                .HasForeignKey(p => p.ProjId)
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
                .OnDelete(DeleteBehavior.Cascade);

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

            //Adress > User 1:1
            builder.Entity<User>()
               .HasOne(u => u.OneAddress)
               .WithOne(a => a.OneUser)
               .HasForeignKey<Address>(a => a.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            //User > Message 1:N
            builder.Entity<Message>()
                .HasOne(m => m.Reciever)
                .WithMany(r => r.MessageList)
                .HasForeignKey(m => m.RecieverId)
                .OnDelete(DeleteBehavior.Restrict);
            }
    }
}
