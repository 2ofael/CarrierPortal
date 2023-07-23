using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.Models.DataModel
{
    public class Answer
    {
        public string Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public int Votes { get; set; }



        // Relationship properties
        public string QuestionId { get; set; }
        public Question Question { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public bool IsApproved { get; set; }
        public List<AnswerVote> AnswerVotes { get; set; }


    }
}
