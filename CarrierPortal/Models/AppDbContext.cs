using CarrierPortal.Models.DataModel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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

            modelBuilder.Entity<BlogPost>()
                .HasOne(bp => bp.ApplicationUser)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
               .HasMany(u => u.AnswerVotes)
               .WithOne(v => v.User)
               .HasForeignKey(v => v.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.QuestionVotes)
            .WithOne(v => v.User)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);


         //   modelBuilder.Entity<ApplicationUser>()
         //.HasMany(u => u.BlogPostsVotes)
         //.WithOne(v => v.User)
         //.HasForeignKey(v => v.UserId)
         //.OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Question>()
           .HasMany(q => q.QuestionVotes)
           .WithOne(v => v.Question)
           .HasForeignKey(v => v.QuestionId)
           .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
              .HasMany(a => a.AnswerVotes)
              .WithOne(v => v.Answer)
              .HasForeignKey(v => v.AnswerId)
              .OnDelete(DeleteBehavior.Cascade);
    
            modelBuilder.Entity<BlogPost>()
                .HasMany(b=>b.Loved)
                .WithOne(L=>L.BlogPost)
                .HasForeignKey(b=>b.BlogId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Actor>()
                .HasMany(a => a.Loved)
                .WithOne(l => l.Actor)
                .HasForeignKey(l => l.ActorId)
                .OnDelete(DeleteBehavior.Cascade);

                
          

      //      modelBuilder.Entity<BlogPost>()
      //.HasMany(p => p.BlogPostVotes)
      //.WithOne(v => v.BlogPost)
      //.HasForeignKey(v => v.BlogPostId)
      //.OnDelete(DeleteBehavior.Cascade);


        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<NewsletterSubscription> NewsletterSubscriptions { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<QuestionVote> QuestionVotes { get; set; }
        public DbSet<AnswerVote> AnswerVotes { get; set; }
      //  public DbSet<BlogPostVote> BlogPostVotes { get; set; }



    }
}
