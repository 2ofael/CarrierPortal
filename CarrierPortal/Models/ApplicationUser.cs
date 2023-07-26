using CarrierPortal.Models.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarrierPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool isProfileUpdated { get; set; } = false;
        public string City { get; set; }

        public Actor Mentor { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Answer> Answers { get; set; }
        public ICollection<Job> PostedJobs { get; set; }
        public ICollection<Applicant> AppliedJobs { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }

        public List<QuestionVote> QuestionVotes { get; set; }
        public List<AnswerVote> AnswerVotes { get; set; }
      ///  public List<BlogPostVote> BlogPostsVotes { get; set;}

        // 
    }
}
