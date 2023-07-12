using System.ComponentModel.DataAnnotations;

namespace CarrierPortal.Models.DataModel
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Salary { get; set; }
        [Required]
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public ICollection<Applicant> Applicants { get; set; }
        public string PostedByUserId { get; set; } // Reference to the user who posted the job
        public ApplicationUser PostedByUser { get; set; }

    }
}
