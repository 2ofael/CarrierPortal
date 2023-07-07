using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.Models.DataModel
{
    public class Question
    {
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreationDate { get; set; }

        public int Votes { get; set; }

        public bool IsApproved { get; set; }

        // Relationship properties
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public List<Answer> Answers { get; set; }

    }
}
