namespace CarrierPortal.Models.DataModel
{
    public class Applicant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Resume Resume { get; set; } // Navigation property for the single resume
        public DateTime AppliedDate { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }
        public string ApplicantUserId { get; set; } // Reference to the user who applied for the job
        public ApplicationUser ApplicantUser { get; set; }
    }
}
