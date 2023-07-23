namespace CarrierPortal.Models.DataModel
{
    public class AnswerVote
    {

        public string Id { get; set; }

        public bool IsUpvote { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AnswerId { get; set; }
        public Answer Answer { get; set; }

    }
}
