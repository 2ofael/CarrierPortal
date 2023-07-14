using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.Models.DataModel
{
    public class NewsletterSubscription
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
