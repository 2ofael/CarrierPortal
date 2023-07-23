namespace CarrierPortal.Models.DataModel
{
    public class QuestionVote
    {

        public string Id { get; set; }

        public bool IsUpvote { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
