using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.Models.DataModel
{
    public class BlogPostVote
    {
     
        public int Id { get; set; }

        public bool IsUpvote { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }

    }
}
