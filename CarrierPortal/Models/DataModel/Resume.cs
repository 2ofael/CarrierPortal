namespace CarrierPortal.Models.DataModel
{
    public class Resume
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }

    }
}
