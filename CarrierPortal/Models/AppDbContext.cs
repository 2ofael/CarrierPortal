using CarrierPortal.Models.DataModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany(u => u.Answers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);





            modelBuilder.Entity<Job>()
           .HasMany(j => j.Applicants)
           .WithOne(a => a.Job)
           .HasForeignKey(a => a.JobId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Applicant>()
                .HasOne(a => a.Resume)
                .WithOne(r => r.Applicant)
                .HasForeignKey<Resume>(r => r.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.PostedJobs)
                .WithOne(j => j.PostedByUser)
                .HasForeignKey(j => j.PostedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.AppliedJobs)
                .WithOne(a => a.ApplicantUser)
                .HasForeignKey(a => a.ApplicantUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<Actor> Actors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }


    }
}
